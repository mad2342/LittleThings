namespace LittleThings
{
    internal class Settings
    {
        public bool MoraleFixes = true;
        public bool InjuryFixes = false;

        public bool FixUIInventoryItems = true;
        public bool FixUIHeraldryScreen = true;
        public bool FixUIEquipmentTooltips = true;
        public bool FixUIMainNavigation = true;

        public bool FixInterleavedDropouts = true;
        public bool FixInspire = true;
        public bool FixIconOverlayAAR = false;
        public bool FixCombatHUDPortraitRightClick = true;
        public bool FixCoilPreviews = true;
        public bool FixRepairNotification = true;
        public bool FixTaurianReputationPostCampaign = true;
        public bool FixStatTooltipFirepower = true;
        public bool FixStatTooltipDurability = true;
        public bool FixInitiativeFloatieForDeadActors = true;

        public bool DisableTutorials = true;
        public bool DisableSimGameCharHighlights = true;
        public bool DisableCareerModeScoring = true;
        public bool DisableHeavyMetalLootPopup = true;

        public bool EnableEnemyInjuryFloaties = true;
        public bool EnableSmallCombatFloaties = true;
        public bool EnableAbilityTooltips = true;
        public bool EnableStockMechReferenceViaMechDefDescriptionModel = true;
        public bool EnableChassisHeatsinks = false;
        public bool EnableSpawnProtection = false;
        public bool EnableLanceConfigurationByTags = true;
        public bool EnableContractsTakeTime = false;
        public bool EnableDFAsRemoveEntrenched = true;
        public bool EnableRepeatableHeavyMetalCampaign = false;

        public bool EnableAIReserve = false;
        public float EnableAIReserveBasePercentage = 25f;

        public bool EnableAdjustedMechPartCost = true;
        public double EnableAdjustedMechPartCostMultiplier = 0.8;

        public bool EnableTrainingNotification = true;
        public int EnableTrainingNotificationLimit = 4;

        public bool EnableAllianceFlashpoints = true;
        public int EnableAllianceFlashpointsAtReputation = 100;
    }
}
