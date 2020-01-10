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

                    __instance.ParentActor?.Combat.MessageCenter.PublishMessage(
                        new AddSequenceToStackMessage(
                            new ShowActorInfoSequence(
                                __instance.ParentActor,
                                //$"HEALTH: {__instance.TotalHealth - __instance.Injuries}/{__instance.TotalHealth}",
                                $"INJURIES: {__instance.Injuries}",
                                FloatieMessage.MessageNature.PilotInjury,
                                true
                            )
                        )
                    );
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
    }
}
