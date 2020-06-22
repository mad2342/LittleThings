using System;
using BattleTech;
using Harmony;
using HBS;
using HBS.Collections;

namespace LittleThings.Patches
{
    class UnlockAllianceFlashpoints
    {
        [HarmonyPatch(typeof(SimGameState), "MeetsTagRequirements")]
        public static class SimGameState_MeetsTagRequirements_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.UnlockAllianceFlashpoints;
            }

            public static void Postfix(ref bool __result, TagSet reqTags)
            {
                try
                {
                    if (reqTags == null || reqTags.Count <= 0)
                    {
                        return;
                    }

                    SimGameState simGameState = SceneSingletonBehavior<UnityGameInstance>.Instance.Game.Simulation;
                    string[] ___items = (string[])AccessTools.Field(typeof(TagSet), "items").GetValue(reqTags);
                    foreach (string tag in ___items)
                    {
                        if (tag.Contains("ALLIED_FACTION_"))
                        {
                            string factionIdentifier = tag.Replace("ALLIED_FACTION_", "");
                            FactionValue factionValue = FactionEnumeration.GetFactionByName(factionIdentifier);
                            int currentFactionReputation = simGameState.GetRawReputation(factionValue);

                            Logger.Info($"[SimGameState_MeetsTagRequirements_POSTFIX] ---");
                            Logger.Info($"[SimGameState_MeetsTagRequirements_POSTFIX] reqTags: {reqTags}");
                            Logger.Info($"[SimGameState_MeetsTagRequirements_POSTFIX] factionIdentifier: {factionIdentifier}");
                            Logger.Info($"[SimGameState_MeetsTagRequirements_POSTFIX] currentFactionReputation: {currentFactionReputation}");

                            if (currentFactionReputation >= LittleThings.Settings.UnlockAllianceFlashpointsAtReputation)
                            {
                                __result = true;
                            }
                        }
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
