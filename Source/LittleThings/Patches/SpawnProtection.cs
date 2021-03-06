﻿using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class SpawnProtection
    {
        // Combat start
        [HarmonyPatch(typeof(TurnDirector), "StartFirstRound")]
        public static class TurnDirector_StartFirstRound_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSpawnProtection;
            }

            public static void Postfix(TurnDirector __instance)
            {
                try
                {
                    Logger.Debug($"[TurnDirector_StartFirstRound_POSTFIX] Protecting units on combat round one");

                    List<AbstractActor> actors = new List<AbstractActor>();
                    foreach (AbstractActor actor in __instance.Combat.AllActors)
                    {
                        if (actor is Mech mech)
                        {
                            Logger.Debug($"[TurnDirector_StartFirstRound_POSTFIX] Applying braced state to mech: {mech.DisplayName}");
                            mech.ApplyBraced();
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Reinforcements
        [HarmonyPatch(typeof(Team), "AddUnit")]
        public static class Team_AddUnit_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSpawnProtection;
            }

            public static void Postfix(Team __instance, AbstractActor unit)
            {
                try
                {
                    if (__instance.Combat.TurnDirector.CurrentRound > 1)
                    {
                        Logger.Debug($"[Team_AddUnit_POSTFIX] Protecting units on combat round: {__instance.Combat.TurnDirector.CurrentRound}");

                        if (unit is Mech mech)
                        {
                            Logger.Debug($"[Team_AddUnit_POSTFIX] Applying braced state to mech: {mech.DisplayName}");
                            mech.ApplyBraced();
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
