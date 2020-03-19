namespace LittleThings
{
    internal class Settings
    {
        public bool MoraleFixes = true;
        public bool FixUIInventoryItems = true;
        public bool FixUIHeraldryScreen = true;
        public bool FixUIEquipmentTooltips = true;
        public bool FixUIMainNavigation = true;
        public bool DisableTutorials = true;
        public bool DisableSimGameCharHighlights = true;
        public bool InjuryFixes = false;
        public bool FixInspire = true;
        public bool FixIconOverlayAAR = false;
        public bool FixCombatHUDPortraitRightClick = true;
        public bool FixCoilPreviews = true;
        public bool FixRepairNotification = true;
        public bool FixTaurianReputationPostCampaign = true;
        public bool FixStatTooltipFirepower = true;
        public int TrainingNotificationLimit = 4;

        public bool FixGalaxyDeleteSaves = false;
        public string GalaxySaveGameRelativeRootPath = "AppData\\Local\\GOG.com\\Galaxy\\Applications\\50593543263669699\\Storage\\Shared\\Files\\";
        public string GalaxySaveGameAbsolutePathOverride = null;

        public bool ShowEnemyInjuries = true;
        public bool ShowAbilityTooltips = true;
        public bool ContractsTakeTime = false;

        public bool EnableStockMechReferenceViaMechDefDescriptionModel = true;
        public bool EnableChassisHeatsinks = false;

        public bool EnableSpawnProtection = false;

        public bool EnableLanceConfigurationByTags = true;

        public bool AdjustMechPartCost = true;
        public double AdjustMechPartCostMultiplier = 0.7;

        public bool ReserveEnable = true;
        public float ReserveBasePercentage = 25f;

        public bool DFAsRemoveEntrenched = true;
    }
}
