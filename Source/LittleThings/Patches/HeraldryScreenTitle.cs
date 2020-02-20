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

            public static void Prefix(HeraldryCreatorPanel __instance, LocalizableText ___screenTitle, LocalizableText ___mechInfoName)
            {
                try
                {
                    Logger.Debug("[HeraldryCreatorPanel_SetData_PREFIX] Fix position of screen title");

                    TMP_FontAsset ___mechInfoName_m_baseFont = (TMP_FontAsset)AccessTools.Field(typeof(LocalizableText), "m_baseFont").GetValue(___mechInfoName);

                    ___screenTitle.fontSize = 44;
                    ___screenTitle.SetFont(___mechInfoName_m_baseFont);
                    //___screenTitle.text = "Heraldry Customization";

                    RectTransform rt = ___screenTitle.GetComponent<RectTransform>();
                    Vector3 pos = rt.localPosition;

                    pos.x = -23;
                    pos.y = 30;
                    rt.localPosition = pos;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
