using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MMGuardStoneMod
{
    [BepInPlugin("MofoMojo.MMGuardStoneMod", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MMGuardStoneMod";
        public const string MonsterEffectArea = "NoMonsterArea";
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

        public static ConfigEntry<bool> MMGuardStoneModEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<float> GuardStoneRadius;
        public static void Init()
        {
            MMGuardStoneModEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMGuardStoneMod", "MMGuardStoneModEnabled", true, "Enables MMGuardStoneMod mod");
            GuardStoneRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMGuardStoneMod", "GuardStoneRadius", 32f, "Sets the GuardStone radius");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }
}
