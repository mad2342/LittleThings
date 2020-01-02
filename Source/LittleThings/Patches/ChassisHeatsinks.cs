using Harmony;
using System;
using BattleTech;
using System.Globalization;
using UnityEngine;
using Localize;

namespace LittleThings.Patches
{
    class ChassisHeatsinks
    {
        [HarmonyPatch(typeof(Mech), "GetHeatSinkDissipation")]
        public static class Mech_GetHeatSinkDissipation_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisHeatsinks;
            }

            public static void Postfix(Mech __instance, ref float __result)
            {
                try
                {
                    if (!__instance.MechDef.Chassis.ChassisTags.Contains("chassis_heatsinks"))
                    {
                        return;
                    }

                    Logger.LogLine("[Mech_GetHeatSinkDissipation_POSTFIX] (" + __instance.MechDef.Description.Id + ") mechDef.Chassis.Heatsinks: " + __instance.MechDef.Chassis.Heatsinks);

                    int additionalHeatSinks = __instance.MechDef.Chassis.Heatsinks;
                    float heatSinkDissipation = __instance.Combat.Constants.Heat.DefaultHeatSinkDissipationCapacity;
                    float additionalHeatSinkDissipation = additionalHeatSinks * heatSinkDissipation;
                    Logger.LogLine("[Mech_GetHeatSinkDissipation_POSTFIX] (" + __instance.MechDef.Description.Id + ") additionalHeatSinkDissipation: " + additionalHeatSinkDissipation);

                    __result += additionalHeatSinkDissipation;
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(StatTooltipData), "SetHeatData")]
        public static class StatTooltipData_SetHeatData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisHeatsinks;
            }

            public static void Postfix(StatTooltipData __instance, MechDef def)
            {
                try
                {
                    if (!def.Chassis.ChassisTags.Contains("chassis_heatsinks"))
                    {
                        return;
                    }

                    Logger.LogLine("[StatTooltipData_SetHeatData_POSTFIX] (" + def.Description.Id + ") def.Chassis.Heatsinks: " + def.Chassis.Heatsinks);

                    CombatGameConstants cgc = CombatGameConstants.GetInstance(UnityGameInstance.BattleTechGame);
                    float num = ((float)def.Chassis.Heatsinks + (float)cgc.Heat.InternalHeatSinkCount) * cgc.Heat.DefaultHeatSinkDissipationCapacity;
                    Logger.LogLine("[StatTooltipData_SetHeatData_POSTFIX] (" + def.Description.Id + ") ChassisDissipationCapacity: " + num);

                    for (int i = 0; i < def.Inventory.Length; i++)
                    {
                        if (def.Inventory[i].ComponentDefType == ComponentType.HeatSink)
                        {
                            HeatSinkDef heatSinkDef = def.Inventory[i].Def as HeatSinkDef;
                            if (heatSinkDef != null)
                            {
                                num += heatSinkDef.DissipationCapacity;
                            }
                        }
                    }
                    Logger.LogLine("[StatTooltipData_SetHeatData_POSTFIX] (" + def.Description.Id + ") TotalDissipationCapacity: " + num);

                    __instance.dataList.Remove("Heat Sinking");
                    __instance.dataList.Add(Strings.T("Heat Sinking"), Strings.T("{0} Heat", new object[]
                    {
                        num.ToString()
                    }));

                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechStatisticsRules), "CalculateHeatEfficiencyStat")]
        public static class MechStatisticsRules_CalculateHeatEfficiencyStat_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisHeatsinks;
            }

            public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
            {
                try
                {
                    if (!mechDef.Chassis.ChassisTags.Contains("chassis_heatsinks"))
                    {
                        return true;
                    }
                    //Logger.LogLine("[MechStatisticsRules_CalculateHeatEfficiencyStat_PREFIX] (" + mechDef.Description.Id + ") mechDef.Chassis.Heatsinks: " + mechDef.Chassis.Heatsinks);



                    // This is a complete copy of the original method except marked parts...

                    // BEN: Add Chassis.Heatsinks to equation...
                    //float num = (float)MechStatisticsRules.Combat.Heat.InternalHeatSinkCount * MechStatisticsRules.Combat.Heat.DefaultHeatSinkDissipationCapacity;
                    float num = ((float)mechDef.Chassis.Heatsinks + (float)MechStatisticsRules.Combat.Heat.InternalHeatSinkCount) * MechStatisticsRules.Combat.Heat.DefaultHeatSinkDissipationCapacity;
                    // :NEB

                    float num2 = 0f;
                    float num3 = 0f;
                    float num4 = (float)MechStatisticsRules.Combat.Heat.MaxHeat;
                    float num5 = 0f;
                    int num6 = 0;
                    for (int i = 0; i < mechDef.Inventory.Length; i++)
                    {
                        MechComponentRef mechComponentRef = mechDef.Inventory[i];
                        if (mechComponentRef.DamageLevel == ComponentDamageLevel.Functional || mechComponentRef.DamageLevel == ComponentDamageLevel.Installing)
                        {
                            if (mechComponentRef.Def == null)
                            {
                                mechComponentRef.RefreshComponentDef();
                            }
                            if (mechComponentRef.ComponentDefType == ComponentType.Weapon)
                            {
                                WeaponDef weaponDef = mechComponentRef.Def as WeaponDef;
                                num3 += (float)weaponDef.HeatGenerated;
                            }
                            else if (mechComponentRef.ComponentDefType == ComponentType.JumpJet)
                            {
                                if (mechComponentRef.DamageLevel < ComponentDamageLevel.NonFunctional)
                                {
                                    num6++;
                                }
                            }
                            else if (mechComponentRef.ComponentDefType == ComponentType.HeatSink)
                            {
                                HeatSinkDef heatSinkDef = mechComponentRef.Def as HeatSinkDef;
                                num += heatSinkDef.DissipationCapacity;
                                if (heatSinkDef.statusEffects != null)
                                {
                                    for (int j = 0; j < heatSinkDef.statusEffects.Length; j++)
                                    {
                                        if (heatSinkDef.statusEffects[j].effectType == EffectType.StatisticEffect)
                                        {
                                            if (heatSinkDef.statusEffects[j].statisticData.statName == "MaxHeat")
                                            {
                                                num4 += float.Parse(heatSinkDef.statusEffects[j].statisticData.modValue, NumberFormatInfo.InvariantInfo);
                                            }
                                            if (heatSinkDef.statusEffects[j].statisticData.statName == "HeatGenerated" && heatSinkDef.statusEffects[j].statisticData.targetCollection == StatisticEffectData.TargetCollection.Weapon)
                                            {
                                                num5 -= 1f - float.Parse(heatSinkDef.statusEffects[j].statisticData.modValue, NumberFormatInfo.InvariantInfo);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (mechComponentRef.ComponentDefType == ComponentType.Upgrade)
                            {
                                UpgradeDef upgradeDef = mechComponentRef.Def as UpgradeDef;
                                if (upgradeDef.statusEffects != null)
                                {
                                    for (int k = 0; k < upgradeDef.statusEffects.Length; k++)
                                    {
                                        if (upgradeDef.statusEffects[k].effectType == EffectType.StatisticEffect)
                                        {
                                            if (upgradeDef.statusEffects[k].statisticData.statName == "MaxHeat")
                                            {
                                                num4 += float.Parse(upgradeDef.statusEffects[k].statisticData.modValue, NumberFormatInfo.InvariantInfo);
                                            }
                                            if (upgradeDef.statusEffects[k].statisticData.statName == "HeatGenerated" && upgradeDef.statusEffects[k].statisticData.targetCollection == StatisticEffectData.TargetCollection.Weapon)
                                            {
                                                num5 -= 1f - float.Parse(upgradeDef.statusEffects[k].statisticData.modValue, NumberFormatInfo.InvariantInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (num6 >= MechStatisticsRules.Combat.MoveConstants.MoveTable.Length)
                    {
                        num6 = MechStatisticsRules.Combat.MoveConstants.MoveTable.Length - 1;
                    }
                    int num7 = 3;
                    if (num6 > 0)
                    {
                        num2 += (float)num6 * MechStatisticsRules.Combat.Heat.JumpHeatUnitSize / (float)num7;
                    }
                    else
                    {
                        num2 = 0f;
                    }
                    num3 += num3 * num5;
                    num *= MechStatisticsRules.Combat.Heat.GlobalHeatSinkMultiplier;
                    float num8 = (num3 + num2) * MechStatisticsRules.Combat.Heat.GlobalHeatIncreaseMultiplier;
                    float num9 = Mathf.Min(num / num8 * 100f, 100f);
                    currentValue = Mathf.Round((num9 - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinHeatEfficiency) / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxHeatEfficiency - UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinHeatEfficiency) * 10f + (num4 - (float)MechStatisticsRules.Combat.Heat.MaxHeat) / 2f);
                    currentValue = Mathf.Max(currentValue, 1f);
                    maxValue = 10f;

                    // Skip original method
                    return false;
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                    return true;
                }
            }
        }
    }
}
