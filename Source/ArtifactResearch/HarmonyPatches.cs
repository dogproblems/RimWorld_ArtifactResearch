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
            if(recipeDef == archo || recipeDef == glitter)
            {
                List<Thing> result = new List<Thing>();

                int total = recipeDef.products.Sum(x => x.count);
                int roll = Verse.Rand.Range(0, total - 1);
                int idx = 0;
                while (roll > 0)
                {
                    roll -= recipeDef.products[idx].count;
                    idx++;
                }

                if(idx<recipeDef.products.Count)
                {
                    result.Add(ThingMaker.MakeThing(recipeDef.products[idx].thingDef));
                    Thing bonus = recipeDef == archo ? ThingMaker.MakeThing(ThingDefOf.Plasteel) : ThingMaker.MakeThing(ThingDefOf.Hyperweave);
                    bonus.stackCount = 25;
                    result.Add(bonus);

                }

                __result = result;
                return false;
            }
            return true;
        }

    }
}
