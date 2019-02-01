using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using Verse.AI;

using RimWorld;

namespace ArtifactResearch
{
    class WorkGiver_Reconfigure : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
    {
        get
        {
            return PathEndMode.Touch;
        }
    }

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        List<Designation> desList = pawn.Map.designationManager.allDesignations;
        for (int i = 0; i < desList.Count; i++)
        {
            if (desList[i].def == DesignationDefOf_Reconfigure.Reconfigure)
            {
                yield return desList[i].target.Thing;
            }
        }
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf_Reconfigure.Reconfigure) == null)
        {
            return false;
        }
        LocalTargetInfo target = t;
        return pawn.CanReserve(target, 1, -1, null, forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
            return new Job(JobDefOf_Reconfigure.JobReconfigure, t);
    }
}
}
