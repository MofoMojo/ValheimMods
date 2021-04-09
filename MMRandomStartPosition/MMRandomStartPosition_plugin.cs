using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMRandomStartPosition
{
    [BepInPlugin("MofoMojo.MMRandomStartPosition", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.1";
        public const string ModName = "MMRandomStartPosition";
        Harmony _Harmony;
        public static Plugin Instance;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose
        }

        private void Awake()
        {

            Instance = this;
            Settings.Init();
            PluginLoggingLevel = Settings.PluginLoggingLevel.Value;

            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }

        public static void Log(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel == LoggingLevel.Verbose) Debug.Log(message);
        }

    }

    internal static class Settings
    {

        public static ConfigEntry<bool> MMRandomStartPositionEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<bool> DisableValkryieRide;
        public static ConfigEntry<float> MaxXDistance;
        public static ConfigEntry<float> MaxZDistance;
        public static ConfigEntry<float> MinXDistance;
        public static ConfigEntry<float> MinZDistance;

        public static void Init()
        {
            MMRandomStartPositionEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "MMRandomStartPositionEnabled", true, "Enables MMRandomStartPosition mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
            DisableValkryieRide = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "DisableValkryieRide", false, "Disables the ride in on the Valkyrie");
            MaxXDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomStartPosition", "MaxXDistance", 5000f, "Constrain X axis search from center of map. This is clamped between 0 and WorldGenerator.meadowsMaxDistance = 5000");
            MaxZDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomStartPosition", "MaxZDistance", 5000f, "Constrain Z axis search from center of map. This is clamped between 0 and WorldGenerator.meadowsMaxDistance = 5000");
            MinXDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomStartPosition", "MinXDistance", 100f, "Constrain X axis search from center of map. This is the minimum distance you wish to be away from center X/0 position");
            MinZDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomStartPosition", "MinZDistance", 100f, "Constrain Z axis search from center of map. This is the minimum distance you wish to be away from center z/0 position");

        }

    }
}
