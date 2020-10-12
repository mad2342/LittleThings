using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using Localize;
using UnityEngine;

namespace LittleThings.Patches
{
    class UACJamming
    {
        // Represents a 2 or 3 on a 2d6 roll
        internal const float jammingChanceBase = 0.0833f;

        // Helper
        public static bool IsUltraAutocannon(Weapon weapon)
        {
            return weapon.WeaponSubType == WeaponSubType.UAC2 || weapon.WeaponSubType == WeaponSubType.UAC5 || weapon.WeaponSubType == WeaponSubType.UAC10 || weapon.WeaponSubType == WeaponSubType.UAC20;
        }

        public static int GetCaliber(Weapon weapon)
        {
            if (weapon.WeaponSubType == WeaponSubType.UAC2)
            {
                return 2;
            }
            else if (weapon.WeaponSubType == WeaponSubType.UAC5)
            {
                return 5;
            }
            else if (weapon.WeaponSubType == WeaponSubType.UAC10)
            {
                return 10;
            }
            else if (weapon.WeaponSubType == WeaponSubType.UAC20)
            {
                return 20;
            }
            else
            {
                return -1;
            }
        }

        public static bool IsJammed(Weapon weapon)
        { 
            Statistic statistic = Utilities.GetOrCreateStatistic<bool>(weapon.StatCollection, "Jammed", false);
            return statistic.Value<bool>();
        }



        // Patches
        [HarmonyPatch(typeof(Weapon), "InitStats")]
        public static class Weapon_InitStats_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableUACJamming;
            }

            public static void Postfix(Weapon __instance, ref StatCollection ___statCollection)
            {
                try
                {
                    if (IsUltraAutocannon(__instance))
                    {
                        ___statCollection.AddStatistic<bool>("Jammed", false);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(AbstractActor), "OnActivationEnd")]
        public static class AbstractActor_OnActivationEnd_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableUACJamming;
            }

