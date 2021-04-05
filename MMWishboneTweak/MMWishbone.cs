using HarmonyLib;
using UnityEngine;

namespace MMWishboneTweak
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
                                Beacon beacon = ((Component)__instance).gameObject.AddComponent<Beacon>();
                                beacon.m_range = 25f;
                                Plugin.LogVerbose($"Found {ht.m_text} Range: {beacon.m_range}");
                                break;
                            }
                        case "$piece_deposit_copper":
                            {
                                Beacon beacon = ((Component)__instance).gameObject.AddComponent<Beacon>();
                                beacon.m_range = 60f;
                                Plugin.LogVerbose($"Found {ht.m_text} Range: {beacon.m_range}");
                                break;
                            }

                    }
                }


            }
        }
    }
}
