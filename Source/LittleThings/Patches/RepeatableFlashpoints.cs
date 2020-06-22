using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;
using HBS.Collections;

namespace LittleThings.Patches
{
    class RepeatableFlashpoints
    {
        [HarmonyPatch(typeof(SimGameState), "_OnAttachUXComplete")]
        public static class SimGameState_OnAttachUXComplete_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.RepeatableHeavyMetalCampaign;
            }

            public static void Postfix(SimGameState __instance, TagSet ___companyTags)
            {
                try
                {
                    Logger.Debug($"[SimGameState_OnAttachUXComplete_POSTFIX] Called");

                    bool heavyMetalCompleted = ___companyTags.Contains("flashpoint_HM7_showdown_complete");
                    TagSet heavyMetalTagSet = new TagSet(___companyTags.ToArray().Where(tag => tag.Contains("HM")).ToList());


                    Logger.Debug($"[SimGameState_OnAttachUXComplete_POSTFIX] SimGameState.companyTags: {___companyTags}");
                    Logger.Debug($"[SimGameState_OnAttachUXComplete_POSTFIX] heavyMetalTagSet: {heavyMetalTagSet}");
                    Logger.Debug($"[SimGameState_OnAttachUXComplete_POSTFIX] SimGameState.completedFlashpoints: {string.Join(",", __instance.completedFlashpoints.ToArray())}");

                    if (heavyMetalCompleted)
                    {
                        ___companyTags.RemoveRange(heavyMetalTagSet);
                        __instance.completedFlashpoints.RemoveAll(item => item.Contains("fp_HM"));

                        // Items are hashed, cannot extract HM-specific ones... just kill em all
                        __instance.AlreadyClickedConversationResponses.Clear();
                    }

                    Logger.Debug("---");
                    Logger.Debug($"[SimGameState_OnAttachUXComplete_POSTFIX] SimGameState.companyTags: {___companyTags}");
                    Logger.Debug($"[SimGameState_OnAttachUXComplete_POSTFIX] SimGameState.completedFlashpoints: {string.Join(",", __instance.completedFlashpoints.ToArray())}");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