            public static void Prefix(AbstractActor __instance)
            {
                try
                {
                    //Logger.Info($"[AbstractActor_OnActivationEnd_PREFIX] {__instance.DisplayName}");

                    foreach (Weapon weapon in __instance.Weapons)
                    {
                        if (IsUltraAutocannon(weapon))
                        {
                            // Already jammed
                            if (IsJammed(weapon))
                            {
                                Logger.Info($"[AbstractActor_OnActivationEnd_PREFIX] {weapon.Name} is jammed");
                            }
                            // Fired this round
                            else if (weapon.roundsSinceLastFire == 0)
                            {
                                Logger.Info($"[AbstractActor_OnActivationEnd_PREFIX] {weapon.Name} was fired this round");
                                Logger.Info($"[AbstractActor_OnActivationEnd_PREFIX] {weapon.Name} is checked for a potential jam");

                                float jammingChance = jammingChanceBase;
                                float jammingRoll = UnityEngine.Random.Range(0f, 1f);
                                Logger.Info($"[AbstractActor_OnActivationEnd_PREFIX] Rolled {jammingRoll} against {jammingChance}");

                                if (jammingRoll <= jammingChance)
                                {
                                    Logger.Info($"[AbstractActor_OnActivationEnd_PREFIX] {weapon.Name} got jammed");

                                    weapon.StatCollection.Set<bool>("Jammed", true);
                                    weapon.StatCollection.Set<bool>("TemporarilyDisabled", true);

                                    __instance.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.GUID, __instance.GUID, "UAC JAMMED", FloatieMessage.MessageNature.Debuff));

                                    AudioEventManager.SetPilotVOSwitch<AudioSwitch_dialog_dark_light>(AudioSwitch_dialog_dark_light.dark, __instance);
                                    AudioEventManager.PlayPilotVO(VOEvents.TakeDamage_WeaponLost, __instance, null, null, true);
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



        [HarmonyPatch(typeof(AbstractActor), "OnNewRound")]
        public static class AbstractActor_OnNewRound_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableUACJamming;
            }

            public static void Prefix(AbstractActor __instance)
            {
                try
                {
                    //Logger.Info($"[AbstractActor_OnNewRound_PREFIX] {__instance.DisplayName}");

                    if (__instance.IsShutDown || __instance.IsProne || __instance.IsDead)
                    {
                        return;
                    }

                    foreach (Weapon weapon in __instance.Weapons)
                    {
                        if (IsUltraAutocannon(weapon) && IsJammed(weapon))
                        {
                            Logger.Info($"[AbstractActor_OnNewRound_PREFIX] {__instance.DisplayName} tries to unjam an {weapon.Name}");
                            Logger.Info($"[AbstractActor_OnNewRound_PREFIX] {weapon.Name} wasn't fired for {weapon.roundsSinceLastFire} rounds");

                            //float unjammingBase = jammingChanceBase;
                            float unjammingBase = weapon.roundsSinceLastFire > 0 ? jammingChanceBase : 0.0f; // Jammed for at least 1 turn!
                            float unjammingModifier = weapon.roundsSinceLastFire * ((float)__instance.GetPilot().Gunnery / 10);
                            float unjammingChance = Mathf.Min(1f, unjammingBase + unjammingModifier);
                            float unjammingRoll = UnityEngine.Random.Range(0f, 1f);
                            Logger.Info($"[AbstractActor_OnNewRound_PREFIX] Rolled {unjammingRoll} against {unjammingChance} ({unjammingBase} + {unjammingModifier})");

                            if (unjammingRoll <= unjammingChance)
                            {
                                Logger.Info($"[AbstractActor_OnNewRound_PREFIX] {weapon.Name} was unjammed");

                                weapon.StatCollection.Set<bool>("Jammed", false);
                                weapon.StatCollection.Set<bool>("TemporarilyDisabled", false);

                                __instance.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.GUID, __instance.GUID, "UAC UNJAMMED", FloatieMessage.MessageNature.Buff));

                                //AudioEventManager.SetPilotVOSwitch<AudioSwitch_dialog_dark_light>(AudioSwitch_dialog_dark_light.dark, __instance);
                                //AudioEventManager.PlayPilotVO(VOEvents.Resupply_Support_Used, __instance, null, null, true);
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



        [HarmonyPatch(typeof(MechComponent), "UIName", MethodType.Getter)]
        public static class MechComponent_UIName_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableUACJamming;
            }

            public static void Postfix(MechComponent __instance, ref Text __result)
            {
                try
                {
                    if (!__instance.IsFunctional || __instance.GetType() != typeof(Weapon))
                    {
                        return;
                    }

                    Weapon weapon = (Weapon)__instance;
                    if (IsJammed(weapon))
                    {
                        string originalUIName = __result.ToString();
                        Color color = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.orangeHalf;

                        __result = new Localize.Text($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{originalUIName}</color>", new object[] { });

                        //string weaponCaliber = new String(originalUIName.Where(Char.IsDigit).ToArray());
                        //__result = new Localize.Text($"JAMMED <size=75%>(UAC/{weaponCaliber})</size>", new object[] { });
                        //__result.Append(" <size=75%>( JAMMED )</size>", new object[0]);
                        //__result = new Localize.Text("UAC JAMMED", new object[] { });
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(CombatHUDWeaponSlot), "RefreshDisplayedWeapon")]
        public static class CombatHUDWeaponSlot_RefreshDisplayedWeapon_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableUACJamming;
            }

            public static void Postfix(CombatHUDWeaponSlot __instance, Weapon ___displayedWeapon)
            {
                try
                {
                    if (___displayedWeapon == null || !___displayedWeapon.IsFunctional)
                    {
                        return;
                    }

                    if (IsJammed(___displayedWeapon))
                    {
                        string weaponCaliber = GetCaliber(___displayedWeapon).ToString();

                        Text jammedUIName = new Text($"JAMMED <size=75%>(UAC/{weaponCaliber})</size>", new object[] { });
                        __instance.WeaponText.SetText(jammedUIName);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(CombatHUDWeaponSlot), "UpdateToolTipsSelf")]
        public static class CombatHUDWeaponSlot_UpdateToolTipsSelf_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableUACJamming;
            }

            public static void Postfix(CombatHUDWeaponSlot __instance)
            {
                try
                {
                    Weapon weapon = __instance.DisplayedWeapon;

                    if (IsUltraAutocannon(weapon) && IsJammed(weapon))
                    {
                        //Logger.Info($"[CombatHUDWeaponSlot_UpdateToolTipsSelf_POSTFIX] Adding tooltip details for jammed UAC");

                        __instance.ToolTipHoverElement.DebuffStrings.Add(new Text("JAMMED", new object[]{ }));
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
