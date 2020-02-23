using System;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using TMPro;
using UnityEngine;

namespace LittleThings.Patches
{
    class HeraldryScreenTitle
    {
        [HarmonyPatch(typeof(HeraldryCreatorPanel), "SetData")]
        public static class HeraldryCreatorPanel_SetData_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.FixUIHeraldryScreen;
            }

            public static void Prefix(HeraldryCreatorPanel __instance, LocalizableText ___screenTitle, LocalizableText ___screenSubTitle, LocalizableText ___mechInfoName)
            {
                try
                {
                    Logger.Debug("[HeraldryCreatorPanel_SetData_PREFIX] Fix position of screen title");

                    TMP_FontAsset ___mechInfoName_m_baseFont = (TMP_FontAsset)AccessTools.Field(typeof(LocalizableText), "m_baseFont").GetValue(___mechInfoName);

                    //___screenTitle.fontSize = 44;
                    ___screenTitle.SetFont(___mechInfoName_m_baseFont);
                    //___screenTitle.text = "Heraldry Customization";

                    // Position title
                    RectTransform screenTitleRT = ___screenTitle.GetComponent<RectTransform>();
                    Vector3 screenTitlePos = screenTitleRT.localPosition;
                    Logger.Info($"[HeraldryCreatorPanel_SetData_PREFIX] CURRENT position of screen title: {screenTitlePos.x}, {screenTitlePos.y}");

                    screenTitlePos.x = -13;
                    //screenTitlePos.y = -22;
                    screenTitleRT.localPosition = screenTitlePos;
                    Logger.Info($"[HeraldryCreatorPanel_SetData_PREFIX] NEW position of screen title: {screenTitlePos.x}, {screenTitlePos.y}");



                    // Position subtitle
                    RectTransform screenSubTitleRT = ___screenSubTitle.GetComponent<RectTransform>();
                    Vector3 screenSubTitlePos = screenSubTitleRT.localPosition;
                    Logger.Info($"[HeraldryCreatorPanel_SetData_PREFIX] CURRENT position of screen subtitle: {screenSubTitlePos.x}, {screenSubTitlePos.y}");

                    screenSubTitlePos.x = -12;
                    //screenSubTitlePos.y = -55.8;
                    screenSubTitleRT.localPosition = screenSubTitlePos;
                    Logger.Info($"[HeraldryCreatorPanel_SetData_PREFIX] NEW position of screen subtitle: {screenSubTitlePos.x}, {screenSubTitlePos.y}");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
