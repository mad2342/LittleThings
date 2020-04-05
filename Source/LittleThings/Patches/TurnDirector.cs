using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    [HarmonyPatch(typeof(TurnDirector), "DoAnyUnitsHaveContactWithEnemy", MethodType.Getter)]
    public static class TurnDirector_DoAnyUnitsHaveContactWithEnemy_Patch
    {
        public static bool Prepare()
        {
            return LittleThings.Settings.FixInterleavedDropouts;
        }

        public static void Postfix(TurnDirector __instance, ref bool __result)
        {
            try
            {
                if (__result == true)
                {
                    return;
                }

                Logger.Debug("---");
                Logger.Debug($"[TurnDirector_DoAnyUnitsHaveContactWithEnemy_POSTFIX] Examine current result({__result}) and try to intercept interleaved drop out bug on escort missions...");

                /*
                // Debug
                StackTrace stackTrace = new StackTrace();
                for (var i = 0; i < stackTrace.FrameCount; i++)
                {
                    MethodBase methodBase = stackTrace.GetFrame(i).GetMethod();
                    Type declaringType = methodBase.DeclaringType;
                    Logger.Info($"[TurnDirector_DoAnyUnitsHaveContactWithEnemy_POSTFIX] StackTraceMethodName: {declaringType.Name}.{methodBase.Name}");
                }
                */


                bool HasAnyContactWithEnemy = false;
                bool HasAnyActorDetectedAnyEnemy = false;

                List<AbstractActor> allActors = __instance.Combat.AllActors;
                allActors.RemoveAll((AbstractActor x) => x.IsDead || x.IsFlaggedForDeath);
                for (int i = 0; i < allActors.Count; i++)
                {
                    //Logger.Info($"[TurnDirector_DoAnyUnitsHaveContactWithEnemy_POSTFIX] ({allActors[i].DisplayName}) HasAnyContactWithEnemy: {allActors[i].HasAnyContactWithEnemy}");
                    //Logger.Info($"[TurnDirector_DoAnyUnitsHaveContactWithEnemy_POSTFIX] ({allActors[i].DisplayName}) DetectedEnemyUnits: {allActors[i].GetDetectedEnemyUnits().Count}");

                    if (allActors[i].HasAnyContactWithEnemy)
                    {
                        HasAnyContactWithEnemy = true;
                    }

                    if (allActors[i].GetDetectedEnemyUnits().Count > 0)
                    {
                        HasAnyActorDetectedAnyEnemy = true;
                    }
                }
                Logger.Info($"[TurnDirector_DoAnyUnitsHaveContactWithEnemy_POSTFIX] HasAnyContactWithEnemy: {HasAnyContactWithEnemy}");
                Logger.Info($"[TurnDirector_DoAnyUnitsHaveContactWithEnemy_POSTFIX] HasAnyActorDetectedAnyEnemy: {HasAnyActorDetectedAnyEnemy}");

                // If there's a mismatch the VisibilityCache probably wasn't updated properly??
                // Nevertheless, this fixes the "Exit/Re-Enter Combat" scenario which happens in escort missions when the convoy is extracted.
                if (__result == false && HasAnyActorDetectedAnyEnemy == true)
                {
                    Logger.Debug($"[TurnDirector_DoAnyUnitsHaveContactWithEnemy_POSTFIX] MISMATCH! DoAnyUnitsHaveContactWithEnemy: {__result}, HasAnyActorDetectedAnyEnemy: {HasAnyActorDetectedAnyEnemy}");
                    Logger.Debug($"[TurnDirector_DoAnyUnitsHaveContactWithEnemy_POSTFIX] FORCING return value to true");
                    Logger.Debug("---");

                    __result = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }



    /*
    [HarmonyPatch(typeof(TurnDirector), "IncrementActiveTurnActor")]
    public static class TurnDirector_IncrementActiveTurnActor_Patch
    {
        public static bool Prefix(TurnDirector __instance)
        {  
            try
            {
                Logger.Debug("---");
                Logger.Debug("[TurnDirector_IncrementActiveTurnActor_PREFIX] CHECK Escort Missions...");

                Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] IsMissionOver: {__instance.IsMissionOver}");
                Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] IsInterleaved: {__instance.IsInterleaved}");
                Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] IsInterleavePending: {__instance.IsInterleavePending}");
                Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] Combat.EncounterLayerData.turnDirectorBehavior: {__instance.Combat.EncounterLayerData.turnDirectorBehavior}");
                Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] DoAnyUnitsHaveContactWithEnemy: {__instance.DoAnyUnitsHaveContactWithEnemy}");
                Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] ActiveTurnActorIndex: {__instance.ActiveTurnActorIndex}");
                Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] NumTurnActors: {__instance.NumTurnActors}");


                bool enemyContactsActive = false;

                List<AbstractActor> allActors = __instance.Combat.AllActors;
                allActors.RemoveAll((AbstractActor x) => x.IsDead || x.IsFlaggedForDeath);
                for (int i = 0; i < allActors.Count; i++)
                {
                    if (allActors[i].HasAnyContactWithEnemy)
                    {
                        Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] ({allActors[i].DisplayName}) HasAnyContactWithEnemy: {allActors[i].HasAnyContactWithEnemy}");
                    }

                    int allDetectedEnemiesCount = allActors[i].GetDetectedEnemyUnits().Count;
                    Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] ({allActors[i].DisplayName}) allDetectedEnemiesCount: {allDetectedEnemiesCount}");

                    if (allDetectedEnemiesCount > 0)
                    {
                        enemyContactsActive = true;
                    }
                }
                Logger.Info($"[TurnDirector_IncrementActiveTurnActor_PREFIX] enemyContactsActive: {enemyContactsActive}");



                // Copy of original method:
                if (__instance.IsMissionOver)
                {
                    return false;
                }
                if (!__instance.IsInterleaved && __instance.DoAnyUnitsHaveContactWithEnemy)
                {
                    //__instance.IsInterleaved = true;
                    new Traverse(__instance).Property("IsInterleaved").SetValue(true);
                    //__instance.IsInterleavePending = false;
                    new Traverse(__instance).Property("IsInterleavePending").SetValue(false);
                    __instance.Combat.MessageCenter.PublishMessage(new InterleaveChangedMessage());
                    //__instance.EndCurrentRound();
                    Traverse EndCurrentRound = Traverse.Create(__instance).Method("EndCurrentRound");
                    EndCurrentRound.GetValue();
                    return false;
                }

                //MAD: Add condition as for the moment a dropship despawns the convoy "DoAnyUnitsHaveContactWithEnemy" returns false even though that's not the case

                //if (__instance.IsInterleaved && __instance.Combat.EncounterLayerData.turnDirectorBehavior == TurnDirectorBehaviorType.Original && !__instance.DoAnyUnitsHaveContactWithEnemy)
                if (__instance.IsInterleaved && __instance.Combat.EncounterLayerData.turnDirectorBehavior == TurnDirectorBehaviorType.Original && !__instance.DoAnyUnitsHaveContactWithEnemy && !enemyContactsActive)
                {
                    //__instance.IsInterleaved = false;
                    new Traverse(__instance).Property("IsInterleaved").SetValue(false);
                    __instance.Combat.MessageCenter.PublishMessage(new InterleaveChangedMessage());
                    //__instance.EndCurrentRound();
                    Traverse EndCurrentRound = Traverse.Create(__instance).Method("EndCurrentRound");
                    EndCurrentRound.GetValue();
                    return false;
                }
                int num = __instance.ActiveTurnActorIndex;
                for (int i = 0; i < __instance.NumTurnActors; i++)
                {
                    num++;
                    if (num >= __instance.NumTurnActors)
                    {
                        num = 0;
                    }
                    if (__instance.TurnActors[num].GetNumUnusedUnitsForCurrentPhase() > 0)
                    {
                        //__instance.ActiveTurnActorIndex = num;
                        new Traverse(__instance).Property("ActiveTurnActorIndex").SetValue(num);
                        //__instance.SendTurnActorActivateMessage(__instance.ActiveTurnActorIndex);
                        Traverse SendTurnActorActivateMessage = Traverse.Create(__instance).Method("SendTurnActorActivateMessage", __instance.ActiveTurnActorIndex);
                        SendTurnActorActivateMessage.GetValue();

                        return false;
                    }
                }
                //__instance.EndCurrentPhase();
                Traverse EndCurrentPhase = Traverse.Create(__instance).Method("EndCurrentPhase");
                EndCurrentPhase.GetValue();

                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e);

                return true;
            } 
        }
    }
    */
}
