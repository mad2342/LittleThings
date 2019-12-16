using System;
using BattleTech;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;

namespace LittleThings.Patches
{
    class Tooltips
    {
        // Fix Bonuses display for Equipment in Tooltips
        [HarmonyPatch(typeof(TooltipPrefab_Equipment), "SetData")]
        public static class TooltipPrefab_Equipment_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.UIFixes;
            }

            public static void Postfix(TooltipPrefab_Equipment __instance, object data, LocalizableText ___bonusesText)
            {
                try
                {
                    Logger.LogLine("[TooltipPrefab_Equipment_SetData_POSTFIX] Comma-separating bonusesText");

                    MechComponentDef mechComponentDef = (MechComponentDef)data;
                    if (string.IsNullOrEmpty(mechComponentDef.BonusValueA) && string.IsNullOrEmpty(mechComponentDef.BonusValueB))
                    {
                        ___bonusesText.SetText("-", Array.Empty<object>());
                    }
                    // BEN: Added these
                    else if (!string.IsNullOrEmpty(mechComponentDef.BonusValueA) && string.IsNullOrEmpty(mechComponentDef.BonusValueB))
                    {
                        ___bonusesText.SetText(mechComponentDef.BonusValueA, Array.Empty<object>());
                    }
                    else if (string.IsNullOrEmpty(mechComponentDef.BonusValueA) && !string.IsNullOrEmpty(mechComponentDef.BonusValueB))
                    {
                        ___bonusesText.SetText(mechComponentDef.BonusValueB, Array.Empty<object>());
                    }
                    // :NEB
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
