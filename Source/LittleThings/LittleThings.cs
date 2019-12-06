using Harmony;
using System.Reflection;
using System;
using Newtonsoft.Json;
using System.IO;
using BattleTech;
using System.Collections.Generic;
using HBS.Data;
using System.Linq;

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
    public static class SimGameState__OnAttachUXComplete_Patch
    {
        public static bool Prepare()
        {
            return LittleThings.Settings.LogComponentLists || LittleThings.Settings.AddInventory;
        }

        public static void Postfix(SimGameState __instance)
        {
            try
            {
                IDataItemStore<string, ChassisDef> ChassisDefs = __instance.DataManager.ChassisDefs;
                IDataItemStore<string, WeaponDef> WeaponDefs = __instance.DataManager.WeaponDefs;
                IDataItemStore<string, UpgradeDef> UpgradeDefs = __instance.DataManager.UpgradeDefs;
                IDataItemStore<string, HeatSinkDef> HeatSinkDefs = __instance.DataManager.HeatSinkDefs;
                IDataItemStore<string, AmmunitionBoxDef> AmmoBoxDefs = __instance.DataManager.AmmoBoxDefs;

                List<string> mechDefIds = new List<string>();
                List<string> weaponDefIds = new List<string>();
                List<string> upgradeDefIds = new List<string>();
                List<string> heatSinkDefIds = new List<string>();
                List<string> ammoBoxDefIds = new List<string>();

                List<string> chassisDefBlacklist = new List<string> {
                    "chassisdef_atlas_AS7-GG",
                    "chassisdef_centurion_TARGETDUMMY",
                    "chassisdef_marauder_MAD-BH",
                    "chassisdef_marauder_MAD-CM",
                    "chassisdef_panther_TARGETDUMMY",
                    "chassisdef_urbanmech_TESTDUMMY",
                    "chassisdef_warhammer_WHM-BW",
                    "chassisdef_crab__fp_gladiator_BSC-27",
                    "chassisdef_archer_ARC-XO",
                    "chassisdef_archer_ARC-LS",
                    "chassisdef_annihilator_ANH-JH",
                    "chassisdef_bullshark_BSK-M3",
                    "chassisdef_bullshark_BSK-MAZ",
                    "chassisdef_rifleman_RFL-RIP"
                };
                List<string> weaponDefBlacklist = new List<string> {
                    "Weapon_MeleeAttack",
                    "Weapon_DFAAttack",
                    "Weapon_Laser_AI_Imaginary",
                    "Weapon_Mortar_Thumper",
                    "Weapon_Mortar_MechMortar",
                    "Weapon_Autocannon_AC20_SPECIAL-Victoria",
                    "Weapon_Flamer_Flamer_SPECIAL-Victoria"
                };
                List<string> upgradeDefBlacklist = new List<string> {
                    "Gear_General_Enhanced_Missilery_System",
                    "Gear_Actuator_Prototype_Hatchet",
                    "Gear_General_Targeting_Baffle",
                    "Gear_General_Rangefinder_Suite",
                    "Gear_General_Intercept_System",
                    "Gear_General_GM_Ballistic_Siege_Compensators",
                    "Gear_General_Close_Quarters_Combat_Suite",
                    "Gear_Sensor_Prototype_ECM",
                    "Gear_General_Optimized_Capacitors",
                    "Gear_Sensor_Prototype_ActiveProbe",
                    "Gear_Sensor_Prototype_EWE",
                    "Gear_Mortar_Thumper",
                    "Gear_Mortar_MechMortar",
                    "TargetDummyMod",
                    "Gear_Cockpit_Tacticon_B2000_Battle_Computer",
                    "Gear_General_Company_Command_Module",
                    "Gear_General_Lance_Command_Module",
                    "Gear_General_Robinson_TG120_Drive_Train"
                };
                List<string> heatSinkDefBlacklist = new List<string>
                {

                };
                List<string> ammoBoxDefBlacklist = new List<string> {
                    "Ammo_AmmunitionBox_Generic_Flamer",
                    "Ammo_AmmunitionBox_Generic_SRMInferno",
                    "Ammo_AmmunitionBox_Generic_Narc"
                };



                // Collect Mechs
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Collecting all valid Mechs");
                foreach (string chassisDefId in ChassisDefs.Keys)
                {
                    if (!chassisDefBlacklist.Contains(chassisDefId))
                    {
                        string id = chassisDefId.Replace("chassisdef", "mechdef");
                        mechDefIds.Add(id);
                    }
                }
                mechDefIds.Sort();

                // Collect Weapons
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Collecting all valid Weapons");
                foreach (string id in WeaponDefs.Keys)
                {
                    if (!weaponDefBlacklist.Contains(id))
                    {
                        weaponDefIds.Add(id);
                    }
                }
                weaponDefIds.Sort();

                // Collect Upgrades
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Collecting all valid Upgrades");
                foreach (string id in UpgradeDefs.Keys)
                {
                    if (!upgradeDefBlacklist.Contains(id))
                    {
                        upgradeDefIds.Add(id);
                    }
                }
                upgradeDefIds.Sort();

                // Collect Heatsinks
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Collecting all valid Heatsinks");
                foreach (string id in HeatSinkDefs.Keys)
                {
                    if (!heatSinkDefBlacklist.Contains(id))
                    {
                        heatSinkDefIds.Add(id);
                    }
                }
                heatSinkDefIds.Sort();

                // Collect Ammunition
                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Collecting all valid AmmunitionBoxes");
                foreach (string id in AmmoBoxDefs.Keys)
                {
                    if (!ammoBoxDefBlacklist.Contains(id))
                    {
                        ammoBoxDefIds.Add(id);
                    }
                }
                ammoBoxDefIds.Sort();



                // Add Inventory
                if (LittleThings.Settings.AddInventory)
                {
                    // Add Mechs
                    if (LittleThings.Settings.AddInventoryMechs)
                    {
                        // Custom list given?
                        if (LittleThings.Settings.AddInventoryMechsList.Any())
                        {
                            foreach (string id in LittleThings.Settings.AddInventoryMechsList)
                            {
                                __instance.AddMechByID(id, true);
                                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + id + " to inventory.");
                            }
                        }
                        else
                        {
                            foreach (string id in mechDefIds)
                            {
                                __instance.AddMechByID(id, true);
                                Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + id + " to inventory.");
                            }
                        }
                    }

                    // Add Weapons
                    if (LittleThings.Settings.AddInventoryComponents)
                    {
                        foreach (string id in weaponDefIds)
                        {
                            int i = 0;
                            while (i < LittleThings.Settings.AddInventoryComponentCount)
                            {
                                __instance.AddItemStat(id, typeof(WeaponDef), false);
                                i++;
                            }
                            Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + id + "(" + LittleThings.Settings.AddInventoryComponentCount + ") to inventory.");
                        }
                    }

                    // Add upgrades
                    if (LittleThings.Settings.AddInventoryComponents)
                    {
                        foreach (string id in upgradeDefIds)
                        {
                            int i = 0;
                            while (i < LittleThings.Settings.AddInventoryComponentCount)
                            {
                                __instance.AddItemStat(id, typeof(UpgradeDef), false);
                                i++;
                            }
                            Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + id + "(" + LittleThings.Settings.AddInventoryComponentCount + ") to inventory.");
                        }
                    }

                    //Add Heatsinks
                    if (LittleThings.Settings.AddInventoryComponents)
                    {
                        foreach (string id in heatSinkDefIds)
                        {

                            int i = 0;
                            while (i < LittleThings.Settings.AddInventoryComponentCount)
                            {
                                __instance.AddItemStat(id, typeof(HeatSinkDef), false);
                                i++;
                            }
                            Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + id + "(" + LittleThings.Settings.AddInventoryComponentCount + ") to inventory.");
                        }
                    }

                    // Add Ammunition
                    if (LittleThings.Settings.AddInventoryComponents)
                    {
                        foreach (string id in ammoBoxDefIds)
                        {

                            int i = 0;
                            while (i < LittleThings.Settings.AddInventoryComponentCount)
                            {
                                __instance.AddItemStat(id, typeof(AmmunitionBoxDef), false);
                                i++;
                            }
                            Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + id + "(" + LittleThings.Settings.AddInventoryComponentCount + ") to inventory.");
                        }
                    }

                    // Add Funds
                    if (LittleThings.Settings.AddInventoryFunds > 0)
                    {
                        __instance.AddFunds(LittleThings.Settings.AddInventoryFunds, null, true);
                        Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Added " + LittleThings.Settings.AddInventoryFunds + " C-Bills to inventory.");
                    }
                }



                // Clean list
                if (LittleThings.Settings.LogComponentLists)
                {
                    Logger.LogLine("------------------------------------------------------------------------------------------------------------------------");
                    Logger.LogLine("[SimGameState__OnAttachUXComplete_POSTFIX] Generate clean, json-ready list of all valid Mechs and Components.");
                    Logger.LogLine("------------------------------");
                    // Mechs
                    foreach (string id in mechDefIds)
                    {
                        Logger.LogLine("\"" + id + "\",", false);
                    }
                    // Weapons
                    foreach (string id in weaponDefIds)
                    {
                        Logger.LogLine("\"" + id + "\",", false);
                    }
                    // Upgrades
                    foreach (string id in upgradeDefIds)
                    {
                        Logger.LogLine("\"" + id + "\",", false);
                    }
                    // Heatsinks
                    foreach (string id in heatSinkDefIds)
                    {
                        Logger.LogLine("\"" + id + "\",", false);

                    }
                    // Ammunition
                    foreach (string id in ammoBoxDefIds)
                    {
                        Logger.LogLine("\"" + id + "\",", false);
                    }
                    Logger.LogLine("------------------------------");
                    Logger.LogLine("------------------------------------------------------------------------------------------------------------------------");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
