using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace ArtifactResearch
{
    public class Building_ResearchBench : Building_WorkTable
    {
        public Building_ResearchBench() : base()
        {
            Verse.Log.Message("ctor ArtifactResearch.Building_ResearchBench");
        }
    }
}
