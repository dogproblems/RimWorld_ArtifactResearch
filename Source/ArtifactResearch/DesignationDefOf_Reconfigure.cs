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
    public static class DesignationDefOf_Reconfigure
    {
        public static DesignationDef Reconfigure;

        static DesignationDefOf_Reconfigure()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DesignationDefOf_Reconfigure));
        }
    }
}

