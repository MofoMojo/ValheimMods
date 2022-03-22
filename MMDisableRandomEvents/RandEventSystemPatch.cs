using System;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace MofoMojo.MMDisableRandomEvents
{
    class MMRandEventSystemPatch
    {

        // this hooks into PrivateArea type's Awake method
        [HarmonyPatch(typeof(RandEventSystem), "Awake")]
        public class HarmonyPatch_Awake
        {

            [HarmonyPrepare]
            static bool IsMMDisableRandomEventsEnabled()
            {
                bool enabled = Settings.MMDisableRandomEventsEnabled.Value;
                Plugin.Log($"MMDisableRandomEventsEnabled {enabled}");
                new Terminal.ConsoleCommand("startrandomevent", "starts a random event", delegate (Terminal.ConsoleEventArgs args)
                {
                    Plugin.StartRandomEvent();

                }, false, false, false, false, false, null);
                return enabled;
            }

            [HarmonyPostfix]
            // Prefix indicates that your code will execute before the actual code
            private static void Postfix(ref RandEventSystem __instance)
            {
                if(null == __instance || __instance.m_events.Count == 0)
                {
                    Plugin.LogWarning($"__instance is null or m_events count is 0");
                    return;
                }

                foreach (RandomEvent randomEvent in __instance.m_events)
                {
                    string randEventName = randomEvent.m_name;
                    string startMessage = Localization.instance.Localize(randomEvent.m_startMessage);

                    // adding to list of known random events
                    Plugin.KnownRandomEvents.Add(randomEvent);

                    Plugin.LogVerbose($"Found Event {randEventName}");

                    if(!Settings.configEntries.Exists(configEntry => configEntry.Definition.Key == randEventName))
                    {
                        Plugin.LogVerbose($"Setting and binding {randEventName} to/from Config File");
                        Settings.configEntries.Add(((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>(Plugin.DisabledEventSectionName, randEventName, false, startMessage));
                    }

                }

                Plugin.Instance.Config.Save();

                // loop through boolean config entries that are set to TRUE for disabling
                foreach(ConfigEntry<bool> configEntry in Settings.configEntries.FindAll(x => x.Value == true))
                {
                    Plugin.LogVerbose($"Reading configEntry {configEntry.Definition.Key}");
                    // Disable any events set to false
                    if (configEntry.Value == true)
                    {
                        RandomEvent randomEvent = Plugin.KnownRandomEvents.Find(x => x.m_name == configEntry.Definition.Key);
                        if (null != randomEvent)
                        {
                            Plugin.LogVerbose($"Disabling event {randomEvent.m_name}");
                            //__instance.m_events.Remove(randomEvent);
                            __instance.m_events.Find(x => x == randomEvent).m_enabled = false;
                        }
                        
                    }

                }

            }

           

        }

        [HarmonyPatch(typeof(RandEventSystem), "GetEvent")]
        //DropItem(Inventory inventory, ItemDrop.ItemData item, int amount, Humanoid __instance, ref bool __result)
        public class HarmonyPatch_GetEvent
        {
            [HarmonyPrepare]
            static bool IsMMDisableRandomEventsEnabled()
            {
                return false;
            }

            [HarmonyPostfix]
            private static void PostFix(string name,ref RandEventSystem __instance, ref RandomEvent __result)
            {
                Plugin.Log($"PostFix GetEvent: {name}");
                if (null != __result) return;
                if (null == name) return;
                
                Plugin.Log($"Looking for... {name}");
                foreach (RandomEvent randomEvent in Plugin.KnownRandomEvents)
                {
                    Plugin.LogVerbose($"Found {randomEvent.m_name}");

                    if (randomEvent.m_name == name) 
                    {
                        Plugin.Log($"GetEvent Found Event {name}");
                        if(randomEvent.m_enabled)
                        {
                            Plugin.Log($"Is Enabled, returning event {name}");
                            __result = randomEvent;
                            return;
                        }
                        
                    }
                }
                __result = null;

            }
        }
    }
}
