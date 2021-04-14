using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMServerMessages
{
    [BepInPlugin("MofoMojo.MMServerMessages", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MMServerMessages";
        Harmony _Harmony;
        public static Plugin Instance;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public static float m_refresh = 0;

        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose,
            Debug
        }

        private void Awake()
        {

            Instance = this;
            Settings.Init();
            PluginLoggingLevel = Settings.MMServerMessagesPluginLoggingLevel.Value;

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
            if (PluginLoggingLevel > LoggingLevel.Verbose) Debug.Log(message);
        }

        public static void LogDebug(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel == LoggingLevel.Debug) Debug.Log(message);
        }

        public static void Update()
        {
            Refresh(Time.deltaTime);
        }

        public static void Refresh(float dt)
        {

            m_refresh += dt;

            // check every 60 seconds by default
            if (m_refresh < Settings.MMServerConfigRefreshInterval.Value) return;

            m_refresh = 0;

            Instance.Config.Reload();
            Settings.Init();
            Plugin.LogVerbose("Config reloaded");
        }

    }

    internal static class Settings
    {

        public static ConfigEntry<bool> MMServerMessagesEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> MMServerMessagesPluginLoggingLevel;

        public static ConfigEntry<bool> SleepNotificationEnabled;
        public static ConfigEntry<float> SleepCheckInterval;
        public static ConfigEntry<string> SleepMessage;
        public static ConfigEntry<MessageHud.MessageType> SleepMessageType;

        public static ConfigEntry<bool> MessageOfTheDayEnabled;
        public static ConfigEntry<float> MessageOfTheDayCheckInterval;
        public static ConfigEntry<string> MessageOfTheDay;
        public static ConfigEntry<MessageHud.MessageType> MessageOfTheDayType;

        public static ConfigEntry<float> MMServerConfigRefreshInterval;

        public static void Init()
        {
            MMServerMessagesEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMServerMessages", "MMServerMessagesEnabled", true, "Enables MMServerMessages mod");
            MMServerConfigRefreshInterval = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMServerMessages", "MMServerConfigRefreshInterval", 60, "How often, in seconds, to check for and/or refresh config");
            MMServerMessagesPluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "MMServerMessagesPluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
            SleepNotificationEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("SleepNotification", "SleepNotificationEnabled", true, "Enables SleepNotification");
            SleepCheckInterval = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("SleepNotification", "SleepCheckInterval", 11, "How often, in seconds, to check for sleepers");
            SleepMessage = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("SleepNotification", "SleepMessage", "Vikings are attempting to sleep", "Message to follow the #ofSleepers/#ofPlayers text");
            SleepMessageType = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<MessageHud.MessageType>("SleepNotification", "SleepMessageType", MessageHud.MessageType.TopLeft, "Message Type you want displayed");
            MessageOfTheDayEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MessageOfTheDay", "MessageOfTheDayEnabled", true, "Enables Message of the Day");
            MessageOfTheDayCheckInterval = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MessageOfTheDay", "MessageOfTheDayCheckInterval", 13, "How often, in seconds, to check for new clients to send the MOTD");
            MessageOfTheDayType = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<MessageHud.MessageType>("MessageOfTheDay", "MessageOfTheDayType", MessageHud.MessageType.Center, "Message Type you want displayed");
            MessageOfTheDay = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("MessageOfTheDay", "MessageOfTheDay", "Welcome to our Dedicated Valheim Server\nGipta!!!", "Message you wish to announce when players log on");
            
        }

    }
}
