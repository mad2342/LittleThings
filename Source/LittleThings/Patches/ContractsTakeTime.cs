using System;
using System.Reflection;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class ContractsTakeTime
    {
        [HarmonyPatch(typeof(SimGameState), "ResolveCompleteContract")]
        public static class SimGameState_ResolveCompleteContract_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.ContractsTakeTime;
            }

            // Must be prefix as CompletedContract may be set to null in original method
            public static void Prefix(SimGameState __instance)
            {
                try
                {
                    Logger.Debug("[SimGameState_ResolveCompleteContract_PREFIX] Calling OnDayPassed(0) now...");

                    // Get and call private method on SimGameState __instance
                    MethodInfo ___onDayPassed = __instance.GetType().GetMethod("OnDayPassed", BindingFlags.NonPublic | BindingFlags.Instance);
                    ___onDayPassed.Invoke(__instance, new object[] { 0 });
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
