using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;


namespace MofoMojo.MMWishboneTweak
{
    [BepInPlugin("MofoMojo.MMWishboneTweak", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.2";
        public const string ModName = "MM's Wishbone Tweaks";
        Harmony _Harmony;
        public static Plugin Instance;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
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
            PluginLoggingLevel = Settings.PluginLoggingLevel.Value;

            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }

        public static void Log(string message)
        {
            if(PluginLoggingLevel > LoggingLevel.None)  Debug.Log(message);
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
            if(PluginLoggingLevel >= LoggingLevel.Verbose) Debug.Log(message);
        }

        public static void LogDebug(string message)
        {
            if (PluginLoggingLevel == LoggingLevel.Debug) Debug.Log(message);
        }

    }

    internal static class Settings
    {
        public static ConfigEntry<int> nexusId;
        public static ConfigEntry<bool> WishBoneTweakEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<float> DetectTinDistance;
        public static ConfigEntry<float> DetectCopperDistance;
        public static ConfigEntry<float> DetectDeathsquitoDistance;
        public static ConfigEntry<float> DetectSilverDistance;
        public static ConfigEntry<float> DetectMudPileDistance;
        public static ConfigEntry<float> DetectBuriedDistance;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            WishBoneTweakEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Wishbone", "WishBoneTweakEnabled", true, "Allows the Wishbone to find Tin and Copper");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None , "Supported values are None, Normal, Verbose");
            DetectTinDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("WishboneDestructibles", "DetectTinDistance", 25f, "Allows the Wishbone to find Tin (Value > 0) and at what distance");
            DetectCopperDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("WishboneDestructibles", "DetectCopperDistance", 60f, "Allows the Wishbone to find Copper (Value > 0) and at what distance");
            DetectSilverDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("WishboneDestructibles", "DetectSilverDistance", 20f, "Allows the Wishbone to find Silver (Value > 0) and at what distance");
            DetectMudPileDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("WishboneDestructibles", "DetectMudPileDistance", 20f, "Allows the Wishbone to find Mud Piles (Value > 0) and at what distance");
            DetectBuriedDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("WishbonePieces", "DetectBuriedDistance", 20f, "Allows the Wishbone to find Buried Objects (Value > 0) and at what distance");
            DetectDeathsquitoDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("WishboneHumanoids", "DetectDeathsquitoDistance", 60f, "Allows the Wishbone to find Deathquitos (Value > 0) and at what distance");
            nexusId = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("General", "NexusID", 906, "Nexus mod ID for updates");
        }

    }

}
