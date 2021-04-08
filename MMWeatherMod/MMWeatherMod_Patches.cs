using System;
using System.Collections.Generic;
using HarmonyLib;

//https://valheim.fandom.com/wiki/Localization

namespace MofoMojo.MMWeatherMod
{
    class MMWeatherMod_Patches
    {
        private static DateTime prayedTime;
        private static List<string> environmentNames = new List<string>();

        [HarmonyPatch(typeof(EnvMan), "Awake")]
        static class EnvMan_Awake_Patch
        {

            [HarmonyPrepare]
            static bool IsMMWeatherModEnabled()
            {
                bool enabled = Settings.MMWeatherModEnabled.Value;
                Plugin.Log($"HideMapToggleEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPostfix]
            static void Postfix(EnvMan __instance)
            {
                try
                {
                    // eh.. .don't do this work if logging isn't verbose anyway...
                    if (Settings.PluginLoggingLevel.Value != Plugin.LoggingLevel.Verbose) return;
                    environmentNames.Clear();

                    foreach(EnvSetup environment in __instance.m_environments)
                    {
                        Plugin.LogVerbose($"Environment Name:{environment.m_name}");
                        environmentNames.Add(environment.m_name);
                    }


                    prayedTime = DateTime.MinValue;
                    /*
                     * As of 148.6? 4/7/21
                        Environment Name:Clear
                        Environment Name:Twilight_Clear
                        Environment Name:Misty
                        Environment Name:Darklands_dark
                        Environment Name:Heath clear
                        Environment Name:DeepForest Mist
                        Environment Name:GDKing
                        Environment Name:Rain
                        Environment Name:LightRain
                        Environment Name:ThunderStorm
                        Environment Name:Eikthyr
                        Environment Name:GoblinKing
                        Environment Name:nofogts
                        Environment Name:SwampRain
                        Environment Name:Bonemass
                        Environment Name:Snow
                        Environment Name:Twilight_Snow
                        Environment Name:Twilight_SnowStorm
                        Environment Name:SnowStorm
                        Environment Name:Moder
                        Environment Name:Ashrain
                        Environment Name:Crypt
                        Environment Name:SunkenCrypt
                     */

                }
                catch (Exception ex)
                {
                    // do nothing, just swallow it up
                    Plugin.LogError(ex.ToString());
                }
            }
        }

        [HarmonyPatch(typeof(Chat), "InputText")]
        static class Chat_InputText
        {

            [HarmonyPrepare]
            static bool IsMMWeatherModEnabled()
            {
                bool enabled = Settings.MMWeatherModEnabled.Value;
                Plugin.Log($"HideMapToggleEnabled: {enabled}");

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
                    bool prayed = false;
                    string[] split;
                    string weather = String.Empty;
                    
                    if(text.ToLower().StartsWith("/pray"))
                    {
                        Plugin.Log("/Prayed");
                        // if player is sitting and not sheltered and not in an interior or if player is on a boat....
                        if ((player.IsSitting() && !player.InShelter() && !player.InInterior()) || (null != Ship.GetLocalShip() && Ship.GetLocalShip().IsPlayerInBoat(player)))
                        {
                            prayed = true;
                            
                            // get the weather prayed for (easter egg) or praying for clear weather
                            // some weather types have a space in their name so only split into 2 substrings max
                            split = text.Split(new char[]{' '}, 2);

                            // found something after /prayed
                            if (split.Length > 1)
                            {
                                weather = split[1];
                            }
                            else
                            {
                                weather = "Clear";
                            }

                            //try to match up to valid environment name
                            foreach(string environmentName in environmentNames)
                            {
                                // the environment names are case sensitive. Find the correct case
                                if (environmentName.ToLower() == weather.ToLower())
                                {
                                    weather = environmentName;
                                    Plugin.LogVerbose($"Weather found: {weather}");
                                    break;
                                }
                            }

                        }
                        else
                        {
                            //MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "You sense you should be sitting outside... or in a boat.");
                            //msg_softdeath The gods are merciful
                            //settings_sit	Sit

                            if (!player.IsSitting())
                            {
                                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, Localization.instance.Localize("$settings_sit..."));
                                return false;
                            }

                            if(player.InInterior() || player.InShelter())
                            {
                                
                                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, Localization.instance.Localize("$se_shelter_start..."));
                                return false;
                            }

                        }

                    }

                    if(prayed)
                    {
                        try
                        {
                            //long longDateTime = m_nview.GetZDO().GetLong("prayed_time");
                            //prayedTime = DateTime.FromBinary(longDateTime);
                            Plugin.Log($"LastPrayedTime: {prayedTime.ToString()}");

                            // has it been more than 30 seconds since the last successful attempt?
                            if(DateTime.Now - prayedTime < TimeSpan.FromSeconds(Settings.PrayerFrequency.Value))
                            {
                                //MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "You sense you have prayed too recently");
                                //npc_haldor_goodbye02	Away with you. I have things to do.
                                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, Localization.instance.Localize("$npc_haldor_goodbye02"));
                                return false;
                            }
                        }
                        catch(Exception ex)
                        {
                            Plugin.LogError(ex.Message);
                        }
                        
                        // record the current prayed time now that an attempt has been made
                        // it still may not succeed
                        prayedTime = DateTime.Now;
                        
                        // 66% chance of failure
                        if (UnityEngine.Random.Range(1, 100) < 66)
                        {

                            // 25% chance that odin will not feel favorable toward you and cause a thunderstorm
                            if (UnityEngine.Random.Range(1, 100) > 25)
                            {

                                //MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "You sense Odin is not willing at this time...");
                                //msg_cantoffer	Your offering is not answered
                                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, Localization.instance.Localize("$msg_cantoffer"));
                                return false;
                            }

                            //MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "Odin is angered...");
                            //tutorial_cold_topic	Be wary of the weather
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, Localization.instance.Localize("$tutorial_cold_topic"));
                            weather = "ThunderStorm";
                            Plugin.Log($"Angered Odin... getting {weather}");
                        }
                        else
                        {
                            // Odin is pleased...
                            // tutorial_stemple4_topic	Odin is pleased
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, Localization.instance.Localize("$tutorial_stemple4_topic"));
                        }



                        EnvMan envMan = EnvMan.instance;
                        EnvSetup newEnvironment = envMan.GetEnv(weather);
                        if (null != newEnvironment)
                        {
                            EnvSetup currentEnvironment = envMan.GetCurrentEnvironment();
                            envMan.QueueEnvironment(newEnvironment);


                           // m_nview.GetZDO().Set("prayed_time", prayedTime.ToBinary());
                        }

                        
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    // do nothing, just swallow it up
                    Plugin.LogError(ex.ToString());
                }

                // go ahead and execute the real method
                return true;
            }
        }
    }
}
