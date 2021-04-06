using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMExplorationTweaks
{
    [BepInPlugin("MofoMojo.MMExploration", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "Exploration Tweaks";
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
            if (_Harmony != null) _Harmony.UnpatchAll(null);
        }

        public static void Log(string message)
        {
            if (PluginLoggingLevel > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            if (PluginLoggingLevel == LoggingLevel.Verbose) Debug.LogError(message);
        }
    }

    internal static class Settings
    {

        public static ConfigEntry<bool> ExploreOnShipRadiusEnabled;
        public static ConfigEntry<float> ExploreOnShipRadius;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            ExploreOnShipRadiusEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Ship", "ExploreOnShipRadiusEnabled", true, "Enable Exploration Radius when on the deck of a boat");
            ExploreOnShipRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("Ship", "ExploreOnShipRadius", 250f, "Sets the exploration radius when on a boat");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }
}
