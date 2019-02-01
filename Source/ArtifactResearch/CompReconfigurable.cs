using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Verse;
using Verse.AI;

using RimWorld;


namespace ArtifactResearch
{
    class CompReconfigurable : ThingComp
    {
        private bool needToReconfigure = false;

        public CompProperties_Reconfigurable Props
        {
            get
            {
                return (CompProperties_Reconfigurable)this.props;
            }
        }

        public bool NeedToReconfigure
        {
            get
            {
                return needToReconfigure;
            }

            set
            {
                needToReconfigure = value;
            }

        }
        
        public override void PostExposeData()
        {
            Scribe_Values.Look<bool>(ref this.needToReconfigure, "ArtifactResearch.CompReconfigurableNeedToReconfigure", false, false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {

            Command_Toggle com = new Command_Toggle();
            com.hotKey = KeyBindingDefOf.Misc5;
            com.defaultLabel = "Reconfigure";
            com.icon = TexCommand.ToggleVent;
            com.isActive = new Func<bool>(delegate { return NeedToReconfigure; });
            com.toggleAction = delegate
            {
                NeedToReconfigure = !NeedToReconfigure;

                Designation des = parent.Map.designationManager.DesignationOn(parent, DesignationDefOf_Reconfigure.Reconfigure);
                if(NeedToReconfigure && des == null)
                {
                    parent.Map.designationManager.AddDesignation(new Designation(parent, DesignationDefOf_Reconfigure.Reconfigure));
                }
                else if(!NeedToReconfigure && des != null)
                {
                    des.Delete();
                }
            };
            if (NeedToReconfigure)
            {
                com.defaultDesc = "This building will be reconfigured by a researcher.";
            }
            else
            {
                com.defaultDesc = "This building will maintain its current function.";
            }
            yield return com;

        }

    }
}
