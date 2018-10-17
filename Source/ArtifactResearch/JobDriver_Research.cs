using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using Verse.AI;
using RimWorld;

namespace ArtifactResearch
{
    public class JobDriver_Research : JobDriver
    {
        private const int JobEndInterval = 4000;

        private ResearchProjectDef Project
        {
            get
            {
                return Find.ResearchManager.currentProj;
            }
        }

        private Building ResearchBench
        {
            get
            {
                return (Building)base.TargetThingA;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.ResearchBench;
            Job job = this.job;
            return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
        }

         protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil research = new Toil();
            research.tickAction = delegate
            {
                Pawn actor = research.actor;
                float num = actor.GetStatValue(StatDefOf.ResearchSpeed, true);
                num *= this.TargetThingA.GetStatValue(StatDefOf.ResearchSpeedFactor, true);
                Find.ResearchManager.ResearchPerformed(num, actor);
                actor.skills.Learn(SkillDefOf.Intellectual, 0.1f, false);
                actor.GainComfortFromCellIfPossible();
            };
            research.FailOn(() => this.Project == null);
            research.FailOn(() => !this.CanBeResearchedAt(this.Project, this.ResearchBench, false));
            research.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            research.WithEffect(EffecterDefOf.Research, TargetIndex.A);
            research.WithProgressBar(TargetIndex.A, delegate
            {
                ResearchProjectDef project = this.Project;
                if (project == null)
                {
                    return 0f;
                }
                return project.ProgressPercent;
            }, false, -0.5f);
            research.defaultCompleteMode = ToilCompleteMode.Delay;
            research.defaultDuration = 4000;
            research.activeSkill = (() => SkillDefOf.Intellectual);
            yield return research;
            yield return Toils_General.Wait(2, TargetIndex.None);
        }

        protected bool CanBeResearchedAt(ResearchProjectDef project, Building bench, bool ignoreResearchBenchPowerStatus)
        {
            if (project.requiredResearchBuilding != null && bench.def != project.requiredResearchBuilding)
            {
                return false;
            }
            if (!ignoreResearchBenchPowerStatus)
            {
                CompPowerTrader comp = bench.GetComp<CompPowerTrader>();
                if (comp != null && !comp.PowerOn)
                {
                    return false;
                }
            }
            if (!project.requiredResearchFacilities.NullOrEmpty<ThingDef>())
            {
                CompAffectedByFacilities affectedByFacilities = bench.TryGetComp<CompAffectedByFacilities>();
                if (affectedByFacilities == null)
                {
                    return false;
                }
                List<Thing> linkedFacilitiesListForReading = affectedByFacilities.LinkedFacilitiesListForReading;
                int i;
                for (i = 0; i < project.requiredResearchFacilities.Count; i++)
                {
                    if (linkedFacilitiesListForReading.Find((Thing x) => x.def == project.requiredResearchFacilities[i] && affectedByFacilities.IsFacilityActive(x)) == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
