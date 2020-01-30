using System;
using BattleTech.UI;
using Harmony;

namespace LittleThings.Patches
{
    class IconOverlay
    {
        [HarmonyPatch(typeof(AAR_UnitStatusWidget), "FillInData")]
        public static class AAR_UnitStatusWidget_FillInData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixIconOverlayAAR;
            }

            public static void Postfix(AAR_UnitStatusWidget __instance, MechBayMechUnitElement ___MechStatusIconWidget)
            {
                try
                {
                    Logger.Debug("[AAR_UnitStatusWidget_FillInData_POSTFIX] Disabling IconOverlay.");
                    ___MechStatusIconWidget.ShowStockIcon(false);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
