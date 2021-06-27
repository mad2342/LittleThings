using System.Globalization;
using System.Linq;
using BattleTech;
using BattleTech.Data;

namespace LittleThings
{
    public static class Utilities
    {
        public static Statistic GetOrCreateStatistic<StatisticType>(StatCollection collection, string statName, StatisticType defaultValue)
        {
            Statistic statistic = collection.GetStatistic(statName);

            if (statistic == null)
            {
                statistic = collection.AddStatistic<StatisticType>(statName, defaultValue);
            }
            return statistic;
        }



        // Rebuilt from StatTooltipData.GetEffectMod() to be used in the prefix patch of StatTooltipData.SetDurabilityData()
        public static float GetEffectMod(EffectData effectData, string statName, StatisticEffectData.TargetCollection target, StatCollection.StatOperation operation, WeaponSubType subTarget = WeaponSubType.NotSet)
        {
            StatisticEffectData statisticData = effectData.statisticData;
            if (statisticData != null && statisticData.statName == statName && statisticData.targetCollection == target && statisticData.operation == operation && statisticData.targetWeaponSubType == subTarget)
            {
                return float.Parse(statisticData.modValue, CultureInfo.InvariantCulture);
            }
            return 0f;
        }



        public static bool HasElectronicWarfareEquipment(MechDef mechDef)
        {
            // Could remove IsFixed condition to also mark Mechs with non-built-in EWE
            return mechDef.Inventory.Any(i => i.IsFixed && (i.Def.ComponentSubType == MechComponentType.Prototype_ElectronicWarfare || i.Def.ComponentSubType == MechComponentType.ElectronicWarfare));
        }



        public static bool IsStockMech(MechDef mechDef)
        {
            string currentMechDefId = mechDef.Description.Id;
            string stockMechDefId = mechDef.ChassisID.Replace("chassisdef", "mechdef");

            //DataManager dataManager = UnityGameInstance.BattleTechGame.DataManager;
            DataManager dataManager = mechDef.DataManager;
            MechDef stockMechDef = dataManager.MechDefs.Get(stockMechDefId);  

            // Armor
            bool isAssignedArmorEqual =
                mechDef.Head.AssignedArmor == stockMechDef.Head.AssignedArmor &&
                mechDef.CenterTorso.AssignedArmor == stockMechDef.CenterTorso.AssignedArmor &&
                mechDef.CenterTorso.AssignedRearArmor == stockMechDef.CenterTorso.AssignedRearArmor &&
                mechDef.LeftTorso.AssignedArmor == stockMechDef.LeftTorso.AssignedArmor &&
                mechDef.LeftTorso.AssignedRearArmor == stockMechDef.LeftTorso.AssignedRearArmor &&
                mechDef.RightTorso.AssignedArmor == stockMechDef.RightTorso.AssignedArmor &&
                mechDef.RightTorso.AssignedRearArmor == stockMechDef.RightTorso.AssignedRearArmor &&
                mechDef.LeftArm.AssignedArmor == stockMechDef.LeftArm.AssignedArmor &&
                mechDef.RightArm.AssignedArmor == stockMechDef.RightArm.AssignedArmor &&
                mechDef.LeftLeg.AssignedArmor == stockMechDef.LeftLeg.AssignedArmor &&
                mechDef.RightLeg.AssignedArmor == stockMechDef.RightLeg.AssignedArmor;

            //Logger.Info($"[Utilities.IsCustomized] isAssignedArmorEqual: {isAssignedArmorEqual}");
            if (!isAssignedArmorEqual)
            {
                return false;
            }

            // Inventory
            if (mechDef.Inventory.Length != stockMechDef.Inventory.Length)
            {
                return false;
            }

            MechComponentRef[] mechDefSortedInventory = mechDef.Inventory.OrderBy(i => i.ComponentDefID).ToArray();
            MechComponentRef[] stockMechDefSortedInventory = stockMechDef.Inventory.OrderBy(i => i.ComponentDefID).ToArray();

            for (int i = 0; i < mechDefSortedInventory.Length; i++)
            {
                //Logger.Info($"[Utilities.IsCustomized] {mechDefSortedInventory[i].ComponentDefID} : {stockMechDefSortedInventory[i].ComponentDefID}");
                if (mechDefSortedInventory[i].ComponentDefID != stockMechDefSortedInventory[i].ComponentDefID)
                {
                    return false;
                }
            }



            return true;
        }
    }
}