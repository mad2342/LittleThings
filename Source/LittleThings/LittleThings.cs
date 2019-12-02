using Harmony;
using System.Reflection;
using System;
using Newtonsoft.Json;
using System.IO;
using BattleTech;

namespace LittleThings
{
    public static class LittleThings
    {
        internal static string LogPath;
        internal static string ModDirectory;
        internal static Settings Settings;

        // BEN: Debug (0: nothing, 1: errors, 2:all)
        internal static int DebugLevel = 2;

        public static void Init(string directory, string settings)
        {
            ModDirectory = directory;

            LogPath = Path.Combine(ModDirectory, "LittleThings.log");
            File.CreateText(LittleThings.LogPath);

            try
            {
                Settings = JsonConvert.DeserializeObject<Settings>(settings);
            }
            catch (Exception)
            {
                Settings = new Settings();
            }

            // Harmony calls need to go last here because their Prepare() methods directly check Settings...
            HarmonyInstance harmony = HarmonyInstance.Create("de.mad.LittleThings");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(SimGameState), "_OnAttachUXComplete")]
    public static class SimGameState__OnAttachUXComplete_AddInventory
    {
        public static bool Prepare()
        {
            return LittleThings.Settings.AddInventory;
        }

        public static void Postfix(SimGameState __instance, StatCollection ___companyStats)
        {
            string[] MechsToAdd = new string[] { "mechdef_warhammer_WHM-6Rb", "mechdef_locust_LCT-1Vb_DAMAGED" };
            string[] WeaponsToAdd = new string[] { "Weapon_PPC_PPCER_0-STOCK", "Weapon_Laser_SmallLaserPulse_0-STOCK" };
            string[] UpgradesToAdd = new string[] { "Gear_Cockpit_StarCorps_Advanced" };
            string[] HeatsinksToAdd = new string[] { "Gear_HeatSink_Generic_Double" };
            string[] AmmoToAdd = new string[] { "Ammo_AmmunitionBox_Generic_GAUSS" };
            int fundsToAdd = 1000000;
            int amount = 15;

            foreach (string Id in MechsToAdd)
            {
                __instance.AddMechByID(Id, true);
            }
            foreach (string Id in WeaponsToAdd)
            {
                int num = amount;
                int i = 0;
                while (i < num)
                {
                    __instance.AddItemStat(Id, typeof(WeaponDef), false);
                    i++;
                }
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + Id + "(" + num + ") to inventory.");
            }
            foreach (string Id in UpgradesToAdd)
            {
                int num = amount;
                int i = 0;
                while (i < num)
                {
                    __instance.AddItemStat(Id, typeof(UpgradeDef), false);
                    i++;
                }
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + Id + "(" + num + ") to inventory.");
            }
            foreach (string Id in HeatsinksToAdd)
            {
                int num = amount;
                int i = 0;
                while (i < num)
                {
                    __instance.AddItemStat(Id, typeof(HeatSinkDef), false);
                    i++;
                }
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + Id + "(" + num + ") to inventory.");
            }
            foreach (string Id in AmmoToAdd)
            {
                int num = amount;
                int i = 0;
                while (i < num)
                {
                    __instance.AddItemStat(Id, typeof(AmmunitionBoxDef), false);
                    i++;
                }
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + Id + "(" + num + ") to inventory.");
            }

            // Funds
            __instance.AddFunds(fundsToAdd, null, true);
        }
    }
}
