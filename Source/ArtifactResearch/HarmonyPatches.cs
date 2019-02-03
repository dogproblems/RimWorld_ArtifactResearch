using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Verse;
using Verse.AI;

using RimWorld;
using RimWorld.Planet;

using Harmony;
using System.Reflection;
using System.Reflection.Emit;

namespace ArtifactResearch
{
    [StaticConstructorOnStartup]
    class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("com.dogproblems.rimworldmods.artifactresearch");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    class PatchMakeRecipeProducts
    {
        static bool Prefix(ref RecipeDef recipeDef, ref IEnumerable<Thing> __result)
        {
            RecipeDef archo = RecipeDefOf_ResearchArtifact.ResearchArtifact_Archotech;
            RecipeDef glitter = RecipeDefOf_ResearchArtifact.ResearchArtifact_Glitterworld;
            if (recipeDef == archo || recipeDef == glitter)
            {
                List<Thing> result = new List<Thing>();

                int total = recipeDef.products.Sum(x => x.count);
                int roll = Verse.Rand.Range(0, total - 1);
                int idx = 0;
                roll -= recipeDef.products[idx].count;
                while (roll > 0)
                {
                    idx++;
                    roll -= recipeDef.products[idx].count;
                }

                if (idx < recipeDef.products.Count)
                {
                    Thing main = ThingMaker.MakeThing(recipeDef.products[idx].thingDef);
                    Verse.Log.Message(String.Format("Main yield: {0}", main.Label));
                    result.Add(main);
                    Thing bonus = recipeDef == archo ? ThingMaker.MakeThing(ThingDefOf.Plasteel) : ThingMaker.MakeThing(ThingDefOf.Hyperweave);
                    bonus.stackCount = 25;
                    Verse.Log.Message(String.Format("Bonus yield: {0}", bonus.Label));
                    result.Add(bonus);

                }

                __result = result;
                return false;
            }
            return true;
        }

    }


    //[HarmonyPatch(typeof(Toils_Recipe), "MakeUnfinishedThingIfNeeded")]
    //class PatchMakeUnfinishedThingIfNeeded
    //{
    //    static bool Prefix(ref Toil __result)
    //    {
    //        Toil toil = new Toil();
    //        toil.initAction = delegate
    //        {
    //            Pawn actor = toil.actor;
    //            Job curJob = actor.jobs.curJob;
    //            if (!curJob.RecipeDef.UsesUnfinishedThing)
    //            {
    //                return;
    //            }
    //            if (curJob.GetTarget(TargetIndex.B).Thing is UnfinishedThing)
    //            {
    //                return;
    //            }
    //            List<Thing> list = CalculateIngredients(curJob, actor);
    //            Thing thing = CalculateDominantIngredient(curJob, list);
    //            for (int i = 0; i < list.Count; i++)
    //            {
    //                Thing thing2 = list[i];
    //                actor.Map.designationManager.RemoveAllDesignationsOn(thing2, false);
    //                if (thing2.Spawned)
    //                {
    //                    thing2.DeSpawn(DestroyMode.Vanish);
    //                }
    //            }
    //            ThingDef stuff = (!curJob.RecipeDef.unfinishedThingDef.MadeFromStuff) ? null : thing.def;
    //            //Verse.Log.Message("Making thing");
    //            UnfinishedThing unfinishedThing = (UnfinishedThing)ThingMaker.MakeThing(curJob.RecipeDef.unfinishedThingDef, stuff);
    //            //Verse.Log.Message("Made thing");
    //            unfinishedThing.Creator = actor;
    //            Verse.Log.Message(String.Format("curJob.bill = {0}, type = {1}", curJob.bill.Label, curJob.bill.GetType().ToString()));
    //            unfinishedThing.BoundBill = (Bill_ProductionWithUft)curJob.bill;
    //            //Verse.Log.Message("Made bill");
    //            unfinishedThing.ingredients = list;
    //            CompColorable compColorable = unfinishedThing.TryGetComp<CompColorable>();
    //            if (compColorable != null)
    //            {
    //                compColorable.Color = thing.DrawColor;
    //            }
    //            GenSpawn.Spawn(unfinishedThing, curJob.GetTarget(TargetIndex.A).Cell, actor.Map, WipeMode.Vanish);
    //            curJob.SetTarget(TargetIndex.B, unfinishedThing);
    //            actor.Reserve(unfinishedThing, curJob, 1, -1, null, true);
    //        };
    //        __result = toil;
    //        return false;
    //    }

