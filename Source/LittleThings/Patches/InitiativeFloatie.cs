using System;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class InitiativeFloatie
    {
        [HarmonyPatch(typeof(AbstractActor), "ForceUnitOnePhaseDown")]
        public static class Pilot_ForceUnitOnePhaseDown_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.DisableInitiativeFloatieForDeadActors;
            }

            public static bool Prefix(AbstractActor __instance)
            {
                try
                {
                    Logger.Debug($"[AbstractActor_ForceUnitOnePhaseDown_PREFIX] ({__instance.DisplayName}) is flagged for death: {__instance.IsFlaggedForDeath}");
                    Logger.Debug($"[AbstractActor_ForceUnitOnePhaseDown_PREFIX] ({__instance.DisplayName}) is dead: {__instance.IsDead}");

                    // Check if actor is dead
                    if (__instance.IsDead || __instance.IsFlaggedForDeath)
                    {
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
