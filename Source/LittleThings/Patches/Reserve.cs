using System;
using Harmony;

namespace LittleThings.Patches
{
    class Reserve
    {
        // Enable Reserve for AI
        [HarmonyPatch(typeof(BehaviorTree), "GetBehaviorVariableValue")]
        public static class BehaviorTree_GetBehaviorVariableValue_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.ReserveEnable;
            }

            public static void Postfix(BehaviorTree __instance, ref BehaviorVariableValue __result, BehaviorVariableName name)
            {
                try
                {
                    if (name == BehaviorVariableName.Bool_ReserveEnabled)
                    {
                        Logger.Debug("[BehaviorTree_GetBehaviorVariableValue_POSTFIX] Overriding BehaviorVariableName.Bool_ReserveEnabled: true");
                        __result.BoolVal = true;
                    }
                    else if (name == BehaviorVariableName.Float_ReserveBasePercentage)
                    {
                        Logger.Debug("[BehaviorTree_GetBehaviorVariableValue_POSTFIX] Overriding BehaviorVariableName.Float_ReserveBasePercentage: " + LittleThings.Settings.ReserveBasePercentage);
                        __result.FloatVal = LittleThings.Settings.ReserveBasePercentage;
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
