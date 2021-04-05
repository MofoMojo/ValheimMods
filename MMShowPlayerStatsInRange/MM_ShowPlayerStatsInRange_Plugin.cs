using System.Reflection;
using BepInEx.Configuration;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace MMShowPlayerStatsInRange
{
    [BepInPlugin("MofoMojo.MMShowPlayerStatsInRange", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "Show PlayerStats In Range";
        Harmony _Harmony;
        public static bool IsDebug = false;
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

        public static ConfigEntry<bool> ShowPlayerStatsInRange;
        public static ConfigEntry<float> ShowPlayerStatsRadius;
        public static ConfigEntry<string> PlayerStatLocation;
        public static ConfigEntry<string> PlayerStatFontName;
        public static ConfigEntry<int> PlayerStatFontSize;
        public static ConfigEntry<bool> showingPlayerStats;
        public static ConfigEntry<string> PlayerStatToggleKey;
        public static ConfigEntry<float> PlayerStatUpdateInterval;
        public static ConfigEntry<float> PlayerStatHealthPercentageWarningValue;
        public static ConfigEntry<float> PlayerStatHealthPercentageCriticalValue;
        public static ConfigEntry<float> PlayerStatHealthPercentageMediumValue;
        public static ConfigEntry<float> PlayerStatHealthWarningValue;
        public static ConfigEntry<float> PlayerStatHealthCriticalValue;
        public static ConfigEntry<float> PlayerStatHealthMediumValue;
        public static ConfigEntry<Color> PlayerStatHealthNormalColor;
        public static ConfigEntry<Color> PlayerStatHealthMediumColor;
        public static ConfigEntry<Color> PlayerStatHealthWarningColor;
        public static ConfigEntry<Color> PlayerStatHealthCriticalColor;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            ShowPlayerStatsInRange = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PlayerStats", "ShowPlayerStatsInRange", true, "When enabled, shows Player Stats in UI in range. Criticality wins. I.e. if your health percentage is 100, but your health is 25 then health level is medium");
            ShowPlayerStatsRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "ShowPlayerStatsRadius", 64f, "Sets the radius for finding players");
            PlayerStatLocation = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("PlayerStats", "PlayerStatLocation", "50%,3%", "Location on the screen to show the player stats (x,y) or (x%,y%). Also, Use mouse cursor to change position in game");
            PlayerStatFontName = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("PlayerStats", "PlayerStatFontName", "Norsebold", "Name of the font to use, possible values Norsebold, AveriaSerifLibre-Bold");
            PlayerStatFontSize = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PlayerStats", "PlayerStatFontSize", 24, "Size of the font to use");
            PlayerStatToggleKey = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("PlayerStats", "PlayerStatToggleKey", "[", "Key to use for toggling player stats. https://docs.unity3d.com/ScriptReference/KeyCode.html");
            PlayerStatHealthPercentageMediumValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthPercentageMediumValue", 75f, "HealthPercentage to be considered medium. Anything higher is normal");
            PlayerStatHealthPercentageWarningValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthPercentageWarningValue", 50f, "HealthPercentage to be considered warning. Anything higher is medium");
            PlayerStatHealthPercentageCriticalValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthPercentageCriticalValue", 25f, "HealthPercentage to be considered critical.Anything higher is warning");
            PlayerStatHealthMediumValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthMediumValue", 30f, "Health value to be considered medium. Anything higher is normal");
            PlayerStatHealthWarningValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthWarningValue", 20f, "Health value to be considered warning . Anything higher is medium");
            PlayerStatHealthCriticalValue = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatHealthCriticalValue", 10f, "Health value to be considered critical . Anything higher is warning");

            PlayerStatHealthNormalColor = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Color>("PlayerStats", "PlayerStatHealthNormalColor", Color.green, "Sets the color for playerstatus at Normal Level, https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html. Supported color names: aqua,black,blue,brown,cyan,darkblue,fuchsia,green,grey,lightblu,lime,magenta,maroon,navy,olive,orange,purple,red,silver,teal,white,yellow");
            PlayerStatHealthMediumColor = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Color>("PlayerStats", "PlayerStatHealthMediumColor", Color.white, "Sets the color for playerstatus at medium Level, can also use color hex tags, like FFFFFF, FF0000, etc,.");
            PlayerStatHealthWarningColor = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Color>("PlayerStats", "PlayerStatHealthWarningColor", Color.yellow, "Sets the color for playerstatus at warning Level");
            PlayerStatHealthCriticalColor = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Color>("PlayerStats", "PlayerStatHealthCriticalColor", new Color(1, 0, 0), "Sets the color for playerstatus at critical Level");
            PlayerStatUpdateInterval = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("PlayerStats", "PlayerStatUpdateInterval", 30f, "How often to update. This value represents frames, so if you're getting 30 FPS and the value is 30, you'll update every second.");
            showingPlayerStats = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PlayerStats", "showingPlayerStats", false, "Used by mod, don't modify");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }
}
