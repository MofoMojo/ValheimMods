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
                    switch (ht.m_text)
                    {
                        case "$piece_deposit_tin":
                            {
                                if(Settings.DetectTinDistance.Value > 0)
                                {
                                    AddBeacon(ref __instance, ht.m_text, Settings.DetectTinDistance.Value);
                                    break;

                                }
                                break;
                            }
                        case "$piece_deposit_copper":
                            {
                                if (Settings.DetectCopperDistance.Value > 0)
                                {
                                    AddBeacon(ref __instance, ht.m_text, Settings.DetectCopperDistance.Value);
                                    break;

                                }
                                break;
                            }

                    }
                }


            }

            private static void AddBeacon(ref Destructible destructible, string item, float distance)
            {
                Beacon beacon = ((Component)destructible).gameObject.AddComponent<Beacon>();
                beacon.m_range = distance;
                Plugin.LogVerbose($"Found {item} Range: {beacon.m_range}");
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
                    Plugin.LogVerbose($"Found {name}");
                    switch (name)
                    {
                        case "$enemy_deathsquito":
                            if (Settings.DetectDeathquitoDistance.Value > 0)
                            {
                                AddBeacon(ref __instance, name, Settings.DetectDeathquitoDistance.Value);
                                break;

                            }
                            break;
                    }
                }


            }

            private static void AddBeacon(ref Humanoid destructible, string item, float distance)
            {
                Beacon beacon = ((Component)destructible).gameObject.AddComponent<Beacon>();
                beacon.m_range = 60f;
                Plugin.LogVerbose($"Found {item} Range: {beacon.m_range}");
            }

        }

    }
}
