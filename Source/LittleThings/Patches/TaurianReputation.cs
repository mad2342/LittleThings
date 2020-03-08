using System;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class TaurianReputation
    {
        internal static string FixTaurianReputationTag = "taurian_reputation_fix_applied";

        [HarmonyPatch(typeof(SimGameState), "_OnAttachUXComplete")]
        public static class SimGameState__OnAttachUXComplete_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixTaurianReputationPostCampaign;
            }

            public static void Postfix(SimGameState __instance)
            {
                try
                {
                    if (!__instance.CompanyTags.Contains(FixTaurianReputationTag) && __instance.CompanyTags.Contains("story_complete"))
                    {
                        Logger.Debug($"[SimGameState__OnAttachUXComplete_POSTFIX] Apply reputation fix for the Taurian Concordat");

                        FactionValue TaurianConcordat = FactionEnumeration.GetFactionByName("TaurianConcordat");
                        int currentReputation = __instance.GetRawReputation(TaurianConcordat);
                        Logger.Debug($"[SimGameState__OnAttachUXComplete_POSTFIX] currentReputation: {currentReputation}");

                        if (currentReputation < -10)
                        {
                            int reputationToAdd = (currentReputation * -1) - 10;
                            Logger.Debug($"[SimGameState__OnAttachUXComplete_POSTFIX] reputationToAdd: {reputationToAdd}");
                            __instance.AddReputation(TaurianConcordat, reputationToAdd, false, null);
                        }

                        // Done
                        __instance.CompanyTags.Add(FixTaurianReputationTag);
                        Logger.Debug($"[SimGameState__OnAttachUXComplete_POSTFIX] Added {FixTaurianReputationTag} to CompanyTags");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
