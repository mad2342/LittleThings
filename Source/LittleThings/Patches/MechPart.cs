using System;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using Harmony;
using HBS;
using HBS.Collections;

namespace LittleThings.Patches
{
    class MechPart
    {
        // Adjust SimGameMechPartCost depending on difficulty setting

        [HarmonyPatch(typeof(Shop), "GetItemDescription")]
        public static class Shop_GetItemDescription_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.AdjustMechPartCost;
            }

            public static void Postfix(Shop __instance, ref DescriptionDef __result, ShopDefItem item, SimGameState ___Sim)
            {
                try
                {
                    if (item.Type != ShopItemType.MechPart)
                    {
                        //Logger.LogLine("[Shop_GetItemDescription_POSTFIX] item.Type: " + item.Type.ToString());
                        return;
                    }
                    Logger.LogLine("[Shop_GetItemDescription_POSTFIX] __result.Cost BEFORE: " + __result.Cost);


                    MechDef mechDef = ___Sim.DataManager.MechDefs.Get(item.ID);
                    Logger.LogLine("[Shop_GetItemDescription_POSTFIX] mechDef.SimGameMechPartCost: " + mechDef.SimGameMechPartCost);
                    Logger.LogLine("[Shop_GetItemDescription_POSTFIX] mechDef.Chassis.Description.Cost: " + mechDef.Chassis.Description.Cost);

                    DescriptionDef description = mechDef.Description;
                    Logger.LogLine("[Shop_GetItemDescription_POSTFIX] description.Name: " + description.Name);
                    //result = new DescriptionDef(description.Id, string.Format("{0} Mech Part", description.Name), description.Details, description.Icon, mechDef.SimGameMechPartCost, description.Rarity, description.Purchasable, description.Manufacturer, description.Model, description.UIName);

                    int defaultMechPartMax = ___Sim.Constants.Story.DefaultMechPartMax;
                    Logger.LogLine("[Shop_GetItemDescription_POSTFIX] ___Sim.Constants.Story.DefaultMechPartMax: " + ___Sim.Constants.Story.DefaultMechPartMax);

                    Logger.LogLine("[Shop_GetItemDescription_POSTFIX] Recalculate MechPartCost: (Chassis.Cost / PartsToAssemble) * Settings.AdjustMechPartCostMultiplier");
                    int AdjustedMechPartCost = (int)Math.Floor( (mechDef.Chassis.Description.Cost / defaultMechPartMax) * LittleThings.Settings.AdjustMechPartCostMultiplier );
                    Logger.LogLine("[Shop_GetItemDescription_POSTFIX] AdjustedMechPartCost: " + AdjustedMechPartCost);


                    __result = new DescriptionDef(description.Id, string.Format("{0} Mech Part", description.Name), description.Details, description.Icon, AdjustedMechPartCost, description.Rarity, description.Purchasable, description.Manufacturer, description.Model, description.UIName);

                    Logger.LogLine("[Shop_GetItemDescription_POSTFIX] __result.Cost AFTER: " + __result.Cost);
                    Logger.LogLine("---");
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }



        // This is NEVER called. Don't understand why...
        /*
        [HarmonyPatch(typeof(MechDef), "SimGameMechPartCost", MethodType.Getter)]
        public static class MechDef_SimGameMechPartCost_Patch
        {
            public static void Postfix(MechDef __instance, ref int __result)
            {
                try
                {
                    Logger.LogLine("[MechDef_SimGameMechPartCost_POSTFIX] __result BEFORE: " + __result);

                    SimGameState simGameState = LazySingletonBehavior<UnityGameInstance>.Instance.Game.Simulation;
                    int defaultMechPartMax = simGameState.Constants.Story.DefaultMechPartMax;
                    Logger.LogLine("[MechDef_SimGameMechPartCost_POSTFIX] simGameState.Constants.Story.DefaultMechPartMax: " + simGameState.Constants.Story.DefaultMechPartMax);
                    Logger.LogLine("[MechDef_SimGameMechPartCost_POSTFIX] __instance.SimGameMechPartCost: " + __instance.SimGameMechPartCost);
                    Logger.LogLine("[MechDef_SimGameMechPartCost_POSTFIX] __instance.Chassis.Description.Cost: " + __instance.Chassis.Description.Cost);

                    int AdjustedMechPartCost = (int)Math.Floor(__instance.Chassis.Description.Cost / defaultMechPartMax * 0.9);
                    Logger.LogLine("[MechDef_SimGameMechPartCost_POSTFIX] AdjustedMechPartCost: " + AdjustedMechPartCost);

                    Logger.LogLine("[MechDef_SimGameMechPartCost_POSTFIX] __result AFTER: " + __result);

                    __result = AdjustedMechPartCost;
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
        */
    }
}
