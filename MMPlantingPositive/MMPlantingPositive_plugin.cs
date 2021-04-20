using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

// Not working on this anymore now that a redux of Planting Plus has been released
// https://www.nexusmods.com/valheim/mods/1042
// Note: Did not get my tree spawning working.. with it enabled, when you picked up an item it would trigger recipe update and that would break with a nullreferenceexception

namespace MofoMojo.MMPlantingPositive
{
    [BepInPlugin("MofoMojo.MMPlantingPositive", Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "0.1";
        public const string ModName = "MMPlantingPositive";
        Harmony _Harmony;
        public static Plugin Instance;
        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public static Game gameinstance;
        public static float timer = 0;

        public static bool done = false;
        public static ItemDrop cultivatorItemDrop;

        public static GameObject firTree_Sapling;
        public static GameObject beechTree_Sapling;
        public static GameObject pineTree_Sapling;

        public static GameObject birch1Object;
        public static GameObject birch2Object;

        public static GameObject BirchTreeSapling;



        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose,
            Debug
        }

        private void Awake()
        {

            Instance = this;
            Settings.Init();
            PluginLoggingLevel = Settings.PluginLoggingLevel.Value;

            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }

        public static void Log(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.Verbose) Debug.Log(message);
        }

        public static void LogDebug(string message)
        {
            message = $"{ModName}: {message}";
            if (PluginLoggingLevel == LoggingLevel.Debug) Debug.Log(message);
        }

