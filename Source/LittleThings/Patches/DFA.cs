using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    [HarmonyPatch(typeof(MechDFASequence), "OnMeleeComplete")]
    public static class MechDFASequence_OnMeleeComplete_Patch
    {
        /*
        public static bool Prepare()
        {
            return LittleThings.Settings.DFAsRemoveEntrenched;
        }
        */

        public static void Prefix(MechDFASequence __instance)
        {
            // Get melee target
            ICombatant DFATarget = (ICombatant)AccessTools.Property(typeof(MechDFASequence), "DFATarget").GetValue(__instance, null);

            if (DFATarget is Mech TargetMech)
            {
                // Remove Entrenched
                if (TargetMech.IsEntrenched)
                {
                    Logger.LogLine("[MechDFASequence_OnMeleeComplete_PREFIX] Removing Entrenched from target");
                    TargetMech.IsEntrenched = false;
                    TargetMech.Combat.MessageCenter.PublishMessage(new FloatieMessage(TargetMech.GUID, TargetMech.GUID, "LOST: ENTRENCHED", FloatieMessage.MessageNature.Debuff));
                }

                // Additional stability damage?
                /*
                float additionalStabilityDamage = __instance.OwningMech.MechDef.Chassis.MeleeInstability / 2;

                Logger.LogLine("[MechDFASequence_OnMeleeComplete_PREFIX] Apply additional stability damage from charging (50% of OwningMech.MechDef.Chassis.MeleeInstability): " + additionalStabilityDamage);
                TargetMech.AddAbsoluteInstability(additionalStabilityDamage, StabilityChangeSource.NotSet, __instance.owningActor.GUID);
                */
            }
        }
    }
}
