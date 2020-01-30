using System;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class Injuries
    {
        [HarmonyPatch(typeof(Pilot), "InitAbilities")]
        public static class Pilot_InitAbilities_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.InjuryFixes;
            }

            public static void Prefix(Pilot __instance, ref bool ModifyStats)
            {
                try
                {
                    Logger.Debug("[Pilot_InitAbilities_PREFIX] ModifyStats (" + __instance.Callsign + "): " + ModifyStats);
                    Logger.Debug("[Pilot_InitAbilities_PREFIX] __instance.Guts: " + __instance.Guts);
                    Logger.Debug("[Pilot_InitAbilities_PREFIX] __instance.Health: " + __instance.Health);

                    // Check if health is correctly set
                    if (__instance.Guts >= 4 && __instance.Health < 4)
                    {
                        ModifyStats = true;
                        Logger.Debug("[Pilot_InitAbilities_PREFIX] Encountered BUG! Forced parameter ModifyStats (" + ModifyStats + ") so that Pilot.ApplyPassiveAbilities() is now called for sure.");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }


        // Fixed with json-merges, changed effectTriggerType to 'Passive' in the relevant Traits
        // Left for reference
        /*
        [HarmonyPatch(typeof(Pilot), "ApplyPassiveAbilities")]
        public static class Pilot_ApplyPassiveAbilities_Patch
        {
            public static void Postfix(Pilot __instance, int stackItemUID)
            {
                try
                {
                    Logger.Debug("[Pilot_ApplyPassiveAbilities_POSTFIX] Reverted if-clause to 1.5.X version to circumvent BUG regarding health display.");

                    for (int i = 0; i < __instance.PassiveAbilities.Count; i++)
                    {
                        if (__instance.ParentActor == null)
                        {
                            for (int j = 0; j < __instance.PassiveAbilities[i].Def.EffectData.Count; j++)
                            {
                                EffectData effectData = __instance.PassiveAbilities[i].Def.EffectData[j];
                                // Reverted to 1.5.X version
                                //if (effectData.targetingData.effectTriggerType == EffectTriggerType.Passive && effectData.effectType == EffectType.StatisticEffect && effectData.statisticData.targetCollection == StatisticEffectData.TargetCollection.Pilot)
                                if (effectData.effectType == EffectType.StatisticEffect && effectData.statisticData.targetCollection == StatisticEffectData.TargetCollection.Pilot)
                                {
                                    Variant variant = new Variant(Type.GetType(effectData.statisticData.modType));
                                    variant.SetValue(effectData.statisticData.modValue);
                                    variant.statName = effectData.statisticData.statName;

                                    StatCollection ___statCollection = Traverse.Create(__instance).Field("statCollection").GetValue<StatCollection>();
                                    ___statCollection.ModifyStatistic(__instance.GUID, stackItemUID, effectData.statisticData.statName, effectData.statisticData.operation, variant, -1, true);
                                }
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
        */
    }
}
