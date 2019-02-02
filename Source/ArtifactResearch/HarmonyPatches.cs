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
            if(recipeDef == RecipeDefOf_ResearchArtifact.ResearchArtifact_Archotech)
            {
                List<Thing> result = new List<Thing>();
                result.Add(ThingMaker.MakeThing(ThingDefOf.PsychicEmanator));
                Thing hyper = ThingMaker.MakeThing(ThingDefOf.Hyperweave);
                hyper.stackCount = 25;
                result.Add(hyper);
                __result = result;
                return false;
            }
            return true;
        }

    }
}
