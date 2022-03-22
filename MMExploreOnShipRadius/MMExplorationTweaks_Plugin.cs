using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace MofoMojo.MMExplorationTweaks
{
    [Flags]
    enum Layers
    {
        Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        Unused3 = 1 << 3,
        Water = 1 << 4,
        UI = 1 << 5,
        Unused6 = 1 << 6,
        Unused7 = 1 << 7,
        effect = 1 << 8,
        character = 1 << 9,
        piece = 1 << 10,
        terrain = 1 << 11,
        item = 1 << 12,
        ghost = 1 << 13,
        character_trigger = 1 << 14,
        static_solid = 1 << 15,
        piece_nonsolid = 1 << 16,
        character_ghost = 1 << 17,
        hitbox = 1 << 18,
        skybox = 1 << 19,
        Default_small = 1 << 20,
        WaterVolume = 1 << 21,
        weapon = 1 << 22,
        blocker = 1 << 23,
        pathblocker = 1 << 24,
        viewblock = 1 << 25,
        character_net = 1 << 26,
        character_noenv = 1 << 27,
        vehicle = 1 << 28,
        Unused29 = 1 << 29,
        Unused30 = 1 << 30,
        smoke = 1 << 31,
    }

    [BepInPlugin("MofoMojo.MMExplorationTweaks", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "2.5";
        public const string ModName = "MMExplorationTweaks";
        Harmony _Harmony;
        public static Plugin Instance;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public static float[] HourImpact;
        public static bool PlayerDied = false;

        const Layers TriggerLayers = Layers.terrain | Layers.viewblock | Layers.static_solid | Layers.Water | Layers.smoke | Layers.piece | Layers.piece_nonsolid;

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
            string[] stringHourImpact = Settings.HourImpact.Value.Split(',');

            HourImpact = new float[stringHourImpact.Length];
            int index = 0;

            foreach(string value in stringHourImpact)
            {
                HourImpact[index] = float.Parse(value);
                LogVerbose("Adding hour impact: " + value + ", index: " + index);
                index++;
            }

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

        public static int GetMask
        {
            get{
                return (int)TriggerLayers;
            }
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

        private static int GetHour()
        {
            float fraction = EnvMan.m_instance.m_smoothDayFraction;
            int hour = (int)(fraction * 24);
           // LogVerbose("Hour {" + hour + "}, Fraction {" + fraction + "}");
            
            return hour;
        }

        public static float GetHourImpact()
        {
            float impact = 0;
            if (!Settings.ExploreTimeTweaksEnabled.Value) return impact;
            
            int hour = GetHour();
            impact = HourImpact[hour];

           // LogVerbose("Hour Impact: " + impact);

            return impact;
        }

        public static float GetWeatherImpact()
        {
            return GetWeatherImpact(out float temp);
        }

        public static float GetWeatherImpact(out float siteDistance)
        {
            float impact = 0;
            siteDistance = Settings.MaximumExplorationDistance.Value;

            if (!Settings.ExploreWeatherTweaksEnabled.Value)
            {
                return impact;
            }

            if (Settings.FogDensityAffectsExplorationDistance.Value) siteDistance = CalculateAdjustedFogDistance(siteDistance);


            string weathername = EnvMan.instance.m_currentEnv.m_name.ToLower();

            Plugin.LogVerbose($"Weather: {weathername}");
            switch(weathername)
            {
                case "clear":
                    impact = Settings.Clear.Value;
                    break;
                case "twilight_clear":
                    impact = Settings.Twilight_Clear.Value;
                    break;
                case "misty":
                    impact = Settings.Misty.Value;
                    break;
                case "darklands_dark":
                    impact = Settings.Darklands_dark.Value;
                    break;
                case "heath clear":
                    impact = Settings.Heath_clear.Value;
                    break;
                case "deepforest_mist":
                    impact = Settings.DeepForest_Mist.Value;
                    break;
                case "gdking":
                    impact = Settings.GDKing.Value;
                    break;
                case "rain":
                    impact = Settings.Rain.Value;
                    break;
                case "lightrain":
                    impact = Settings.LightRain.Value;
                    break;
                case "thunderstorm":
                    impact = Settings.ThunderStorm.Value;
                    break;
                case "eikthyr":
                    impact = Settings.Eikthyr.Value;
                    break;
                case "goblinking":
                    impact = Settings.GoblinKing.Value;
                    break;
                case "nofogts":
                    impact = Settings.nofogts.Value;
                    break;
                case "swamprain":
                    impact = Settings.SwampRain.Value;
                    break;
                case "bonemass":
                    impact = Settings.Bonemass.Value;
                    break;
                case "snow":
                    impact = Settings.Snow.Value;
                    break;
                case "twilight_snow":
                    impact = Settings.Twilight_Snow.Value;
                    break;
                case "twilight_snowstorm":
                    impact = Settings.Twilight_SnowStorm.Value;
                    break;
                case "snowstorm":
                    impact = Settings.SnowStorm.Value;
                    break;
                case "moder":
                    impact = Settings.Clear.Value;
                    break;
                case "ashrain":
                    impact = Settings.Clear.Value;
                    break;
                case "crypt":
                    impact = Settings.Clear.Value;
                    break;
                case "sunkencrypt":
                    impact = Settings.Clear.Value;
                    break;
                default :
                    impact = Settings.Default.Value;
                    break;
            }
            //LogVerbose("Weather Impact: " + impact);
            return impact;
        }

        private static float CalculateAdjustedFogDistance(float siteDistance)
        {
            float fogstartDistance = RenderSettings.fogStartDistance;
            float fogEndDistance = RenderSettings.fogEndDistance;
            float fogDensity = RenderSettings.fogDensity;

            //fogDistance = fogstartDistance - (fogstartDistance * fogDensity * 10);
            // (.02 * 100) = 2
            // 300/(.02 * 100) = 150

            float adjustedSiteDistance = (siteDistance / (fogDensity * 1000)) * Settings.FogDensityDistanceMultiplier.Value;

            // don't allow the adjustedSiteDistance to be greater than the initial siteDistance settings before action modifiers are applied
            adjustedSiteDistance = Mathf.Clamp(adjustedSiteDistance, 0, siteDistance);

            Player player = Player.m_localPlayer;

            if (player.IsRunning())
            {
                adjustedSiteDistance *= Settings.RunningImpact.Value;
            }
            else if (player.IsSwiming())
            {
                adjustedSiteDistance *= Settings.SwimmingImpact.Value;
            }
            else if (player.IsSneaking() || player.IsCrouching())
            {
                adjustedSiteDistance *= Settings.SneakingImpact.Value;
            }

            // REDO to ensure that the adjustedSiteDistance is not greater than initial site distance after applying action modifiers
            adjustedSiteDistance = Mathf.Clamp(adjustedSiteDistance, 0, siteDistance);

            Plugin.LogVerbose($"fogstartDistance: {fogstartDistance} fogEndDistance: {fogEndDistance} fogDensity: {fogDensity} adjustedSiteDistance: {adjustedSiteDistance}");

            return adjustedSiteDistance;
        }

    }

    internal static class Settings
    {
        public static ConfigEntry<bool> ExploreMapTweaksEnabled;
        public static ConfigEntry<bool> ExploreWeatherTweaksEnabled;
        public static ConfigEntry<bool> ExploreTimeTweaksEnabled;
        public static ConfigEntry<bool> AlsoExploreLineOfSight;
        public static ConfigEntry<bool> ResetMapOnDeath;
        public static ConfigEntry<bool> RemovePinsOnDeath;
        public static ConfigEntry<float> ExploreOnShipRadius;
        public static ConfigEntry<float> ExploreOnFootRadius;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<float> RunningImpact;
        public static ConfigEntry<float> SwimmingImpact;
        public static ConfigEntry<float> SneakingImpact;
        public static ConfigEntry<float> MaximumExplorationRadius;
        public static ConfigEntry<float> MinimumExplorationRadius;
        public static ConfigEntry<float> MaximumExplorationDistance;
        public static ConfigEntry<float> OnShipEnvironmentalPenalty;
        public static ConfigEntry<float> Default;
        public static ConfigEntry<float> Clear;
        public static ConfigEntry<float> Twilight_Clear;
        public static ConfigEntry<float> Misty;
        public static ConfigEntry<float> Darklands_dark;
        public static ConfigEntry<float> Heath_clear;
        public static ConfigEntry<float> DeepForest_Mist;
        public static ConfigEntry<float> GDKing;
        public static ConfigEntry<float> Rain;
        public static ConfigEntry<float> LightRain;
        public static ConfigEntry<float> ThunderStorm;
        public static ConfigEntry<float> Eikthyr;
        public static ConfigEntry<float> GoblinKing;
        public static ConfigEntry<float> nofogts;
        public static ConfigEntry<float> SwampRain;
        public static ConfigEntry<float> Bonemass;
        public static ConfigEntry<float> Snow;
        public static ConfigEntry<float> Twilight_Snow;
        public static ConfigEntry<float> Twilight_SnowStorm;
        public static ConfigEntry<float> SnowStorm;
        public static ConfigEntry<float> Moder;
        public static ConfigEntry<float> Ashrain;
        public static ConfigEntry<float> Crypt;
        public static ConfigEntry<float> SunkenCrypt;
        public static ConfigEntry<string> HourImpact;
        public static ConfigEntry<bool> FogDensityAffectsExplorationDistance;
        public static ConfigEntry<float> FogDensityDistanceMultiplier;


        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            ExploreMapTweaksEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Plugin", "ExploreMapTweaksEnabled", true, "Enable Exploration Radius Tweaks");
            ExploreWeatherTweaksEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Plugin", "ExploreWeatherTweaksEnabled", true, "Enable Exploration Radius Tweaks based on weather");
            ExploreTimeTweaksEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Plugin", "ExploreTimeTweaksEnabled", true, "Enable Exploration Radius Tweaks based on time");
            AlsoExploreLineOfSight = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Plugin", "AlsoExploreLineOfSight", true, "When enabled and the Exploration triggers, you will also explore the area around where you are looking");
            ResetMapOnDeath = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Plugin", "ResetMapOnDeath", false, "Resets the player map on death");
            RemovePinsOnDeath = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Plugin", "RemovePinsOnDeath", false, "Removes all pins on player map on death");

            ExploreOnFootRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("ExplorationRadius", "ExploreOnFootRadius", 75f, "Sets the exploration radius when on foot");
            ExploreOnShipRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("ExplorationRadius", "ExploreOnShipRadius", 150f, "Sets the exploration radius when on a boat");
            MaximumExplorationRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("ExplorationRadius", "MaximumExplorationRadius", 300f, "After penalties or bonuses are applied, this clamps the maximum exploration radius to this value");
            MinimumExplorationRadius = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("ExplorationRadius", "MinimumExplorationRadius", 5f, "After penalties or bonuses are applied, this clamps the minimum exploration radius to this value (minimum value is 0)");
            MaximumExplorationDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("ExplorationRadius", "MaximumExplorationDistance", 900f, "This is the maximum distance you can 'explore' via line of sight");
            if (MinimumExplorationRadius.Value < 0f) MinimumExplorationRadius.Value = 0f;

            RunningImpact = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("ActionImpact", "Running", -0.50f, "Sets the impact percentage to the exploration radius when running, positive numbers increase radius, negative numbers decrease it");
            SwimmingImpact = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("ActionImpact", "Swimming", -0.75f, "Sets the impact percentage to the exploration radius when swimming, positive numbers increase radius, negative numbers decrease it");
            SneakingImpact = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("ActionImpact", "Sneaking", -0.33f, "Sets the impact percentage to the exploration radius when sneaking, positive numbers increase radius, negative numbers decrease it");

            OnShipEnvironmentalPenalty = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "OnShipEnvironmentalPenalty", -0.25f, "When there is an environmental impact, it incurs additional penalties when on a ship");
            FogDensityAffectsExplorationDistance = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("EnvironmentalImpact", "FogDensityAffectsExplorationDistance", true, "The Fog Density influences the maximum distance for exploration");
            FogDensityDistanceMultiplier = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "FogDensityDistanceMultiplier", 2.0f, "If FogDensity affects exploration distance, then this is the multiplier applied to distance viewable");

            Default = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Default",0f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease, positive numbers increase radius, negative numbers decrease");
            Clear = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Clear",0.10f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Twilight_Clear = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Twilight_Clear",0.10f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Misty = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Misty",-0.33f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Darklands_dark = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Darklands_dark",0f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Heath_clear = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Heath clear", 0f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            DeepForest_Mist = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "DeepForest Mist", -0.25f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            GDKing = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "GDKing",0f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Rain = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Rain",-0.33f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            LightRain = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "LightRain",-0.20f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            ThunderStorm = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "ThunderStorm",-0.50f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Eikthyr = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Eikthyr",-0.20f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            GoblinKing = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "GoblinKing",-0.20f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            nofogts = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "nofogts",0f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            SwampRain = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "SwampRain",-0.33f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Bonemass = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Bonemass",-0.33f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Snow = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Snow",-0.20f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Twilight_Snow = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Twilight_Snow",-0.20f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Twilight_SnowStorm = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Twilight_SnowStorm", -0.50f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            SnowStorm = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "SnowStorm", -0.50f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Moder = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Moder", -0.30f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Ashrain = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Ashrain", -0.30f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            Crypt = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "Crypt", -0.30f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            SunkenCrypt = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("EnvironmentalImpact", "SunkenCrypt", -0.30f, "Impact percentage this weather type has on exploration radius, positive numbers increase radius, negative numbers decrease");
            HourImpact = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("TimeImpact", "Hours", "-0.50,-0.45,-0.40,-0.35,-0.30,-0.25,-0.1,0,0,0,0.10,0.15,0.20,0.25,0.25,0.2,0.15,-0.10,-0.15,-0.20,-0.25,-0.30,-0.40,-0.50","Percentage Impact the hour has on exploration radius. Hour 0 - 23. There must be 24 values separated by comma");

            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }
}
