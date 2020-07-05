using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class HeavyMetalLootPopup
    {
        [HarmonyPatch(typeof(SimGameState), "FlashpointDayPassed")]
        public static class SimGameState_FlashpointDayPassed_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.DisableHeavyMetalLootPopup;
            }

            public static void Prefix(SimGameState __instance, ref StatCollection ___companyStats)
            {
                if (__instance.DataManager.ContentPackIndex.IsContentPackOwned("heavymetal", true) && !___companyStats.ContainsStatistic("HasSeenHeavyMetalLootPopup"))
                {
                    ___companyStats.AddStatistic<int>("HasSeenHeavyMetalLootPopup", 1);
                }
            }
        }
    }
}
