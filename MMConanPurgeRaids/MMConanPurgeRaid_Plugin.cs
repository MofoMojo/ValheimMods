using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace MofoMojo.MMConanPurgeRaids
{

    [BepInPlugin("MofoMojo.MMConanPurgeRaids", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MMConanPurgeRaids";
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
        public static ConfigEntry<bool> MMConanPurgeRaidsEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<int> PurgeLevel;
        public static ConfigEntry<bool> ApplyPlayerCountMultiplier;
        public static ConfigEntry<float> PerPlayerMultiplier;

        public static ConfigEntry<int> PurgePeriocity;
        public static ConfigEntry<bool> RestrictPurgeTime;
        public static ConfigEntry<float> PurgeRestrictionWeekdayEnd;
        public static ConfigEntry<float> PurgeRestrictionWeekdayStart;
        public static ConfigEntry<float> PurgeRestrictionWeekendEnd;
        public static ConfigEntry<float> PurgeRestrictionWeekendStart;
        public static ConfigEntry<int> PurgePreparationTime;
        public static ConfigEntry<int> PurgeDuration;
        public static ConfigEntry<int> MinPurgeOnlinePlayers;
        public static ConfigEntry<bool> AllowBuilding;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            MMConanPurgeRaidsEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Plugin", "MMConanPurgeRaidsEnabled", true, "Enable this mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("Plugin", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");

            PurgePeriocity = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Purge", "PurgePeriocity", 1, "How many purges can occur in a given day");
            RestrictPurgeTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Purge", "RestrictPurgeTime", true, "Supports Purges only during specific times of day");
            PurgePreparationTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Purge", "PurgePreparationTime", 5, "Supports Purges only during specific times of day");

            PurgeRestrictionWeekdayEnd = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("Purge", "PurgeRestrictionWeekdayEnd", 1, "How many purges can occur in a given day");

            PurgeDuration = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Purge", "PurgeDuration", 5, "How Long the purge should last");
            MinPurgeOnlinePlayers = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Purge", "MinPurgeOnlinePlayers", 1, "How many players must be playing before Purge will occur");
            AllowBuilding = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Purge", "AllowBuilding", true, "Players can build/repair during purge");

        }

    }
}
