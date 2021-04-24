using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MofoMojo.MMRandomSpawnPoint
{
    class MMRandomSpawnPoint_patch
    {
        [HarmonyPatch(typeof(EnvMan), "Awake")]
        static class Game_Awake
        {
            [HarmonyPrepare]
            static bool IsMMRandomSpawnPointEnabled()
            {
                bool enabled = Settings.MMRandomSpawnPointEnabled.Value;
                Plugin.Log($"MMRandomSpawnPointEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            static bool Prefix()
            {
                // reset this on a new game (just in case)
                Game_FindSpawnPoint_Patch.Reset();
                return true;
            }
        }

        [HarmonyPatch(typeof(Game), "FindSpawnPoint")]
        static class Game_FindSpawnPoint_Patch
        {

            public enum SpawnPointStatus
            {
                Ready,
                Found,
                NotFound
            }

            public static SpawnPointStatus FindSpawnPointStatus = SpawnPointStatus.Ready;
            public static Vector3 SpawnPoint = Vector3.zero;

            [HarmonyPrepare]
            static bool IsMMRandomSpawnPointEnabled()
            {
                bool enabled = Settings.MMRandomSpawnPointEnabled.Value;
                Plugin.Log($"MMRandomSpawnPointEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            static bool Prefix(out Vector3 point, out bool usedLogoutPoint, float dt, Game __instance, ref bool __result)
            {
                Plugin.Log("FindSpawnPoint_Patch");

                point = SpawnPoint;
                usedLogoutPoint = false;

                try
                {
                    // let the original code run
                    if (!Settings.MMRandomSpawnPointEnabled.Value) return true;

                    // we previously found a spawn point...
                    if (FindSpawnPointStatus == SpawnPointStatus.Found)
                    {
                        ZNet.instance.SetReferencePosition(SpawnPoint);
                        __result = ZNetScene.instance.IsAreaReady(SpawnPoint);
                        return false;
                    }

                    // if the status NotFound, then it's already searched and not found a spawn point, so run the default.
                    if (FindSpawnPointStatus == SpawnPointStatus.NotFound) return true;

                    Plugin.LogVerbose("Does Player HaveLogoutPoint?");
                    if (__instance.m_playerProfile.HaveLogoutPoint()) return true;

                    Plugin.LogVerbose("Is Player dead?");
                    // If RandomSpawnOnDeath is TRUE and player is dead, then clear spawn point and also clear homepoint
                    // if this is not the players first respawn in the game, then the player died....
                    // but the Player object isn't accessible at this level
                    bool playerDied = false;

                    if (!__instance.m_firstSpawn)
                    {
                        Plugin.LogVerbose($"Player died? yes");
                        playerDied = true;
                        if (Settings.RandomSpawnOnDeath.Value)
                        {
                            Plugin.LogVerbose("Clear CustomSpawnPoint and Clearing SetHomePoint");
                            __instance.m_playerProfile.ClearCustomSpawnPoint();
                            __instance.m_playerProfile.SetHomePoint(Vector3.zero);
                        }
                    }
                    else
                    {
                        Plugin.LogVerbose($"Player died? no");
                    }

                    // we got here so... do the things....
                    __instance.m_respawnWait += dt;
                    usedLogoutPoint = false;

                    // copied this code because this needs to run to see if the player has a bed nearby and what to do 
                    // Above, we clear the CustomSpawnPoint if the settings are configured and the player has died...
                    if (__instance.m_playerProfile.HaveCustomSpawnPoint())
                    {
                        Plugin.LogVerbose("Player has CustomSpawnPoint");
                        Vector3 customSpawnPoint = __instance.m_playerProfile.GetCustomSpawnPoint();
                        ZNet.instance.SetReferencePosition(customSpawnPoint);
                        if (__instance.m_respawnWait > 8f && ZNetScene.instance.IsAreaReady(customSpawnPoint))
                        {
                            Bed bed = __instance.FindBedNearby(customSpawnPoint, 5f);
                            if (bed != null)
                            {
                                Plugin.Log($"Found bed at custom spawn point");
                                point = bed.GetSpawnPoint();
                                __result = true;
                                return false;
                            }
                            Plugin.Log($"Failed to find bed at custom spawn point");
                            __instance.m_playerProfile.ClearCustomSpawnPoint();

                            // finally, If the player had a custom spawn point because of bed, but now does not... clear the home point.
                            if(Settings.RandomSpawnOnDeathIfNoBed.Value)
                            {

                                __instance.m_playerProfile.SetHomePoint(Vector3.zero);
                            }
                        }
                        else
                        {
                            // respawn wait time not long enough and/or scene isn't ready so return until it is
                            // this tells the callee it's not found yet..
                            __result = false;

                            // this tells the patch not to run the original code
                            return false;
                        }
                    }

                    // the homepoint is set by UpdateRespawn however, we've patched it so it's never called (because it makes no sense for the original method to need to set this
                    Vector3 homePoint = __instance.m_playerProfile.GetHomePoint();

                    // if the homepoint isn't vector3.zero AND the player had died... then use the homePoint
                    // note: The homepoint was reset to Vector3.zero if the player needed to be forced to a new position. 
                    if (homePoint != Vector3.zero && playerDied)
                    {
                        point = homePoint;
                        FindSpawnPointStatus = SpawnPointStatus.Found;
                        SpawnPoint = point;
                        ZNet.instance.SetReferencePosition(SpawnPoint);
                        __result = ZNetScene.instance.IsAreaReady(SpawnPoint);
                        __instance.m_playerProfile.SetHomePoint(SpawnPoint);
                        return false;
                    }

                    bool FoundSpawnPoint = FindNewSpawnPoint(Settings.Biome.Value, Settings.BiomeAreaType.Value, Settings.MinXDistance.Value, Settings.MaxXDistance.Value, Settings.MinZDistance.Value, Settings.MaxZDistance.Value, Settings.IgnoreWaterDepthCheck.Value, Settings.IgnoreWorldGeneratorConstraints.Value, Settings.MaxSpawnPointChecks.Value, out point);
                    
                    if(FoundSpawnPoint)
                    {
                        // this identifies that a new spawnpoint has been found (it's reset to Ready after user spawns)
                        FindSpawnPointStatus = SpawnPointStatus.Found;
                        SpawnPoint = point;
                        ZNet.instance.SetReferencePosition(SpawnPoint);
                        __result = ZNetScene.instance.IsAreaReady(SpawnPoint);
                        __instance.m_playerProfile.SetHomePoint(SpawnPoint);
                        return false;
                    }
                    else
                    {
                        // not found so give up and go with the default search behavior
                        FindSpawnPointStatus = SpawnPointStatus.NotFound;

                        // just ensure the SpawnPoint is reset
                        SpawnPoint = Vector3.zero;
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    // do nothing, just log and swallow it up
                    Plugin.LogError(ex.ToString());
                }

                // just run the default... somehow we got here.
                return true;
            }

            public static void Reset()
            {
                SpawnPoint = Vector3.zero;
                FindSpawnPointStatus = SpawnPointStatus.Ready;
            }
        }
 
        [HarmonyPatch(typeof(Game), "UpdateRespawn")]
        static class Game_UpdateRespawn
        {
            [HarmonyPrepare]
            static bool IsMMRandomSpawnPointEnabled()
            {
                bool enabled = Settings.MMRandomSpawnPointEnabled.Value;
                Plugin.Log($"MMRandomStartPositionEnabled: {enabled}");

                return enabled;
            }

            // rewrote UpdateRespawn so that HomePoint is never reset by UpdateRespawn.
            [HarmonyPrefix]
            static bool Prefix(float dt, Game __instance)
            {
                if (__instance.m_requestRespawn && __instance.FindSpawnPoint(out var point, out var usedLogoutPoint, dt))
                {
                    __instance.SpawnPlayer(point);
                    __instance.m_requestRespawn = false;

                    //Reset the status so this will work again if need be
                    Game_FindSpawnPoint_Patch.Reset();

                    if (__instance.m_firstSpawn)
                    {
                        __instance.m_firstSpawn = false;
                        Chat.instance.SendText(Talker.Type.Shout, "I have arrived!");
                    }
                    GC.Collect();
                }

                return false;
            }

        }

        // Path Player OnSpawned to override the m_firstSpawn value on player awake if necessary
        [HarmonyPatch(typeof(Player), "OnSpawned")]
        static class Player_OnSpawned
        {
            [HarmonyPrepare]
            static bool IsMMRandomStartPositionEnabled()
            {
                bool enabled = Settings.MMRandomSpawnPointEnabled.Value & Settings.DisableValkryieRide.Value;
                Plugin.Log($"MMRandomStartPositionEnabled & DisableValkryieRide: {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            static bool Prefix(Player __instance)
            {
                // DisableValkryieRide if this is first spawn and player wants to
                if (null != __instance && Settings.DisableValkryieRide.Value)
                {
                    // m_firstSpawn is technically the marker for when a person first spawns into a session. i.e. doesn't matter if new character or existing character... this is the first time they've spawned into this instance of the server running
                    __instance.m_valkyrie = null;
                }

                return true;
            }

            [HarmonyPostfix]
            static void Postfix(Player __instance)
            {
                // DisableValkryieRide if this is first spawn and player wants to
                if (Settings.MMRandomSpawnPointEnabled.Value)
                {
                    // if we didn't find a spawn point then show a message
                    if(Game_FindSpawnPoint_Patch.FindSpawnPointStatus == Game_FindSpawnPoint_Patch.SpawnPointStatus.NotFound)
                    {
                        // Odin is pleased....
                        //MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, Localization.instance.Localize("$tutorial_stemple4_topic"));
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Hugin takes pity on you...");
                    }
                }

            }
        }

        static bool FindNewSpawnPoint(Heightmap.Biome requestedBiome, Heightmap.BiomeArea requestedBiomeArea, float minXDistance, float maxXDistance, float minZDistance, float maxZDistance, bool ignoreWaterDepthCheck, bool IgnoreWorldGeneratorConstraints, int MaxSpawnPointChecks, out Vector3 point)
        {
            Vector3 tempNewSpawnPosition = Vector3.zero;
            bool found = false;

            int x = 0;
            float y = 0;
            int z = 0;
            Vector3 tempLocation = new Vector3(x, y, z);

            // for maxDistance to the worldSize value...
            float maxDistance = WorldGenerator.worldSize;
            float minDistance = 0;

            // if not ignoring WorldGenerator constraints on biomes, then enforce them
            if (IgnoreWorldGeneratorConstraints)
            {
                Plugin.Log("Ignoring WorldGenerator Min/Max Constraints");
            }
            else
            {
                switch (requestedBiome)
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
            float maxX = UtilityClass.Clamp(maxXDistance, minXDistance, maxDistance);
            float maxZ = UtilityClass.Clamp(maxZDistance, minZDistance, maxDistance);

            // make sure Min#Distance > 0
            float minX = (minXDistance > 0) ? minXDistance : 0;
            float minZ = (minZDistance > 0) ? minZDistance : 0;

            // clamp minDinstance between actual minDistance for biome and maxdistance
            minX = UtilityClass.Clamp(minX, minDistance, maxDistance);
            minZ = UtilityClass.Clamp(minZ, minDistance, maxDistance);

            if (maxXDistance != maxX) Plugin.LogVerbose($"Modified MaxXDistance from {maxXDistance} to {maxX}");
            if (maxZDistance != maxZ) Plugin.LogVerbose($"Modified MaxZDistance from {maxZDistance} to {maxZ}");
            if (minXDistance != minX) Plugin.LogVerbose($"Modified MinXDistance from {minXDistance} to {minX}");
            if (minZDistance != minZ) Plugin.LogVerbose($"Modified MinZDistance from {minZDistance} to {minZ}");

            Plugin.Log($"Looking for Biome: {requestedBiome} - Type {requestedBiomeArea}");
            Plugin.Log($"Looking minX Distance: {minX}, maxX Distance:{maxX}");
            Plugin.Log($"Looking minZ Distance: {minZ}, maxZ Distance:{maxZ}");
            int spawnPointCheckCount = 0;

            while (tempNewSpawnPosition == Vector3.zero && spawnPointCheckCount <= MaxSpawnPointChecks)
            {
                spawnPointCheckCount++;

                // set this as our bounds
                x = UnityEngine.Random.Range((int)Math.Round(minX), (int)Math.Round(maxX));
                z = UnityEngine.Random.Range((int)Math.Round(minZ), (int)Math.Round(maxZ));

                // using the range above, now randomly decide to to switch one to negative or leave positive. This provides a potential range of distance in the spawn away from 0,0
                // Int based Random.Range the max is exclusive (unlike float)
                if (UnityEngine.Random.Range(1, 3) == 1) x = -x;
                if (UnityEngine.Random.Range(1, 3) == 1) z = -z;
                Plugin.LogVerbose($"SpawnCheckCount: {spawnPointCheckCount}, X:{x} , z:{z}");

                // note: In Vector3 X is LEFT to RIGHT on map? Y is HEIGHT and Z is TOP/BOTTOM???
                tempLocation.x = x;
                tempLocation.z = z;

                Heightmap.Biome foundBiome = Heightmap.Biome.None;
                Heightmap.BiomeArea foundBiomeArea = Heightmap.BiomeArea.Everything;

                //try to find a valid height line...
                float height = WorldGenerator.instance.GetHeight(tempLocation.x, tempLocation.z);
                Plugin.LogVerbose($"Initial coords: {tempLocation}");

                // only search here if the height is heigher than the water level as it still has a tendency to pull other biomes
                if (height > ZoneSystem.instance.m_waterLevel || ignoreWaterDepthCheck) // && height < WorldGenerator.mountainBaseHeightMin * 85)
                {
                    tempLocation.y = height;

                    //This is less churn
                    foundBiome = WorldGenerator.instance.GetBiome(tempLocation);
                    foundBiomeArea = WorldGenerator.instance.GetBiomeArea(tempLocation);

                    Plugin.Log($"found {foundBiome}, {foundBiomeArea} @ {tempLocation}");

                    // match on the returned biome type unless the player didn't care and chose Everything
                    // match on biome and biometype but don't match on actual biome of None....
                    // Settings.Biome.Value.None will match on anything, Settings.biome.BiomesMax will match on anything

                    if (foundBiome != Heightmap.Biome.None && (foundBiome == requestedBiome || Settings.Biome.Value == Heightmap.Biome.None || Settings.Biome.Value == Heightmap.Biome.BiomesMax) && (foundBiomeArea == requestedBiomeArea || Settings.BiomeAreaType.Value == Heightmap.BiomeArea.Everything))
                    {
                        Plugin.Log($"found new spawnpoint {foundBiome} @ {tempLocation}");
                        tempNewSpawnPosition = tempLocation;
                        found = true;
                    }
                }
            }

            point = tempNewSpawnPosition;
            return found;
        }

        [HarmonyPatch(typeof(Chat), "InputText")]
        static class Chat_InputText
        {

            [HarmonyPrepare]
            static bool IsMMWeatherModEnabled()
            {
                bool enabled = Settings.MMRandomSpawnPointEnabled.Value;
                Plugin.Log($"MMRandomSpawnPointEnabled: {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            static bool Prefix(Chat __instance)
            {
                try
                {
                    Player player = Player.m_localPlayer;
                    if (null == player) return true;

                    string text = __instance.m_input.text;

                    if (text.ToLower().StartsWith("/resethomepoint"))
                    {
                        Plugin.LogDebug("Resetting Player SpawnPoint");

                        if (ZoneSystem.instance.GetLocationIcon(Game.instance.m_StartLocation, out var pos))
                        {
                            Vector3 point = pos + Vector3.up * 2f;
                            Game.instance.m_playerProfile.SetHomePoint(Vector3.zero);
                            Plugin.Log($"Player Spawn reset to {Game.instance.m_playerProfile.GetHomePoint()}");
                            return false;
                        }
                        else
                        {
                            Plugin.LogWarning($"Player Spawn could not be reset. GetLocationIcon returned no pos");
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Plugin.LogError($"Exception: {ex}");
                }

                return true;
            }
        }
    }
}

