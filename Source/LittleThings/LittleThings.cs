using Harmony;
using System.Reflection;
using System;
using Newtonsoft.Json;
using System.IO;
using BattleTech;
using System.Collections.Generic;

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
            List<string> MechsToAdd = LittleThings.Settings.AddInventoryMechs;
            List<string> WeaponsToAdd = LittleThings.Settings.AddInventoryWeapons;
            List<string> UpgradesToAdd = LittleThings.Settings.AddInventoryUpgrades;
            List<string> HeatsinksToAdd = LittleThings.Settings.AddInventoryHeatsinks;
            List<string> AmmoToAdd = LittleThings.Settings.AddInventoryAmmo;
            int fundsToAdd = LittleThings.Settings.AddInventoryFunds;
            int amount = 1;

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
            Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + fundsToAdd + "cbills to inventory.");
        }
    }
}
