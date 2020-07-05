using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace LittleThings.Patches
{
    class CareerModeScoring
    {
        [HarmonyPatch(typeof(SGHeaderWidget), "RefreshCountdown")]
        public static class SGHeaderWidget_RefreshCountdown_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.DisableCareerModeScoring;
            }

            public static void Postfix(SGHeaderWidget __instance, SimGameState ___simState, GameObject ___careerModeArea)
            {
                if (___simState.IsCareerMode())
                {
                    ___careerModeArea.SetActive(false);
                }  
            }
        }

        [HarmonyPatch(typeof(SimGameState), "OnCareerModeCompleted")]
        public static class SimGameState_OnCareerModeCompleted_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.DisableCareerModeScoring;
            }

            public static bool Prefix(SimGameState __instance)
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(SGCampaignOutcomeScreen), "SetMode")]
        public static class SGCampaignOutcomeScreen_SetMode_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.DisableCareerModeScoring;
            }

            public static void Postfix(SGCampaignOutcomeScreen __instance, SGCampaignOutcomeScreen.ScreenMode mode, SimGameState ___simState, HBSButton ___btnTertiary)
            {
                if (mode == SGCampaignOutcomeScreen.ScreenMode.OUT_OF_FUNDS && ___simState.SimGameMode == SimGameState.SimGameType.CAREER)
                {
                    ___btnTertiary.gameObject.SetActive(false);
                }
            }
        }
    }
}
