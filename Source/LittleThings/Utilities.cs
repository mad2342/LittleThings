using System.Globalization;
using BattleTech;

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
    }
}