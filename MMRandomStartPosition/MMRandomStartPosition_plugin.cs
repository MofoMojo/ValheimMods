using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMRandomStartPosition
{
    [BepInPlugin("MofoMojo.MMRandomStartPosition", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "2.0";
        public const string ModName = "MMRandomStartPosition";
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
        public static ConfigEntry<bool> MMRandomStartPositionEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<bool> DisableValkryieRide;
        public static ConfigEntry<float> MaxXDistance;
        public static ConfigEntry<float> MaxZDistance;
        public static ConfigEntry<float> MinXDistance;
        public static ConfigEntry<float> MinZDistance;
        //public static ConfigEntry<bool> AddStartTemple;
        public static ConfigEntry<Heightmap.Biome> Biome;
        public static ConfigEntry<Heightmap.BiomeArea> BiomeAreaType;
        public static ConfigEntry<int> MaxSpawnPointChecks;
        public static ConfigEntry<bool> IgnoreWorldGeneratorConstraints;
        public static ConfigEntry<bool> RandomSpawnOnDeath;
        public static ConfigEntry<bool> IgnoreWaterDepthCheck;

        public static void Init()
        {
            MMRandomStartPositionEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "MMRandomStartPositionEnabled", true, "Enables MMRandomStartPosition mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
            DisableValkryieRide = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "DisableValkryieRide", false, "Disables the ride in on the Valkyrie");
            MaxXDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomStartPosition", "MaxXDistance", 5000f, "Constrain X axis search from center of map. This is clamped between 0 and 10000");
            MaxZDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomStartPosition", "MaxZDistance", 5000f, "Constrain Z axis search from center of map. This is clamped between 0 and 10000");
            MinXDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomStartPosition", "MinXDistance", 500f, "Constrain X axis search from center of map. This is the minimum distance you wish to be away from center X/0 position");
            MinZDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomStartPosition", "MinZDistance", 500f, "Constrain Z axis search from center of map. This is the minimum distance you wish to be away from center z/0 position");
            //AddStartTemple = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "AddStartTemple", false, "VERY FAULTY BEHAVIOR. DO NOT USE. Adds an additional start temple near you");
            Biome = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Heightmap.Biome> ("MMRandomStartPosition", "Biome", Heightmap.Biome.Meadows, "Attempts to spawns you into this type of Biome");
            BiomeAreaType = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Heightmap.BiomeArea>("MMRandomStartPosition", "BiomeAreaType", Heightmap.BiomeArea.Median, "Attempts to spawn you into this type of BiomeArea");
            MaxSpawnPointChecks = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("MMRandomStartPosition", "MaxSpawnPointChecks", 1000 ,"The maximum number of times mod will search for a good spawnpoint before handing off to normal spawn code");
            IgnoreWorldGeneratorConstraints = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "IgnoreWorldGeneratorConstraints", false, "The mod attempts to set biome search constraints based on values in WorldGenerator. Set this to true to disable these constraints.");
            RandomSpawnOnDeath = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "RandomSpawnOnDeath", false, "If true, You will generate a new respawn point on every death");
            IgnoreWaterDepthCheck = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomStartPosition", "IgnoreWaterDepthCheck", false, "If true, this will not ignore water depth level for spawn checks. I don't recommend this you masochist!!!");
            nexusId = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("General", "NexusID", 952, "Nexus mod ID for updates");
        }

    }
}
