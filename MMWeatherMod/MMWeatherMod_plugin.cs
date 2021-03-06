using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMWeatherMod
{
    [BepInPlugin("MofoMojo.MMWeatherMod", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.1";
        public const string ModName = "MMWeatherMod";
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
        public static ConfigEntry<int> nexusId;
        public static ConfigEntry<bool> MMWeatherModEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<int> PrayerFrequency;
        public static ConfigEntry<float> SuccessChance;
        public static ConfigEntry<float> AngryChance;
        public static ConfigEntry<bool> TrophyKillInfluence;

        public static void Init()
        {
            MMWeatherModEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMWeatherMod", "MMWeatherModEnabled", true, "Enables MMWeatherMod mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
            PrayerFrequency = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("MMWeatherMod", "PrayerFrequency", 60, "How many seconds must pass between attempts");
            SuccessChance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMWeatherMod", "SuccessChance", 33f, "What are the odds Odin will grant your prayer wish");
            AngryChance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMWeatherMod", "AngryChance", 25f, "What are the odds you'll anger Odin if your prayer is unsuccessful");
            TrophyKillInfluence = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMWeatherMod", "TrophyKillInfluence", true, "Each new mob type you've slain gives you a 2% increase in success");
            nexusId = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("General", "NexusID", 946, "Nexus mod ID for updates");
        }

    }
}
