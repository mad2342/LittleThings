using Harmony;
using System.Reflection;
using System;
using Newtonsoft.Json;
using System.IO;

namespace LittleThings
{
    public static class LittleThings
    {
        public static string LogPath;
        public static string ModDirectory;
        internal static Settings Settings;

        // BEN: Debug (0: nothing, 1: errors, 2:all)
        internal static int DebugLevel = 2;

        public static void Init(string directory, string settings)
        {
            ModDirectory = directory;

            LogPath = Path.Combine(ModDirectory, "LittleThings.log");
            File.CreateText(LittleThings.LogPath);

            var harmony = HarmonyInstance.Create("de.mad.LittleThings");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            try
            {
                Settings = JsonConvert.DeserializeObject<Settings>(settings);
            }
            catch (Exception)
            {
                Settings = new Settings();
            }
        }
    }
}
