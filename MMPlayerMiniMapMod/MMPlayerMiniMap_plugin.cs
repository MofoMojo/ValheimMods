using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace MofoMojo.MMPlayerMiniMapMod
{
    [BepInPlugin("MofoMojo.MMPlayerMiniMapMod", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MM's Player Minimap Toggle";
        Harmony _Harmony;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose
        }

        public static Plugin Instance;

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
            if (PluginLoggingLevel == LoggingLevel.Verbose) Debug.LogError(message);
        }

        private void Update()
        {
            // don't do anything if not enabled or shouldn't be...
            if (!Settings.HideMapToggleEnabled.Value || MofoMojo.UtilityClass.IgnoreKeyPresses() || !PressedToggleKey()) return;

            // if toggled, change the state
            if (PressedToggleKey())
            {
                Minimap instance = Minimap.instance;

                if (null != instance && instance.m_mode == Minimap.MapMode.Small)
                {

                    //toggle minimap
                    Settings.IsMapVisible.Value = !Settings.IsMapVisible.Value;
                    instance.m_smallRoot.SetActive(Settings.IsMapVisible.Value);

                    /*
                    switch(instance.m_mode)
                    {
                        case Minimap.MapMode.Small:
                            Settings.IsMapVisible.Value = false;
                            instance.SetMapMode(Minimap.MapMode.None);
                            break;
                        case Minimap.MapMode.None:
                            Settings.IsMapVisible.Value = true;
                            instance.SetMapMode(Minimap.MapMode.Small);
                            break;
                    }
                    */

                }

            }
        }

        private bool PressedToggleKey()
        {
            try
            {
                return Input.GetKeyDown(Settings.HideMapToggleKey.Value.ToLower());
            }
            catch
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(Minimap), "Awake")]
        static class Minimap_Awake_Patch
        {

            [HarmonyPrepare]
            static bool IsHideMapToggleEnabled()
            {
                bool enabled = Settings.HideMapToggleEnabled.Value;
                Plugin.Log($"HideMapToggleEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPostfix]
            static void Postfix(Minimap __instance)
            {
                try
                {

                    if (!Settings.HideMapToggleEnabled.Value)
                        return;

                    // set state to last state saved in config file
                    if (!Settings.IsMapVisible.Value)
                    {
                        __instance.m_smallRoot.SetActive(false);
                        //__instance.SetMapMode(Minimap.MapMode.None);
                    }

                }
                catch (Exception ex)
                {
                    // do nothing, just swallow it up
                    Plugin.LogError(ex.ToString());
                }
            }



        }

        [HarmonyPatch(typeof(Minimap), "SetMapMode")]
        static class Minimap_SetMapMode
        {

            [HarmonyPrepare]
            static bool IsHideMapToggleEnabled()
            {
                bool enabled = Settings.HideMapToggleEnabled.Value;
                Plugin.Log($"HideMapToggleEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPostfix]
            static void Postfix(Minimap __instance)
            {
                try
                {
                    // if it's not supposed to be visible then re-hide the small minimap after switching mapmodes
                    if (__instance.m_mode == Minimap.MapMode.Small && !Settings.IsMapVisible.Value)
                    {
                        __instance.m_smallRoot.SetActive(false);
                        //__instance.SetMapMode(Minimap.MapMode.None);
                    }

                }
                catch (Exception ex)
                {
                    // do nothing, just swallow it up
                    Plugin.LogError(ex.ToString());
                }
            }



        }

        [HarmonyPatch(typeof(Minimap), "Awake")]
        static class Minimap_UpdatePlayerMarker
        {

            [HarmonyPrepare]
            static bool IsHideMapToggleEnabled()
            {
                bool enabled = Settings.AdjustMapMarkerScale.Value;
                Plugin.Log($"HideMapToggleEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPostfix]
            static void PostFix(Minimap __instance)
            {
                try
                {
                    Vector3 newSmallMarkerLocalScale = __instance.transform.localScale * Settings.SmallMarkerLocalScale.Value;
                    Vector3 newLargeMarkerLocalScale = __instance.transform.localScale * Settings.LargeMarkerLocalScale.Value;
                    __instance.m_smallMarker.localScale = newSmallMarkerLocalScale;
                    __instance.m_largeMarker.localScale = newLargeMarkerLocalScale;

                }
                catch (Exception ex)
                {
                    // do nothing, just swallow it up
                    Plugin.LogError(ex.ToString());
                }
            }



        }

    }

    internal static class Settings
    {
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;
        public static ConfigEntry<bool> HideMapToggleEnabled;
        public static ConfigEntry<string> HideMapToggleKey;
        public static ConfigEntry<bool> AdjustMapMarkerScale;
        public static ConfigEntry<float> SmallMarkerLocalScale;
        public static ConfigEntry<float> LargeMarkerLocalScale;
        public static ConfigEntry<bool> IsMapVisible;

        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            HideMapToggleEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MiniMap", "HideMapToggleEnabled", true, "Turn on ability to hide map mode");
            HideMapToggleKey = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<string>("MiniMap", "HideMapToggleKey", "]", "Key to use for toggling player stats. https://docs.unity3d.com/ScriptReference/KeyCode.html");
            IsMapVisible = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MiniMap", "IsMapVisible", true, "Tracks status of the map");

            AdjustMapMarkerScale = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MiniMap", "AdjustMapMarkerScale", true, "Turn on ability to adjust the direction marker scales");
            SmallMarkerLocalScale = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MiniMap", "SmallMarkerLocalScale", 1.5f, "Scales the direction marker of the small map");
            LargeMarkerLocalScale = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<float>("MiniMap", "LargeMarkerLocalScale", 2f, "Scales the direction marker of the large map");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
        }

    }
}
