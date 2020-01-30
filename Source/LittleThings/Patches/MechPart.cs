using System;
using BattleTech;
using Harmony;

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
                    // Info only
                    /*
                    if (item.Type == ShopItemType.Mech)
                    {
                        Logger.Debug("[Shop_GetItemDescription_POSTFIX] item.Type: " + item.Type.ToString());
                        Logger.Debug("[Shop_GetItemDescription_POSTFIX] item.ID: " + item.ID);
                        // BEN: Beware! Description.Cost gets calculated in MechDef.RefreshBattleValue called by MechDef.CopyFrom and possibly some MechDef constructors...
                        Logger.Debug("[Shop_GetItemDescription_POSTFIX] ___Sim.DataManager.MechDefs.Get(item.ID).Description.Id: " + ___Sim.DataManager.MechDefs.Get(item.ID).Description.Id);
                        Logger.Debug("[Shop_GetItemDescription_POSTFIX] ___Sim.DataManager.MechDefs.Get(item.ID).Description.Name: " + ___Sim.DataManager.MechDefs.Get(item.ID).Description.Name);
                        Logger.Debug("[Shop_GetItemDescription_POSTFIX] ___Sim.DataManager.MechDefs.Get(item.ID).Description.Cost: " + ___Sim.DataManager.MechDefs.Get(item.ID).Description.Cost);
                        Logger.Debug("[Shop_GetItemDescription_POSTFIX] __result.Cost: " + __result.Cost);

                        return;
                    }
                    */

                    if (item.Type != ShopItemType.MechPart)
                    {
                        return;
                    }

                    //Logger.Debug("[Shop_GetItemDescription_POSTFIX] __result.Cost BEFORE: " + __result.Cost);

                    MechDef mechDef = ___Sim.DataManager.MechDefs.Get(item.ID);
                    DescriptionDef description = mechDef.Description;
                    int defaultMechPartMax = ___Sim.Constants.Story.DefaultMechPartMax;
                    int AdjustedMechPartCost = (int)Math.Floor((mechDef.Chassis.Description.Cost / defaultMechPartMax) * LittleThings.Settings.AdjustMechPartCostMultiplier);

                    __result = new DescriptionDef(description.Id, string.Format("{0} Mech Part", description.Name), description.Details, description.Icon, AdjustedMechPartCost, description.Rarity, description.Purchasable, description.Manufacturer, description.Model, description.UIName);

                    /*
                    Logger.Debug("[Shop_GetItemDescription_POSTFIX] description.Name: " + description.Name);
                    Logger.Debug("[Shop_GetItemDescription_POSTFIX] mechDef.SimGameMechPartCost: " + mechDef.SimGameMechPartCost);
                    Logger.Debug("[Shop_GetItemDescription_POSTFIX] mechDef.Chassis.Description.Cost: " + mechDef.Chassis.Description.Cost);
                    Logger.Debug("[Shop_GetItemDescription_POSTFIX] ___Sim.Constants.Story.DefaultMechPartMax: " + ___Sim.Constants.Story.DefaultMechPartMax);
                    Logger.Debug("[Shop_GetItemDescription_POSTFIX] Recalculate MechPartCost (Chassis.Cost / PartsToAssemble) * Settings.AdjustMechPartCostMultiplier: (" + mechDef.Chassis.Description.Cost + " / " + defaultMechPartMax + ") * " + LittleThings.Settings.AdjustMechPartCostMultiplier);
                    Logger.Debug("[Shop_GetItemDescription_POSTFIX] AdjustedMechPartCost: " + AdjustedMechPartCost);
                    Logger.Debug("[Shop_GetItemDescription_POSTFIX] __result.Cost AFTER: " + __result.Cost);
                    Logger.Debug("---");
                    */
                }
                catch (Exception e)
                {
                    Logger.Error(e);
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
                    Logger.Debug("[MechDef_SimGameMechPartCost_POSTFIX] __result BEFORE: " + __result);

                    SimGameState simGameState = LazySingletonBehavior<UnityGameInstance>.Instance.Game.Simulation;
                    int defaultMechPartMax = simGameState.Constants.Story.DefaultMechPartMax;
                    Logger.Debug("[MechDef_SimGameMechPartCost_POSTFIX] simGameState.Constants.Story.DefaultMechPartMax: " + simGameState.Constants.Story.DefaultMechPartMax);
                    Logger.Debug("[MechDef_SimGameMechPartCost_POSTFIX] __instance.SimGameMechPartCost: " + __instance.SimGameMechPartCost);
                    Logger.Debug("[MechDef_SimGameMechPartCost_POSTFIX] __instance.Chassis.Description.Cost: " + __instance.Chassis.Description.Cost);

                    int AdjustedMechPartCost = (int)Math.Floor(__instance.Chassis.Description.Cost / defaultMechPartMax * 0.9);
                    Logger.Debug("[MechDef_SimGameMechPartCost_POSTFIX] AdjustedMechPartCost: " + AdjustedMechPartCost);

                    Logger.Debug("[MechDef_SimGameMechPartCost_POSTFIX] __result AFTER: " + __result);

                    __result = AdjustedMechPartCost;
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
