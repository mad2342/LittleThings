using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.Data;
using BattleTech.Framework;
using Harmony;

namespace LittleThings.Patches
{
    class LanceConfigurationByTags
    {
        [HarmonyPatch(typeof(SimGameState), "StartLanceConfiguration")]
        public static class SimGameState_StartLanceConfiguration_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableLanceConfigurationByTags;
            }

            public static void Prefix(SimGameState __instance)
            {
                try
                {
                    Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] Allow forced player units to be selected by tags...");

                    Contract contract;
                    if (__instance.HasTravelContract)
                    {
                        contract = __instance.ActiveTravelContract;
                    }
                    else
                    {
                        contract = __instance.SelectedContract;
                    }
                    Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] contract.Name: {contract.Name}");
                    Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] contract.Override.player1Team.lanceOverrideList.Count: {contract.Override.player1Team.lanceOverrideList.Count}");

                    if (contract.Override != null && contract.Override.player1Team.lanceOverrideList.Count > 0)
                    {
                        Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] contract.Override.ID: {contract.Override.ID}");
                        Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] contract.Override.difficulty: {contract.Override.difficulty}");
                        Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] contract.Override.finalDifficulty: {contract.Override.finalDifficulty}");

                        foreach (UnitSpawnPointOverride unitSpawnPointOverride in contract.Override.player1Team.lanceOverrideList[0].unitSpawnPointOverrideList)
                        {
                            if (!string.IsNullOrEmpty(unitSpawnPointOverride.pilotDefId) && unitSpawnPointOverride.pilotDefId != UnitSpawnPointGameLogic.PilotDef_Commander && unitSpawnPointOverride.pilotDefId != UnitSpawnPointGameLogic.PilotDef_InheritLance && !__instance.DataManager.PilotDefs.Exists(unitSpawnPointOverride.pilotDefId))
                            {
                                Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] unitSpawnPointOverride.pilotDefId: {unitSpawnPointOverride.pilotDefId}");
                            }
                            if (!string.IsNullOrEmpty(unitSpawnPointOverride.unitDefId) && unitSpawnPointOverride.unitDefId != "mechDef_None" && !__instance.DataManager.MechDefs.Exists(unitSpawnPointOverride.unitDefId) && unitSpawnPointOverride.unitDefId == "Tagged")
                            {
                                Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] unitSpawnPointOverride.unitDefId: {unitSpawnPointOverride.unitDefId}");

                                // Add tags depending on difficulty on all non urbies
                                if (!unitSpawnPointOverride.unitTagSet.Contains("unit_urbie"))
                                {
                                    if (contract.Override.finalDifficulty >= 8)
                                    {
                                        unitSpawnPointOverride.unitTagSet.Add("unit_heavy");
                                    }
                                    else if (contract.Override.finalDifficulty >= 5)
                                    {
                                        unitSpawnPointOverride.unitTagSet.Add("unit_medium");
                                    }
                                    else if (contract.Override.finalDifficulty >= 0)
                                    {
                                        unitSpawnPointOverride.unitTagSet.Add("unit_light");
                                    }
                                }
                                // Urbie
                                else
                                {
                                    unitSpawnPointOverride.unitExcludedTagSet.Clear();

                                    if (contract.Override.finalDifficulty >= 8)
                                    {
                                        unitSpawnPointOverride.unitTagSet.Add("unit_components_plusplusplus");
                                    }
                                    else if (contract.Override.finalDifficulty >= 6)
                                    {
                                        unitSpawnPointOverride.unitTagSet.Add("unit_components_plusplus");
                                    }
                                    else if (contract.Override.finalDifficulty >= 4)
                                    {
                                        unitSpawnPointOverride.unitTagSet.Add("unit_components_plus");
                                    }
                                }

                                // Clear spawn effects too (ie "spawn_poorly_maintained_25")?
                                //unitSpawnPointOverride.spawnEffectTags.Clear();

                                Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] unitSpawnPointOverride.unitTagSet: {unitSpawnPointOverride.unitTagSet}");

                                // Select new mech
                                UnitDef_MDD unitDef_MDD = UnitSpawnPointOverride.SelectTaggedUnitDef(MetadataDatabase.Instance, unitSpawnPointOverride.unitTagSet, unitSpawnPointOverride.unitExcludedTagSet, "unknown", "unknown", -1, null, null);
                                Logger.Debug($"[SimGameState_StartLanceConfiguration_PREFIX] unitDef_MDD.UnitDefID: {unitDef_MDD.UnitDefID}");

                                // Set
                                unitSpawnPointOverride.unitDefId = unitDef_MDD.UnitDefID;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        /*
        // Try to randomize callsigns of forced ally pilots, no joy. Way too much hardcoding going on...
        [HarmonyPatch(typeof(LanceConfiguration), "AddUnits")]
        public static class LanceConfiguration_AddUnits_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableLanceConfigurationByTags;
            }

            public static void Prefix(LanceConfiguration __instance, ref IEnumerable<SpawnableUnit> units)
            {
                try
                {
                    Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] Called...");

                    PilotNameGenerator pilotNameGenerator = new PilotNameGenerator();

                    foreach (SpawnableUnit unit in units)
                    {
                        if(unit.Pilot != null)
                        {
                            Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] unit.Pilot.Description.Callsign: {unit.Pilot.Description.Callsign}");
                            Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] unit.Pilot.Description.FirstName: {unit.Pilot.Description.FirstName}");
                            Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] unit.Pilot.Description.LastName: {unit.Pilot.Description.LastName}");

                            PilotDef pDef = (PilotDef)AccessTools.Property(typeof(SpawnableUnit), "Pilot").GetValue(unit, null);
                            //HumanDescriptionDef desc = (HumanDescriptionDef)AccessTools.Property(typeof(PilotDef), "Description").GetValue(pDef, null);
                            //desc.SetCallsign("WTF!");

                            string callSignOverride = pilotNameGenerator.GetCallsign();
                            string nameOverride = callSignOverride;
                            string firstNameOverride = pilotNameGenerator.GetFirstName(unit.Pilot.Description.Gender);
                            string lastNameOverride = pilotNameGenerator.GetLastName();

                            HumanDescriptionDef descOverride = new HumanDescriptionDef(unit.PilotId, nameOverride, firstNameOverride, lastNameOverride, callSignOverride, unit.Pilot.Description.Gender, unit.Pilot.Description.FactionValue, unit.Pilot.Description.Age, unit.Pilot.Description.Details, unit.Pilot.Description.Icon);

                            new Traverse(pDef).Property("Description").SetValue(descOverride);

                            Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] ---");
                            Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] unit.Pilot.Description.Callsign: {unit.Pilot.Description.Callsign}");
                            Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] unit.Pilot.Description.FirstName: {unit.Pilot.Description.FirstName}");
                            Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] unit.Pilot.Description.LastName: {unit.Pilot.Description.LastName}");
                            Logger.Debug($"[LanceConfiguration_AddUnits_PREFIX] ---");
                        }
                    }
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
