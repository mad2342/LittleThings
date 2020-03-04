using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace LittleThings.Patches
{
    class Camera
    {
        internal static Vector3 lastGroundPos;
        internal static Vector3 lastCameraPos;
        internal static Quaternion lastCameraRot;

        internal static List<string> camEventFullList = new List<string>()
        {
            "BattleTech.ActiveProbeSequence.FireWave",
            "BattleTech.ActorMovementSequence.OnBlipAcquired",
            "BattleTech.ActorMovementSequence.OnPlayerVisChanged",
            "BattleTech.ActorMovementSequence.ShowCamera",
            "BattleTech.ArtilleryObjectiveSequence.SetState",
            "BattleTech.ArtillerySequence.SetState",
            "BattleTech.AttackStackSequence.OnActorDestroyed",
            "BattleTech.AttackStackSequence.OnAttackBegin",
            "BattleTech.AttackStackSequence.OnChildSequenceAdded",
            "BattleTech.MechDFASequence.BuildMeleeDirectorSequence",
            "BattleTech.MechDFASequence.BuildWeaponDirectorSequence",
            "BattleTech.MechDFASequence.OnAdded",
            "BattleTech.MechDisplacementSequence.ShowCamera",
            "BattleTech.MechJumpSequence.ShowCamera",
            "BattleTech.MechMeleeSequence.BuildMeleeDirectorSequence",
            "BattleTech.MechMeleeSequence.BuildWeaponDirectorSequence",
            "BattleTech.MechMeleeSequence.FireWeaponsFinal",
            "BattleTech.MechMeleeSequence.OnAdded",
            "BattleTech.MechMortarSequence.SetState",
            "BattleTech.MechStandSequence.OnAdded",
            "BattleTech.MechStartupSequence.OnAdded",
            "BattleTech.MissionEndSequence.ShowCamera",
            "BattleTech.MultiSequence.FocusCamera",
            "BattleTech.SensorLockSequence.OnAdded",
            "BattleTech.ShowActorInfoSequence.OnChildSequenceAdded",
            "BattleTech.ShowActorInfoSequence.SetState",
            "BattleTech.StrafeSequence.SetState"
        };

        internal static List<string> camEventWhitelist = new List<string>()
        {
            "BattleTech.ArtilleryObjectiveSequence.SetState",
            "BattleTech.ArtillerySequence.SetState",
            "BattleTech.MechMortarSequence.SetState",
            "BattleTech.MissionEndSequence.ShowCamera",
            "BattleTech.MultiSequence.FocusCamera",
            "BattleTech.StrafeSequence.SetState"
        };



        // Focus on enemies standing up
        [HarmonyPatch(typeof(MechStandSequence), "OnAdded")]
        public static class MechStandSequence_OnAdded_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(MechStandSequence __instance)
            {
                try
                {
                    if (__instance.owningActor.team.IsLocalPlayer)
                    {
                        return;
                    }

                    Logger.Debug($"[MechStandSequence_OnAdded_POSTFIX] Focus on enemy standing up...");

                    CombatGameState ___combatGameState = (CombatGameState)AccessTools.Property(typeof(MechStandSequence), "Combat").GetValue(__instance, null);

                    if (___combatGameState.LocalPlayerTeam.VisibilityToTarget(__instance.owningActor) == VisibilityLevel.LOSFull)
                    {
                        CameraControl.Instance.SetMovingToGroundPos(__instance.owningActor.CurrentPosition, 0.95f);
                    } 
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Focus on enemies powering up
        [HarmonyPatch(typeof(MechStartupSequence), "OnAdded")]
        public static class MechStartupSequence_OnAdded_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(MechStartupSequence __instance)
            {
                try
                {
                    if (__instance.owningActor.team.IsLocalPlayer)
                    {
                        return;
                    }

                    Logger.Debug($"[MechStartupSequence_OnAdded_POSTFIX] Focus on enemy powering up...");

                    CombatGameState ___combatGameState = (CombatGameState)AccessTools.Property(typeof(MechStartupSequence), "Combat").GetValue(__instance, null);

                    if (___combatGameState.LocalPlayerTeam.VisibilityToTarget(__instance.owningActor) == VisibilityLevel.LOSFull)
                    {
                        CameraControl.Instance.SetMovingToGroundPos(__instance.owningActor.CurrentPosition, 0.95f);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Focus on enemy jumping
        [HarmonyPatch(typeof(MechJumpSequence), "OnAdded")]
        public static class MechJumpSequence_OnAdded_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(MechJumpSequence __instance)
            {
                try
                {
                    if (__instance.OwningMech.team.IsLocalPlayer)
                    {
                        return;
                    }

                    Logger.Debug($"[MechJumpSequence_OnAdded_POSTFIX] Focus on enemy jumping...");

                    CombatGameState ___combatGameState = (CombatGameState)AccessTools.Property(typeof(MechJumpSequence), "Combat").GetValue(__instance, null);

                    if (___combatGameState.LocalPlayerTeam.CanDetectPosition(__instance.OwningMech.CurrentPosition, __instance.OwningMech))
                    {
                        CameraControl.Instance.SetMovingToGroundPos(__instance.OwningMech.CurrentPosition, 0.95f);
                    }
                    else if (___combatGameState.LocalPlayerTeam.CanDetectPosition(__instance.FinalPos, __instance.OwningMech))
                    {
                        CameraControl.Instance.SetMovingToGroundPos(__instance.FinalPos, 0.95f);
                    }
                    else
                    {
                        // Nothing?
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Focus on enemy movement
        [HarmonyPatch(typeof(ActorMovementSequence), "OnAdded")]
        public static class ActorMovementSequence_OnAdded_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(ActorMovementSequence __instance)
            {
                try
                {
                    if (__instance.OwningActor.team.IsLocalPlayer)
                    {
                        return;
                    }

                    Logger.Debug($"[ActorMovementSequence_OnAdded_POSTFIX] Focus on enemy movement...");

                    CombatGameState ___combatGameState = (CombatGameState)AccessTools.Property(typeof(ActorMovementSequence), "Combat").GetValue(__instance, null);

                    if (___combatGameState.LocalPlayerTeam.CanDetectPosition(__instance.OwningActor.CurrentPosition, __instance.OwningActor))
                    {
                        CameraControl.Instance.SetMovingToGroundPos(__instance.OwningActor.CurrentPosition, 0.95f);
                    }
                    else if (___combatGameState.LocalPlayerTeam.CanDetectPosition(__instance.FinalPos, __instance.OwningActor))
                    {
                        CameraControl.Instance.SetMovingToGroundPos(__instance.FinalPos, 0.95f);
                    }
                    else
                    {
                        // Nothing?
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Focus on sensor lock target
        [HarmonyPatch(typeof(SensorLockSequence), "OnAdded")]
        public static class SensorLockSequence_OnAdded_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(SensorLockSequence __instance)
            {
                try
                {
                    if (__instance.owningActor.team.IsLocalPlayer)
                    {
                        return;
                    }

                    Logger.Debug($"[SensorLockSequence_OnAdded_POSTFIX] Focus on sensor lock target...");

                    CameraControl.Instance.SetMovingToGroundPos(__instance.Target.CurrentPosition, 0.95f);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Focus on melee target
        [HarmonyPatch(typeof(MechMeleeSequence), "OnAdded")]
        public static class MechMeleeSequence_OnAdded_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(MechMeleeSequence __instance)
            {
                try
                {
                    Logger.Debug($"[MechMeleeSequence_OnAdded_POSTFIX] Focus on melee target...");

                    CombatGameState ___combatGameState = (CombatGameState)AccessTools.Property(typeof(MechMeleeSequence), "Combat").GetValue(__instance, null);

                    if (
                        __instance.owningActor.TeamId == ___combatGameState.LocalPlayerTeamGuid ||
                        __instance.MeleeTarget.team.GUID == ___combatGameState.LocalPlayerTeamGuid ||
                        (___combatGameState.HostilityMatrix.IsLocalPlayerFriendly(__instance.owningActor.TeamId) || ___combatGameState.HostilityMatrix.IsLocalPlayerFriendly(__instance.MeleeTarget.team.GUID)) ||
                        ___combatGameState.LocalPlayerTeam.CanDetectPosition(__instance.MeleeTarget.CurrentPosition, __instance.owningActor)
                    )
                    {
                        CameraControl.Instance.SetMovingToGroundPos(__instance.MeleeTarget.CurrentPosition, 0.95f);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Focus on attacked target
        [HarmonyPatch(typeof(AttackStackSequence), "OnAttackBegin")]
        public static class AttackStackSequence_OnAttackBegin_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(AttackStackSequence __instance, MessageCenterMessage message)
            {
                try
                {
                    Logger.Debug($"[AttackStackSequence_OnAttackBegin_POSTFIX] Focus on attacked target...");

                    CombatGameState ___combatGameState = (CombatGameState)AccessTools.Property(typeof(AttackStackSequence), "Combat").GetValue(__instance, null);
                    AttackSequenceBeginMessage attackSequenceBeginMessage = message as AttackSequenceBeginMessage;
                    AttackDirector.AttackSequence attackSequence = ___combatGameState.AttackDirector.GetAttackSequence(attackSequenceBeginMessage.sequenceId);

                    if (attackSequence == null)
                    {
                        return;
                    }

                    if (attackSequence.stackItemUID == __instance.SequenceGUID && !__instance.hasOwningSequence)
                    {
                        CameraControl.Instance.SetMovingToGroundPos(attackSequence.chosenTarget.CurrentPosition, 0.95f);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Main suppression
        [HarmonyPatch(typeof(MultiSequence), "SetCamera")]
        public static class MultiSequence_SetCamera_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static bool Prefix(MultiSequence __instance)
            {
                try
                {
                    //Logger.Debug($"[MultiSequence_SetCamera_PREFIX] Suppressing camera focus...");

                    StackTrace stackTrace = new StackTrace();
                    for (var i = 0; i < stackTrace.FrameCount; i++)
                    {
                        MethodBase methodBase = stackTrace.GetFrame(i).GetMethod();
                        Type declaringType = methodBase.DeclaringType;
                        string caller = $"{declaringType}.{methodBase.Name}";

                        if (camEventFullList.Contains(caller))
                        {
                            Logger.Info($"[MultiSequence_SetCamera_PREFIX] SUPRESSED: {caller}");
                        }
                        
                        if (camEventWhitelist.Contains(caller))
                        {
                            Logger.Debug($"[MultiSequence_SetCamera_PREFIX] WHITELISTED: {caller}");
                            return true;
                        }
                    }

                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);

                    return true;
                }
            }
        }



        /*
        [HarmonyPatch(typeof(CombatSelectionHandler), "TrySelectTarget")]
        public static class CombatSelectionHandler_TrySelectTarget_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(CombatSelectionHandler __instance, ICombatant target)
            {
                try
                {
                    Logger.Debug($"[CombatSelectionHandler_TrySelectTarget_POSTFIX] Called");

                    CombatGameState ___combatGameState = (CombatGameState)AccessTools.Property(typeof(CombatSelectionHandler), "Combat").GetValue(__instance, null);
                    
                    if (target != null && target.team != ___combatGameState.LocalPlayerTeam && !target.IsDead && target != __instance.SelectedTarget)
                    {
                        CameraControl.Instance.SetMovingToGroundPos(target.CurrentPosition, 0.95f);
                    }
                    
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */



        /*
        // Don't touch players camera height
        [HarmonyPatch(typeof(CameraControl), "ForceMovingToGroundPos")]
        public static class CameraControl_ForceMovingToGroundPos_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(CameraControl __instance, Vector3 i_dest, ref Vector3 ___smoothToGroundPosCamDest, Transform ___cTrans)
            {
                try
                {
                    Logger.Debug($"[CameraControl_ForceMovingToGroundPos_POSTFIX] Don't touch players camera height...");

                    CombatGameState ___combatGameState = (CombatGameState)AccessTools.Property(typeof(CameraControl), "Combat").GetValue(__instance, null);

                    Logger.Debug($"[CameraControl_ForceMovingToGroundPos_POSTFIX] i_dest: {i_dest}");
                    Logger.Debug($"[CameraControl_ForceMovingToGroundPos_POSTFIX] ___cTrans.forward: {___cTrans.forward}");

                    Logger.Debug($"[CameraControl_ForceMovingToGroundPos_POSTFIX] ___cTrans.position.y: {___cTrans.position.y}");

                    float height = ((___combatGameState.Constants.CameraConstants.MinHeightAboveTerrain + ___combatGameState.Constants.CameraConstants.MaxHeightAboveTerrain) * 0.8f);
                    Logger.Debug($"[CameraControl_ForceMovingToGroundPos_POSTFIX] height: {height}");

                    Logger.Debug($"[CameraControl_ForceMovingToGroundPos_POSTFIX] __instance.CurrentHeight: {__instance.CurrentHeight}");

                    // Org
                    //___smoothToGroundPosCamDest = i_dest - ___cTrans.forward * ((___combatGameState.Constants.CameraConstants.MinHeightAboveTerrain + ___combatGameState.Constants.CameraConstants.MaxHeightAboveTerrain) * 0.8f);

                    ___smoothToGroundPosCamDest = i_dest - ___cTrans.forward * ___cTrans.position.y;

                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */



        /*
        [HarmonyPatch(typeof(AbstractActor), "OnActivationBegin")]
        public static class AbstractActor_OnActivationBegin_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(AbstractActor __instance)
            {
                try
                {
                    Logger.Debug($"[AbstractActor_OnActivationBegin_POSTFIX] Called");

                    if (__instance.team.IsLocalPlayer)
                    {
                        return;
                    }

                    if (__instance.Combat.LocalPlayerTeam.CanDetectPosition(__instance.CurrentPosition, __instance) && __instance.HasBegunActivation)
                    {
                        lastCameraPos = CameraControl.Instance.CameraPos;
                        lastCameraRot = CameraControl.Instance.CameraRot;
                        lastGroundPos = CameraControl.Instance.ScreenCenterToGroundPosition;
                        Logger.Debug($"[AbstractActor_OnActivationBegin_POSTFIX] lastCameraPos: {lastCameraPos}");
                        Logger.Debug($"[AbstractActor_OnActivationBegin_POSTFIX] lastCameraRot: {lastCameraRot}");
                        Logger.Debug($"[AbstractActor_OnActivationBegin_POSTFIX] lastGroundPos: {lastGroundPos}");

                        CameraControl.Instance.SetMovingToGroundPos(__instance.CurrentPosition, 0.95f);
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
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(AbstractActor __instance)
            {
                try
                {
                    Logger.Debug($"[AbstractActor_OnActivationEnd_POSTFIX] Called");

                    if (__instance.team.IsLocalPlayer)
                    {
                        return;
                    }

                    if (__instance.HasActivatedThisRound)
                    {
                        //CameraControl.Instance.SetMovingToGroundPos(lastGroundPos, 0.95f);
                    }

                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */



        /*
        [HarmonyPatch(typeof(CameraControl), "Update")]
        public static class CameraControl_Update_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(CameraControl __instance, DebugFlyCameraControl ___debugFlyCameraControl)
            {
                try
                {
                    Logger.Debug($"[CameraControl_Update_POSTFIX] Called");

                    if (__instance.IsInTutorialMode)
                    {
                        return;
                    }

                    ___debugFlyCameraControl.enabled = false;
                    Traverse UpdatePlayerControl = Traverse.Create(__instance).Method("UpdatePlayerControl", Array.Empty<object>());
                    UpdatePlayerControl.GetValue();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */



        /*
        [HarmonyPatch(typeof(CameraControl), "Init")]
        public static class CameraControl_Init_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableTotalCameraControl;
            }

            public static void Postfix(CameraControl __instance)
            {
                try
                {
                    Logger.Debug($"[CameraControl_Init_POSTFIX] Called");

                    if(__instance.IsInTutorialMode)
                    {
                        return;
                    }

                    __instance.DEBUG_TakeCompleteControl = true;

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
