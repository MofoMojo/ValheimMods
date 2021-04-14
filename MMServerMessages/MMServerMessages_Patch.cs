using System;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MofoMojo.MMServerMessages
{
    class MMServerMessages_Patch
    {
        [HarmonyPatch(typeof(Game), "Update")]
        static class Game_Update_Patch
        {
            public static float m_sleepTimer = 0;
            public static float m_motdTimer = 0;
            public static float m_refresh = 0;

            [HarmonyPrepare]
            static bool IsMMServerMessagesEnabled()
            {
                bool enabled = Settings.MMServerMessagesEnabled.Value;
                Plugin.Log($"HideMapToggleEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPostfix]
            static void Postfix(ref Game __instance)
            {
                try
                {
                    // only do this on Dedicated Servers and don't do anything while saving
                    if (null != ZNet.instance && ZNet.instance.IsServer() && !ZNet.instance.IsSaving())
                    {
                        if (Settings.SleepNotificationEnabled.Value) CheckSleepers(Time.fixedDeltaTime);
                        if (Settings.MessageOfTheDayEnabled.Value) CheckMotd(Time.fixedDeltaTime);
                    }
                }
                catch (Exception ex)
                {
                    // do nothing, just swallow it up
                    Plugin.LogError(ex.ToString());
                }
            }

            // comparing UpdateSaving
            public static void CheckSleepers(float dt)
            {
                
                m_sleepTimer += dt;
                // check every 10 seconds
                if (m_sleepTimer > Settings.SleepCheckInterval.Value)
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
                            string text = $"{totalCharsInBed}/{totalChars} " + Settings.SleepMessage.Value;
                            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ShowMessage", (int)Settings.SleepMessageType.Value, text);
                        }
                    }
                   
                }
            }

            // comparing UpdateSaving
            public static void CheckMotd(float dt)
            {
                m_motdTimer += dt;

                // check every 10 seconds
                if (m_motdTimer < Settings.MessageOfTheDayCheckInterval.Value) return;
                
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
                                ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "ShowMessage", (int)Settings.MessageOfTheDayType.Value, Settings.MessageOfTheDay.Value);
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
}
