using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace ArtifactResearch
{
    class CompProperties_Reconfigurable : CompProperties
    {
        public ThingDef resultDef;

        public CompProperties_Reconfigurable()
        {
            this.compClass = typeof(CompReconfigurable);
        }
    }
}
