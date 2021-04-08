using HarmonyLib;
using System;
using UnityEngine;

namespace MofoMojo.MMRandomStartPosition
{
    class MMRandomStartPosition_patch
    {
        private static Vector3 spawnPoint = Vector3.zero;
        
        [HarmonyPatch(typeof(EnvMan), "Awake")]
        static class Game_Awake
        {
            [HarmonyPrepare]
            static bool IsMMRandomStartPositionEnabled()
            {
                bool enabled = Settings.MMRandomStartPositionEnabled.Value;
                Plugin.Log($"MMRandomStartPositionEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            static bool Prefix()
            {
                // reset the spawnpoint in case it's a new game
                spawnPoint = Vector3.zero;
                return true;
            }
        }

        [HarmonyPatch(typeof(Game), "FindSpawnPoint")]
        static class EnvMan_FindSpawnPoint_Patch
        {

            [HarmonyPrepare]
            static bool IsMMRandomStartPositionEnabled()
            {
                bool enabled = Settings.MMRandomStartPositionEnabled.Value;
                Plugin.Log($"MMRandomStartPositionEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            static bool Prefix(out Vector3 point, out bool usedLogoutPoint, float dt, Game __instance, ref bool __result)
            {
                Plugin.Log("FindSpawnPoint_Patch");
                point = Vector3.zero;
                usedLogoutPoint = false;

                try
                {
                    // let the original code run
                    if(__instance.m_playerProfile.HaveLogoutPoint()) return true;
                    if (__instance.m_playerProfile.HaveCustomSpawnPoint()) return true;
                   
                    int x = 0;
                    float y = 0;
                    int z = 0;
                    Vector3 tempLocation = new Vector3(x, y, z);
                    
                    // meadows won't spawn out farther than this distance from the middle of the map
                    float maxMeadowsDistance = WorldGenerator.meadowsMaxDistance;

                    float maxX = UtilityClass.Clamp(Settings.MaxXDistance.Value, 0, maxMeadowsDistance);
                    float maxz = UtilityClass.Clamp(Settings.MaxZDistance.Value, 0, maxMeadowsDistance);

                    while (spawnPoint == Vector3.zero)
                    {
                        Plugin.Log("Finding Spawn");
                        // set this as our bounds
                        x = UnityEngine.Random.Range(-(int)Math.Round(maxX), (int)Math.Round(maxX));
                        z = UnityEngine.Random.Range(-(int)Math.Round(maxz), (int)Math.Round(maxz));

                        // note: In Vector3 X is LEFT to RIGHT on map? Y is HEIGHT and Z is TOP/BOTTOM???
                        tempLocation.x = x;
                        tempLocation.z = z;

                        Heightmap.Biome biome = Heightmap.Biome.None;

                        float height = WorldGenerator.instance.GetHeight(tempLocation.x,tempLocation.z);
                        Plugin.LogVerbose($"searching {tempLocation}");
                        // only search if the height is heigher than the water level as it still has a tendency to pull other biomes
                        if (height > ZoneSystem.instance.m_waterLevel && height < WorldGenerator.mountainBaseHeightMin * 85)
                        {
                            Plugin.LogVerbose($"Checking {tempLocation}");
                            tempLocation.y = height;

                            biome = WorldGenerator.instance.GetBiome(tempLocation);

                            //biome = Heightmap.FindBiome(tempLocation);

                             Plugin.Log($"found {biome}");
                            if (biome == Heightmap.Biome.Meadows)
                            {
                                Plugin.Log($"found spawnpoint {biome} @ {tempLocation}");
                                spawnPoint = tempLocation;
                            }
                        }
                    }

                    Plugin.LogVerbose("Waiting for area to be ready");
                    point = spawnPoint;
                    ZNet.instance.SetReferencePosition(point);
                    __result = ZNetScene.instance.IsAreaReady(point);
                    return false;

                }
                catch (Exception ex)
                {
                    // do nothing, just swallow it up
                    Plugin.LogError(ex.ToString());
                }

                return true;
            }
        }

        // Path Player OnSpawned to override the m_firstSpawn value on player awake if necessary
        [HarmonyPatch(typeof(Player), "OnSpawned")]
        static class Player_OnSpawned
        {
            [HarmonyPrepare]
            static bool IsMMRandomStartPositionEnabled()
            {
                bool enabled = Settings.MMRandomStartPositionEnabled.Value & Settings.DisableValkryieRide.Value;
                Plugin.Log($"MMRandomStartPositionEnabled & DisableValkryieRide: {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            static bool Prefix(Player __instance)
            {
                // DisableValkryieRide if this is first spawn and player wants to
                if (null != Player.m_localPlayer)
                {
                    if (__instance.m_firstSpawn == true) __instance.m_firstSpawn = false;
                }

                return true;
            }
        }
    }
}

