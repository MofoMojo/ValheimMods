using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


namespace MofoMojo.MMPersonalTweaks
{
    class patch
    {
        #region OceanFishingMultiplier
        // This hooks into Pickup on Fish
        [HarmonyPatch(typeof(Fish), "Pickup")]
        public class HarmonyPatch_OceanFishing
        {

            // Only enable if EnableFishingInOceanMultiplierEnabled is set
            [HarmonyPrepare]
            static bool IsEnableFishingInOceanMultiplierEnabled()
            {
                bool enabled = Settings.FishingInOceanMultiplierEnabled.Value;
                Plugin.Log($"EnableFishingInOceanMultiplier {enabled}");

                return enabled;

            }

            [HarmonyPrefix]
            private static void PickupPrefix(Fish __instance)
            {

                var new_m_pickupItemStackSize = __instance.m_pickupItemStackSize;

                Plugin.Log($"Player Fishing PickupPrefix {new_m_pickupItemStackSize}");

                bool inOcean = false;

                List<Player> players = new List<Player>();

                Player.GetPlayersInRange(__instance.gameObject.transform.position, 50, players);

                /*
                foreach (Player player in players)
                {
                    // find the first player in the ocean near the fish
                    if (player.GetCurrentBiome() == Heightmap.Biome.Ocean)
                    {
                        inOcean = true;
                        break;
                    }
                }
                */

                // get the local player catching the fish
                if (Player.m_localPlayer.GetCurrentBiome() == Heightmap.Biome.Ocean)
                {
                    inOcean = true;
                }

                // if the player is in the ocean when fishing, increase fish caught count
                if (inOcean)
                {
                    new_m_pickupItemStackSize = UnityEngine.Random.Range(2, 4) * __instance.m_pickupItemStackSize;
                    __instance.m_pickupItemStackSize = new_m_pickupItemStackSize;
                    Plugin.Log($"Player Fishing PickupPrefix modified: {new_m_pickupItemStackSize}");
                }
                else
                {
                    Plugin.Log($"Player not found in the ocean");
                }



            }
        }
        #endregion

        #region EnableFeatherMultiplier
        /*
         * There are two GEtDropList methods in DropTable. One takes a parameter and one does not, patching normally causes an ambiguousmatchexception
         * https://outward.fandom.com/wiki/Advanced_Modding_Guide/Hooks had a good guide on it
         * ambiguousmatchexception
         * public List<GameObject> GetDropList()
         * private List<GameObject> GetDropList(int amount)
         * 
         * Interestingly... this is a client side patch even on a dedicated server
        */

        [HarmonyPatch(typeof(DropTable), "GetDropList", new Type[] { typeof(int) })]
        public class HarmonyPatch_FeatherMultiplier
        {
            // Only enable if EnableFeatherMultiplier is set
            [HarmonyPrepare]
            static bool IsEnableFeatherMultiplierEnabled()
            {
                bool enabled = Settings.FeatherMultiplierEnabled.Value;
                Plugin.Log($"EnableFeatherMultiplier {enabled}");

                return enabled;
            }

            // This is the method that will be called AFTER GetDropList is called so that we can modify the drop list
            /*
             * Crow and Seagal [sp] utilizes DropOnDestroyed
             * DropOnDestroyed has an OnDestroyed method
             * OnDestroyed calls m_dropWhenDestroyed's GetDropList() method which returns a List of GameObjects
             * we loop through the dropList and create/instantiate objects
             * Other creature types don't do this, they utilize CharacterDrop's GenerateDropList
             * GetDropList() calls GetDropList(amount) and returns it
             */
            [HarmonyPostfix]
            private static void MM_AdjustDropList(ref List<GameObject> __result, DropTable __instance)
            {
                //Clone the old list
                List<GameObject> list = new List<GameObject>(__result);

                // now loop through the old list 
                foreach (GameObject obj in __result)
                {
                    Plugin.Log($"MM_AdjustDropList: GameObject: {obj.name}");

                    //add 1-3 additional feathers) to the NEW list
                    if (obj.name.ToLower() == "feathers")
                    {
                        // Return a random int within [minInclusive..maxExclusive) (Read Only).
                        // Notes on UnityEngine.Random.Range(min int, max int) 
                        // Float numbers work differently where min/max are both inclusive
                        // these are ints below
                        int amount = UnityEngine.Random.Range(1, 3);
                        for (int i = 0; i < amount; i++)
                        {
                            list.Add(obj);
                        }

                        // added the feathers, now don't add any more...
                        // break here so that if there are additional feather entries in the result list we don't find and continue to add more
                        break;
                    }
                }

                // replace the initial List<GameObject> result with the new list
                __result = list;
            }

