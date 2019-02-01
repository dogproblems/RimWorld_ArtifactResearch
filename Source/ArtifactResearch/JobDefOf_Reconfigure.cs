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
    public static class JobDefOf_Reconfigure
    {
        public static JobDef JobReconfigure;

        static JobDefOf_Reconfigure()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf_Reconfigure));
        }
    }
}
