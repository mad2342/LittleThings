using System;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class EnemyInjuries
    {
        [HarmonyPatch(typeof(Pilot), "InjurePilot")]
        public static class Pilot_InjurePilot_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.ShowEnemyInjuries;
            }

            public static void Postfix(Pilot __instance)
            {
                try
                {
                    if (__instance.Injuries == __instance.TotalHealth || __instance.Injuries == 0)
                    {
                        return;
                    }

                    // Player can check health in HUD
                    if (__instance.Team.LocalPlayerControlsTeam)
                    {
                        return;
                    }

                    // Will only display once BonusHealth (Cockpit Mods anyone?) is reduced to 0
                    __instance.ParentActor?.Combat.MessageCenter.PublishMessage(
                        new AddSequenceToStackMessage(
                            new ShowActorInfoSequence(
                                __instance.ParentActor,
                                $"INJURIES: {__instance.Injuries}",
                                FloatieMessage.MessageNature.PilotInjury,
                                true
                            )
                        )
                    );
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
