using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace MofoMojo.MMPlayerSink
{
    [BepInPlugin("MofoMojo.MMPlayerSink", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0";
        public const string ModName = "MMPlayerSink";

        public static Plugin Instance;
        Harmony _Harmony;

        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose,
            Debug
        }
        public static ConfigEntry<int> nexusId;
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<LoggingLevel> PluginLoggingLevel;
        public static bool isSunk = false;
        public static bool wasSwimming = false;

        private void Awake()
        {
            Instance = this;
            LoadConfig();
            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }

        public static void Log(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel.Value > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel.Value > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel.Value > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel.Value > LoggingLevel.Verbose) Debug.Log(message);
        }

        public static void LogDebug(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel.Value == LoggingLevel.Debug) Debug.Log(message);
        }

        public void Refresh()
        {
            Config.Reload();
            LoadConfig();
        }

        public void LoadConfig()
        {
            Enabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMPlayerSink", "Enabled", true, "Enables MMPlayerSink mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<LoggingLevel>("LoggingLevel", "PluginLoggingLevel", LoggingLevel.None, "Supported values are None, Normal, Verbose, Debug");
            nexusId = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("General", "NexusID", 0, "Nexus mod ID for updates. Do not modify.");
        }

        public void Update()
        {
            if(isSunk)
            {
                Player player = Player.m_localPlayer;
                if(null != player)
                {
                    // When in the water, there's a minimum water distance set to 0.3f.
                    // This minimum keeps the camera from actually entering the water. i.e the camera can't go lower than 0.3f from the water
                    // this is a hack to allow the camera to go under water without changing all the code associated with it
                    GameCamera.instance.m_minWaterDistance = -100f;

                    // this places a force in the direction the camera is basically pointing but it's seems rather binary, like you either go up or down
                    // ideally I'd like to be able to actually just "swim" but not investing much time. I expect this may be forth coming in the ocean biome update
                    //player.m_body.AddForceAtPosition(GameCamera.instance.transform.forward, player.transform.position, ForceMode.VelocityChange);

                    // this places a downward force on the player body
                    // https://docs.unity3d.com/ScriptReference/ForceMode.html
                    player.m_body.AddForceAtPosition(Vector3.down, base.transform.position, ForceMode.VelocityChange);
                    wasSwimming = true;
                }
            }
            else
            {
                // reset the camera min distance back to 0.3f
                if (wasSwimming)
                {
                    if (null != GameCamera.instance)
                    {
                        GameCamera.instance.m_minWaterDistance = 0.3f;
                        wasSwimming = false;
                    }
                }

            }
        }

        public static class HarmonyPatches
        {


            [HarmonyPatch(typeof(Chat), "InputText")]
            static class Chat_InputText
            {
                [HarmonyPrepare]
                static bool IsMMPlayerSink()
                {
                    bool enabled = Enabled.Value;
                    Plugin.Log($"MMPlayerSink: {enabled}");

                    return enabled;
                }

                [HarmonyPrefix]
                static bool Prefix(Chat __instance)
                {
                    try
                    {
                        Player player = Player.m_localPlayer;
                        if (null == player) return true;

                        string text = __instance.m_input.text;

                        if (text.ToLower().StartsWith("/sink"))
                        {
                            isSunk = !isSunk;
                          
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    return true;
                }
            }
        }

    }
}
