using System;
using BattleTech.UI;
using Harmony;

namespace LittleThings.Patches
{
    class PortraitRightClick
    {
        internal static bool IsPortraitRightClick = false;

        [HarmonyPatch(typeof(CombatHUDMWStatus), "OnPortraitRightClicked")]
        public static class CombatHUDMWStatus_OnPortraitRightClicked_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixCombatHUDPortraitRightClick;
            }

            public static void Prefix(CombatHUDMWStatus __instance)
            {
                try
                {
                    // Setting a special state to prevent backing out in CombatSelectionHandler.ProcessRightRelease()
                    IsPortraitRightClick = true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(CombatSelectionHandler), "ProcessRightRelease")]
        public static class CombatSelectionHandler_ProcessRightRelease_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixCombatHUDPortraitRightClick;
            }

            public static bool Prefix(CombatSelectionHandler __instance)
            {
                try
                {
                    Logger.Debug($"[CombatSelectionHandler_ProcessRightRelease_PREFIX] IsPortraitRightClick: {IsPortraitRightClick}");

                    // Cancel backing out for regular right clicks IF a portrait is the target
                    if (IsPortraitRightClick)
                    {
                        IsPortraitRightClick = false;
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
