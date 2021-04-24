using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MofoMojo.MMRandomStartPosition
{
    class MMRandomStartPosition_patch
    {
        private static Vector3 spawnPoint = Vector3.zero;
        private const int MinDistanceBetweenSearchScope = 200;
        private static int SpawnCheckCount = 0;
        
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
                SpawnCheckCount = 0;
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
                    UnityEngine.Random.InitState(DateTime.Now.Year + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Millisecond);
                    // let the original code run
                    if (!Settings.MMRandomStartPositionEnabled.Value) return true;
                    Plugin.LogVerbose("Does Player HaveLogoutPoint?");
                    if (__instance.m_playerProfile.HaveLogoutPoint()) return true;

                    Plugin.LogVerbose("Does Player HaveCustomSpawnPoint?");
                    if (__instance.m_playerProfile.HaveCustomSpawnPoint() && !Settings.RandomSpawnOnDeath.Value) return true;

                    // get the HomePoint for the player. It's Vector3.Zero by default
                    Plugin.LogVerbose("Does Player GetHomePoint?");
                    if(!Settings.RandomSpawnOnDeath.Value) spawnPoint = __instance.m_playerProfile.GetHomePoint();

                    Plugin.LogVerbose($"HomePoint is {spawnPoint}");

                    // if we've executed on this more than MaxSpawnPointChecks times, just return true
                    if (SpawnCheckCount > Settings.MaxSpawnPointChecks.Value)
                    {
                        Plugin.Log($"Breaking out, SpawnCheckCount is {SpawnCheckCount}");
                        return true;
                    }

                    if (spawnPoint == Vector3.zero)
                    {
                        int x = 0;
                        float y = 0;
                        int z = 0;
                        Vector3 tempLocation = new Vector3(x, y, z);

                        // meadows won't spawn out farther than this distance from the middle of the map so no point supporting a number higher than this
                        float maxDistance = WorldGenerator.worldSize;
                        float minDistance = 0;

                        // if not ignoring WorldGenerator constraints on biomes, then enforce them
                        if (Settings.IgnoreWorldGeneratorConstraints.Value)
                        {
                            Plugin.Log("Ignoring WorldGenerator Min/Max Constraints");
                        }
                        else
                        {
                            switch (Settings.Biome.Value)
                            {
                                case Heightmap.Biome.AshLands:
                                    minDistance = WorldGenerator.ashlandsMinDistance;
                                    break;
                                case Heightmap.Biome.BlackForest:
                                    maxDistance = WorldGenerator.maxDeepForestDistance;
                                    minDistance = WorldGenerator.minDeepForestDistance;
                                    break;
                                case Heightmap.Biome.DeepNorth:
                                    minDistance = WorldGenerator.deepNorthMinDistance;
                                    break;
                                case Heightmap.Biome.Meadows:
                                    maxDistance = WorldGenerator.meadowsMaxDistance;
                                    break;
                                case Heightmap.Biome.Mountain:
                                    minDistance = WorldGenerator.m_instance.m_minMountainDistance;
                                    break;
                                case Heightmap.Biome.Swamp:
                                    maxDistance = WorldGenerator.maxMarshDistance;
                                    minDistance = WorldGenerator.minMarshDistance;
                                    break;
                            }
                        }

                        // make sure max is less than max distance and more than minX
                        float maxX = UtilityClass.Clamp(Settings.MaxXDistance.Value, Settings.MinXDistance.Value, maxDistance);
                        float maxZ = UtilityClass.Clamp(Settings.MaxZDistance.Value, Settings.MinZDistance.Value, maxDistance);

                        // make sure Min#Distance > 0
                        float minX = (Settings.MinXDistance.Value > 0) ? Settings.MinXDistance.Value : 0;
                        float minZ = (Settings.MinZDistance.Value > 0) ? Settings.MinZDistance.Value : 0;

                        // clamp minDinstance between actual minDistance for biome and maxdistance
                        minX = UtilityClass.Clamp(minX, minDistance, maxDistance);
                        minZ = UtilityClass.Clamp(minZ, minDistance, maxDistance);

                        if (Settings.MaxXDistance.Value != maxX) Plugin.LogVerbose($"Modified MaxXDistance from {Settings.MaxXDistance.Value} to {maxX}");
                        if (Settings.MaxZDistance.Value != maxZ) Plugin.LogVerbose($"Modified MaxZDistance from {Settings.MaxZDistance.Value} to {maxZ}");
                        if (Settings.MinXDistance.Value != minX) Plugin.LogVerbose($"Modified MinXDistance from {Settings.MinXDistance.Value} to {minX}");
                        if (Settings.MinZDistance.Value != minZ) Plugin.LogVerbose($"Modified MinZDistance from {Settings.MinZDistance.Value} to {minZ}");
                        
                        Plugin.Log($"Looking for Biome: {Settings.Biome.Value} - Type {Settings.BiomeAreaType.Value}");
                        Plugin.Log($"Looking minX Distance: {minX}, maxX Distance:{maxX}");
                        Plugin.Log($"Looking minZ Distance: {minZ}, maxZ Distance:{maxZ}");
                        while (spawnPoint == Vector3.zero)
                        {

                            if (SpawnCheckCount > Settings.MaxSpawnPointChecks.Value)
                            {
                                Plugin.Log($"Breaking out SpawnCheck is {SpawnCheckCount}");
                                return true;
                            }

                            SpawnCheckCount++;
                            
                            // set this as our bounds
                            x = UnityEngine.Random.Range((int)Math.Round(minX), (int)Math.Round(maxX));
                            z = UnityEngine.Random.Range((int)Math.Round(minZ), (int)Math.Round(maxZ));
                            
                            // using the range above, now randomly decide to to switch one to negative or leave positive. This provides a potential range of distance in the spawn away from 0,0
                            // Int based Random.Range the max is exclusive (unlike float)
                            if (UnityEngine.Random.Range(1, 3) == 1) x = -x;
                            if (UnityEngine.Random.Range(1, 3) == 1) z = -z;
                            Plugin.LogVerbose($"SpawnCheckCount: {SpawnCheckCount}, X:{x} , z:{z}");

                            // note: In Vector3 X is LEFT to RIGHT on map? Y is HEIGHT and Z is TOP/BOTTOM???
                            tempLocation.x = x;
                            tempLocation.z = z;

                            Heightmap.Biome biome = Heightmap.Biome.None;
                            Heightmap.BiomeArea biomeArea = Heightmap.BiomeArea.Everything;

                            //try to find a valid height line...
                            float height = WorldGenerator.instance.GetHeight(tempLocation.x, tempLocation.z);
                            Plugin.LogVerbose($"Initial coords: {tempLocation}");
                            
                            // only search here if the height is heigher than the water level as it still has a tendency to pull other biomes
                            if (height > ZoneSystem.instance.m_waterLevel || Settings.IgnoreWaterDepthCheck.Value) // && height < WorldGenerator.mountainBaseHeightMin * 85)
                            {
                                tempLocation.y = height;

                                //This is less churn
                                biome = WorldGenerator.instance.GetBiome(tempLocation);
                                biomeArea = WorldGenerator.instance.GetBiomeArea(tempLocation);


                                Plugin.Log($"found {biome}, {biomeArea} @ {tempLocation}");

                                // match on the returned biome type unless the player didn't care and chose Everything
                                // match on biome and biometype but don't match on actual biome of None....
                                // Settings.Biome.Value.None will match on anything, Settings.biome.BiomesMax will match on anything

                                if (biome != Heightmap.Biome.None && (biome == Settings.Biome.Value || Settings.Biome.Value == Heightmap.Biome.None || Settings.Biome.Value == Heightmap.Biome.BiomesMax) && (biomeArea == Settings.BiomeAreaType.Value || Settings.BiomeAreaType.Value == Heightmap.BiomeArea.Everything))
                                {
                                    Plugin.Log($"found new spawnpoint {biome} @ {tempLocation}");
                                    spawnPoint = tempLocation;
                                    __instance.m_playerProfile.SetHomePoint(spawnPoint + Vector3.up * 2f);
                                    // register a starter location nearby
                                    // just a method to dump these starter locations out when verbose is enabled and only then
                                    if (Settings.PluginLoggingLevel.Value == Plugin.LoggingLevel.Verbose)
                                    {
                                        Plugin.LogVerbose("Dumping Zones Coordinates");
                                        foreach (var zone in ZoneSystem.instance.m_generatedZones)
                                        {
                                            Plugin.LogVerbose($"Zone Vector2i: {zone}");
                                            Plugin.LogVerbose($"  Zone Position: {ZoneSystem.instance.GetZonePos(zone)}");
                                        }

                                        Plugin.LogVerbose("Dumping ZoneLocation Names");
                                        foreach (ZoneSystem.ZoneLocation item in ZoneSystem.instance.m_locations)
                                        {

                                            Plugin.LogVerbose($"ZoneLocation Name: {item.m_prefabName}");
                                            //if (item.m_prefabName == "StartTemple")
                                            if (item.m_prefabName == "Eikthyrnir" || item.m_prefabName == "StartTemple")
                                            {
                                                //Remove previous locations of this?
                                                //ZoneSystem.instance.RemoveUnplacedLocations(item);
                                                //item.m_unique = false;
                                                //ZoneSystem.instance.RegisterLocation(item, spawnPoint, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }


                    point = spawnPoint + Vector3.up * 2f;
                    ZNet.instance.SetReferencePosition(point);
                    __result = ZNetScene.instance.IsAreaReady(point);
                    Plugin.Log($"Waiting for area to be ready. Spawnpoint {point}");

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
                if (null != Player.m_localPlayer && Settings.MMRandomStartPositionEnabled.Value) 
                {
                    if (__instance.m_firstSpawn == true) __instance.m_firstSpawn = false;
                }

                return true;
            }

            [HarmonyPostfix]
            static void Postfix(Player __instance)
            {
                // DisableValkryieRide if this is first spawn and player wants to
                if (null != Player.m_localPlayer && Settings.MMRandomStartPositionEnabled.Value)
                {
                    //clear the original spawnpoint just in case...
                    spawnPoint = Vector3.zero;
                }
            }
        }
    }
}

