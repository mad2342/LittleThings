using System;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class SimGameChar
    {
        [HarmonyPatch(typeof(SimGameCharacter), "SetHighlightColor")]
        public static class SimGameCharacter_SetHighlightColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.DisableSimGameCharHighlights;
            }

            public static void Prefix(SimGameCharacter __instance, ref bool state)
            {
                try
                {
                    // Limit to certain characters
                    if
                    (
                        __instance.character == SimGameState.SimGameCharacterType.HERALDRY || 
                        __instance.character == SimGameState.SimGameCharacterType.MEMORIAL ||
                        __instance.character == SimGameState.SimGameCharacterType.CONTRACTS
                    )
                    {
                        Logger.Debug($"[SimGameCharacter_SetHighlightColor_PREFIX] Disable character highlight for {__instance.character}");
                        state = false;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(SimGameCharacter), "PulseHighlight")]
        public static class SimGameCharacter_PulseHighlight_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.DisableSimGameCharHighlights;
            }

            public static bool Prefix(SimGameCharacter __instance)
            {
                try
                {
                    return false;
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
