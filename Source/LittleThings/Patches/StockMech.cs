using System;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;

namespace LittleThings.Patches
{
    public static class StockMech
    {
        // This method duplicates functionality of MechLabStockInfoPopup.StockMechDefLoaded
        static void OverrideStockMechDefLoaded(this MechLabStockInfoPopup __instance, string id, MechDef def)
        {
            Logger.LogLine("[MechLabStockInfoPopup.OverrideStockMechDefLoaded] called.");

            MechDef ___stockMechDef = Traverse.Create(__instance).Field("stockMechDef").GetValue<MechDef>();
            MechBayMechInfoWidget ___mechInfoWidget = Traverse.Create(__instance).Field("mechInfoWidget").GetValue<MechBayMechInfoWidget>();
            DataManager ___dataManager = Traverse.Create(__instance).Field("dataManager").GetValue<DataManager>();
            LocalizableText ___descriptionText = Traverse.Create(__instance).Field("descriptionText").GetValue<LocalizableText>();

            ___stockMechDef = def;
            ___mechInfoWidget.SetData(___stockMechDef, ___dataManager);
            ___descriptionText.SetText(___stockMechDef.Description.Details, Array.Empty<object>());
        }



        // Change MechDef for stock popup in MechLab if applicable...
        [HarmonyPatch(typeof(MechLabStockInfoPopup), "LoadStockMech")]
        public static class MechLabStockInfoPopup_LoadStockMech_Patch
        {
            public static bool Prepare()
            {
                return LittleThings.Settings.EnableStockMechReferenceViaMechDefDescriptionModel;
            }

            public static bool Prefix(MechLabStockInfoPopup __instance, ref string ___stockMechDefId, MechDef ___baseMechDef, UIManager ___uiManager)
            {
                try
                {
                    Logger.LogLine("[MechLabStockInfoPopup_LoadStockMech_PREFIX] ___baseMechDef.Description.Id: " + ___baseMechDef.Description.Id);

                    if (!string.IsNullOrEmpty(___baseMechDef.Description.Model))
                    {
                        Logger.LogLine("[MechLabStockInfoPopup_LoadStockMech_PREFIX] ___baseMechDef.Description.Model: " + ___baseMechDef.Description.Model);

                        ___stockMechDefId = ___baseMechDef.Description.Model.Replace("model", "mechdef");
                        Logger.LogLine("[MechLabStockInfoPopup_LoadStockMech_PREFIX] ___stockMechDefId: " + ___stockMechDefId);



                        LoadRequest loadRequest = ___uiManager.dataManager.CreateLoadRequest(null, false);
                        // Created extension method MechLabStockInfoPopup.OverrideStockMechDefLoaded for callback
                        loadRequest.AddLoadRequest<MechDef>(BattleTechResourceType.MechDef, ___stockMechDefId, new Action<string, MechDef>(__instance.OverrideStockMechDefLoaded), false);
                        loadRequest.ProcessRequests(10u);

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                    return true;
                }
            }
        }
    }
}
