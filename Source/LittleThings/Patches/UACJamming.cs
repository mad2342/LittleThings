using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using Localize;

namespace LittleThings.Patches
{
    class UACJamming
    {
        internal const float unjammingChanceBase = 0.82f;
        internal static float unjammingChance = 0.82f;

        public static bool IsUltraAutocannon(Weapon weapon)
        {
            return weapon.WeaponSubType == WeaponSubType.UAC2 || weapon.WeaponSubType == WeaponSubType.UAC5 || weapon.WeaponSubType == WeaponSubType.UAC10 || weapon.WeaponSubType == WeaponSubType.UAC20;
        }

        public static bool IsJammed(Weapon weapon)
        { 
            //return weapon.StatCollection.GetStatistic("Jammed").Value<bool>();
            
            Statistic statistic = Utilities.GetOrCreateStatistic<bool>(weapon.StatCollection, "Jammed", false);
            return statistic.Value<bool>();
        }



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



        [HarmonyPatch(typeof(AttackDirector), "CreateAttackSequence")]
        public static class AttackDirector_CreateAttackSequence_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableUACJamming;
            }

            public static void Prefix(AttackDirector __instance, AttackDirector.AttackSequence __result, AbstractActor attacker, ref List<Weapon> selectedWeapons)
            {
                try
                {
                    for (int i = selectedWeapons.Count - 1; i >= 0; i--)
                    {
                        Weapon weapon = selectedWeapons[i];

                        if (IsUltraAutocannon(weapon))
                        {
                            Logger.Info($"[AttackDirector_CreateAttackSequence_PREFIX] {attacker.DisplayName} tries to fire an UAC");

                            float jammingChance = 0.49f;
                            float jammingRoll = UnityEngine.Random.Range(0f, 1f);

                            if (jammingRoll <= jammingChance)
                            {
                                Logger.Info($"[AttackDirector_CreateAttackSequence_PREFIX] {weapon.UIName} jammed");

                                weapon.StatCollection.Set<bool>("Jammed", true);
                                weapon.StatCollection.Set<bool>("TemporarilyDisabled", true);

                                // Remove from selected weapons
                                selectedWeapons.RemoveAt(i);

                                __instance.Combat.MessageCenter.PublishMessage(new FloatieMessage(attacker.GUID, attacker.GUID, "UAC JAMMED", FloatieMessage.MessageNature.Debuff));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            public static void Postfix(AttackDirector __instance, AttackDirector.AttackSequence __result, AbstractActor attacker, List<Weapon> selectedWeapons)
            {
                try
                {
                    if (selectedWeapons.Count < 1)
                    {
                        AttackSequenceEndMessage m = new AttackSequenceEndMessage(__result.stackItemUID, __result.id);
                        //attackSequence.chosenTarget.ResolveAttackSequence(attackSequence.attacker.GUID, sequenceId, attackSequence.stackItemUID, this.Combat.HitLocation.GetAttackDirection(attackSequence.attackPosition, attackSequence.chosenTarget));
                        __instance.Combat.MessageCenter.PublishMessage(m);
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
                    Logger.Info($"[AbstractActor_OnActivationEnd_PREFIX] {__instance.DisplayName} HasFiredThisRound: {__instance.HasFiredThisRound}");

                    if (!__instance.HasFiredThisRound)
                    {
                        unjammingChance += 0.9f;
                    }
                    else
                    {
                        unjammingChance = unjammingChanceBase;
                    }
                    Logger.Info($"[AbstractActor_OnActivationEnd_PREFIX] {__instance.DisplayName} unjammingChance: {unjammingChance}");
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
                    Logger.Info($"[AbstractActor_OnNewRound_PREFIX] {__instance.DisplayName}");

                    foreach (Weapon weapon in __instance.Weapons)
                    {
                        if (IsUltraAutocannon(weapon) && IsJammed(weapon))
                        {
                            Logger.Info($"[AbstractActor_OnNewRound_PREFIX] {__instance.DisplayName} tries to unjam an UAC");

                            float unjammingRoll = UnityEngine.Random.Range(0f, 1f);
                            Logger.Info($"[AbstractActor_OnNewRound_PREFIX] Rolled {unjammingRoll} against {unjammingChance}");

                            if (unjammingRoll <= unjammingChance)
                            {
                                Logger.Info($"[AbstractActor_OnNewRound_PREFIX] {weapon.UIName} unjammed");

                                __instance.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.GUID, __instance.GUID, "UAC UNJAMMED", FloatieMessage.MessageNature.Buff));

                                weapon.StatCollection.Set<bool>("Jammed", false);
                                weapon.StatCollection.Set<bool>("TemporarilyDisabled", false);
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
                        __result.Append(" (JAM)", new object[0]);
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
