using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace LittleThings.Patches
{
    class TutorialPopups
    {
        // Suppress tutorial slides for Raven and Three Years Later
        [HarmonyPatch(typeof(CombatHUD), "OnTriggerDialog")]
        public static class CombatHUD_OnTriggerDialog_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.DisableTutorials;
            }

            public static bool Prefix(CombatHUD __instance, MessageCenterMessage message)
            {
                try
                {
                    TriggerDialog triggerDialog = message as TriggerDialog;

                    if (triggerDialog == null)
                    {
                        return false;
                    }

                    if (
                        triggerDialog.DialogID == __instance.Combat.Constants.RavenTutorialDialogID ||
                        triggerDialog.DialogID == __instance.Combat.Constants.Story2TutorialSlides1ID ||
                        triggerDialog.DialogID == __instance.Combat.Constants.Story2TutorialSlides2ID ||
                        triggerDialog.DialogID == __instance.Combat.Constants.Story2TutorialSlides3ID ||
                        triggerDialog.DialogID == __instance.Combat.Constants.Story2TutorialSlides4ID 
                        )
                    {
                        Logger.LogLine("[CombatHUD_OnTriggerDialog_PREFIX] Supress tutorial: "+ triggerDialog.DialogID);
                        return false;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                    return true;
                }
            }
        }
    }
}
