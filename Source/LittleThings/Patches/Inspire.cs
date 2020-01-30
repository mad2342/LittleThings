using System;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class Inspire
    {
        // Dead or incapacitated actors won't get inspired
        [HarmonyPatch(typeof(Mech), "InspireActor")]
        public static class Mech_InspireActor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixInspire;
            }

            public static bool Prefix(Mech __instance)
            {
                try
                {
                    // @ToDo: Use this?
                    //__instance.CanBeInspired;
                    bool ActorCanBeInspired = !__instance.IsDead && !__instance.IsFlaggedForDeath && !__instance.pilot.IsIncapacitated;

                    if (ActorCanBeInspired)
                    {
                        Logger.Debug("[Mech_InspireActor_PREFIX] Pilot (" + __instance.pilot.Callsign + ") can be inspired. Calling original method...");
                        
                        // Call original method
                        return true;
                    }
                    else
                    {
                        Logger.Debug("[Mech_InspireActor_PREFIX] Pilot (" + __instance.pilot.Callsign + ") is either dead or incapacitated. Skipping original method...");

                        // Skip original method
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
