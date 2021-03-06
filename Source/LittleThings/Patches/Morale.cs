﻿using System;
using Harmony;
using BattleTech;

namespace LittleThings.Patches
{
    class Morale
    {
        // Fix wrong getters for Resolve/Turn (obsolete with BTG 1.9.0)
        /*
        [HarmonyPatch(typeof(SimGameState), "CurResolvePerTurn", MethodType.Getter)]
        public static class SimGameState_CurResolvePerTurn_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.MoraleFixes;
            }

            public static void Postfix(SimGameState __instance, ref int __result)
            {
                try
                {
                    MoraleConstantsDef moraleConstants = __instance.CombatConstants.MoraleConstants;
                    int resolvePerTurn = 0;
                    for (int i = moraleConstants.BaselineAddFromSimGameThresholds.Length - 1; i >= 0; i--)
                    {
                        // Comparison was > but must be >=
                        // @ToDo: Check if this is really fixed with BTG 1.9.X
                        if (__instance.Morale >= moraleConstants.BaselineAddFromSimGameThresholds[i])
                        {
                            resolvePerTurn = moraleConstants.BaselineAddFromSimGameValues[i];
                            break;
                        }
                    }
                    if (resolvePerTurn != __result)
                    {
                        Logger.Debug("[SimGameState_CurResolvePerTurn_POSTFIX] SimGameState.CurResolvePerTurn BEFORE: " + __result);
                        __result = resolvePerTurn;
                        Logger.Debug("[SimGameState_CurResolvePerTurn_POSTFIX] SimGameState.CurResolvePerTurn AFTER: " + __result);
                    }  
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        // Fix another wrong calculation for Resolve/Turn (obsolete with BTG 1.9.0)
        [HarmonyPatch(typeof(Team), "CollectSimGameBaseline")]
        public static class Team_CollectSimGameBaseline_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.MoraleFixes;
            }

            public static void Postfix(Team __instance, MoraleConstantsDef moraleConstants, ref int __result)
            {
                try
                {
                    if (moraleConstants.BaselineAddFromSimGame)
                    {
                        int resolvePerTurn = 0;
                        for (int i = moraleConstants.BaselineAddFromSimGameThresholds.Length - 1; i >= 0; i--)
                        {
                            // Comparison was > but must be >=
                            if (__instance.CompanyMorale >= moraleConstants.BaselineAddFromSimGameThresholds[i])
                            {
                                resolvePerTurn = moraleConstants.BaselineAddFromSimGameValues[i];
                                break;
                            }
                        }
                        if (resolvePerTurn != __result)
                        {
                            Logger.Debug("[Team_CollectSimGameBaseline_POSTFIX] Team.CollectSimGameBaseline BEFORE: " + __result);
                            __result = resolvePerTurn;
                            Logger.Debug("[Team_CollectSimGameBaseline_POSTFIX] Team.CollectSimGameBaseline AFTER: " + __result);
                        }
                    } 
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */

        // Fix hardcoded(!) tooltip descriptions
        [HarmonyPatch(typeof(MoraleTooltipData), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(SimGameState), typeof(int) })]
        public static class MoraleTooltipData_Constructor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.MoraleFixes;
            }

            static void Postfix(MoraleTooltipData __instance, SimGameState sim, int moraleLevel)
            {
                try
                {
                    SimGameState simGameState = sim ?? UnityGameInstance.BattleTechGame.Simulation;
                    if (simGameState == null)
                    {
                        return;
                    }

                    // Get
                    //BaseDescriptionDef levelDescription = (BaseDescriptionDef)AccessTools.Property(typeof(MoraleTooltipData), "levelDescription").GetValue(__instance, null);

                    string levelDescriptionName = "Morale: " + simGameState.GetDescriptorForMoraleLevel(moraleLevel);
                    string levelDescriptionDetails = "At this level of morale, you will gain " + __instance.resolvePerTurn + " Resolve points per round of battle.";
                    //Logger.Debug("[MoraleTooltipData_Constructor_POSTFIX] levelDescriptionName: " + levelDescriptionName);
                    //Logger.Debug("[MoraleTooltipData_Constructor_POSTFIX] levelDescriptionDetails: " + levelDescriptionDetails);
                    BaseDescriptionDef customLevelDescription = new BaseDescriptionDef("TooltipMoraleCustom", levelDescriptionName, levelDescriptionDetails, "");

                    // Set
                    //BaseDescriptionDef levelDescription = AccessTools.Property(typeof(MoraleTooltipData), "levelDescription").SetValue(__instance, customLevelDescription, null);
                    new Traverse(__instance).Property("levelDescription").SetValue(customLevelDescription);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
