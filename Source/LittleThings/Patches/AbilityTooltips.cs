using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using Harmony;
using HBS;
using HBS.Extensions;
using SVGImporter;

namespace LittleThings.Patches
{
    class AbilityTooltips
    {
        [HarmonyPatch(typeof(SGBarracksSkillPip), "Initialize")]
        public static class SGBarracksSkillPip_Initialize_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableAbilityTooltips;
            }

            public static void Postfix(SGBarracksSkillPip __instance, string type, int index, bool hasPassives, AbilityDef ability)
            {
                if (!hasPassives)
                {
                    return;
                }

                SimGameState simGameState = LazySingletonBehavior<UnityGameInstance>.Instance.Game.Simulation;
                if (simGameState == null)
                {
                    return;
                }

                // Get the abilities that are not primary
                List<AbilityDef> abilities = simGameState.GetAbilityDefFromTree(type, index).Where(x => !x.IsPrimaryAbility).ToList();

                // Gets the first ability that has a tooltip
                AbilityDef passiveAbility = abilities.Find(x => x.DisplayParams == AbilityDef.DisplayParameters.ShowInPilotToolTip && !(string.IsNullOrEmpty(x.Description.Name) || string.IsNullOrEmpty(x.Description.Details)));

                // Clear the dot on tooltip-less dots
                if (passiveAbility == null)
                {
                    Traverse.Create(__instance).Field("skillPassiveTraitDot").GetValue<SVGImage>().gameObject.SetActive(false);
                }
                if (passiveAbility != null)
                {
                    __instance.gameObject.FindFirstChildNamed("obj-pip").GetComponent<HBSTooltip>().SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(passiveAbility.Description));
                }
            }
        }
    }
}
