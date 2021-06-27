using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using UnityEngine;

namespace LittleThings.Patches
{
    class CustomColors
    {
        internal static Color ballisticColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.GetUIColor(UIColor.BallisticColor);
        internal static Color missileColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.GetUIColor(UIColor.MissileColor);
        internal static Color energyColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.GetUIColor(UIColor.EnergyColor);
        internal static Color supportColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.GetUIColor(UIColor.SmallColor);
        internal static Color equipmentColor = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.GetUIColor(UIColor.EquipmentColor);

        /*
        internal static Color ballisticColorLosTech = Color.Lerp(ballisticColor, Color.cyan, 0.2f);
        internal static Color missileColorLosTech = Color.Lerp(missileColor, Color.magenta, 0.15f);
        internal static Color energyColorLosTech = Color.Lerp(energyColor, Color.green, 0.15f);
        internal static Color supportColorLosTech = Color.Lerp(supportColor, Color.yellow, 0.15f);
        internal static Color equipmentColorLosTech = Color.Lerp(equipmentColor, Color.white, 0.2f);
        */

        /*
        internal static Color ballisticColorLosTech = Color.Lerp(ballisticColor, Color.white, 0.3f);
        internal static Color missileColorLosTech = Color.Lerp(missileColor, Color.white, 0.3f);
        internal static Color energyColorLosTech = Color.Lerp(energyColor, Color.white, 0.3f);
        internal static Color supportColorLosTech = Color.Lerp(supportColor, Color.white, 0.3f);
        internal static Color equipmentColorLosTech = Color.Lerp(equipmentColor, Color.white, 0.3f);
        */

        internal static Color ballisticColorLosTech = new Color32(79, 150, 161, 255);
        internal static Color missileColorLosTech = new Color32(140, 87, 134, 255);
        internal static Color energyColorLosTech = new Color32(112, 140, 73, 255);
        internal static Color supportColorLosTech = new Color32(168, 138, 77, 255);
        internal static Color equipmentColorLosTech = new Color32(79, 92, 107, 255);



        // Helper
        internal static bool HasOverrideColor(MechComponentDef mechComponentDef, out Color overrideColor)
        {
            bool result = false;
            overrideColor = Color.red;

            if (mechComponentDef.ComponentType == ComponentType.Weapon)
            {
                WeaponDef weaponDef = mechComponentDef as WeaponDef;

                if (weaponDef.ComponentTags.Contains("component_type_lostech"))
                {
                    if (weaponDef.WeaponCategoryValue.IsBallistic)
                    {
                        overrideColor = ballisticColorLosTech;
                        result = true;
                    }
                    else if (weaponDef.WeaponCategoryValue.IsMissile)
                    {
                        overrideColor = missileColorLosTech;
                        result = true;
                    }
                    else if (weaponDef.WeaponCategoryValue.IsEnergy)
                    {
                        overrideColor = energyColorLosTech;
                        result = true;
                    }
                    else if (weaponDef.WeaponCategoryValue.IsSupport)
                    {
                        overrideColor = supportColorLosTech;
                        result = true;
                    }
                }
            }
            else if (mechComponentDef.ComponentType == ComponentType.AmmunitionBox)
            {
                AmmunitionBoxDef ammunitionBoxDef = mechComponentDef as AmmunitionBoxDef;

                if (ammunitionBoxDef.ComponentTags.Contains("component_type_lostech"))
                {
                    if (ammunitionBoxDef.AmmoID == "Ammunition_GAUSS" || ammunitionBoxDef.AmmoID == "Ammunition_LB2X" || ammunitionBoxDef.AmmoID == "Ammunition_LB5X" || ammunitionBoxDef.AmmoID == "Ammunition_LB10X" || ammunitionBoxDef.AmmoID == "Ammunition_LB20X")
                    {
                        overrideColor = ballisticColorLosTech;
                        result = true;
                    }
                    else if (ammunitionBoxDef.AmmoID == "Ammunition_SRMStreak" || ammunitionBoxDef.AmmoID == "Ammunition_Narc")
                    {
                        overrideColor = missileColorLosTech;
                        result = true;
                    }
                }
            }
            else if (mechComponentDef.ComponentType == ComponentType.HeatSink)
            {
                HeatSinkDef heatSinkDef = mechComponentDef as HeatSinkDef;

                if (heatSinkDef.ComponentTags.Contains("component_type_lostech"))
                {
                    overrideColor = equipmentColorLosTech;
                    result = true;
                }
            }
            else if (mechComponentDef.ComponentType == ComponentType.Upgrade)
            {
                UpgradeDef upgradeDef = mechComponentDef as UpgradeDef;

                if (upgradeDef.ComponentTags.Contains("component_type_lostech") || upgradeDef.ComponentTags.Contains("component_type_prototype"))
                {
                    overrideColor = equipmentColorLosTech;
                    result = true;
                }
            }

            if (result)
            {
                Logger.Debug($"[CustomColors.HasOverrideColor] Overriding color for {mechComponentDef.Description.UIName}");
            }

            return result;
        }



