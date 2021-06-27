using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.Data;
using Harmony;
using UnityEngine;

namespace LittleThings.Patches
{
    class MechScaling
    {
        private static Dictionary<string, Vector3[]> losSourcePositions = new Dictionary<string, Vector3[]>();
        private static Dictionary<string, Vector3[]> losTargetPositions = new Dictionary<string, Vector3[]>();

        private static readonly float combatScaleDefault = 1.25f; // Vanilla default: 1.25f
        private static readonly float simGameScaleDefault = 1.0f; // Vanilla default: Vector3.one (1.0f)

        private static Dictionary<string, float> chassisScaleFactors = LittleThings.Settings.EnableMechScalingChassisScaleFactors;

        /*
        // 1.0f: 80%; 1.125f: 90%; 1.25f: 100% (default); 1.375f: 110%; 1.5f: 120%
        private static Dictionary<string, float> chassisScaleFactors = new Dictionary<string, float>()
        {
            { "chassisdef_rifleman_RFL-3N-2", 1.375f },
            { "chassisdef_marauder_MAD-5A", 1.375f }
        };
        */



        private static Vector3 GetScaleVector(float scale)
        {
            return new Vector3(scale, scale, scale);
        }

        private static Vector3 GetScaleVector(string chassisID, float defaultScale, bool isSimGame = false)
        {
            if (chassisScaleFactors.ContainsKey(chassisID))
            {
                // Sim
                if (isSimGame)
                {
                    // Calculate scaling for SimGame
                    float adjustedScale = chassisScaleFactors[chassisID] / combatScaleDefault;
                    Logger.Info($"[MechScaling_GetSizeMultiplier] ({chassisID}) adjustedScale: {adjustedScale}");

                    return GetScaleVector(adjustedScale);
                }

                // Combat
                return GetScaleVector(chassisScaleFactors[chassisID]);
            }

            // Fallback to internal defaults
            return GetScaleVector(defaultScale);
        }

        private static Vector3[] LOSSourcePositions(string identifier, Vector3[] originalSourcePositions, Vector3 scaleFactor)
        {
            if (losSourcePositions.ContainsKey(identifier))
            {
                return losSourcePositions[identifier];
            }

            Logger.Info($"[MechScaling_LOSSourcePositions] SourcePositions for {identifier}:");

            Vector3[] adjustedSourcePositions = new Vector3[originalSourcePositions.Length];
            for (var i = 0; i < originalSourcePositions.Length; i++)
            {
                adjustedSourcePositions[i] = Vector3.Scale(originalSourcePositions[i], scaleFactor);
                Logger.Info($"[MechScaling_LOSSourcePositions] {i} orig: [{originalSourcePositions[i].x},{originalSourcePositions[i].y},{originalSourcePositions[i].z}] | scaled: [{adjustedSourcePositions[i].x},{adjustedSourcePositions[i].y},{adjustedSourcePositions[i].z}]");
            }
            losSourcePositions[identifier] = adjustedSourcePositions;

            return losSourcePositions[identifier];
        }

        private static Vector3[] LOSTargetPositions(string identifier, Vector3[] originalTargetPositions, Vector3 scaleFactor)
        {
            if (losTargetPositions.ContainsKey(identifier))
            {
                return losTargetPositions[identifier];
            }

            Logger.Info($"[MechScaling_LOSTargetPositions] TargetPositions for {identifier}:");

            Vector3[] adjustedTargetPositions = new Vector3[originalTargetPositions.Length];
            for (var i = 0; i < originalTargetPositions.Length; i++)
            {
                adjustedTargetPositions[i] = Vector3.Scale(originalTargetPositions[i], scaleFactor);
                Logger.Info($"[MechScaling_LOSTargetPositions] {i} orig: [{originalTargetPositions[i].x},{originalTargetPositions[i].y},{originalTargetPositions[i].z}] | scaled: [{adjustedTargetPositions[i].x},{adjustedTargetPositions[i].y},{adjustedTargetPositions[i].z}]");
            }
            losTargetPositions[identifier] = adjustedTargetPositions;

            return losTargetPositions[identifier];
        }



        [HarmonyPatch(typeof(MechRepresentation), "Init", new Type[] { typeof(Mech), typeof(Transform), typeof(bool) })]
        public static class MechRepresentation_Init_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableMechScaling;
            }

            static void Postfix(MechRepresentation __instance, Mech mech)
            {
                if (!chassisScaleFactors.ContainsKey(mech.MechDef.ChassisID))
                {
                    return;
                }

                Logger.Debug($"[MechRepresentation_Init_POSTFIX] Rescaling...");

                string identifier = mech.MechDef.ChassisID;
                float fallbackScale = combatScaleDefault;

                Vector3 adjustedLocalScale = GetScaleVector(identifier, fallbackScale);
                Logger.Debug($"[MechRepresentation_Init_POSTFIX] {identifier}: {adjustedLocalScale}");

                Vector3[] originalLOSSourcePositions = Traverse.Create(mech).Field("originalLOSSourcePositions").GetValue<Vector3[]>();
                Vector3[] originalLOSTargetPositions = Traverse.Create(mech).Field("originalLOSTargetPositions").GetValue<Vector3[]>();
                Vector3[] adjustedSourcePositions = LOSSourcePositions(identifier, originalLOSSourcePositions, adjustedLocalScale);
                Vector3[] adjustedTargetPositions = LOSTargetPositions(identifier, originalLOSTargetPositions, adjustedLocalScale);

                Traverse.Create(mech).Field("originalLOSSourcePositions").SetValue(adjustedSourcePositions);
                Traverse.Create(mech).Field("originalLOSTargetPositions").SetValue(adjustedTargetPositions);
                Traverse.Create(__instance.thisTransform).Property("localScale").SetValue(adjustedLocalScale);
            }
        }

        [HarmonyPatch(typeof(MechRepresentationSimGame), "Init", new Type[] { typeof(DataManager), typeof(MechDef), typeof(Transform), typeof(HeraldryDef) })]
        public static class MechRepresentationSimGame_Init_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableMechScaling;
            }

            static void Postfix(MechRepresentationSimGame __instance, MechDef mechDef)
            {
                if (!chassisScaleFactors.ContainsKey(mechDef.ChassisID))
                {
                    return;
                }

                Logger.Debug($"[MechRepresentationSimGame_Init_POSTFIX] Rescaling...");

                string identifier = mechDef.ChassisID;
                float fallbackScale = simGameScaleDefault;

                Vector3 adjustedLocalScale = GetScaleVector(identifier, fallbackScale, true); 
                Logger.Debug($"[MechRepresentationSimGame_Init_POSTFIX] {identifier}: {adjustedLocalScale}");

                __instance.rootTransform.localScale = adjustedLocalScale;
            }
        }
    }
}
