using BepInEx;
using UnityEngine;
using HarmonyLib;

namespace MofoMojo.MMExplorationTweaks
{
    class Patch
    {
        #region EnableExplorationTweaks
        [HarmonyPatch(typeof(Minimap), "UpdateExplore")]
        public class HarmonyPatch_UpdateExplore
        {
            // Only enable if ExploreMapTweaksEnabled is set
            [HarmonyPrepare]
            static bool IsExploreMapTweaksEnabled()
            {
                bool enabled = Settings.ExploreMapTweaksEnabled.Value;
                Plugin.Log($"ExploreMapTweaksEnabled  {enabled}");
                Plugin.Log($"ExploreTimeTweaksEnabled  {Settings.ExploreTimeTweaksEnabled.Value}");
                Plugin.Log($"ExploreWeatherTweaksEnabled  {Settings.ExploreWeatherTweaksEnabled.Value}");

                return enabled;
            }

            [HarmonyPostfix]
            private static void UpdateExplorePostfix(ref Minimap __instance, float dt, Player player)
            {
                // if it's not time to update the map, return...
                if (__instance.m_exploreTimer != 0f || null == player) return;

                // if the game is paused, do nothing...
                if (Game.IsPaused()) return;

                // is the player dead?
                if(player.IsDead())
                {
                    // do nothing and return if PlayerDied is true, as we've already reset map
                    if (Plugin.PlayerDied) return;
                    
                    // track that the player has died so we don't continuosly reset the map
                    Plugin.PlayerDied = true;
                    
                    if (Settings.ResetMapOnDeath.Value)
                    {
                        // reset the map
                        Minimap.instance.Reset();
                     }

                    if(Settings.RemovePinsOnDeath.Value)
                    {
                        Minimap.instance.ClearPins();
                    }
                    
                    // return
                    return;
                }

                // reset player died if it was set already
                Plugin.PlayerDied = false;

                float hourImpact = Plugin.GetHourImpact();
                float adjustedSiteDistance = 300;
                float weatherImpact = Plugin.GetWeatherImpact(out adjustedSiteDistance);
                float totalImpact = hourImpact + weatherImpact;


                // if the player is controlling a ship
                if ((bool)player.GetControlledShip() || null != Ship.GetLocalShip())
                {
                    // recalculate ship impact with additional penalty if there's weather impact
                    if (weatherImpact < 0)
                    {
                        weatherImpact += Settings.OnShipEnvironmentalPenalty.Value;
                        totalImpact = hourImpact + weatherImpact;
                    }

                    dt = Settings.ExploreOnShipRadius.Value;
                }
                else
                {
                    dt = Settings.ExploreOnFootRadius.Value;

                    Character playerchar = player as Character;

                    if (playerchar.IsRunning())
                    {
                        // Impact running
                        totalImpact += Settings.RunningImpact.Value;
                    }
                    else if (playerchar.IsSwiming())
                    {
                        // impact swimming
                        totalImpact += Settings.SwimmingImpact.Value;
                    }
                    else if (playerchar.IsSneaking())
                    {
                        // impact swimming
                        totalImpact += Settings.SneakingImpact.Value;
                    }

                }

                // calculate new map view distance
                float modifier = dt * totalImpact;
                
                dt = dt + modifier;


                // clamp the distance. 
                dt = Mathf.Clamp(dt, Settings.MinimumExplorationRadius.Value, Settings.MaximumExplorationRadius.Value);

                if(Settings.FogDensityAffectsExplorationDistance.Value)
                {
                    dt = Mathf.Clamp(dt, 0, adjustedSiteDistance);
                }

                Plugin.LogVerbose($"Weather Impact: {weatherImpact} TimeImpact: {hourImpact} TotalImpact: {totalImpact} Modifier: {modifier} DT: {dt} adjustedSiteDistance: {adjustedSiteDistance}");

                // If there's still a net positive value to the exploration radius, then explore it. 
                if (dt > 0f)
                {
                    Plugin.LogVerbose($"DT: {dt}");
                    // original
                    //__instance.Explore(player.transform.position, dt);
                    // Explore where player is looking
                    if(Settings.AlsoExploreLineOfSight.Value)
                    {
                        try
                        {
                            Plugin.LogVerbose($"Exploring Ling of Sight");

                            float maxDistance = Settings.MaximumExplorationDistance.Value;

                            if (Settings.FogDensityAffectsExplorationDistance.Value) maxDistance = adjustedSiteDistance;

                            Plugin.LogVerbose($"Max Distance {maxDistance}");

                            //if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out var hit, maxDistance, ZoneSystem.instance.m_solidRayMask))
                            if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out var hit, maxDistance, (int)Plugin.GetMask))
                            {
                                try
                                {
                                    Vector3 target = hit.point;

                                    /*
                                     * zi = (xi – min(x)) / (max(x) – min(x))

                                        where,

                                        xi – Value of the current iteration in your dataset
                                        min(x) – Minimum value in the dataset
                                        max(x) – Maximum value in the dataset
                                        zi – Normalized value of the current iteration
                                     */
                                    float distance = Vector3.Distance(player.transform.position, target);
                                    Plugin.LogVerbose($"Distance pre-clamp: {distance}");
                                    distance = Mathf.Clamp(distance, Settings.MinimumExplorationRadius.Value + 1, Settings.MaximumExplorationDistance.Value);
                                    float percentage = 1 - ((distance - Settings.MinimumExplorationRadius.Value) / (Settings.MaximumExplorationDistance.Value - Settings.MinimumExplorationRadius.Value));

                                    float adjustedDistanceRadius = dt * percentage;
                                    //dt = Mathf.Clamp(dt, 10, Settings.MaximumExplorationRadius.Value);
                                    Plugin.LogVerbose($"Distance: {distance} Percentage: {percentage} adjustedDistanceRadius: {adjustedDistanceRadius}");

                                    __instance.Explore(target, adjustedDistanceRadius);
                                }
                                catch (System.Exception ex)
                                {
                                    Plugin.LogError($"{ex}");
                                    return;
                                }
                            }
                        }
                        catch(System.Exception ex)
                        {
                            Plugin.LogError(ex.Message);
                        }
                        

                    }

                    __instance.Explore(player.transform.position, dt);

                }
                


            }
        }
        #endregion
    }
}
