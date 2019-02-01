using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using Verse.AI;

using RimWorld;

namespace ArtifactResearch
{
    class JobDriver_Reconfigure : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo targetA = this.job.targetA;
            Job job = this.job;
            return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            //this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOn(delegate
            {
                Designation designation = this.Map.designationManager.DesignationOn(this.TargetThingA, DesignationDefOf_Reconfigure.Reconfigure);
                return designation == null;
            });
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return Toils_General.Wait(100, TargetIndex.None);//.FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            Toil finalize = new Toil();
            finalize.initAction = delegate
            {
                Pawn actor = finalize.actor;
                ThingWithComps thing = (ThingWithComps)actor.CurJob.targetA.Thing;

                CompReconfigurable comp = thing.TryGetComp<CompReconfigurable>();
                if (comp != null && comp.NeedToReconfigure)
                {
                    //replace here
                    ThingWithComps replacement = (ThingWithComps)ThingMaker.MakeThing(comp.Props.resultDef, thing.Stuff);
                    replacement.SetFactionDirect(thing.Faction);
                    Verse.Log.Warning("thing.Map=" + thing.Map is null ? "NULL" : thing.Map.GetHashCode().ToString());
                    var spawned = GenSpawn.Spawn(replacement, thing.Position, thing.Map, thing.Rotation);
                    Verse.Log.Warning("HitPoints");
                    spawned.HitPoints = thing.HitPoints;
                    Verse.Log.Warning("Selector");
                    Find.Selector.Select(spawned, false);

                }

                Designation designation = this.Map.designationManager.DesignationOn(thing, DesignationDefOf_Reconfigure.Reconfigure);
                if (designation != null)
                {
                    designation.Delete();
                }


                
            };
            finalize.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return finalize;
        }

    }
}