        // ESSENTIAL BUGFIX: If an override color is set, we need to mark the related UIColor as a custom one. Otherwise colors won't update when switching between items. Please check "UIColorRefTracker.SetUIColor()"
        [HarmonyPatch(typeof(UIColorRefTracker), "OverrideWithColor")]
        public static class UIColorRefTracker_OverrideWithColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Prefix(UIColorRefTracker __instance, Color newColor)
            {
                try
                {
                    //Logger.Debug($"[UIColorRefTracker.OverrideWithColor_PREFIX] newColor: {newColor}");
                    //Logger.Info($"[UIColorRefTracker.OverrideWithColor_PREFIX] Marking __instance.colorRef.UIColor as UIColor.Custom");
                    __instance.colorRef.UIColor = UIColor.Custom;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Shops
        [HarmonyPatch(typeof(InventoryDataObject_ShopWeapon), "RefreshItemColor")]
        public static class InventoryDataObject_ShopWeapon_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(InventoryDataObject_ShopWeapon __instance, InventoryItemElement theWidget)
            {
                try
                {
                    if (HasOverrideColor(__instance.weaponDef, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(InventoryDataObject_ShopGear), "RefreshItemColor")]
        public static class InventoryDataObject_ShopGear_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(InventoryDataObject_ShopGear __instance, InventoryItemElement theWidget)
            {
                try
                {
                    if (HasOverrideColor(__instance.componentDef, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Salvage
        [HarmonyPatch(typeof(InventoryDataObject_SalvageWeapon), "RefreshItemColor")]
        public static class InventoryDataObject_SalvageWeapon_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(InventoryDataObject_SalvageWeapon __instance, InventoryItemElement theWidget)
            {
                try
                {
                    if (__instance.componentDef == null)
                    {
                        return;
                    }



                    /*
                    // Colorize icon
                    UIColorRefTracker iconColor = theWidget.icon.GetComponent<UIColorRefTracker>();
                    if (iconColor == null)
                    {
                        iconColor = theWidget.icon.gameObject.AddComponent<UIColorRefTracker>();
                    }
                    iconColor.OverrideWithColor(Color.black);
                    */



                    if (HasOverrideColor(__instance.componentDef, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(InventoryDataObject_SalvageGear), "RefreshItemColor")]
        public static class InventoryDataObject_SalvageGear_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(InventoryDataObject_SalvageWeapon __instance, InventoryItemElement theWidget)
            {
                try
                {
                    if (__instance.componentDef == null)
                    {
                        return;
                    }

                    if (HasOverrideColor(__instance.componentDef, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ListElementController_SalvageWeapon_NotListView), "RefreshItemColor")]
        public static class ListElementController_SalvageWeapon_NotListView_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(ListElementController_SalvageWeapon_NotListView __instance, InventoryItemElement_NotListView theWidget)
            {
                try
                {
                    if (__instance.componentDef == null)
                    {
                        return;
                    }

                    if (HasOverrideColor(__instance.componentDef, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ListElementController_SalvageGear_NotListView), "RefreshItemColor")]
        public static class ListElementController_SalvageGear_NotListView_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(ListElementController_SalvageGear_NotListView __instance, InventoryItemElement_NotListView theWidget)
            {
                try
                {
                    if (__instance.componentDef == null)
                    {
                        return;
                    }

                    if (HasOverrideColor(__instance.componentDef, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Inventory
        [HarmonyPatch(typeof(InventoryItemElement), "RefreshItemColor")]
        public static class InventoryItemElement_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(InventoryItemElement __instance, MechComponentRef ___componentRef)
            {
                try
                {
                    if (HasOverrideColor(___componentRef.Def, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = __instance.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(InventoryItemElement_NotListView), "RefreshItemColor")]
        public static class InventoryItemElement_NotListView_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(InventoryItemElement_NotListView __instance, MechComponentRef ___componentRef)
            {
                try
                {
                    if (HasOverrideColor(___componentRef.Def, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = __instance.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(InventoryDataObject_InventoryWeapon), "RefreshItemColor")]
        public static class InventoryDataObject_InventoryWeapon_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(InventoryDataObject_InventoryWeapon __instance, InventoryItemElement theWidget)
            {
                try
                {
                    if (HasOverrideColor(__instance.componentRef.Def, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(InventoryDataObject_InventoryGear), "RefreshItemColor")]
        public static class InventoryDataObject_InventoryGear_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(InventoryDataObject_InventoryGear __instance, InventoryItemElement theWidget)
            {
                try
                {
                    if (HasOverrideColor(__instance.componentRef.Def, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ListElementController_InventoryWeapon_NotListView), "RefreshItemColor")]
        public static class ListElementController_InventoryWeapon_NotListView_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(ListElementController_InventoryWeapon_NotListView __instance, InventoryItemElement_NotListView theWidget)
            {
                try
                {
                    if (HasOverrideColor(__instance.componentRef.Def, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ListElementController_InventoryGear_NotListView), "RefreshItemColor")]
        public static class ListElementController_InventoryGear_NotListView_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(ListElementController_InventoryGear_NotListView __instance, InventoryItemElement_NotListView theWidget)
            {
                try
                {
                    if (HasOverrideColor(__instance.componentRef.Def, out Color overrideColor))
                    {
                        UIColorRefTracker[] array = theWidget.iconBGColors;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].OverrideWithColor(overrideColor);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // MechLab
        [HarmonyPatch(typeof(MechLabItemSlotElement), "RefreshItemColor")]
        public static class MechLabItemSlotElement_RefreshItemColor_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(MechLabItemSlotElement __instance, UIColorRefTracker ___backgroundColor)
            {
                try
                {
                    if (HasOverrideColor(__instance.ComponentRef.Def, out Color overrideColor))
                    {
                        ___backgroundColor.OverrideWithColor(overrideColor);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        // Mech loadouts
        [HarmonyPatch(typeof(LanceMechEquipmentListItem), "SetTooltipData")]
        public static class LanceMechEquipmentListItem_SetTooltipData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableCustomColors;
            }

            public static void Postfix(LanceMechEquipmentListItem __instance, MechComponentDef MechDef, UIColorRefTracker ___backgroundColor)
            {
                try
                {
                    if (HasOverrideColor(MechDef, out Color overrideColor)) // @ToDo: Respect damage level? See "LanceMechEquipmentList.SetLoadout()"
                    {
                        ___backgroundColor.OverrideWithColor(overrideColor);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
