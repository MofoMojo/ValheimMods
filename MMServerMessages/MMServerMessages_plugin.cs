
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace MofoMojo.MMServerMessages
{
    [BepInPlugin("MofoMojo.MMServerMessages", ModName, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static string previousMotd = string.Empty;
        public const string Version = "1.1";
        public const string ModName = "MMServerMessages";
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

        private Plugin()
        {

            Instance = this;
            gameinstance = Game.instance;

            LoadConfig();
            PluginLoggingLevel = MMServerMessagesPluginLoggingLevel.Value;
            previousMotd = MessageOfTheDay.Value;

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
                Refresh(Time.deltaTime);
                if (gameinstance == null) gameinstance = Game.instance;

                // only do this on Dedicated Servers and don't do anything while saving
                if (MMServerMessagesEnabled.Value && gameinstance != null && null != ZNet.instance && ZNet.instance.IsServer() && !ZNet.instance.IsSaving())
                {
                    if (SleepNotificationEnabled.Value) CheckSleepers(Time.fixedDeltaTime);
                    if (MessageOfTheDayEnabled.Value) CheckMotd(Time.fixedDeltaTime);
                    
 
                }
            }
            catch (Exception ex)
            {
                // do nothing, just swallow it up
                LogError(ex.ToString());
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
            LogVerbose($"Config reloaded. MOTD = '{MessageOfTheDay.Value}'");

            if(previousMotd != MessageOfTheDay.Value)
            {
                previousMotd = MessageOfTheDay.Value;
                CheckMotd(Time.deltaTime, true);
            }

        }

        public void LoadConfig()
        {
            MMServerMessagesEnabled = ((BaseUnityPlugin)Instance).Config.Bind<bool>("MMServerMessages", "MMServerMessagesEnabled", true, "Enables MMServerMessages mod");
            MMServerConfigRefreshInterval = ((BaseUnityPlugin)Instance).Config.Bind<float>("MMServerMessages", "MMServerConfigRefreshInterval", 60, "How often, in seconds, to check for and/or refresh config");
            MMServerMessagesPluginLoggingLevel = ((BaseUnityPlugin)Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "MMServerMessagesPluginLoggingLevel", LoggingLevel.None, "Supported values are None, Normal, Verbose");
            SleepNotificationEnabled = ((BaseUnityPlugin)Instance).Config.Bind<bool>("SleepNotification", "SleepNotificationEnabled", true, "Enables SleepNotification");
            SleepCheckInterval = ((BaseUnityPlugin)Instance).Config.Bind<float>("SleepNotification", "SleepCheckInterval", 11, "How often, in seconds, to check for sleepers");
            SleepMessage = ((BaseUnityPlugin)Instance).Config.Bind<string>("SleepNotification", "SleepMessage", "Vikings are attempting to sleep", "Message to follow the #ofSleepers/#ofPlayers text");
            SleepMessageType = ((BaseUnityPlugin)Instance).Config.Bind<MessageHud.MessageType>("SleepNotification", "SleepMessageType", MessageHud.MessageType.TopLeft, "Message Type you want displayed");
            MessageOfTheDayEnabled = ((BaseUnityPlugin)Instance).Config.Bind<bool>("MessageOfTheDay", "MessageOfTheDayEnabled", true, "Enables Message of the Day");
            MessageOfTheDayCheckInterval = ((BaseUnityPlugin)Instance).Config.Bind<float>("MessageOfTheDay", "MessageOfTheDayCheckInterval", 13, "How often, in seconds, to check for new clients to send the MOTD");
            MessageOfTheDayType = ((BaseUnityPlugin)Instance).Config.Bind<MessageHud.MessageType>("MessageOfTheDay", "MessageOfTheDayType", MessageHud.MessageType.Center, "Message Type you want displayed");
            MessageOfTheDay = ((BaseUnityPlugin)Instance).Config.Bind<string>("MessageOfTheDay", "MessageOfTheDay", "Welcome to our Dedicated Valheim Server\nGipta!!!", "Message you wish to announce when players log on");
        }

        // comparing UpdateSaving
        public static void CheckSleepers(float dt)
        {

            m_sleepTimer += dt;
            // check every 10 seconds
            if (m_sleepTimer > SleepCheckInterval.Value)
            {
                LogDebug($"CheckSleepers Note: Refresh {m_refresh}");
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
                        string text = $"{totalCharsInBed}/{totalChars} " + SleepMessage.Value;
                        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ShowMessage", (int)SleepMessageType.Value, text);
                    }
                }

            }
        }

        // comparing UpdateSaving
        public static void CheckMotd(float dt, bool ignoreSeen = false)
        {
            m_motdTimer += dt;

            // check every 10 seconds and return if lower than the interval and ignoreSeen is false.
            if (m_motdTimer < MessageOfTheDayCheckInterval.Value && !ignoreSeen) return;

            LogDebug($"Checking Motd. Note: Refresh {m_refresh}");

            // reset the motd timer
            m_motdTimer = 0f;

            //SavePlayerProfile(setLogoutPoint: false);
            // validate this instance is good
            if (ZNet.instance)
            {
                // get all the characters
                List<ZDO> allCharacterZDOS = ZNet.instance.GetAllCharacterZDOS();
                LogVerbose($"allCharacterZDOS count: {allCharacterZDOS.Count}");

                // set character count
                foreach (ZDO characterZDO in allCharacterZDOS)
                {
                    LogVerbose($"Found uid: {characterZDO.m_uid} owner: {characterZDO.m_owner}");

                    // get last Message OF The Day displayed time
                    bool SeenMOTD = characterZDO.GetBool("mm_shownmotd", false);

                    // if the message hasn't been seen OR we're ignoring that it's been seen....
                    if (!SeenMOTD || ignoreSeen)
                    {
                        //GetPeerByPlayerName
                        string playerName = characterZDO.GetString("playerName");
                        LogVerbose($"playerName {playerName}");

                        ZNetPeer peer = ZNet.instance.GetPeerByPlayerName(playerName);

                        //ZNetPeer peer = ZNet.instance.GetPeer(peerid);
                        LogVerbose($"Peer not found: {(peer == null)}");

                        if (null != peer && peer.IsReady())
                        {
                            LogVerbose($"Sending Message to {peer.m_uid}");
                            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "ShowMessage", (int)MessageOfTheDayType.Value, MessageOfTheDay.Value);
                            characterZDO.Set("mm_shownmotd", true);
                        }
                        else
                        {
                            LogVerbose($"Peer {characterZDO.m_owner} not ready or peer not found");
                        }
                    }

                }

            }


        }

        public static long GetPeerFromId(long id)
        {
            foreach (ZNetPeer peer in ZNet.instance.GetPeers())
            {
                LogVerbose($"peer: {peer.m_uid}, characterId: {peer.m_characterID}");
                long uid = peer.m_uid;
                if (id == peer.m_uid) return id;
            }

            return 0;
        }

    }

}
