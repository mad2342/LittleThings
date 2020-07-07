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
            return LittleThings.Settings.EnableSmallCombatFloaties;
        }

        public static void Prefix(CombatHUDFloatieAnchor __instance, ref float fontSize, CombatGameState ___Combat)
        {
            try
            {
                fontSize = ___Combat.Constants.CombatUIConstants.floatieSizeSmall;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
