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

                    if(null != Player.m_localPlayer)
                    {
                        Player.m_localPlayer.m_firstSpawn = !Settings.DisableValkryieRide.Value;
                    }

                    Plugin.Log("Finding Spawn");
                    // find a new starting position
                    bool foundSpot = false;
                   
                    int x = 0;
                    float y = 0;
                    int z = 0;
                    Vector3 tempLocation = new Vector3(x, y, z);

                    while (spawnPoint == Vector3.zero)
                    { 
                        x = UnityEngine.Random.Range(-2500, 2020);
                        z = UnityEngine.Random.Range(-1549, 908);
                        tempLocation.x = x;
                        tempLocation.z = z;
                        // note: In Vector3 X is LEFT to RIGHT on map? Y is HEIGHT and Z is TOP/BOTTOM???

                        Vector3 newLocation = new Vector3(0, 0, 0);
                        //y = ZoneSystem.instance.GetGroundHeight(tempLocation);

                        Heightmap.Biome biome = Heightmap.Biome.None;

                        float height = WorldGenerator.instance.GetHeight(tempLocation.x,tempLocation.z);
                        Plugin.Log($"searching {tempLocation}");
                        tempLocation.y = height;
                        biome = WorldGenerator.instance.GetBiome(tempLocation);

                        Plugin.Log($"foind {biome}");
                        if (biome == Heightmap.Biome.Meadows)
                        {
                            spawnPoint = tempLocation;
                        }
 
                    }

                    point = spawnPoint;
                    ZNet.instance.SetReferencePosition(point);
                    __result = ZNetScene.instance.IsAreaReady(point);
                    Plugin.LogVerbose("Waiting for area to be ready");
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
    }
}