        public void Update()
        {
            // don't do anything if we've already handled it all
            if (done) return;

            // this prevents doing anything too quickly
            timer += Time.deltaTime;

            if (timer > 10)
            {
                timer = 0;

                if (gameinstance == null) gameinstance = Game.instance;

                try
                {
                    // only do this on active game instance and if it hasn't been done yet
                    if (Settings.MMPlantingPositiveEnabled.Value && gameinstance != null && null != ZNet.instance && null != ZNetScene.instance)
                    {
                        cultivatorItemDrop = ObjectDB.instance.m_items.First(item => item.GetComponent<ItemDrop>().name == "Cultivator").GetComponent<ItemDrop>();
                        firTree_Sapling = ZNetScene.instance.GetPrefab("FirTree_Sapling");
                        beechTree_Sapling = ZNetScene.instance.GetPrefab("Beech_Sapling");
                        pineTree_Sapling = ZNetScene.instance.GetPrefab("PineTree_Sapling");


                        if (ZNetScene.instance != null)
                        {
                            AddPickableRecipeToCultivator("Raspberry Bush", "RaspberryBush", "Raspberry", "Plant raspberries to grow a Raspberry Bush", Settings.RaspberryAmount.Value);
                            AddPickableRecipeToCultivator("BlueberryBush", "BlueberryBush", "Blueberries", "Plant blueberries to grow a Blueberry Bush", Settings.BlueberryAmount.Value);
                            AddPickableRecipeToCultivator("Mushrooms", "Pickable_Mushroom", "Mushroom", "Plant mushrooms to grow harvestable mushrooms", Settings.MushroomAmount.Value);
                            AddPickableRecipeToCultivator("Yellow Mushrooms", "Pickable_Mushroom_yellow", "MushroomYellow", "Plant yellow mushrooms to grow harvestable yellow mushrooms", Settings.MushroomYellowAmount.Value);
                            AddPickableRecipeToCultivator("Dandelions", "Pickable_Dandelion", "Dandelion", "Plant dandelions to grow harvestable dandelions", Settings.DandelionAmount.Value);
                            AddPickableRecipeToCultivator("Thistle", "Pickable_Thistle", "Thistle", "Plant thistles to grow harvestable thistle", Settings.ThistleAmount.Value);
                            AddPickableRecipeToCultivator("Cloudberry Bush", "CloudberryBush", "Cloudberry", "Plant Cloudberries to grow a Cloudberry Bush", Settings.CloudberryAmount.Value);


                            //AddTreeRecipeToCultivator(ref BirchTreeSapling, "Birch Sapling", "MMBirch_Sapling", "FineWood", new string[] {"Birch1","Birch2"},"Plant to get a birch sapling", 10);
                            done = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Plugin.LogError($"Exception: {ex.ToString()}");
                    return;
                }



            }
        }

        public void InitSaplings()
        {
            pineTree_Sapling = ZNetScene.instance.GetPrefab("PineTree_Sapling");
            firTree_Sapling = ZNetScene.instance.GetPrefab("FirTree_Sapling");
            beechTree_Sapling = ZNetScene.instance.GetPrefab("Beech_Sapling");
        }

        public void AddPickableRecipeToCultivator(string recipeName, string prefabName, string itemDropName, string description, int amount, int respawnTime = 0)
        {
            try
            {
                LogDebug($"Attempting to add recipe {recipeName}, {prefabName}, {itemDropName}, {description}");
                ItemDrop tempItemDrop = ObjectDB.instance.m_items.First(item => item.GetComponent<ItemDrop>().name == itemDropName).GetComponent<ItemDrop>();

                GameObject tempPrefab = ZNetScene.instance.GetPrefab(prefabName);
                if (null != tempPrefab && null != tempItemDrop)
                {
                    // check to see if we already know a recipe for this prefab
                    if (!cultivatorItemDrop.m_itemData.m_shared.m_buildPieces.m_pieces.Contains(tempPrefab))
                    {
                        Plugin.LogDebug($"{prefabName} instantiated GameObject Name: {tempPrefab.gameObject.name}");

                        // get the pickable. You can set other properties on the pickable
                        // the pickable is what is placed by the cultivator
                        Pickable tempPickable = tempPrefab.GetComponent<Pickable>();
                        if (tempPickable != null)
                        {
                            if (respawnTime > 0)
                            {
                                tempPickable.m_respawnTimeMinutes = respawnTime;
                            }

                        }

                        // create the piece. This is recipe
                        Piece tempPiece = tempPrefab.AddComponent<Piece>();
                        tempPiece.m_name = recipeName;
                        tempPiece.m_description = description;
                        tempPiece.m_category = Piece.PieceCategory.Misc;
                        tempPiece.m_cultivatedGroundOnly = false;
                        tempPiece.m_groundOnly = true;
                        tempPiece.m_groundPiece = true;
                        tempPiece.m_canBeRemoved = true;

                        // match the icon of the drop item
                        tempPiece.m_icon = tempItemDrop.m_itemData.GetIcon();

                        // setup the resource required to build it. There's one for each requirement
                        tempPiece.m_resources = (Piece.Requirement[])new Piece.Requirement[1];
                        tempPiece.m_resources[0] = new Piece.Requirement();
                        tempPiece.m_resources[0].m_resItem = tempItemDrop;
                        tempPiece.m_resources[0].m_amount = amount;
                        tempPiece.m_resources[0].m_recover = false;

                        // now attempt to add the recipe
                        cultivatorItemDrop.m_itemData.m_shared.m_buildPieces.m_pieces.Add(tempPrefab);

                        Plugin.LogDebug($"Added recipe {recipeName}, {prefabName}, {itemDropName}, {description}");
                    }
                    else
                    {
                        Plugin.LogDebug($"{recipeName} already added...");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.LogError($"Exception: {ex.ToString()}");

                // now re-throw so the calling function can handle it.
                // do this so that done doesn't get set...
                throw ex;
            }

        }
        public void AddTreeRecipeToCultivator(ref GameObject gameObject, string recipeName, string prefabName, string itemDropName, string[] grownPrefabNames, string description, int amount, int respawnTime = 0)
        {
            try
            {
                LogDebug($"Attempting to add recipe {recipeName}, {prefabName}, {itemDropName}, {description}");
                List<GameObject> grownPrefabs = new List<GameObject>();

                foreach(string grownprefabName in grownPrefabNames)
                {
                    GameObject temp = ZNetScene.instance.GetPrefab(grownprefabName);
                    if(null != temp)
                    {
                        Plugin.LogDebug($"Found temp: {temp}");
                        grownPrefabs.Add(temp);
                    }
                }

                // get the itemdrop required to build this recipe
                ItemDrop tempItemDrop = ObjectDB.instance.m_items.First(item => item.GetComponent<ItemDrop>().name == itemDropName).GetComponent<ItemDrop>();

                // instantiate a temporary prefab for the sapling
                GameObject tempPrefab = UnityEngine.Object.Instantiate<GameObject>(pineTree_Sapling);

                // rename the sapling prefab 
                tempPrefab.name = prefabName;
                

                // is this required?
                tempPrefab.hideFlags = HideFlags.HideInHierarchy;

                UnityEngine.Object.DontDestroyOnLoad(tempPrefab);

                if (null != tempPrefab && null != tempItemDrop)
                {
                    // check to see if we already know a recipe for this prefab
                    if (!cultivatorItemDrop.m_itemData.m_shared.m_buildPieces.m_pieces.Contains(tempPrefab))
                    {
                        Plugin.LogDebug($"{prefabName} instantiated GameObject Name: {tempPrefab.gameObject.name}");

                        // get the pickable. You can set other properties on the pickable
                        // the pickable is what is placed by the cultivator
                        Plant tempPlant = tempPrefab.GetComponent<Plant>();
                        if (tempPlant != null)
                        {
                            tempPlant.m_name = recipeName;
                            tempPlant.m_needCultivatedGround = false;
                            tempPlant.m_minScale = 0.75f;
                            tempPlant.m_maxScale = 1.25f;
                            tempPlant.m_grownPrefabs = grownPrefabs.ToArray<GameObject>();
                            //tempPlant.m_grownPrefabs = (GameObject[])new GameObject[1] { ZNetScene.instance.GetPrefab(grownPrefabNames[0]) };

                            if (respawnTime > 0)
                            {
                                tempPlant.m_growTime = respawnTime;
                                tempPlant.m_growTimeMax = respawnTime;
                            }

                            Plugin.LogDebug($"{tempPlant} created TempPlant: {recipeName}");

                        }

                        // create the piece. This is recipe
                        Piece tempSapling = tempPrefab.gameObject.AddComponent<Piece>();
                        tempSapling.m_name = recipeName;
                        tempSapling.m_description = description;
                        tempSapling.m_category = Piece.PieceCategory.Misc;
                        tempSapling.m_cultivatedGroundOnly = false;
                        tempSapling.m_groundOnly = true;
                        tempSapling.m_groundPiece = true;


                        // match the icon of the drop item
                        tempSapling.m_icon = tempItemDrop.m_itemData.GetIcon();

                        // setup the resource required to build it. There's one for each requirement
                        tempSapling.m_resources = (Piece.Requirement[])new Piece.Requirement[1];
                        tempSapling.m_resources[0] = new Piece.Requirement();
                        tempSapling.m_resources[0].m_resItem = tempItemDrop;
                        tempSapling.m_resources[0].m_amount = amount;
                        tempSapling.m_resources[0].m_recover = false;

                        //since this is a new item, kind of, add an instance to the ObjectDB?
                        gameObject = tempPrefab;
                        ZNetScene.instance.m_prefabs.Add(tempPrefab);
                        ObjectDB.instance.m_items.Add(tempPrefab);

                        // now attempt to add the recipe
                        cultivatorItemDrop.m_itemData.m_shared.m_buildPieces.m_pieces.Add(tempPrefab);
                        Plugin.LogDebug($"Added recipe {recipeName}, {prefabName}, {itemDropName}, {description}");
                    }
                    else
                    {
                        Plugin.LogDebug($"{recipeName} already added...");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.LogError($"Exception: {ex.ToString()}");

                // now re-throw so the calling function can handle it.
                // do this so that done doesn't get set...
                throw ex;
            }

        }
    }

    internal static class Settings
    {

        public static ConfigEntry<bool> MMPlantingPositiveEnabled;
        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;

        public static ConfigEntry<int> RaspberryAmount;
        public static ConfigEntry<int> BlueberryAmount;
        public static ConfigEntry<int> MushroomAmount;
        public static ConfigEntry<int> MushroomYellowAmount;
        public static ConfigEntry<int> ThistleAmount;
        public static ConfigEntry<int> CloudberryAmount;
        public static ConfigEntry<int> DandelionAmount;

        public static ConfigEntry<int> RasberryRespawnTime;
        public static ConfigEntry<int> BlueberryRespawnTime;
        public static ConfigEntry<int> MushroomRespawnTime;
        public static ConfigEntry<int> MushroomYellowRespawnTime;
        public static ConfigEntry<int> ThistleRespawnTime;
        public static ConfigEntry<int> CloudberryespawnTime;
        public static ConfigEntry<int> DandelionRespawnTime;

        public static void Init()
        {
            MMPlantingPositiveEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("MMPlantingPositive", "MMPlantingPositiveEnabled", true, "Enables MMPlantingPositive mod");
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.Debug, "Supported values are None, Normal, Verbose");

            RaspberryAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PickableAmounts", "RaspberryAmount", 10, "How many are required to plant");
            BlueberryAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PickableAmounts", "BlueberryAmount", 10, "How many are required to plant");
            MushroomAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PickableAmounts", "MushroomAmount", 5, "How many are required to plant");
            MushroomYellowAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PickableAmounts", "MushroomYellowAmount", 5, "How many are required to plant");
            ThistleAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PickableAmounts", "ThistleAmount", 10, "How many are required to plant");
            DandelionAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PickableAmounts", "DandelionAmount", 10, "How many are required to plant");
            CloudberryAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("PickableAmounts", "CloudberryAmount", 5, "How many are required to plant");

            RasberryRespawnTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RespawnTimes", "RasberryRespawnTime", 0, "Time (in real minutes) for respawn. 0 = defaults");
            BlueberryRespawnTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RespawnTimes", "BlueberryRespawnTime", 0, "Time (in real minutes) for respawn. 0 = defaults");
            MushroomRespawnTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RespawnTimes", "MushroomRespawnTime", 0, "Time (in real minutes) for respawn. 0 = defaults");
            MushroomYellowRespawnTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RespawnTimes", "MushroomYellowRespawnTime", 0, "Time (in real minutes) for respawn. 0 = defaults");
            ThistleRespawnTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RespawnTimes", "ThistleRespawnTime", 0, "Time (in real minutes) for respawn. 0 = defaults");
            DandelionRespawnTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RespawnTimes", "DandelionRespawnTime", 0, "Time (in real minutes) for respawn. 0 = defaults");
            CloudberryespawnTime = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RespawnTimes", "CloudberryespawnTime", 0, "Time (in real minutes) for respawn. 0 = defaults");

        }

    }
}
