using System;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;
using HBS;
using HBS.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleThings.Patches
{
    class ChassisClassification
    {
        internal static Color starleagueColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.gold;
        internal static Color prototypeColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.blue;
        internal static Color eweColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.red;
        internal static Color stockColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray;

        internal static String starleagueAbbr = $"<color=#{ColorUtility.ToHtmlStringRGBA(starleagueColor)}>SLDF</color>";
        internal static String prototypeAbbr = $"<color=#{ColorUtility.ToHtmlStringRGBA(prototypeColor)}>PROTO</color>";
        internal static String eweAbbr = $"<color=#{ColorUtility.ToHtmlStringRGBA(eweColor)}>EWE</color>";
        internal static String stockAbbr = $"<color=#{ColorUtility.ToHtmlStringRGBA(stockColor)}>STOCK</color>";

        internal static String starleagueDesc = $"<color=#{ColorUtility.ToHtmlStringRGBA(starleagueColor)}>STAR LEAGUE</color>";
        internal static String prototypeDesc = $"<color=#{ColorUtility.ToHtmlStringRGBA(prototypeColor)}>PROTOTYPE</color>";
        internal static String eweDesc = $"<color=#{ColorUtility.ToHtmlStringRGBA(eweColor)}>ELECTRONIC WARFARE</color>";
        internal static String stockDesc = $"<color=#{ColorUtility.ToHtmlStringRGBA(stockColor)}>STOCK CONFIGURATION</color>";

        internal static TagSet starleagueTagSet = new TagSet(new string[] { "chassis_sldf" });
        internal static TagSet prototypeTagSet = new TagSet(new string[] { "chassis_prototype" });
        internal static TagSet eweTagSet = new TagSet(new string[] { "chassis_electronicWarfare" }); // Determined via inventory check for MechDefs but still needed for ChassisDefs



        // Helper
        internal static bool GetChassisClassification(ChassisDef chassisDef, out string cAbbr, out string cDesc)
        {
            if (chassisDef.ChassisTags.ContainsAny(starleagueTagSet))
            {
                Logger.Debug($"[ChassisClassification.GetChassisClassification] ({chassisDef.Description.Id}) is a SLDF chassis");
                cAbbr = starleagueAbbr;
                cDesc = starleagueDesc;
                return true;
            }
            else if (chassisDef.ChassisTags.ContainsAny(prototypeTagSet))
            {
                Logger.Debug($"[ChassisClassification.GetChassisClassification] ({chassisDef.Description.Id}) is a PROTOTYPE");
                cAbbr = prototypeAbbr;
                cDesc = prototypeDesc;
                return true;
            }
            else if (chassisDef.ChassisTags.ContainsAny(eweTagSet))
            {
                Logger.Debug($"[ChassisClassification.GetChassisClassification] ({chassisDef.Description.Id}) has Electronic Warfare Equipment");
                cAbbr = eweAbbr;
                cDesc = eweDesc;
                return true;
            }
            else
            {
                cAbbr = null;
                cDesc = null;
                return false;
            }
        }

        // Overload for MechDefs
        internal static bool GetMechClassification(MechDef mechDef, out string cAbbr, out string cDesc)
        {
            // Check Chassis first
            if (GetChassisClassification(mechDef.Chassis, out string classificationAbbr, out string classificationDesc))
            {
                cAbbr = classificationAbbr;
                cDesc = classificationDesc;
                return true;
            }
            // Go deeper and check MechDef too
            else if (Utilities.HasElectronicWarfareEquipment(mechDef))
            {
                Logger.Debug($"[ChassisClassification.GetMechClassification] ({mechDef.Description.Id}) has Electronic Warfare Equipment installed");
                cAbbr = eweAbbr;
                cDesc = eweDesc;
                return true;
            }
            else if (Utilities.IsStockMech(mechDef))
            {
                Logger.Debug($"[ChassisClassification.GetMechClassification] ({mechDef.Description.Id}) is in STOCK configuration");
                cAbbr = stockAbbr;
                cDesc = stockDesc;
                return true;
            }
            else
            {
                cAbbr = null;
                cDesc = null;
                return false;
            }
        }

        internal static LocalizableText SpawnChassisClassificationField(LocalizableText blueprint, UIModule parent)
        {
            if (parent.GetComponentsInChildren<LocalizableText>().Any(c => c.name == "ChassisType"))
            {
                LocalizableText t = parent.GetComponentsInChildren<LocalizableText>().First(c => c.name == "ChassisType");
                UnityEngine.Object.DestroyImmediate(t);
            }
            LocalizableText chassisClassification = UnityEngine.Object.Instantiate(blueprint, parent.transform);
            chassisClassification.name = "ChassisType";

            TMP_FontAsset font = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(f => f.name == "UnitedSansCond-Medium SDF");

            chassisClassification.SetFont(font);
            chassisClassification.fontSize = 14f;
            chassisClassification.fontWeight = 100;

            chassisClassification.enableAutoSizing = false;
            chassisClassification.enableWordWrapping = false;
            chassisClassification.overflowMode = TextOverflowModes.Overflow;
            chassisClassification.alignment = TextAlignmentOptions.Left;

            RectTransform chassisClassificationRT = chassisClassification.GetComponent<RectTransform>();
            Vector3 chassisClassificationPos = chassisClassificationRT.localPosition;
            chassisClassificationPos.x = 0;
            chassisClassificationPos.y = 40;
            chassisClassificationRT.localPosition = chassisClassificationPos;

            return chassisClassification;
        }



        // Patches
        [HarmonyPatch(typeof(MechLabMechInfoWidget), "SetData")]
        public static class MechLabMechInfoWidget_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisClassification["MechLab"];
            }

            public static void Postfix(MechLabMechInfoWidget __instance, MechLabPanel ___mechLab, LocalizableText ___mechDetails)
            {
                try
                {
                    if (___mechLab.activeMechDef == null || UnityGameInstance.BattleTechGame.Simulation == null)
                    {
                        return;
                    }

                    ___mechDetails.alpha = 1.0f;

                    if (GetChassisClassification(___mechLab.activeMechDef.Chassis, out string abbreviation, out string description) && !String.IsNullOrEmpty(description))
                    {
                        ___mechDetails.SetText("{0}{1} - {2} - {3} - {4}{5}", new object[]
                        {
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray)}>",
                            ___mechLab.activeMechDef.Chassis.Description.Name,
                            ___mechLab.activeMechDef.Chassis.VariantName,
                            ___mechLab.activeMechDef.Chassis.weightClass.ToString(),
                            "</color>",
                            description
                        });
                    }
                    else
                    {
                        ___mechDetails.SetText("{0}{1} - {2} - {3}{4}", new object[]
                        {
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray)}>",
                            ___mechLab.activeMechDef.Chassis.Description.Name,
                            ___mechLab.activeMechDef.Chassis.VariantName,
                            ___mechLab.activeMechDef.Chassis.weightClass.ToString(),
                            "</color>"
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(MechBayMechUnitElement), "SetData", new Type[] { typeof(IMechLabDropTarget), typeof(DataManager), typeof(int), typeof(MechDef), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
        public static class MechBayMechUnitElement_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisClassification["MechBaySlot"];
            }

            public static void Postfix(MechBayMechUnitElement __instance, MechDef ___mechDef, LocalizableText ___mechNickname)
            {
                try
                {
                    if (___mechDef == null || UnityGameInstance.BattleTechGame.Simulation == null)
                    {
                        return;
                    }

                    LocalizableText chassisClassification = SpawnChassisClassificationField(___mechNickname, __instance);

                    chassisClassification.gameObject.SetActive(false);
                    if (GetMechClassification(___mechDef, out string abbreviation, out string description))
                    {
                        chassisClassification.SetText(abbreviation);
                        chassisClassification.gameObject.SetActive(true);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechBayChassisUnitElement), "SetData")]
        public static class MechBayChassisUnitElement_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisClassification["MechBaySlot"];
            }

            public static void Postfix(MechBayChassisUnitElement __instance, ChassisDef ___chassisDef, LocalizableText ___chassisName)
            {
                try
                {
                    if (___chassisDef == null || UnityGameInstance.BattleTechGame.Simulation == null)
                    {
                        return;
                    }

                    LocalizableText chassisClassification = SpawnChassisClassificationField(___chassisName, __instance);

                    chassisClassification.gameObject.SetActive(false);
                    if (GetChassisClassification(___chassisDef, out string abbreviation, out string description))
                    { 
                        chassisClassification.SetText(abbreviation);
                        chassisClassification.gameObject.SetActive(true);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(MechBayMechInfoWidget), "SetDescriptions")]
        public static class MechBayMechInfoWidget_SetDescriptions_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisClassification["MechBayInfo"];
            }

            public static void Postfix(MechBayMechInfoWidget __instance, MechDef ___selectedMech, LocalizableText ___mechConfiguration)
            {
                try
                {
                    if (___selectedMech == null || UnityGameInstance.BattleTechGame.Simulation == null)
                    {
                        return;
                    }
                    Logger.Debug($"[MechBayMechInfoWidget_SetDescriptions_POSTFIX] ({___selectedMech.Description.Id}) ChassisTags: {___selectedMech.Chassis.ChassisTags.ToString()}");

                    if (GetMechClassification(___selectedMech, out string abbreviation, out string description) && !String.IsNullOrEmpty(description))
                    {
                        ___mechConfiguration.SetText("{0} - {1}\n{2} - {3}", new object[]
                        {
                            ___selectedMech.Chassis.Description.Name,
                            ___selectedMech.Chassis.VariantName,
                            ___selectedMech.Chassis.weightClass.ToString(),
                            description
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechBayChassisInfoWidget), "SetDescriptions")]
        public static class MechBayChassisInfoWidget_SetDescriptions_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisClassification["MechBayInfo"];
            }

            public static void Postfix(MechBayChassisInfoWidget __instance, ChassisDef ___selectedChassis, LocalizableText ___mechConfiguration)
            {
                try
                {
                    if (___selectedChassis == null || UnityGameInstance.BattleTechGame.Simulation == null)
                    {
                        return;
                    }
                    Logger.Debug($"[MechBayChassisInfoWidget_SetDescriptions_POSTFIX] ({___selectedChassis.Description.Id}) ChassisTags: {___selectedChassis.ChassisTags.ToString()}");

                    if (GetChassisClassification(___selectedChassis, out string abbreviation, out string description) && !String.IsNullOrEmpty(description))
                    {
                        ___mechConfiguration.SetText("{0} - {1}\n{2} - {3}", new object[]
                        {
                            ___selectedChassis.Description.Name,
                            ___selectedChassis.VariantName,
                            ___selectedChassis.weightClass.ToString(),
                            description
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(TooltipPrefab_Mech), "SetData")]
        public static class TooltipPrefab_Mech_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisClassification["Tooltips"];
            }

            public static void Postfix(TooltipPrefab_Mech __instance, object data, LocalizableText ___TonnageField, LocalizableText ___WeightField, LocalizableText ___NameField, LocalizableText ___VariantField)
            {
                try
                {
                    if (!(data is MechDef mechDef) || UnityGameInstance.BattleTechGame.Simulation == null)
                    {
                        return;
                    }

                    ___TonnageField.alpha = 1.0f;

                    if (GetMechClassification(mechDef, out string abbreviation, out string description) && !String.IsNullOrEmpty(description))
                    {
                        ___TonnageField.SetText("{0}{1} Tons - {2} - {3}{4}", new object[]
                        {
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray)}>",
                            mechDef.Chassis.Tonnage.ToString(),
                            mechDef.Chassis.weightClass,
                            "</color>",
                            description
                        });
                    }
                    else
                    {
                        ___TonnageField.SetText("{0}{1} Tons - {2}{3}", new object[]
                        {
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray)}>",
                            mechDef.Chassis.Tonnage.ToString(),
                            mechDef.Chassis.weightClass,
                            "</color>"
                        });
                    }

                    ___WeightField.SetText("");
                    ___NameField.SetText($"{mechDef.Chassis.Description.Name} <color=#{ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray)}>- {mechDef.Chassis.VariantName}</color>");
                    ___VariantField.SetText("");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(TooltipPrefab_Chassis), "SetData")]
        public static class TooltipPrefab_Chassis_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableChassisClassification["Tooltips"];
            }

            public static void Postfix(TooltipPrefab_Chassis __instance, object data, LocalizableText ___tonnageText, LocalizableText ___weightClassText, LocalizableText ___chassisNameText, LocalizableText ___variantNameText)
            {
                try
                {
                    if (!(data is ChassisDef chassisDef) || UnityGameInstance.BattleTechGame.Simulation == null)
                    {
                        return;
                    }

                    ___tonnageText.alpha = 1.0f;

                    if (GetChassisClassification(chassisDef, out string abbreviation, out string description) && !String.IsNullOrEmpty(description))
                    {
                        ___tonnageText.SetText("{0}{1} Tons - {2} - {3}{4}", new object[]
                        {
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray)}>",
                            chassisDef.Tonnage.ToString(),
                            chassisDef.weightClass,
                            "</color>",
                            description
                        });
                    }
                    else
                    {
                        ___tonnageText.SetText("{0}{1} Tons - {2}{3}", new object[]
                        {
                            $"<color=#{ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray)}>",
                            chassisDef.Tonnage.ToString(),
                            chassisDef.weightClass,
                            "</color>"
                        });
                    }

                    ___weightClassText.SetText("");
                    ___chassisNameText.SetText($"{chassisDef.Description.Name} <color=#{ColorUtility.ToHtmlStringRGBA(LazySingletonBehavior<UIManager>.Instance.UIColorRefs.medGray)}>- {chassisDef.VariantName}</color>");
                    ___variantNameText.SetText("");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
