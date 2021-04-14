using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;

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
        public static float m_sleepTimer = 0;
        public static float m_motdTimer = 0;
 
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

        public static Game gameinstance;

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
            LoadConfig();
            PluginLoggingLevel = MMServerMessagesPluginLoggingLevel.Value;

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

        public void Update()
        {
            try
            {
                // only do this on Dedicated Servers and don't do anything while saving
                if (null != ZNet.instance && ZNet.instance.IsServer() && !ZNet.instance.IsSaving() && gameinstance != null)
                {
                    if (SleepNotificationEnabled.Value) CheckSleepers(Time.fixedDeltaTime);
                    if (MessageOfTheDayEnabled.Value) CheckMotd(Time.fixedDeltaTime);
                    
                    Refresh(Time.deltaTime);
                }
            }
            catch (Exception ex)
            {
                // do nothing, just swallow it up
                Plugin.LogError(ex.ToString());
            }
        }

        public void Refresh(float dt)
        {
            m_refresh += dt;

            // check every 60 seconds by default, if it hasn't been, return...
            if (m_refresh < MMServerConfigRefreshInterval.Value) return;

            // reset the refresh interval checker
            m_refresh = 0;

            Config.Reload();
            LoadConfig();
            Plugin.LogVerbose("Config reloaded");
        }

        public void LoadConfig()
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

        // comparing UpdateSaving
        public static void CheckSleepers(float dt)
        {

            m_sleepTimer += dt;
            // check every 10 seconds
            if (m_sleepTimer > Plugin.SleepCheckInterval.Value)
            {
                Plugin.LogDebug("CheckSleepers");
                // reset the sleep checker
                m_sleepTimer = 0f;

                int totalChars = 0;
                int totalCharsInBed = 0;

                //SavePlayerProfile(setLogoutPoint: false);
                // validate this instance is good
                if (ZNet.instance)
                {
                    // get all the characters
                    List<ZDO> allCharacterZDOS = ZNet.instance.GetAllCharacterZDOS();

                    // set character count
                    totalChars = allCharacterZDOS.Count;

                    foreach (ZDO item in allCharacterZDOS)
                    {
                        // are these characters in bed?
                        if (item.GetBool("inBed", false))
                        {
                            totalCharsInBed++;
                        }
                    }

                    // if there are people in bed and not everyone, then send message
                    if (totalCharsInBed != 0 && totalChars > totalCharsInBed)
                    {
                        string text = $"{totalCharsInBed}/{totalChars} " + Plugin.SleepMessage.Value;
                        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ShowMessage", (int)Plugin.SleepMessageType.Value, text);
                    }
                }

            }
        }

        // comparing UpdateSaving
        public static void CheckMotd(float dt)
        {
            m_motdTimer += dt;

            // check every 10 seconds
            if (m_motdTimer < Plugin.MessageOfTheDayCheckInterval.Value) return;

            Plugin.LogDebug("Checking Motd");

            // reset the motd timer
            m_motdTimer = 0f;

            //SavePlayerProfile(setLogoutPoint: false);
            // validate this instance is good
            if (ZNet.instance)
            {
                // get all the characters
                List<ZDO> allCharacterZDOS = ZNet.instance.GetAllCharacterZDOS();
                Plugin.LogVerbose($"allCharacterZDOS count: {allCharacterZDOS.Count}");

                // set character count
                foreach (ZDO characterZDO in allCharacterZDOS)
                {
                    Plugin.LogVerbose($"Found uid: {characterZDO.m_uid} owner: {characterZDO.m_owner}");

                    // get last Message OF The Day displayed time
                    bool SeenMOTD = characterZDO.GetBool("mm_shownmotd", false);

                    if (!SeenMOTD)
                    {
                        //GetPeerByPlayerName
                        string playerName = characterZDO.GetString("playerName");
                        Plugin.LogVerbose($"playerName {playerName}");

                        ZNetPeer peer = ZNet.instance.GetPeerByPlayerName(playerName);

                        //ZNetPeer peer = ZNet.instance.GetPeer(peerid);
                        Plugin.LogVerbose($"Peer not found: {(peer == null)}");

                        if (null != peer && peer.IsReady())
                        {
                            Plugin.LogVerbose($"Sending Message to {peer.m_uid}");
                            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "ShowMessage", (int)Plugin.MessageOfTheDayType.Value, Plugin.MessageOfTheDay.Value);
                            characterZDO.Set("mm_shownmotd", true);
                        }
                        else
                        {
                            Plugin.LogVerbose($"Peer {characterZDO.m_owner} not ready or peer not found");
                        }
                    }

                }

            }


        }

        public static long GetPeerFromId(long id)
        {
            foreach (ZNetPeer peer in ZNet.instance.GetPeers())
            {
                Plugin.LogVerbose($"peer: {peer.m_uid}, characterId: {peer.m_characterID}");
                long uid = peer.m_uid;
                if (id == peer.m_uid) return id;
            }

            return 0;
        }

    }

}
