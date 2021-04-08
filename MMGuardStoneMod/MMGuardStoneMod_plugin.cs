using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;

namespace MofoMojo.MMGuardStoneMod
{
    [BepInPlugin("MofoMojo.MMGuardStoneMod", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MMGuardStoneMod";
        public const string NoMonsterEffectAreaName = "NoMonsterArea";
        Harmony _Harmony;
        public static Plugin Instance;
        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose
        }

        public enum WardInteractBehavior
        {
            Original,
            OwnerOnly,
            OwnerAndPermitted,
            All
        }

        public enum WardBehavior
        {
            Original,
            NoMonsters
        }

        private void Awake()
        {

            Instance = this;
            Settings.Init();

            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }

        public static void Log(string message)
        {
            message = $"{ModName}: {message}";
            if (Settings.PluginLoggingLevel.Value > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            message = $"{ModName}: {message}";
            if (Settings.PluginLoggingLevel.Value > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            message = $"{ModName}: {message}";
            if (Settings.PluginLoggingLevel.Value > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            message = $"{ModName}: {message}";
            if (Settings.PluginLoggingLevel.Value == LoggingLevel.Verbose) Debug.LogError(message);
        }

    }

    internal static class Settings
    {

        public static ConfigEntry<bool> MMGuardStoneModEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<float> GuardStoneRadius;
        public static ConfigEntry<KeyCode> InteractModifier;
        public static ConfigEntry<Plugin.WardInteractBehavior> WardInteractBehavior;
        public static ConfigEntry<Plugin.WardBehavior> WardBehavior;
        public static void Init()
        {
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
            MMGuardStoneModEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMGuardStoneMod", "MMGuardStoneModEnabled", true, "Enables MMGuardStoneMod mod");
            GuardStoneRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMGuardStoneMod", "GuardStoneRadius", 32f, "Sets the GuardStone radius");
            InteractModifier = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<KeyCode>("MMGuardStoneMod", "InteractModifier", KeyCode.LeftShift, "Sets the interact modifier for players to add themselves to permitted list");
            WardInteractBehavior = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.WardInteractBehavior>("MMGuardStoneMod", "WardInteractBehavior", Plugin.WardInteractBehavior.All, "Controls the interaction behavior of the Wards. Must be the same between client and server");
            WardBehavior = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.WardBehavior>("MMGuardStoneMod", "WardBehavior", Plugin.WardBehavior.NoMonsters, "Controls the behavior of the Wards. Must be the same between client and server");
        }

    }
}
