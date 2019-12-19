﻿using System;
using BattleTech.UI;
using Harmony;
using TMPro;
using UnityEngine.Events;

namespace LittleThings.Patches
{
    class InventoryItem
    {
        // Keep Bonuses display in one line
        [HarmonyPatch(typeof(SG_Shop_ItemSelectedPanel), "Initialize")]
        public static class SG_Shop_ItemSelectedPanel_Initialize_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.UIFixes;
            }

            public static void Postfix(SG_Shop_ItemSelectedPanel __instance, InventoryItemElement ___SelectedItemWidget)
            {
                try
                {
                    Logger.LogLine("[SG_Shop_ItemSelectedPanel_Initialize_POSTFIX] Expanding bonusText fields");

                    ___SelectedItemWidget.gearBonusText.enableAutoSizing = false;
                    ___SelectedItemWidget.gearBonusText.enableWordWrapping = false;
                    ___SelectedItemWidget.gearBonusText.overflowMode = TextOverflowModes.Overflow;

                    ___SelectedItemWidget.gearBonusTextB.enableAutoSizing = false;
                    ___SelectedItemWidget.gearBonusTextB.enableWordWrapping = false;
                    ___SelectedItemWidget.gearBonusTextB.overflowMode = TextOverflowModes.Overflow;


                    ___SelectedItemWidget.bonusStat1.enableAutoSizing = false;
                    ___SelectedItemWidget.bonusStat1.enableWordWrapping = false;
                    ___SelectedItemWidget.bonusStat1.overflowMode = TextOverflowModes.Overflow;

                    ___SelectedItemWidget.bonusStat2.enableAutoSizing = false;
                    ___SelectedItemWidget.bonusStat2.enableWordWrapping = false;
                    ___SelectedItemWidget.bonusStat2.overflowMode = TextOverflowModes.Overflow;

                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(InventoryItemElement), "SetData", new Type[] { typeof(InventoryDataObject_BASE), typeof(IMechLabDropTarget), typeof(int), typeof(bool), typeof(UnityAction<InventoryItemElement>) })]
        public static class InventoryItemElement_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.UIFixes;
            }

            public static void Postfix(InventoryItemElement __instance)
            {
                try
                {
                    Logger.LogLine("[InventoryItemElement_SetData_POSTFIX] Expanding bonusText fields");

                    __instance.gearBonusText.enableAutoSizing = false;
                    __instance.gearBonusText.enableWordWrapping = false;
                    __instance.gearBonusText.overflowMode = TextOverflowModes.Overflow;

                    __instance.gearBonusTextB.enableAutoSizing = false;
                    __instance.gearBonusTextB.enableWordWrapping = false;
                    __instance.gearBonusTextB.overflowMode = TextOverflowModes.Overflow;


                    __instance.bonusStat1.enableAutoSizing = false;
                    __instance.bonusStat1.enableWordWrapping = false;
                    __instance.bonusStat1.overflowMode = TextOverflowModes.Overflow;

                    __instance.bonusStat2.enableAutoSizing = false;
                    __instance.bonusStat2.enableWordWrapping = false;
                    __instance.bonusStat2.overflowMode = TextOverflowModes.Overflow;

                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(InventoryItemElement_NotListView), "SetData", new Type[] { typeof(ListElementController_BASE_NotListView), typeof(IMechLabDropTarget), typeof(int), typeof(bool), typeof(UnityAction<InventoryItemElement_NotListView>) })]
        public static class InventoryItemElement_NotListView_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.UIFixes;
            }

            public static void Postfix(InventoryItemElement_NotListView __instance)
            {
                try
                {
                    Logger.LogLine("[InventoryItemElement_NotListView_SetData_POSTFIX] Expanding bonusText fields");

                    __instance.gearBonusText.enableAutoSizing = false;
                    __instance.gearBonusText.enableWordWrapping = false;
                    __instance.gearBonusText.overflowMode = TextOverflowModes.Overflow;

                    __instance.gearBonusTextB.enableAutoSizing = false;
                    __instance.gearBonusTextB.enableWordWrapping = false;
                    __instance.gearBonusTextB.overflowMode = TextOverflowModes.Overflow;


                    __instance.bonusStat1.enableAutoSizing = false;
                    __instance.bonusStat1.enableWordWrapping = false;
                    __instance.bonusStat1.overflowMode = TextOverflowModes.Overflow;

                    __instance.bonusStat2.enableAutoSizing = false;
                    __instance.bonusStat2.enableWordWrapping = false;
                    __instance.bonusStat2.overflowMode = TextOverflowModes.Overflow;

                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
    }
}
