using HarmonyLib;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MofoMojo.MMPlantingPositive
{
    class Patches
    {
        public static Dictionary<string, Plant> plants = new Dictionary<string, Plant>();
        public static Dictionary<string, Pickable> pickables = new Dictionary<string, Pickable>();

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        static class ZNetScene_Awake
        {
            [HarmonyPrepare]
            static bool IsMMPlantingPositiveEnabled()
            {
                bool enabled = Settings.MMPlantingPositiveEnabled.Value;
                Plugin.Log($"MMPlantingPositiveEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            static void Awake_Prefix(ref ZNetScene __instance)
            {
                // add birchtree if it needs to be?
                if (null != Plugin.BirchTreeSapling && !__instance.m_prefabs.Contains(Plugin.BirchTreeSapling)) __instance.m_prefabs.Add(Plugin.BirchTreeSapling);

            }
        }

        [HarmonyPatch(typeof(Pickable), "Awake")]
        static class Pickable_Awake
        {
            [HarmonyPrepare]
            static bool IsMMPlantingPositiveEnabled()
            {
                bool enabled = Settings.MMPlantingPositiveEnabled.Value;
                Plugin.Log($"MMPlantingPositiveEnabled: {enabled}");

                return false;
            }

            [HarmonyPostfix]
            static void Awake_Postfix(ref Pickable __instance)
            {
                if (__instance.name.Contains("(Clone)(Clone)")) return;
                // reset the spawnpoint in case it's a new game
                // lock this object so collection is not modified or used elsewhere
                lock (pickables)
                {
                    try {
                        if(!pickables.ContainsKey(__instance.name))
                        {
                            Pickable temp = GameObject.Instantiate<Pickable>(__instance);
                            pickables.Add(__instance.name, temp);
                        }
                    }
                    catch(Exception ex)
                    {
                        Plugin.LogError($"Exception: {ex}");
                    }

                }
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "GetItemPrefab", new Type[] { typeof(string) })]
        [HarmonyPatch(typeof(ObjectDB), "GetItemPrefab", new Type[] { typeof(int) })]
        static class ObjectDB_GetItemPrefab
        {
            public static bool done = false;
            [HarmonyPrepare]
            static bool IsMMPlantingPositiveEnabled()
            {
                bool enabled = (Settings.MMPlantingPositiveEnabled.Value && Settings.PluginLoggingLevel.Value == Plugin.LoggingLevel.Debug);
                Plugin.Log($"MMPlantingPositiveEnabled Debug Stuff: {enabled}");

                return false;
            }

            [HarmonyPostfix]
            static void GetItemPrefab_Postfix(ref ObjectDB __instance)
            {
                if(!done)
                {
                    Plugin.LogDebug($"Dumping Gameobjects");

                    foreach (GameObject gameObject in __instance.m_items)
                    {
                        Plugin.LogDebug($"GameOjbect: {gameObject.name}");
                    }
                    done = true;
                }

            }
        }
    }
}
