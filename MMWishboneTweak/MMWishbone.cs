using HarmonyLib;
using UnityEngine;

namespace MofoMojo.MMWishboneTweak
{
    class MMWishbone
    {

        [HarmonyPatch(typeof(Destructible), "Start")]
        private static class Destructible_StartPatch
        {
            [HarmonyPrepare]
            static bool IsWishBoneTweakEnabled()
            {
                bool enabled = Settings.WishBoneTweakEnabled.Value;

                Plugin.Log($"WishBoneTweakEnabled: {enabled}");

                return enabled;
            }
            // patch into the Start method of the destructible item
            // the Beacon class is what allows the Wishbone to find items
            // any object with the Beacon component will be "findable"
            private static void Postfix(ref Destructible __instance)
            {
                HoverText ht = __instance.GetComponent<HoverText>();
                if (null != ht)
                {
                    //Plugin.LogDebug($"Destructible {__instance.name} HT {ht.m_text}");

                    switch (ht.m_text)
                    {
                        case "$piece_deposit_tin": //verified
                            {
                                    GameObject gameObject = __instance.gameObject;
                                    UpdateBeacon(ref gameObject, ht.m_text, Settings.DetectTinDistance.Value);
                                    break;

                            }
                        case "$piece_deposit_copper": //verified
                            {
                                    GameObject gameObject = __instance.gameObject;
                                    UpdateBeacon(ref gameObject, ht.m_text, Settings.DetectCopperDistance.Value);
                                    break;

                            }
                        case "$piece_deposit_silvervein": //verified... actually added so find the correct one...
                            {
                                    GameObject gameObject = __instance.gameObject;
                                    UpdateBeacon(ref gameObject, ht.m_text, Settings.DetectSilverDistance.Value);
                                    break;
                            }
                        case "$piece_mudpile": //verified   
                            {
                                    GameObject gameObject = __instance.gameObject;
                                    UpdateBeacon(ref gameObject, ht.m_text, Settings.DetectMudPileDistance.Value);
                                    break;
                            }

                    }
                }


            }
        }

        [HarmonyPatch(typeof(Humanoid), "Awake")]
        private static class Humanoid_Awake_Patch
        {
            [HarmonyPrepare]
            static bool IsWishBoneTweakEnabled()
            {
                bool enabled = Settings.WishBoneTweakEnabled.Value;

                Plugin.Log($"WishBoneTweakEnabled: {enabled}");

                return enabled;
            }
            // patch into the Start method of the destructible item
            // the Beacon class is what allows the Wishbone to find items
            // any object with the Beacon component will be "findable"
            private static void Postfix(ref Humanoid __instance)
            {
                string name = __instance.m_name;

                if (null != name)
                {
                    //Plugin.LogDebug($"Humanoid {name} ");
                    switch (name)
                    {
                        case "$enemy_deathsquito": //verified
                                GameObject gameObject = __instance.gameObject;
                                UpdateBeacon(ref gameObject, name, Settings.DetectDeathsquitoDistance.Value);
                                break;

                    }
                }


            }
        }

        [HarmonyPatch(typeof(Piece), "Awake")]
        private static class Piece_Awake_Patch
        {
            [HarmonyPrepare]
            static bool IsWishBoneTweakEnabled()
            {
                bool enabled = Settings.WishBoneTweakEnabled.Value;

                Plugin.Log($"WishBoneTweakEnabled: {enabled}");

                return enabled;
            }
            // patch into the Start method of the destructible item
            // the Beacon class is what allows the Wishbone to find items
            // any object with the Beacon component will be "findable"
            private static void Postfix(ref Piece __instance)
            {
                //Plugin.LogDebug($"Piece Name: {__instance.name}, m_name: {__instance.m_name} ");

                if(__instance.name.ToLower().Contains("_buried"))
                {
                    GameObject gameObject = __instance.gameObject;
                    UpdateBeacon(ref gameObject, __instance.name, Settings.DetectBuriedDistance.Value);
                }
               
            }
        }

        private static void AddBeacon(ref GameObject gameObject, string item, float distance)
        {
            Beacon beacon = gameObject.AddComponent<Beacon>();
            beacon.m_range = distance;
            Plugin.LogVerbose($"Added Beacon to {item} Range: {beacon.m_range}");
        }

        private static void UpdateBeacon(ref GameObject gameObject, string item, float distance)
        {

            Beacon[] beacons = gameObject.GetComponents<Beacon>();

            if(beacons.Length == 0 && distance > 0)
            {
                AddBeacon(ref gameObject, item, distance);
            }

            for (int i = 0; i < beacons.Length; i++)
            {
                Beacon beacon = beacons[i];
                if(distance != beacon.m_range)
                {
                    Plugin.LogVerbose($"UpdateBeacon: GameObject: {gameObject.name} Old Distance {beacon.m_range}, New Distance {distance}");
                    if (distance == 0)
                    {
                        Plugin.LogVerbose($"UpdateBeacon: GameObject: {gameObject.name} Destroying Beacon");
                        Game.Destroy(beacon);
                    }
                    else
                    {
                        beacon.m_range = distance;
                    }
                }
            }

        }
    }
}
