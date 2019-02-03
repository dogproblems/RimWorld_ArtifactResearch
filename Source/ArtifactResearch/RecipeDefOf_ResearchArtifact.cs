using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using Verse.AI;
using RimWorld;

namespace ArtifactResearch
{

    [DefOf]
    public static class RecipeDefOf_ResearchArtifact
    {
        public static RecipeDef ResearchArtifact_Archotech;
        public static RecipeDef ResearchArtifact_Glitterworld;

        static RecipeDefOf_ResearchArtifact()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RecipeDefOf_ResearchArtifact));
        }
    }
}

