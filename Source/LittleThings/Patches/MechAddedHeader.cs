using System;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class MechAddedHeader
    {
        [HarmonyPatch(typeof(SimGameState), "AddMech")]
        public static class SimGameState_AddMech_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixMechAddedHeader;
            }

            public static void Prefix(SimGameState __instance, bool displayMechPopup, ref string mechAddedHeader)
            {
                try
                {
                    if (displayMechPopup && !string.IsNullOrEmpty(mechAddedHeader) && mechAddedHeader.Contains("Purchased"))
                    {
                        mechAddedHeader = mechAddedHeader.Replace("Purchased", "Acquired");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
