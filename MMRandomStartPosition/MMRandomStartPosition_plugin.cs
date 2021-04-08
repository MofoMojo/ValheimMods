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
        public const string Version = "1.0";
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

        public static void Init()
        {
            MMRandomStartPositionEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "MMRandomStartPositionEnabled", true, "Enables MMRandomStartPosition mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.Normal, "Supported values are None, Normal, Verbose");
            DisableValkryieRide = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "DisableHuginRide", true, "Disables the ride in with Hugin");
        }

    }
}
