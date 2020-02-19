using System;
using System.Globalization;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using UnityEngine;

namespace LittleThings.Patches
{
    class Coil
    {
        [HarmonyPatch(typeof(CombatHUDWeaponSlot), "GetWeaponCOILStateColor")]
        public static class CombatHUDWeaponSlot_GetWeaponCOILStateColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixCoilPreviews;
            }

            // Original method fails if a potential melee target is hovered/selected as the color is determined by the current evasive pips preview UIModule IF THAT IS VISIBLE...
            // ...which IT IS NOT while contemplating a melee attack. Fallback is current evasive pips of the parent actor which might be much more than the current pathing suggests...
            // ...resulting in a coil-charged color for the default damage. Sigh...
            public static void Postfix(CombatHUDWeaponSlot __instance, ref Color __result, Color fallbackColor)
            {
                // Simple workaround: Change the color to "charged" depending on displayed damage value (which is correctly determined by Weapon.GetCOILDamageFromEvasivePips()!)
                // Far from good but at least not broken
                try
                {
                    float displayedDamage = float.Parse(__instance.DamageText.text, CultureInfo.InvariantCulture.NumberFormat);

                    // It's necessary to even look at the WeaponDef directly because the getter of Weapon.DamagePerShot is polluted with COIL-Logic
                    float weaponDefDamage = __instance.DisplayedWeapon.weaponDef.Damage;
                    //Logger.Info($"[CombatHUDWeaponSlot_GetWeaponCOILStateColor_POSTFIX] displayedDamage: {displayedDamage}");
                    //Logger.Info($"[CombatHUDWeaponSlot_GetWeaponCOILStateColor_POSTFIX] weaponDefDamage: {weaponDefDamage}");

                    if (displayedDamage <= weaponDefDamage)
                    {
                        __result = fallbackColor;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Always show the damage of a charged Coil with the special color
        [HarmonyPatch(typeof(CombatHUDWeaponSlot), "ShowTextColor")]
        public static class CombatHUDWeaponSlot_ShowTextColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixCoilPreviews;
            }

            public static void Postfix(CombatHUDWeaponSlot __instance, Weapon ___displayedWeapon)
            {
                try
                {
                    if (___displayedWeapon != null && ___displayedWeapon.Type == WeaponType.COIL)
                    {
                        float displayedDamage = float.Parse(__instance.DamageText.text, CultureInfo.InvariantCulture.NumberFormat);
                        float weaponDefDamage = __instance.DisplayedWeapon.weaponDef.Damage;
                        //Logger.Info($"[CombatHUDWeaponSlot_ShowTextColor_POSTFIX] displayedDamage: {displayedDamage}");
                        //Logger.Info($"[CombatHUDWeaponSlot_ShowTextColor_POSTFIX] weaponDefDamage: {weaponDefDamage}");

                        if (displayedDamage > weaponDefDamage)
                        {
                            __instance.DamageText.color = LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.EvasivePipsCharged.color;
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
