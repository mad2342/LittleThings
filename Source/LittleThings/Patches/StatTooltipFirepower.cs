using System;
using BattleTech;
using Harmony;
using Localize;

namespace LittleThings.Patches
{
    class StatTooltipFirepower
    {
        [HarmonyPatch(typeof(StatTooltipData), "SetFirepowerData")]
        public static class StatTooltipData_SetFirepowerData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixStatTooltipFirepower;
            }

            public static bool Prefix(StatTooltipData __instance, MechDef def)
            {
                try
                {
                    Logger.Debug($"[StatTooltipData_SetFirepowerData_PREFIX] Fixing max stability damage value and respecting special gear...");

                    float allDmg = 0f;
                    float stbDmg = 0f;

                    float energyDmg = 0f;
                    float ballisticDmg = 0f;
                    float missileDmg = 0f;
                    float supportDmg = 0f;

                    bool hasOptimizedCapacitors = false;
                    bool hasBallisticSiegeCompensators = false;

                    for (int i = 0; i < def.Inventory.Length; i++)
                    {
                        if ((def.Inventory[i].DamageLevel == ComponentDamageLevel.Functional || def.Inventory[i].DamageLevel == ComponentDamageLevel.Installing) && def.Inventory[i].DamageLevel != ComponentDamageLevel.Destroyed)
                        {
                            if (def.Inventory[i].Def.Description.Id == "Gear_General_Optimized_Capacitors")
                            {
                                hasOptimizedCapacitors = true;
                                Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] hasOptimizedCapacitors: {hasOptimizedCapacitors}");
                            }
                            if (def.Inventory[i].Def.Description.Id == "Gear_General_GM_Ballistic_Siege_Compensators")
                            {
                                hasBallisticSiegeCompensators = true;
                                Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] hasBallisticSiegeCompensators: {hasBallisticSiegeCompensators}");
                            }


                            if (def.Inventory[i].ComponentDefType == ComponentType.Weapon)
                            {
                                WeaponDef weaponDef = def.Inventory[i].Def as WeaponDef;
                                if (weaponDef != null)
                                {
                                    // Energy weapons
                                    if (weaponDef.WeaponCategoryValue.IsEnergy)
                                    {
                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] {weaponDef.Description.Name} is an energy weapon");

                                        if (weaponDef.Type == WeaponType.COIL)
                                        {
                                            allDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * 3f * (float)weaponDef.ShotsWhenFired) : (weaponDef.Damage * 3f));
                                            stbDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Instability * (float)weaponDef.ShotsWhenFired) : weaponDef.Instability);

                                            energyDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * 3f * (float)weaponDef.ShotsWhenFired) : (weaponDef.Damage * 3f));
                                        }
                                        else
                                        {
                                            allDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);
                                            stbDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Instability * (float)weaponDef.ShotsWhenFired) : weaponDef.Instability);

                                            energyDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);
                                        }

                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] energyDmg: {energyDmg}");
                                    }
                                    // Ballistic weapons
                                    else if (weaponDef.WeaponCategoryValue.IsBallistic)
                                    {
                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] {weaponDef.Description.Name} is a ballistic weapon");

                                        allDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);
                                        stbDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Instability * (float)weaponDef.ShotsWhenFired) : weaponDef.Instability);

                                        ballisticDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);

                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] ballisticDmg: {ballisticDmg}");
                                    }
                                    // Missiles
                                    else if (weaponDef.WeaponCategoryValue.IsMissile)
                                    {
                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] {weaponDef.Description.Name} is a missile weapon");

                                        allDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);
                                        stbDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Instability * (float)weaponDef.ShotsWhenFired) : weaponDef.Instability);

                                        missileDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);

                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] missileDmg: {missileDmg}");
                                    }
                                    // Support
                                    else if (weaponDef.WeaponCategoryValue.IsSupport)
                                    {
                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] {weaponDef.Description.Name} is a support weapon");

                                        if (weaponDef.Type == WeaponType.COIL)
                                        {
                                            allDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * 3f * (float)weaponDef.ShotsWhenFired) : (weaponDef.Damage * 3f));
                                            stbDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Instability * (float)weaponDef.ShotsWhenFired) : weaponDef.Instability);

                                            supportDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * 3f * (float)weaponDef.ShotsWhenFired) : (weaponDef.Damage * 3f));
                                        }
                                        else
                                        {
                                            allDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);
                                            stbDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Instability * (float)weaponDef.ShotsWhenFired) : weaponDef.Instability);

                                            supportDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);
                                        }

                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] supportDmg: {supportDmg}");
                                    }
                                    // Everything else
                                    else
                                    {
                                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] {weaponDef.Description.Name} is uncategorized");

                                        allDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Damage * (float)weaponDef.ShotsWhenFired) : weaponDef.Damage);
                                        stbDmg += ((weaponDef.ShotsWhenFired > 0) ? (weaponDef.Instability * (float)weaponDef.ShotsWhenFired) : weaponDef.Instability);
                                    }
                                }
                            }
                        }
                    }

                    string energyDmgStr = $"{energyDmg}";
                    if (hasOptimizedCapacitors)
                    {
                        float additionalEnergyDmg = (energyDmg * 0.2f);
                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] additionalEnergyDmg: {additionalEnergyDmg}");

                        allDmg += additionalEnergyDmg;
                        energyDmgStr = $"{energyDmg} ({energyDmg + additionalEnergyDmg})";
                    }

                    string ballisticDmgStr = $"{ballisticDmg}";
                    if (hasBallisticSiegeCompensators)
                    {
                        float additionalBallisticDmg = (ballisticDmg * 0.2f);
                        Logger.Info($"[StatTooltipData_SetFirepowerData_PREFIX] additionalBallisticDmg: {additionalBallisticDmg}");

                        allDmg += additionalBallisticDmg;
                        ballisticDmgStr = $"{ballisticDmg} ({ballisticDmg + additionalBallisticDmg})";
                    }

                    // Column 1
                    __instance.dataList.Add(Strings.T("Max Dmg"), allDmg.ToString());
                    __instance.dataList.Add(Strings.T("Energy"), energyDmgStr);
                    __instance.dataList.Add(Strings.T("Support"), supportDmg.ToString());
                    
                    // Column 2
                    __instance.dataList.Add(Strings.T("Max Stability Dmg"), stbDmg.ToString());
                    __instance.dataList.Add(Strings.T("Ballistic"), ballisticDmgStr);
                    __instance.dataList.Add(Strings.T("Missile"), missileDmg.ToString());

                    // Skipping original method
                    return false;
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
