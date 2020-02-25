using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class RepairNotification
    {
        [HarmonyPatch(typeof(SimGameState), "ShowMechRepairsNeededNotif")]
        public static class SimGameState_ShowMechRepairsNeededNotif_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixRepairNotification;
            }

            public static bool Prefix(SimGameState __instance)
            {
                try
                {
                    Logger.Debug($"[SimGameState_ShowMechRepairsNeededNotif_PREFIX] Called");

                    List<MechDef> fieldableActiveMechs = new List<MechDef>();
                    foreach (MechDef mechDef in __instance.ActiveMechs.Values)
                    {
                        if (MechValidationRules.ValidateMechCanBeFielded(__instance, mechDef))
                        {
                            fieldableActiveMechs.Add(mechDef);
                        }
                    }

                    if (fieldableActiveMechs.Count > 4)
                    {
                        Logger.Debug($"[SimGameState_ShowMechRepairsNeededNotif_PREFIX] Suppress repair notification as we have more than 4 fieldable Mechs");
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }
    }
}
