using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using Localize;

namespace LittleThings.Patches
{
    [HarmonyPatch(typeof(CombatHUDFloatieAnchor), "CreateFloatie")]
    public static class CombatHUDFloatieAnchor_CreateFloatie_Patch
    {
        public static bool Prepare()
        {
            return LittleThings.Settings.SmallCombatFloaties;
        }

        public static void Prefix(CombatHUDFloatieAnchor __instance, Text text,  float fontSize, CombatGameState ___Combat)
        {
            try
            {
                if (fontSize > ___Combat.Constants.CombatUIConstants.floatieSizeSmall)
                {
                    Logger.Debug($"[CombatHUDFloatieAnchor_CreateFloatie_PREFIX] Decreasing floatie font size...");
                    Logger.Info($"[CombatHUDFloatieAnchor_CreateFloatie_PREFIX] text: {text}");
                    fontSize = ___Combat.Constants.CombatUIConstants.floatieSizeSmall;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
