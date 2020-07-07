using BattleTech;
using Harmony;

namespace LittleThings.Patches
{
    class DFA
    {
        // DFAs remove Entrenched
        [HarmonyPatch(typeof(MechDFASequence), "OnMeleeComplete")]
        public static class MechDFASequence_OnMeleeComplete_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableDFAsRemoveEntrenched;
            }

            public static void Prefix(MechDFASequence __instance, ref MessageCenterMessage message)
            {
                // Only remove entrenched if the attach actually did hit?
                /*
                AttackCompleteMessage attackCompleteMessage = (AttackCompleteMessage)message;
                if (attackCompleteMessage.attackSequence.attackCompletelyMissed)
                {
                    return;
                }
                */

                // Get melee target
                ICombatant DFATarget = (ICombatant)AccessTools.Property(typeof(MechDFASequence), "DFATarget").GetValue(__instance, null);

                if (DFATarget is Mech TargetMech)
                {
                    // Remove Entrenched
                    if (TargetMech.IsEntrenched)
                    {
                        Logger.Debug("[MechDFASequence_OnMeleeComplete_PREFIX] Removing Entrenched from target");
                        TargetMech.IsEntrenched = false;
                        TargetMech.Combat.MessageCenter.PublishMessage(new FloatieMessage(TargetMech.GUID, TargetMech.GUID, "LOST: ENTRENCHED", FloatieMessage.MessageNature.Debuff));
                    }
                    else
                    {
                        Logger.Debug("[MechDFASequence_OnMeleeComplete_PREFIX] Target wasn't entrenched");
                    }
                }
            }
        }
    }
}
