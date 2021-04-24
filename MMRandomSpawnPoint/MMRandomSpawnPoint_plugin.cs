using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMRandomSpawnPoint
{
    [BepInPlugin("MofoMojo.MMRandomSpawnPoint", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "2.0";
        public const string ModName = "MMRandomSpawnPoint";
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

    }

    internal static class Settings
    {
        public static ConfigEntry<int> nexusId;
        public static ConfigEntry<bool> MMRandomSpawnPointEnabled;
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
        public static ConfigEntry<bool> RandomSpawnOnDeathIfNoBed;
        public static ConfigEntry<bool> IgnoreWaterDepthCheck;

        public static void Init()
        {
            MMRandomSpawnPointEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomSpawnPoint", "MMRandomSpawnPointEnabled", false, "Enables MMRandomSpawnPoint mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose, Debug");
            DisableValkryieRide = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomSpawnPoint", "DisableValkryieRide", false, "Disables the ride in on the Valkyrie");
            MaxXDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomSpawnPoint", "MaxXDistance", 5000f, "Constrain X axis search from center of map. This is clamped between 0 and 10000");
            MaxZDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomSpawnPoint", "MaxZDistance", 5000f, "Constrain Z axis search from center of map. This is clamped between 0 and 10000");
            MinXDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomSpawnPoint", "MinXDistance", 500f, "Constrain X axis search from center of map. This is the minimum distance you wish to be away from center X/0 position");
            MinZDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MMRandomSpawnPoint", "MinZDistance", 500f, "Constrain Z axis search from center of map. This is the minimum distance you wish to be away from center z/0 position");
            Biome = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Heightmap.Biome>("MMRandomSpawnPoint", "Biome", Heightmap.Biome.Meadows, "Attempts to spawns you into this type of Biome");
            BiomeAreaType = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Heightmap.BiomeArea>("MMRandomSpawnPoint", "BiomeAreaType", Heightmap.BiomeArea.Median, "Attempts to spawn you into this type of BiomeArea");
            MaxSpawnPointChecks = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("MMRandomSpawnPoint", "MaxSpawnPointChecks", 1000, "The maximum number of times mod will search for a good spawnpoint before handing off to normal spawn code");
            IgnoreWorldGeneratorConstraints = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomSpawnPoint", "IgnoreWorldGeneratorConstraints", false, "The mod attempts to set biome search constraints based on values in WorldGenerator. Set this to true to disable these constraints.");
            RandomSpawnOnDeath = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomSpawnPoint", "RandomSpawnOnDeath", false, "If true, You will generate a new respawn point on every death");
            RandomSpawnOnDeathIfNoBed = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomSpawnPoint", "RandomSpawnOnDeathIfNoBed", false, "If true, You will generate a new respawn point on death, IF you don't have a bed spawn point");
            IgnoreWaterDepthCheck = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMRandomSpawnPoint", "IgnoreWaterDepthCheck", false, "If true, this will not ignore water depth level for spawn checks. I don't recommend this you masochist!!!");
            nexusId = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("General", "NexusID", 952, "Nexus mod ID for updates");
        }

    }
}
