using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using HBS;
using UnityEngine;

namespace LittleThings
{
    public static class Utilities
    {
        public static int GetReadiedWeaponCount(List<Weapon> weapons, ICombatant target, bool log = false)
        {
            int readiedWeaponCount = 0;
            foreach (Weapon w in weapons)
            {
                bool willFireAtTarget = w.WillFireAtTarget(target);
                if (w.IsEnabled && willFireAtTarget)
                {
                    if (log)
                    {
                        Logger.LogLine($"[Utilities_GetReadiedWeaponCount] {w.Name} is enabled and will fire at target({target.DisplayName})!");
                    }
                    readiedWeaponCount++;
                }
                else
                {
                    if (log)
                    {
                        Logger.LogLine($"[Utilities_GetReadiedWeaponCount] {w.Name} is not ready!");
                    }
                }
            }
            return readiedWeaponCount;
        }



        public static string GetLocationHealthString(ICombatant combatant, ArmorLocation location)
        {
            string result = "";

            if (combatant.UnitType != UnitType.Mech)
            {
                return "NO MECH";
            }

            Mech m = combatant as Mech;

            Color g = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.gold;
            Color w = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.whiteHalf;
            string colorTagG = $"<color=#{ColorUtility.ToHtmlStringRGBA(g)}>";
            string colorTagW = $"<color=#{ColorUtility.ToHtmlStringRGBA(w)}>";
            string closeColorTag = "</color>";

            int currentArmor = (int)m.GetCurrentArmor(location);
            int maxArmor = (int)HUDMechArmorReadout.GetInitialArmorForLocation(m.MechDef, location);
            int currentStructure = Math.Max(1, ((int)m.GetCurrentStructure(MechStructureRules.GetChassisLocationFromArmorLocation(location))));
            int maxStructure = (int)m.GetMaxStructure(MechStructureRules.GetChassisLocationFromArmorLocation(location));
            int totalCurrent = currentArmor + currentStructure;
            int totalMax = maxArmor + maxStructure;

            // Simple
            if (currentStructure < maxStructure || currentArmor <= 0)
            {
                result = $"{colorTagG}LOCATION HEALTH: {totalCurrent} / {totalMax}{closeColorTag}";
            }
            else
            {
                result = $"{colorTagW}LOCATION HEALTH: {totalCurrent} / {totalMax}{closeColorTag}";
            }

            // Differentiated
            //result = $"{colorTagG}STRUCTURE: {currentStructure} / {maxStructure}{closeColorTag}      {colorTagW}ARMOR: {currentArmor} / {maxArmor}{closeColorTag}";

            return result;
        }



        public static string GetReadiedWeaponsString(List<Weapon> weapons, ICombatant target)
        {
            string result = "";
            foreach (Weapon w in weapons)
            {
                bool willFireAtTarget = w.WillFireAtTarget(target);
                if (w.IsEnabled && willFireAtTarget)
                {
                    result += $" | {w.Name}";
                }
            }
            if(string.IsNullOrEmpty(result))
            {
                result = "-NO WEAPONS READY-";
            }
            else
            {
                result = result.Substring(3);
            }
            return result;
        }



        public static int GetMaxAllowedWeaponCountForHeadshots(Pilot p, bool log = false)
        {
            int allowedWeaponCount = 1;
            int gunnery = p.Gunnery;

            if (log)
            {
                Logger.LogLine($"[Utilities_GetMaxAllowedWeaponCountForHeadshots] {p.Callsign} has gunnery: {gunnery}");
            }

            if (gunnery >= 7)
            {
                allowedWeaponCount += 1;
            }
            if (gunnery >= 10)
            {
                allowedWeaponCount += 2;
            }

            return allowedWeaponCount;
        }
    }
}