    //    private static List<Thing> CalculateIngredients(Job job, Pawn actor)
    //    {
    //        UnfinishedThing unfinishedThing = job.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
    //        if (unfinishedThing != null)
    //        {
    //            List<Thing> ingredients = unfinishedThing.ingredients;
    //            job.RecipeDef.Worker.ConsumeIngredient(unfinishedThing, job.RecipeDef, actor.Map);
    //            job.placedThings = null;
    //            return ingredients;
    //        }
    //        List<Thing> list = new List<Thing>();
    //        if (job.placedThings != null)
    //        {
    //            for (int i = 0; i < job.placedThings.Count; i++)
    //            {
    //                if (job.placedThings[i].Count <= 0)
    //                {
    //                    Log.Error(string.Concat(new object[]
    //                    {
    //                        "PlacedThing ",
    //                        job.placedThings[i],
    //                        " with count ",
    //                        job.placedThings[i].Count,
    //                        " for job ",
    //                        job
    //                    }), false);
    //                }
    //                else
    //                {
    //                    Thing thing;
    //                    if (job.placedThings[i].Count < job.placedThings[i].thing.stackCount)
    //                    {
    //                        thing = job.placedThings[i].thing.SplitOff(job.placedThings[i].Count);
    //                    }
    //                    else
    //                    {
    //                        thing = job.placedThings[i].thing;
    //                    }
    //                    job.placedThings[i].Count = 0;
    //                    if (list.Contains(thing))
    //                    {
    //                        Log.Error("Tried to add ingredient from job placed targets twice: " + thing, false);
    //                    }
    //                    else
    //                    {
    //                        list.Add(thing);
    //                        if (job.RecipeDef.autoStripCorpses)
    //                        {
    //                            IStrippable strippable = thing as IStrippable;
    //                            if (strippable != null)
    //                            {
    //                                strippable.Strip();
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        job.placedThings = null;
    //        return list;
    //    }

    //    private static Thing CalculateDominantIngredient(Job job, List<Thing> ingredients)
    //    {
    //        UnfinishedThing uft = job.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
    //        if (uft != null && uft.def.MadeFromStuff)
    //        {
    //            return uft.ingredients.First((Thing ing) => ing.def == uft.Stuff);
    //        }
    //        if (ingredients.NullOrEmpty<Thing>())
    //        {
    //            return null;
    //        }
    //        if (job.RecipeDef.productHasIngredientStuff)
    //        {
    //            return ingredients[0];
    //        }
    //        if (job.RecipeDef.products.Any((ThingDefCountClass x) => x.thingDef.MadeFromStuff))
    //        {
    //            return (from x in ingredients
    //                    where x.def.IsStuff
    //                    select x).RandomElementByWeight((Thing x) => (float)x.stackCount);
    //        }
    //        return ingredients.RandomElementByWeight((Thing x) => (float)x.stackCount);
    //    }

    //}

    //[HarmonyPatch(typeof(BillUtility), "MakeNewBill")]
    //class PatchMakeNewBill
    //{
    //    public static bool Prefix(ref RecipeDef recipe, ref Bill __result)
    //    {
    //        Verse.Log.Message(String.Format("Making bill, recipe = {0}, uft = ", recipe.defName, recipe.unfinishedThingDef == null ? "NULL" : recipe.unfinishedThingDef.defName));
    //        if (recipe.UsesUnfinishedThing)
    //        {
    //            __result = new Bill_ProductionWithUft(recipe);
    //            return false;
    //        }
    //        __result = new Bill_Production(recipe);
    //        return false;
    //    }
    //}


}
