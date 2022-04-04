using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace MofoMojo.MMDisableRandomEvents
{

    [BepInPlugin("MofoMojo.MMDisableRandomEvents", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MMDisableRandomEvents";
        public const string DisabledEventSectionName = "DisabledEvents";
        Harmony _Harmony;
        public static Plugin Instance;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public static List<RandomEvent> KnownRandomEvents = new List<RandomEvent>();

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

        public static void StartRandomEvent()
        {
            try
            {
                int count = Plugin.KnownRandomEvents.Count;
                int random = UnityEngine.Random.Range(0, count-1);
                if (RandEventSystem.instance != null)
                {
                    BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                    MethodInfo SetRandomEvent = RandEventSystem.instance.GetType().GetMethod("SetRandomEvent", flags);
                    if (null != SetRandomEvent)
                    {
                        RandomEvent randEvent = Plugin.KnownRandomEvents[random].Clone();
                        randEvent.m_enabled = true;
                        Plugin.Log($"Starting random event {randEvent.m_name}");
                        SetRandomEvent.Invoke(RandEventSystem.instance, new object[] { randEvent, Player.m_localPlayer.transform.position });

                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.LogError($"Exception calling SetRandomEvent. {ex.Message}");
            }

        }

    }

    internal static class Settings
    {
        public static ConfigEntry<bool> MMDisableRandomEventsEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static System.Collections.Generic.List<ConfigEntry<bool>> configEntries = new System.Collections.Generic.List<ConfigEntry<bool>>();

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            MMDisableRandomEventsEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Plugin", "MMDisableRandomEventsEnabled", true, "Enable this mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("Plugin", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");

        }

    }
}
