using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace MMPlayerMiniMapMod
{
    [BepInPlugin("MofoMojo.MMPlayerMiniMapMod", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MM's Player Minimap Toggle";
        Harmony _Harmony;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose
        }

        public static Plugin Instance;

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
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<bool> HideMapToggleEnabled;
        public static ConfigEntry<string> HideMapToggleKey;
        public static ConfigEntry<bool> AdjustMapMarkerScale;
        public static ConfigEntry<float> SmallMarkerLocalScale;
        public static ConfigEntry<float> LargeMarkerLocalScale;
        public static ConfigEntry<bool> IsMapVisible;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            HideMapToggleEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MiniMap", "HideMapToggleEnabled", true, "Turn on ability to hide map mode");
            HideMapToggleKey = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("MiniMap", "HideMapToggleKey", "]", "Key to use for toggling player stats. https://docs.unity3d.com/ScriptReference/KeyCode.html");
            IsMapVisible = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MiniMap", "IsMapVisible", true, "Tracks status of the map");

            AdjustMapMarkerScale = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MiniMap", "AdjustMapMarkerScale", true, "Turn on ability to adjust the direction marker scales");
            SmallMarkerLocalScale = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MiniMap", "SmallMarkerLocalScale", 1.5f, "Scales the direction marker of the small map");
            LargeMarkerLocalScale = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MiniMap", "LargeMarkerLocalScale", 2f, "Scales the direction marker of the large map");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }
}
