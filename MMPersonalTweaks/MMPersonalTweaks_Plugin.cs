using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMPersonalTweaks
{
    [BepInPlugin("MofoMojo.MMPersonalTweaks", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.1";
        public const string ModName = "MofoMojo's Personal Tweaks";
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
            if (PluginLoggingLevel == LoggingLevel.Verbose) Debug.LogError(message);
        }
    }

    internal static class Settings
    {

        public static ConfigEntry<bool> FeatherMultiplierEnabled;
        public static ConfigEntry<bool> FishingInOceanMultiplierEnabled;
        public static ConfigEntry<bool> RememberLastConnectedIpEnabled;
         public static ConfigEntry<string> LastConnectedIP;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            FeatherMultiplierEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Tweaks", "FeatherMultiplierEnabled", true, "Birds will drop additional feathers");
            FishingInOceanMultiplierEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Tweaks", "FishingInOceanMultiplierEnabled", true, "When fishing in the ocean, you get a multiplier on fish caught");
            RememberLastConnectedIpEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Tweaks", "RememberLastConnectedIpEnabled", true, "Remember the last server/ip address connected - not implemented yet");
            LastConnectedIP = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("Tweaks", "LastConnectedIP", "", "This is the last connect string (used for storage)");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }
}
