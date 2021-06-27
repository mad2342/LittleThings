using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace LittleThings.Patches
{
    class SmartIndirectFire
    {
        // Cache
        internal static CombatGameState CGS = null;
        internal static CombatGameConstants CGC = null;
        internal static CombatHUD HUD = null;

        internal static Weapon WEAPON = null;
        internal static ICombatant TARGET = null;



        // Utils
        private static bool ShouldSmartIndirect(AbstractActor attacker, ICombatant target)
        {
            return CanSmartIndirect(attacker, attacker.CurrentPosition, attacker.CurrentRotation, target, false);
        }

        private static bool CanSmartIndirect(AbstractActor attacker, Vector3 attackPosition, Quaternion attackRotation, ICombatant target, bool checkWeapon = true)
        {
            bool pointless = CGS.ToHit.GetIndirectModifier(attacker) >= CGC.ToHit.ToHitCoverObstructed;
            bool unreachable = !attacker.IsTargetPositionInFiringArc(target, attackPosition, attackRotation, target.CurrentPosition);
            bool impossible = checkWeapon && !CanFireIndirectWeapon(attacker, Vector3.Distance(attackPosition, target.CurrentPosition));

            if (pointless || unreachable || impossible)
            {
                return false;
            }

            LineOfFireLevel lof = CGS.LOFCache.GetLineOfFire(attacker, attackPosition, target, target.CurrentPosition, target.CurrentRotation, out _);
            return lof == LineOfFireLevel.LOFObstructed;
        }

        private static bool CanFireIndirectWeapon(AbstractActor attacker, float dist)
        {
            if (attacker.Weapons.Any(w => w.IndirectFireCapable && w.IsEnabled && w.CanFire && w.MaxRange > dist))
            {
                return true;
            }   
            return false;
        }



        // Patches
        [HarmonyPatch(typeof(CombatHUD), "Init", new Type[] { typeof(CombatGameState) })]
        public static class CombatHUD_Init_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSmartIndirectFire;
            }

            public static void Postfix(CombatHUD __instance, CombatGameState Combat)
            {
                HUD = __instance;
                CGS = Combat;
                CGC = Combat?.Constants;
            }
        }

        [HarmonyPatch(typeof(CombatHUD), "OnCombatGameDestroyed")]
        public static class CombatHUD_OnCombatGameDestroyed_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSmartIndirectFire;
            }

            public static void Postfix(CombatHUD __instance)
            {
                HUD = null;
                CGS = null;
                CGC = null;
                WEAPON = null;
                TARGET = null;
            }
        }

        [HarmonyPatch(typeof(ToHit), "GetAllModifiers")]
        public static class ToHit_GetAllModifiers_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSmartIndirectFire;
            }

            public static void Prefix(ToHit __instance, Weapon weapon, ICombatant target)
            {
                WEAPON = weapon;
                TARGET = target;
            }
        }

        [HarmonyPatch(typeof(CombatHUDWeaponSlot), "UpdateToolTipsFiring")]
        public static class CombatHUDWeaponSlot_UpdateToolTipsFiring_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSmartIndirectFire;
            }

            public static void Prefix(CombatHUDWeaponSlot __instance, ICombatant target)
            {
                CombatHUDWeaponSlot slot = __instance;
                WEAPON = slot.DisplayedWeapon;
                TARGET = target;
            }
        }



        [HarmonyPatch(typeof(FiringPreviewManager), "GetPreviewInfo")]
        public static class FiringPreviewManager_GetPreviewInfo_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSmartIndirectFire;
            }

            public static void Postfix(FiringPreviewManager __instance, ref FiringPreviewManager.PreviewInfo __result, ICombatant target)
            {
                try
                {
                    if (__result.availability != FiringPreviewManager.TargetAvailability.PossibleDirect)
                    {
                        return;
                    }

                    AbstractActor selectedActor = HUD?.SelectedActor;
                    SelectionState activeState = HUD?.SelectionHandler?.ActiveState;
                    float dist = Vector3.Distance(activeState.PreviewPos, target.CurrentPosition);

                    if (selectedActor.Weapons.Any(w => !w.IndirectFireCapable && w.IsEnabled && w.CanFire && w.MaxRange > dist))
                    {
                        Logger.Info($"[FiringPreviewManager_GetPreviewInfo_POSTFIX] Smart indirect lof preview blocked by some direct fire only weapon");
                        return;
                    }
                        
                    if (!CanSmartIndirect(selectedActor, activeState.PreviewPos, activeState.PreviewRot, target))
                    {
                        return;
                    }



                    __result.availability = FiringPreviewManager.TargetAvailability.PossibleIndirect;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ToHit), "GetCoverModifier")]
        public static class ToHit_GetCoverModifier_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSmartIndirectFire;
            }

            public static void Postfix(ToHit __instance, ref float __result, AbstractActor attacker, ICombatant target)
            {
                try
                {
                    if (__result == 0 || !WEAPON.IndirectFireCapable || !ShouldSmartIndirect(attacker, target))
                    {
                        return;
                    }



                    __result = attacker.team.IsLocalPlayer ? 0 : __instance.GetIndirectModifier(attacker);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ToHit), "GetIndirectModifier", new Type[] { typeof(AbstractActor), typeof(bool) })]
        public static class ToHit_GetIndirectModifier_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSmartIndirectFire;
            }

            public static void Postfix(ToHit __instance, ref float __result, AbstractActor attacker, bool isIndirect)
            {
                try
                {
                    if (isIndirect || !WEAPON.IndirectFireCapable || !attacker.team.IsLocalPlayer || !ShouldSmartIndirect(attacker, TARGET))
                    {
                        return;
                    }



                    __result = __instance.GetIndirectModifier(attacker);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(MissileLauncherEffect), "SetupMissiles")]
        public static class MissileLauncherEffect_SetupMissiles_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableSmartIndirectFire;
            }

            public static void Postfix(MissileLauncherEffect __instance, ref bool ___isIndirect)
            {
                try
                {
                    if (___isIndirect || !__instance.weapon.IndirectFireCapable || !ShouldSmartIndirect(__instance.weapon.parent, CGS.FindCombatantByGUID(__instance.hitInfo.targetId)))
                    {
                        return;
                    }



                    ___isIndirect = true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
