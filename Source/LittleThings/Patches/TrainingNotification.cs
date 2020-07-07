using System;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class TrainingNotification
    {
        [HarmonyPatch(typeof(SimGameState), "ShowMechWarriorTrainingNotif")]
        public static class SimGameState_ShowMechWarriorTrainingNotif_Patch
        {
            public static bool Prepare()
            {
                // Vanilla limit is hardcoded to 2
                return LittleThings.Settings.EnableTrainingNotification && LittleThings.Settings.EnableTrainingNotificationLimit > 2;
            }

            public static bool Prefix(SimGameState __instance)
            {
                try
                {
                    Logger.Debug($"[SimGameState_ShowMechWarriorTrainingNotif_PREFIX] Suppressing training notification if number of trainable pilots < {LittleThings.Settings.EnableTrainingNotificationLimit}");

                    int trainablePilotCount = 0;
                    foreach (Pilot pilot in __instance.PilotRoster)
                    {
                        int[] skillLevels = new int[]
                        {
                            pilot.Gunnery,
                            pilot.Tactics,
                            pilot.Guts,
                            pilot.Piloting
                        };
                        int currentLevel = 0;
                        foreach (int i in skillLevels)
                        {
                            if (i > currentLevel && i < 10)
                            {
                                currentLevel = i;
                            }
                        }
                        if (currentLevel > 0 && pilot.UnspentXP > __instance.GetLevelCost(currentLevel + 1))
                        {
                            trainablePilotCount++;
                        }
                    }
                    Logger.Info($"[SimGameState_ShowMechWarriorTrainingNotif_PREFIX] trainablePilotCount: {trainablePilotCount}");
                    Logger.Info($"[SimGameState_ShowMechWarriorTrainingNotif_PREFIX] LittleThings.Settings.TrainingNotificationLimit: {LittleThings.Settings.EnableTrainingNotificationLimit}");

                    if (trainablePilotCount >= LittleThings.Settings.EnableTrainingNotificationLimit)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }       
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
