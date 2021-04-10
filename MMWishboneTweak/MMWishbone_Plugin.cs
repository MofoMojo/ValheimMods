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
        public const string Version = "1.1";
        public const string ModName = "Wishbone Tweaks";
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
            if(PluginLoggingLevel == LoggingLevel.Verbose) Debug.Log(message);
        }        

    }

    internal static class Settings
    {
       
        public static ConfigEntry<bool> WishBoneTweakEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<float> DetectTinDistance;
        public static ConfigEntry<float> DetectCopperDistance;
        public static ConfigEntry<float> DetectDeathquitoDistance;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            WishBoneTweakEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Wishbone", "WishBoneTweakEnabled", true, "Allows the Wishbone to find Tin and Copper");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None , "Supported values are None, Normal, Verbose");
            DetectTinDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("Wishbone", "DetectTinDistance", 25f, "Allows the Wishbone to find Tin (Value > 0) and at what distance");
            DetectCopperDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("Wishbone", "DetectCopperDistance", 60f, "Allows the Wishbone to find Copper (Value > 0) and at what distance");
            DetectDeathquitoDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("Wishbone", "DetectDeathquitoDistance", 60f, "Allows the Wishbone to find Deathquitos (Value > 0) and at what distance");
        }

    }

}
