using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using UnityEngine;

namespace LittleThings.Patches
{
    class SingleWeaponHeadshots
    {
        // NOTE: This setter is only called for Called Shots. See CombatHUDAttackModeSelector.Update() for the surrounding if-clause...
        [HarmonyPatch(typeof(CombatHUDAttackModeSelector), "DisplayedLocation", MethodType.Setter)]
        public static class CombatHUDAttackModeSelector_DisplayedLocation_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.SingleWeaponHeadshots;
            }

            public static void Prefix(CombatHUDAttackModeSelector __instance)
            {
                try
                {
                    __instance.DescriptionText.SetAllDirty();
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }

            public static void Postfix(CombatHUDAttackModeSelector __instance, ArmorLocation ___displayedLocation)
            {
                try
                {
                    //Logger.LogLine("[CombatHUDAttackModeSelector_DisplayedLocation_POSTFIX] Called.");

                    CombatHUD ___HUD = (CombatHUD)AccessTools.Property(typeof(CombatHUDAttackModeSelector), "HUD").GetValue(__instance, null);

                    // Prepping Called Shot
                    if (___displayedLocation == ArmorLocation.None)
                    {
                        // Safeguarding even more...
                        if (
                            ___HUD.SelectionHandler.ActiveState.SelectionType != SelectionType.FireMorale ||
                            ___HUD.SelectedTarget.UnitType != UnitType.Mech
                        )
                        {
                            return;
                        }

                        if (___HUD.SelectedTarget.IsShutDown || ___HUD.SelectedTarget.IsProne)
                        {
                            Color c = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.whiteHalf;
                            __instance.DescriptionText.SetText("{0}{1}IMMOBILIZED{2}", new object[]
                            {
                                "^^^ SELECT A LOCATION FOR CALLED SHOT ^^^ \n",
                                "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + "><size=80%>",
                                "</size></color>"
                            });
                            return;
                        }

                        if (___HUD.CalledShotPopUp.ShownAttackDirection == AttackDirection.FromBack)
                        {
                            Color c = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.whiteHalf;
                            __instance.DescriptionText.SetText("{0}{1}REAR ARC{2}", new object[]
                            {
                                "^^^ SELECT A LOCATION FOR CALLED SHOT ^^^ \n",
                                "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + "><size=80%>",
                                "</size></color>"
                            });
                            return;
                        }

                        // Regular Precision Strike from front or sides
                        Pilot selectedPilot = ___HUD.SelectedActor.GetPilot();
                        int enabledWeaponCount = Utilities.GetReadiedWeaponCount(___HUD.SelectedActor.Weapons, ___HUD.SelectedTarget);
                        int maxAllowedWeapons = Utilities.GetMaxAllowedWeaponCountForHeadshots(selectedPilot);
                        int weaponsToDisable = Math.Max(0, enabledWeaponCount - maxAllowedWeapons);
                        //string pluralizeSuffix = maxAllowedWeapons > 1 ? "S" : "";
                        string pluralizeSuffix = weaponsToDisable > 1 ? "S" : "";

                        //Logger.LogLine("[CombatHUDAttackModeSelector_DisplayedLocation_POSTFIX] Appending headshot note...");
                        if (weaponsToDisable >= 1)
                        {
                            Color c = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.whiteHalf;
                            __instance.DescriptionText.SetText("{0}{1}DISABLE {2} MORE WEAPON{3} FOR A HEADSHOT{4}", new object[]
                            {
                                "^^^ SELECT A LOCATION FOR CALLED SHOT ^^^ \n",
                                "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + "><size=80%>",
                                weaponsToDisable,
                                pluralizeSuffix,
                                "</size></color>"
                            });
                        }
                        else
                        {
                            Color c = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.gold;
                            __instance.DescriptionText.SetText("{0}{1}HEADSHOT ENABLED{2}", new object[]
                            {
                                "^^^ SELECT A LOCATION FOR CALLED SHOT ^^^ \n",
                                "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + "><size=80%>",
                                "</size></color>"
                            });
                        }
                    }
                    // Called Shot set at location
                    else
                    {
                        //string targetName = ___HUD.SelectedActor.DisplayName;
                        //string readiedWeaponList = Utilities.GetReadiedWeaponsString(___HUD.SelectedActor.Weapons, ___HUD.SelectedTarget);
                        string locationHealth = Utilities.GetLocationHealthString(___HUD.SelectedTarget, ___displayedLocation);

                        __instance.DescriptionText.SetText("CALLED SHOT: {0}\n{1}{2}{3}", new object[]
                        {
                            Mech.GetLongArmorLocation(___displayedLocation),
                            "<size=80%>",
                            locationHealth,
                            "</size>"
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        /*
        // Fix small UI problems with the decription text box resizing...
        [HarmonyPatch(typeof(SelectionStateFire), "FireButtonString", MethodType.Getter)]
        public static class SelectionStateFire_FireButtonString_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.SingleWeaponHeadshots;
            }

            public static void Postfix(SelectionStateFire __instance, ref string __result)
            {
                try
                {
                    if (!__instance.NeedsCalledShot || __instance.HasCalledShot)
                    {
                        return;
                    }
                    if (__instance.SelectionType == SelectionType.FireMorale)
                    {
                        Logger.LogLine("[SelectionStateFire_FireButtonString_POSTFIX] Appending placeholder note...");
                        __result = $" ^^^ SELECT A LOCATION FOR CALLED SHOT ^^^ \n<size=80%> </size>";
                    }   
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
        */

        [HarmonyPatch(typeof(CombatHUDCalledShotPopUp), "UpdateMechDisplay")]
        public static class CombatHUDCalledShotPopUp_UpdateMechDisplay_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.SingleWeaponHeadshots;
            }

            public static void Postfix(CombatHUDCalledShotPopUp __instance, CombatHUD ___HUD, AbstractActor ___displayedActor, AttackDirection ___shownAttackDirection)
            {
                try
                {
                    //Logger.LogLine("[CombatHUDCalledShotPopUp_UpdateMechDisplay_POSTFIX] Visually disabling head display if it may not be targeted...");

                    if (__instance.ShownAttackDirection == AttackDirection.FromBack)
                    {
                        return;
                    }

                    int enabledWeaponCount = Utilities.GetReadiedWeaponCount(___HUD.SelectedActor.Weapons, ___HUD.SelectedTarget);
                    Pilot selectedPilot = ___HUD.SelectedActor.GetPilot();
                    int maxAllowedWeapons = Utilities.GetMaxAllowedWeaponCountForHeadshots(selectedPilot);
                    bool validWeaponCountForPrecisionStrike = enabledWeaponCount <= maxAllowedWeapons ? true : false;

                    bool headCanBeTargeted = ___HUD.SelectedTarget.IsShutDown || ___HUD.SelectedTarget.IsProne || validWeaponCountForPrecisionStrike;

                    if (!headCanBeTargeted)
                    {
                        Color cOutline = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.darkGray;
                        Color cFill = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.darkGray;
                        UIHelpers.SetImageColor(__instance.MechArmorDisplay.ArmorOutline[0], cOutline);
                        UIHelpers.SetImageColor(__instance.MechArmorDisplay.Armor[0], cFill);
                        UIHelpers.SetImageColor(__instance.MechArmorDisplay.Structure[0], cFill);

                        __instance.ShownTargetReticles[0].SetActive(false);
                    }
                    else
                    {
                        if (___displayedActor.UnitType == UnitType.Mech)
                        {
                            __instance.MechArmorDisplay.UpdateMechStructureAndArmor(___shownAttackDirection);
                        }
                        __instance.ShownTargetReticles[0].SetActive(true);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(CombatHUD), "OnWeaponModified")]
        public static class CombatHUD_OnWeaponModified_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.SingleWeaponHeadshots;
            }

            public static void Postfix(CombatHUD __instance)
            {
                try
                {
                    // Only relevant for Precision Strike
                    if (__instance.SelectionHandler.ActiveState == null || __instance.SelectionHandler.ActiveState.SelectionType != SelectionType.FireMorale)
                    {
                        return;
                    }
                    // No headshots from behind
                    if (__instance.CalledShotPopUp.ShownAttackDirection == AttackDirection.FromBack)
                    {
                        return;
                    }

                    Logger.LogLine("[CombatHUD_OnWeaponModified_POSTFIX] Clear confirmed called headshot if enabled weapons change to an invalid amount...");

                    bool isCalledShotPopupVisible = __instance.CalledShotPopUp.Visible;
                    Logger.LogLine("[CombatHUD_OnWeaponModified_POSTFIX] isCalledShotPopupVisible: " + isCalledShotPopupVisible);

                    int enabledWeaponCount = Utilities.GetReadiedWeaponCount(__instance.SelectedActor.Weapons, __instance.SelectedTarget, true);
                    Pilot selectedPilot = __instance.SelectedActor.GetPilot();
                    int maxAllowedWeapons = Utilities.GetMaxAllowedWeaponCountForHeadshots(selectedPilot);
                    bool validWeaponCountForPrecisionStrike = enabledWeaponCount <= maxAllowedWeapons ? true : false;
                    Logger.LogLine("[CombatHUD_OnWeaponModified_POSTFIX] enabledWeaponCount: " + enabledWeaponCount);
                    Logger.LogLine("[CombatHUD_OnWeaponModified_POSTFIX] validWeaponCountForPrecisionStrike: " + validWeaponCountForPrecisionStrike);

                    ArmorLocation baseCalledShotLocation = (ArmorLocation)typeof(SelectionStateFire).GetField("calledShotLocation", AccessTools.all).GetValue(__instance.SelectionHandler.ActiveState);
                    bool headIsTargeted = baseCalledShotLocation == ArmorLocation.Head;
                    Logger.LogLine("[CombatHUD_OnWeaponModified_POSTFIX] headIsTargeted: " + headIsTargeted);

                    //bool isMoraleAttack = __instance.SelectionHandler.ActiveState is SelectionStateMoraleAttack;
                    //Logger.LogLine("[CombatHUD_OnWeaponModified_POSTFIX] isMoraleAttack: " + isMoraleAttack);

                    bool shouldBackOut = headIsTargeted && !isCalledShotPopupVisible && !validWeaponCountForPrecisionStrike;
                    Logger.LogLine("[CombatHUD_OnWeaponModified_POSTFIX] shouldBackOut: " + shouldBackOut);

                    if (shouldBackOut) {
                        Logger.LogLine("[CombatHUD_OnWeaponModified_POSTFIX] " + __instance.SelectionHandler.ActiveState.ToString() + ".BackOut()");
                        __instance.SelectionHandler.ActiveState.BackOut();
                    }

                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(HUDMechArmorReadout), "SetHoveredArmor", new Type[] { typeof(ArmorLocation) })]
        public static class HUDMechArmorReadout_SetHoveredArmor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.SingleWeaponHeadshots;
            }

            public static void Postfix(HUDMechArmorReadout __instance, ArmorLocation location, Mech ___displayedMech)
            {
                try
                {
                    if (__instance.UseForCalledShots && location == ArmorLocation.Head)
                    {
                        Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] Clear head selection if too many weapons are enabled...");

                        int enabledWeaponCount = Utilities.GetReadiedWeaponCount(__instance.HUD.SelectedActor.Weapons, __instance.HUD.SelectedTarget);
                        Pilot selectedPilot = __instance.HUD.SelectedActor.GetPilot();
                        int maxAllowedWeapons = Utilities.GetMaxAllowedWeaponCountForHeadshots(selectedPilot);
                        bool validWeaponCountForPrecisionStrike = enabledWeaponCount <= maxAllowedWeapons ? true : false;

                        bool headCanBeTargeted = __instance.HUD.SelectedTarget.IsShutDown || __instance.HUD.SelectedTarget.IsProne || validWeaponCountForPrecisionStrike;

                        Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] __instance.HUD.SelectedActor: " + __instance.HUD.SelectedActor.DisplayName);
                        Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] enabledWeaponCount: " + enabledWeaponCount);
                        Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] validWeaponCountForPrecisionStrike: " + validWeaponCountForPrecisionStrike);

                        Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] ___displayedMech: " + ___displayedMech.DisplayName);
                        Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] ___displayedMech.IsShutDown: " + ___displayedMech.IsShutDown);
                        Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] ___displayedMech.IsProne: " + ___displayedMech.IsProne);
                        Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] headCanBeTargeted: " + headCanBeTargeted);

                        if (!headCanBeTargeted)
                        {
                            Logger.LogLine("[HUDMechArmorReadout_SetHoveredArmor_POSTFIX] Prevent targeting of head...");
                            __instance.ClearHoveredArmor(ArmorLocation.Head);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(SelectionStateFire), "SetCalledShot", new Type[] { typeof(ArmorLocation) })]
        public static class SelectionStateFire_SetCalledShot_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.SingleWeaponHeadshots;
            }

            public static void Postfix(SelectionStateFire __instance, ArmorLocation location)
            {
                try
                {
                    if (location == ArmorLocation.Head)
                    {
                        Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] Disable headshot if too many weapons are enabled...");

                        int enabledWeaponCount = Utilities.GetReadiedWeaponCount(__instance.SelectedActor.Weapons, __instance.TargetedCombatant);
                        Pilot selectedPilot = __instance.SelectedActor.GetPilot();
                        int maxAllowedWeapons = Utilities.GetMaxAllowedWeaponCountForHeadshots(selectedPilot);
                        bool validWeaponCountForPrecisionStrike = enabledWeaponCount <= maxAllowedWeapons ? true : false;

                        bool headCanBeTargeted = __instance.TargetedCombatant.IsShutDown || __instance.TargetedCombatant.IsProne || validWeaponCountForPrecisionStrike;

                        Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] __instance.SelectedActor: " + __instance.SelectedActor.DisplayName);
                        Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] enabledWeaponCount: " + enabledWeaponCount);
                        Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] validWeaponCountForPrecisionStrike: " + validWeaponCountForPrecisionStrike);

                        Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] __instance.TargetedCombatant.DisplayName: " + __instance.TargetedCombatant.DisplayName);
                        Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] __instance.TargetedCombatant.IsShutDown: " + __instance.TargetedCombatant.IsShutDown);
                        Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] __instance.TargetedCombatant.IsProne: " + __instance.TargetedCombatant.IsProne);
                        Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] headCanBeTargeted: " + headCanBeTargeted);

                        if (!headCanBeTargeted)
                        {
                            Logger.LogLine("[SelectionStateFire_SetCalledShot_POSTFIX] Disabling headshot...");
                            Traverse.Create(__instance).Method("ClearCalledShot").GetValue();
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
    }
}