            /*
             * this is not used....
             * but preserving in case I want to make adjustments elsewhere
             * not applicable to birds as I had hoped but this hooks into the drop table successfully
             * Also not application to FISH... lol. 
             * 
            // This hooks into GenerateDropList on CharacterDrop
            [HarmonyPatch(typeof(CharacterDrop), "GenerateDropList")]
            public class HarmonyPatch_RememberServer
            {

                // Only enable if EnableFeatherMultiplier is set
                [HarmonyPrepare]
                static bool IsEnableFeatherMultiplierEnabled()
                {
                    bool enabled = Settings.EnableFeatherMultiplier.Value;
                    Plugin.Log($"EnableFeatherMultiplier {enabled}");

                    return enabled;

                }

                // This is the method that will be called AFTER GenerateDropList is called so that we can modify the drop list
                [HarmonyPostfix]
                private static void MM_AdjustLoot( ref List<KeyValuePair<GameObject, int>> __result, CharacterDrop __instance)
                {
                    Plugin.Log($"Hooked post GenerateDropList");


                    //Clone the old list
                    List<KeyValuePair<GameObject, int>> list = new List<KeyValuePair<GameObject, int>>(__result);

                    foreach (KeyValuePair<GameObject, int> obj in __result)
                    {
                        Plugin.Log($"KeyValuePair: {obj.Key.name}, Amount {obj.Value}");

                        //add 3-4 additional feathers)
                        if(obj.Key.name.ToLower() == "feathers")
                        {
                            int amount = UnityEngine.Random.Range(1, 4);
                            list.Add(new KeyValuePair<GameObject, int>(obj.Key, amount));

                        }
                    }

                    // return the new list
                    __result = list;
                }
            }
            */


        }

        #endregion

        // Modify the QueueServerJoin method of ZSteamMatchmaking
        // OnJoinIPConnect gets the text from the JoinIPPanel which sets m_joinIPAddress and perhaps m_joinHostPort
        // If everything is on the up and up, it calls ZSteamMatchmaking.instance.QueueServerJoin(text + ":" + num);
        // so hook here for getting the server:port combo
        [HarmonyPatch(typeof(ZSteamMatchmaking), "QueueServerJoin")]
        private static class QueueServerJoin_Patch
        {
            // check to see if it's enabled and if not, it won't patch for this mod
            [HarmonyPrepare]
            static bool IsRemeberLastConnectedIpEnabled()
            {
                bool enabled = Settings.RememberLastConnectedIpEnabled.Value;
                Debug.Log($"RemeberLastConnectedIpEnabled: {enabled}");

                return enabled;
            }

            // prefix attach to QueueServerJoin
            // get the ipaddress and save it to the settings
            [HarmonyPrefix]

            public static void ZSteamMatchmaking_QueueServerJoin(string addr)
            {
                Debug.Log($"Setting LastConnectedIP to: {addr}");
                Settings.LastConnectedIP.Value = addr;
            }

        }

        [HarmonyPatch(typeof(FejdStartup), "OnJoinIPOpen")]
        public static class OnJoinIPOpen_Patch
        {
            // check to see if it's enabled and if not, it won't patch for this mod
            [HarmonyPrepare]
            static bool IsRemeberLastConnectedIpEnabled()
            {
                bool enabled = Settings.RememberLastConnectedIpEnabled.Value;
                Debug.Log($"RemeberLastConnectedIpEnabled: {enabled}");

                return enabled;
            }

            // Set the value of the input field to the value stored in settings if it's not blank or null
            // OnJoinIPOpen sets the IP panel to active and activates the input field...
            // hook here to set m_joinIPAddress prior to this...
            [HarmonyPrefix]
            public static void FejdStartup_OnJoinIPOpen(ref InputField ___m_joinIPAddress)
            {
                string value = Settings.LastConnectedIP.Value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    ___m_joinIPAddress.text = value;
                }
            }
        }
    }
}
