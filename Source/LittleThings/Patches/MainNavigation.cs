using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using SVGImporter;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace LittleThings.Patches
{
    class MainNavigation
    {
        [HarmonyPatch(typeof(SGNavigationButton), "CollapseSet")]
        public static class SGNavigationButton_CollapseSet_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixUIMainNavigation;
            }

            public static void Prefix(SGNavigationButton __instance, bool isCollapsed)
            {
                try
                {
                    //Logger.Debug($"[SGNavigationButton_CollapseSet_PREFIX] Simulating OnPointerEnter | OnPointerExit depending on parameter isCollapsed ({isCollapsed})");
                    Logger.Sleep();

                    PointerEventData dummyPointerEventData = new PointerEventData(EventSystem.current);
                    if (isCollapsed)
                    {
                        __instance.OnPointerEnter(dummyPointerEventData);
                    }
                    else
                    {
                        __instance.OnPointerExit(dummyPointerEventData);
                    }
                    Logger.Wake();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(SGNavigationButton), "OnPointerEnter")]
        public static class SGNavigationButton_OnPointerEnter_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixUIMainNavigation;
            }

            public static void Prefix(SGNavigationButton __instance, PointerEventData eventData, SGNavigationList ___buttonParent)
            {
                try
                {
                    Logger.Debug($"[SGNavigationButton_OnPointerEnter_PREFIX] Collapse all submenus before opening this one's");

                    // Note that this method isn't used in vanilla. I patched it above to be actually useful...
                    ___buttonParent.CollapseSet(false);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(SGNavigationButton), "OnPointerExit")]
        public static class SGNavigationButton_OnPointerExit_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixUIMainNavigation;
            }

            public static bool Prefix(SGNavigationButton __instance, PointerEventData eventData)
            {
                try
                {
                    Logger.Debug($"[SGNavigationButton_OnPointerExit_PREFIX] Prevent collapsing of submenu due to the 1px gap...");
                    Logger.Info($"[SGNavigationButton_OnPointerExit_PREFIX] eventData.position.x: {eventData.position.x}");
                    Logger.Info($"[SGNavigationButton_OnPointerExit_PREFIX] Screen.width: {Screen.width}");

                    // Note that the values are resolution dependent! BUT the bug seems to only occur on resultions with width 1920, so still ok to check hard values here...
                    //if ((eventData.delta.x == 1f || eventData.delta.x == -1f) && eventData.position.x == 317)
                    if (Screen.width == 1920 && eventData.position.x == 317)
                    {
                        Logger.Debug($"[SGNavigationButton_OnPointerExit_PREFIX] Gap hit! Skipping original method...");
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return true;
                }
            }
        }






        /*
        [HarmonyPatch(typeof(SGNavFlyoutButton), "SetData")]
        public static class SGNavFlyoutButton_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixUIMainNavigation;
            }

            public static void Postfix(SGNavFlyoutButton __instance, SGNavigationButton parent, string text, DropshipMenuType menu)
            {
                try
                {
                    List<SGNavFlyoutButton> ___flyoutButtonList = (List<SGNavFlyoutButton>)AccessTools.Field(typeof(SGNavigationButton), "FlyoutButtonList").GetValue(parent);
                    LocalizableText ___text = (LocalizableText)AccessTools.Field(typeof(SGNavigationButton), "text").GetValue(parent);

                    Logger.Debug($"[SGNavFlyoutButton_SetData_POSTFIX] THIS button ({text}) belongs to parent button ({___text.text})");

                    void OnPointerEnter()
                    {
                        Logger.Debug($"[SGNavFlyoutButton_SetData_POSTFIX] SetData_OnPointerEnter()");
                    }

                    void OnPointerExit()
                    {
                        Logger.Debug($"[SGNavFlyoutButton_SetData_POSTFIX] SetData_OnPointerExit()");
                    }

                    __instance.OnPointerEnterAction = new UnityAction(OnPointerEnter);
                    __instance.OnPointerExitAction = new UnityAction(OnPointerExit);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(SGNavigationButton), "ResetFlyoutsToPrefab")]
        public static class SGNavigationButton_ResetFlyoutsToPrefab_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixUIMainNavigation;
            }

            public static void Postfix(SGNavigationButton __instance, int ___flyoutButtonCount, List<SGNavFlyoutButton> ___FlyoutButtonList, ref SVGImage ___flyoutContainer, LocalizableText ___text)
            {
                try
                {
                    Logger.Debug("---");
                    Logger.Debug($"[SGNavigationButton_ResetFlyoutsToPrefab_POSTFIX] Button: {___text.text}");
                    Logger.Debug($"[SGNavigationButton_ResetFlyoutsToPrefab_POSTFIX] ___flyoutButtonCount: {___flyoutButtonCount}");

                    float subMenuItemHeight = ___FlyoutButtonList[0].GetComponent<RectTransform>().rect.height;
                    Logger.Debug($"[SGNavigationButton_ResetFlyoutsToPrefab_POSTFIX] subMenuItemHeight: {subMenuItemHeight}");

                    float flyOutContainerHeight = ___flyoutContainer.rectTransform.rect.height;
                    Logger.Debug($"[SGNavigationButton_ResetFlyoutsToPrefab_POSTFIX] flyOutContainerHeight: {flyOutContainerHeight}");

                    ___flyoutContainer.SetLayoutDirty();

                    //RectTransform rt = ___flyoutContainer.rectTransform;
                    RectTransform rt = (RectTransform)AccessTools.Field(typeof(SVGImage), "m_RectTransform").GetValue(___flyoutContainer);

                    rt.anchorMax = new Vector2(1f, ___flyoutButtonCount * (subMenuItemHeight + 2));

                    //Rect r = rt.rect;

                    //r.height = ___flyoutButtonCount * (subMenuItemHeight + 2);


                    Logger.Debug($"[SGNavigationButton_ResetFlyoutsToPrefab_POSTFIX] flyOutContainerHeight: {flyOutContainerHeight}");
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
