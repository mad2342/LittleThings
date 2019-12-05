using System;
using BattleTech;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;

namespace LittleThings.Patches
{
    class Tooltips
    {
        // Fix Bonuses display for Equipment
        [HarmonyPatch(typeof(TooltipPrefab_Equipment), "SetData")]
        public static class TooltipPrefab_Equipment_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.TooltipFixes;
            }

            public static void Postfix(TooltipPrefab_Equipment __instance, object data, LocalizableText ___bonusesText)
            {
                try
                {
                    Logger.LogLine("[TooltipPrefab_Equipment_SetData_POSTFIX] In");

                    MechComponentDef mechComponentDef = (MechComponentDef)data;
                    if (string.IsNullOrEmpty(mechComponentDef.BonusValueA) && string.IsNullOrEmpty(mechComponentDef.BonusValueB))
                    {
                        ___bonusesText.SetText("-", Array.Empty<object>());
                    }
                    else
                    {   
                        // BEN: Added missing comma
                        ___bonusesText.SetText("{0}, {1}", new object[]
                        {
                            mechComponentDef.BonusValueA,
                            mechComponentDef.BonusValueB
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
    }
}
