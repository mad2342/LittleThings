using System;
using System.IO;
using System.Linq;
using BattleTech.Save.Core;
using Harmony;

namespace LittleThings.Patches
{
    class GalaxyDeleteFile
    {
        [HarmonyPatch(typeof(GOGFileOperations), "DeleteFile")]
        public static class GOGFileOperations_DeleteFile_Patch
        {
            public static bool Prepare()
            {
                Type GOGFileOperations = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            from type in assembly.GetTypes()
                            where type.FullName == "BattleTech.Save.Core.GOGFileOperations"
                            select type).FirstOrDefault();

                return GOGFileOperations != null && LittleThings.Settings.FixGalaxyDeleteSaves;
            }

            public static void Prefix(GOGFileOperations __instance, string path)
            {
                try
                {
                    /* Reference
                    try
                    {
                        GalaxyInstance.Storage().FileDelete(path);
                    }
                    catch (Exception message)
                    {
                        Debug.LogError(message);
                    }
                    */

                    if (String.IsNullOrEmpty(LittleThings.Settings.GalaxySaveGameRelativeRootPath) && String.IsNullOrEmpty(LittleThings.Settings.GalaxySaveGameAbsolutePathOverride))
                    {
                        Logger.Debug($"[FileOperations_DeleteFile_PREFIX] LittleThings.Settings.GalaxySaveGameRelativeRootPath is NOT SET");
                        Logger.Debug($"[FileOperations_DeleteFile_PREFIX] LittleThings.Settings.GalaxySaveGameAbsolutePathOverride is NOT SET");
                        Logger.Debug($"[FileOperations_DeleteFile_PREFIX] Exiting");
                        return;
                    }

                    Logger.Debug($"[FileOperations_DeleteFile_PREFIX] path: {path}");
                    Logger.Debug($"[FileOperations_DeleteFile_PREFIX] LittleThings.Settings.GalaxySaveGameRelativeRootPath: {LittleThings.Settings.GalaxySaveGameRelativeRootPath}");
                    Logger.Debug($"[FileOperations_DeleteFile_PREFIX] LittleThings.Settings.GalaxySaveGameAbsolutePathOverride: {LittleThings.Settings.GalaxySaveGameAbsolutePathOverride}");

                    string finalPath;
                    if (!String.IsNullOrEmpty(LittleThings.Settings.GalaxySaveGameAbsolutePathOverride))
                    {
                        finalPath = Path.Combine(LittleThings.Settings.GalaxySaveGameAbsolutePathOverride, path);
                    }
                    else
                    {
                        string homePath = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? Environment.GetEnvironmentVariable("HOME") : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                        Logger.Debug($"[FileOperations_DeleteFile_PREFIX] homePath: {homePath}");
                        finalPath = Path.Combine(homePath, LittleThings.Settings.GalaxySaveGameRelativeRootPath, path);
                    }

                    Logger.Debug($"[FileOperations_DeleteFile_PREFIX] finalPath: {finalPath}");
                    Logger.Debug($"[FileOperations_DeleteFile_PREFIX] File.Exists(finalPath): {File.Exists(finalPath)}");

                    if (File.Exists(finalPath))
                    {
                        File.Delete(finalPath);
                        Logger.Debug($"[FileOperations_DeleteFile_PREFIX] File deleted!");
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
