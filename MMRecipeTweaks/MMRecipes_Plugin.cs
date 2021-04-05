using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace MMRecipeTweaks
{
    [BepInPlugin("MofoMojo.MMRecipeTweaks", Plugin.ModName, Plugin.Version)]
    [BepInDependency("com.bepinex.plugins.jotunnlib")]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MMRecipeTweaks";
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

        public static ConfigEntry<bool> FishingRodRecipeEnabled;
        public static ConfigEntry<bool> LoxMeatSurpriseRecipeEnabled;
        public static ConfigEntry<bool> FishingBaitRecipeEnabled;
        public static ConfigEntry<bool> ChainsRecipeEnabled;
        public static ConfigEntry<bool> BronzeTweakEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("Wishbone", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
            FishingRodRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "FishingRodRecipeEnabled", true, "Enables  a recipe for Fishing Rods");
            FishingBaitRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "FishingBaitRecipeEnabled", true, "Enables  a recipe for bait made from Necktails");
            LoxMeatSurpriseRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "LoxMeatSurpriseRecipeEnabled", true, "Enables a recipe and item for Lox Meat Surprise");
            ChainsRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "ChainsRecipeEnabled", true, "Enables a recipe for making chains (4 Iron = 1 chain)");
            BronzeTweakEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("LoggingLevel", "BronzeTweakEnabled", true, "Changes Bronze Recipe from 2 copper+1 tin = 1 bronze to 2+1=3 (and the x5 recipe too)");
        }

    }
}
