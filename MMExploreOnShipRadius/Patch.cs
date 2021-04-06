using BepInEx;
using UnityEngine;
using HarmonyLib;

namespace MofoMojo.MMExplorationTweaks
{
    class Patch
    {
        #region EnableExploreShipRadiusTweak
        [HarmonyPatch(typeof(Minimap), "UpdateExplore")]
        public class HarmonyPatch_UpdateExplore
        {
            // Only enable if EnableExploreShipRadiusTweak is set
            [HarmonyPrepare]
            static bool IsExploreShipRadiusTweakEnabled()
            {
                bool enabled = Settings.ExploreOnShipRadiusEnabled.Value;
                Plugin.Log($"EnableExploreShipRadiusTweak  {enabled}");

                return enabled;
            }

            [HarmonyPostfix]
            private static void UpdateExplorePrefix(ref Minimap __instance, float dt, Player player)
            {
                // if it's not time to update the map, return...
                if (__instance.m_exploreTimer != 0f || null == player) return;

                bool extendExploreRadius = false;

                // if the player is controlling a ship
                if ((bool)player.GetControlledShip() || null != Ship.GetLocalShip())
                {
                    extendExploreRadius = true;
                }


                if (extendExploreRadius)
                {
                    __instance.Explore(player.transform.position, Settings.ExploreOnShipRadius.Value);
                }

            }
        }
        #endregion
    }
}
