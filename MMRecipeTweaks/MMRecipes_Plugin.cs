using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System.Reflection;
using UnityEngine;




namespace MofoMojo.MMRecipeTweaks
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "MofoMojo.MMRecipeTweaks";
        public const string PluginName = "MMRecipeTweaks";
        public const string PluginVersion = "2.0";
        public static Plugin Instance;
        
        Harmony _Harmony;

        public static LoggingLevel PluginLoggingLevel = LoggingLevel.None;
        public enum LoggingLevel
        {
            None,
            Normal,
            Verbose
        }

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake()
        {
            Instance = this;
            Settings.Init();
            PluginLoggingLevel = Settings.PluginLoggingLevel.Value;

            // Jotunn comes with MonoMod Detours enabled for hooking Valheim's code
            // https://github.com/MonoMod/MonoMod
            On.FejdStartup.Awake += FejdStartup_Awake;

            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("MMRecipeTweaks2 has landed");

            // To learn more about Jotunn's features, go to
            // https://valheim-modding.github.io/Jotunn/tutorials/overview.html
            AddAndUpdateRecipes();

            if(Settings.AllowPortalOverrides.Value) PrefabManager.OnVanillaPrefabsAvailable += InitPortalOverrides;
            if(Settings.MasonryChangesEnabled.Value) PrefabManager.OnVanillaPrefabsAvailable += DoStoneWork;

            _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);


        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchSelf();
        }

        public static void Log(string message)
        {
            message = $"{PluginName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            message = $"{PluginName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            message = $"{PluginName}: {message}";
            if (PluginLoggingLevel > LoggingLevel.None) Debug.LogError(message);
        }

        public static void LogVerbose(string message)
        {
            message = $"{PluginName}: {message}";
            if (PluginLoggingLevel == LoggingLevel.Verbose) Debug.Log(message);
        }

        private void FejdStartup_Awake(On.FejdStartup.orig_Awake orig, FejdStartup self)
        {
            // This code runs before Valheim's FejdStartup.Awake
            Jotunn.Logger.LogInfo("FejdStartup is going to awake");

            // Call this method so the original game method is invoked
            orig(self);

            // This code runs after Valheim's FejdStartup.Awake
            Jotunn.Logger.LogInfo("FejdStartup has awoken");
        }

        private void AddAndUpdateRecipes()
        {
            if (Settings.FishingRodRecipeEnabled.Value) ItemManager.Instance.AddRecipe(registerFishingRod());
            if (Settings.FishingBaitRecipeEnabled.Value) ItemManager.Instance.AddRecipe(registerFishingBait());
            if (Settings.ChainsRecipeEnabled.Value) ItemManager.Instance.AddRecipe(registerChainsRecipe());
            if (Settings.LeatherRecipeEnabled.Value) ItemManager.Instance.AddRecipe(registerLeatherRecipe());
            if (Settings.LeatherScrapsRecipeEnabled.Value) ItemManager.Instance.AddRecipe(registerLeatherScrapsRecipe());
            if (Settings.FineWoodRecipeEnabled.Value)
            {
                ItemManager.Instance.AddRecipe(registerFineWoodRecipe());
                ItemManager.Instance.AddRecipe(registerFineWoodx3Recipe());
            }
        }

        public static void DoStoneWork()
        {
            PrefabManager prefabManager = PrefabManager.Instance;

            foreach(string key in Settings.stoneWork.Keys)
            {
                LogVerbose($"Attempting to fixup {key}");
                //CustomPiece piece = pieceManager.GetPiece(key);
                Piece piece = prefabManager.GetPrefab(key).GetComponent<Piece>();

                if (null != piece)
                {
                    piece.m_resources[0].m_amount = Settings.stoneWork[key];
                    LogVerbose($"Found {key}, new amount {piece.m_resources[0].m_amount}");
                }
            }

        }

        private void InitPortalOverrides()
        {
            if (Settings.AllowIronOreTeleportationEnabled.Value) {
                Plugin.LogVerbose("Setting IronOre Portal(able) ");
                ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>("IronOre");
                item.m_itemData.m_shared.m_teleportable = true;
            }

            if (Settings.AllowIronScrapTeleportationEnabled.Value)
            {
                Plugin.LogVerbose("Setting IronScrap Portal(able) ");
                ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>("IronScrap");
                item.m_itemData.m_shared.m_teleportable = true;
            }

            if (Settings.AllowTinOreTeleportationEnabled.Value) {
                Plugin.LogVerbose("Setting TinOre Portal(able) ");
                ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>("TinOre");
                item.m_itemData.m_shared.m_teleportable = true;
            }
            if (Settings.AllowBlackMetalScrapTeleportationEnabled.Value) {
                Plugin.LogVerbose("Setting BlackMetalScrap Portal(able) ");
                ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>("BlackMetalScrap");
                item.m_itemData.m_shared.m_teleportable = true;
            }
            if (Settings.AllowSilverOreTeleportationEnabled.Value) {
                Plugin.LogVerbose("Setting SilverOre Portal(able) ");
                ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>("SilverOre");
                item.m_itemData.m_shared.m_teleportable = true;
            }
            if (Settings.AllowFlametalOreTeleportationEnabled.Value) {
                Plugin.LogVerbose("Setting FlametalOre Portal(able) ");
                ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>("FlametalOre");
                item.m_itemData.m_shared.m_teleportable = true;
            }
            if (Settings.AllowCopperOreTeleportationEnabled.Value) {
                Plugin.LogVerbose("Setting CopperOre Portal(able) ");
                ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>("CopperOre");
                item.m_itemData.m_shared.m_teleportable = true;
            }

        }
        #region FishingRodRecipe
        private CustomRecipe registerFishingRod()
        {

            // Create a custom recipe with a RecipeConfig
            CustomRecipe fishingRod = new CustomRecipe(new RecipeConfig()
            {
                Name = "Recipe_MMFishingRod",
                Item = "FishingRod",                    // Name of the item prefab to be crafted
                MinStationLevel = 2,
                Amount = 1,
                CraftingStation = "piece_workbench",
                Requirements = new RequirementConfig[]  // Resources and amount needed for it to be crafted
                {
                    new RequirementConfig { Item = "Wood", Amount = 2 },
                    new RequirementConfig { Item = "LinenThread", Amount = 2 }
                }
            });

            return fishingRod;
        }
        #endregion
        private CustomRecipe registerFishingBait()
        {
            CustomRecipe fishingBait = new CustomRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MMFishingBait",

                // Name of the prefab for the crafted item
                Item = "FishingBait",

                Amount = 5,

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_workbench",

                // List of requirements to craft your item
                Requirements = new RequirementConfig[]
                            {
                    new RequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "NeckTail",

                        // Amount required
                        Amount = 1
                    }
                }
            });

            return fishingBait;
        }

        private CustomRecipe registerChainsRecipe()
        {
            CustomRecipe chains = new CustomRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MMChain",

                // Name of the prefab for the crafted item
                Item = "Chain",

                Amount = 1,

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "forge",

                // List of requirements to craft your item
                Requirements = new RequirementConfig[]
                            {
                    new RequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "Iron",

                        // Amount required
                        Amount = 4
                    }
                }
            });

            return chains;
        }

        private CustomRecipe registerLeatherRecipe()
        {
            CustomRecipe leather = new CustomRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MMDeerHide",

                // Name of the prefab for the crafted item
                Item = "DeerHide",

                Amount = 1,

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_workbench",

                // List of requirements to craft your item
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "LeatherScraps",

                        // Amount required
                        Amount = 3
                    }
    }
            });
            return leather;
        }

        private CustomRecipe registerLeatherScrapsRecipe()
        {
            CustomRecipe leatherscraps = new CustomRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MMLeatherScraps",

                // Name of the prefab for the crafted item
                Item = "LeatherScraps",

                Amount = 3,

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_workbench",

                // List of requirements to craft your item
                Requirements = new RequirementConfig[]
                            {
                    new RequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "DeerHide",

                        // Amount required
                        Amount = 1
                    }
                }
            });

            return leatherscraps;



        }

        private CustomRecipe registerFineWoodRecipe()
        {
            CustomRecipe fineWood = new CustomRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MMFineWood",

                // Name of the prefab for the crafted item
                Item = "FineWood",

                Amount = 1,

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_workbench",

                MinStationLevel = 3,

                // List of requirements to craft your item
                Requirements = new RequirementConfig[]
                            {
                    new RequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "Wood",

                        // Amount required
                        Amount = 5
                    }
                }
            });

            return fineWood;



        }

        private CustomRecipe registerFineWoodx3Recipe()
        {
            CustomRecipe fineWood = new CustomRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MMFineWoodx3",

                // Name of the prefab for the crafted item
                Item = "FineWood",

                Amount = 3,

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_workbench",

                MinStationLevel = 3,

                // List of requirements to craft your item
                Requirements = new RequirementConfig[]
                            {
                    new RequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "Wood",

                        // Amount required
                        Amount = 15
                    }
                }
            });

            return fineWood;



        }


        /*
        private void registerLoxMeatSurprise()
        {

            // Items
            ObjectManager.Instance.RegisterItem("MMLoxMeatSurprise");

            // Recipes seconds
            ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
            {
                // Name of the recipe (defaults to "Recipe_YourItem")
                Name = "Recipe_MMLoxMeatSurprise",

                // Name of the prefab for the crafted item
                Item = "MMLoxMeatSurprise",

                // Name of the prefab for the crafting station we wish to use
                // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                CraftingStation = "piece_cauldron",

                // List of requirements to craft your item
                Requirements = new PieceRequirementConfig[]
                {
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "CarrotSoup",

                        // Amount required
                        Amount = 2
                    },
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "LoxPie",

                        // Amount required
                        Amount = 1
                    }
                }
            });
        }
        */


        #region BronzeTweak
        // Modify the Awake method of ObjectDB
        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        private static class MMRecipeTweaks
        {
            // check to see if it's enabled and if not, it won't patch for this mod
            [HarmonyPrepare]
            static bool IsRecipeTweaksEnabled()
            {
                bool enabled = Settings.RecipeTweaksEnabled.Value;
                Plugin.Log($"RecipeTweaksEnabled: {enabled}");

                return enabled;
            }

            // https://valheim-modding.github.io/Jotunn/data/objects/recipe-list.html
            // postfix attach to Awake method of ObjectDB
            [HarmonyPostfix]
            public static void ObjectDB_MMRecipeTweaks(ref ObjectDB __instance)
            {

                foreach (Recipe recipe in __instance.m_recipes)
                {
                    //Plugin.Log($"Looking at {recipe.name}");
                    switch(recipe.name.ToLower())
                    {
                        case "recipe_bronze" :
                            if (Settings.BronzeTweakEnabled.Value) recipe.m_amount = 3;
                            Plugin.Log($"recipe_bronze: {3}");
                            break;
                        case "recipe_bronze5":
                            if (Settings.BronzeTweakEnabled.Value) recipe.m_amount = 15;
                            Plugin.Log($"recipe_bronze5: {15}");
                            break;
                        case "recipe_carrotsoup":
                            recipe.m_amount = Settings.CarrotSoupAmount.Value;
                            Plugin.Log($"recipe_carrotsoup: {Settings.CarrotSoupAmount.Value}");
                            break;
                        case "recipe_serpentstew":
                            recipe.m_amount = Settings.SerpentStewAmount.Value;
                            Plugin.Log($"recipe_serpentstew: {Settings.SerpentStewAmount.Value}");
                            break;
                        case "recipe_deerstew":
                            recipe.m_amount = Settings.DeerStewAmount.Value;
                            Plugin.Log($"recipe_deerstew: {Settings.DeerStewAmount.Value}");
                            break;
                        case "recipe_bloodpudding":
                            recipe.m_amount = Settings.DeerStewAmount.Value;
                            Plugin.Log($"recipe_bloodpudding: {Settings.SerpentStewAmount.Value}");
                            break;
                        case "recipe_fishwraps":
                            recipe.m_amount = Settings.FishWrapsAmount.Value;
                            Plugin.Log($"recipe_fishwraps: {Settings.FishWrapsAmount.Value}");
                            break;
                        case "recipe_mincemeatsauce":
                            recipe.m_amount = Settings.MinceMeatSauceAmount.Value;
                            Plugin.Log($"recipe_mincemeatsauce: {Settings.MinceMeatSauceAmount.Value}");
                            break;
                        case "recipe_meadbasefrostresist":
                            recipe.m_amount = Settings.MeadBaseFrostResistAmount.Value;
                            Plugin.Log($"recipe_meadbasefrostresist: {Settings.MeadBaseFrostResistAmount.Value}");
                            break;
                        case "recipe_meadbasehealthmedium":
                            recipe.m_amount = Settings.MeadBaseHealthMediumAmount.Value;
                            Plugin.Log($"recipe_meadbasehealthmedium: {Settings.MeadBaseHealthMediumAmount.Value}");
                            break;
                        case "recipe_meadbasehealthminor":
                            recipe.m_amount = Settings.MeadBaseHealthMinorAmount.Value;
                            Plugin.Log($"recipe_meadbasehealthminor: {Settings.MeadBaseHealthMinorAmount.Value}");
                            break;
                        case "recipe_meadbasepoisonresist":
                            recipe.m_amount = Settings.MeadBasePoisonResistAmount.Value;
                            Plugin.Log($"recipe_meadbasepoisonresist: {Settings.MeadBasePoisonResistAmount.Value}");
                            break;
                        case "recipe_meadbasestaminamedium":
                            recipe.m_amount = Settings.MeadBaseStaminaMediumAmount.Value;
                            Plugin.Log($"recipe_meadbasestaminamedium: {Settings.MeadBaseStaminaMediumAmount.Value}");
                            break;
                        case "recipe_meadbasestaminaminor":
                            recipe.m_amount = Settings.MeadBaseStaminaMinorAmount.Value;
                            Plugin.Log($"recipe_meadbasestaminaminor: {Settings.MeadBaseStaminaMinorAmount.Value}");
                            break;
                        case "recipe_meadbasetasty":
                            recipe.m_amount = Settings.MeadBaseTastyAmount.Value;
                            Plugin.Log($"recipe_meadbasetasty: {Settings.MeadBaseTastyAmount.Value}");
                            break;
                        case "recipe_turnipstew":
                            recipe.m_amount = Settings.TurnipStewAmount.Value;
                            Plugin.Log($"recipe_turnipstew: {Settings.TurnipStewAmount.Value}");
                            break;
                    }

                }
            }
        }
        #endregion


    }

    internal static class Settings
    {
        // https://valheim-modding.github.io/Jotunn/data/objects/recipe-list.html
        public static ConfigEntry<bool> FishingRodRecipeEnabled;
        public static ConfigEntry<bool> LoxMeatSurpriseRecipeEnabled;
        public static ConfigEntry<bool> FishingBaitRecipeEnabled;
        public static ConfigEntry<bool> ChainsRecipeEnabled;
        public static ConfigEntry<bool> RecipeTweaksEnabled;
        public static ConfigEntry<bool> LeatherScrapsRecipeEnabled;
        public static ConfigEntry<bool> LeatherRecipeEnabled;
        public static ConfigEntry<bool> FineWoodRecipeEnabled;
        public static ConfigEntry<bool> BronzeTweakEnabled;

        public static ConfigEntry<bool> AllowPortalOverrides;
        public static ConfigEntry<bool> AllowCopperOreTeleportationEnabled;
        public static ConfigEntry<bool> AllowIronOreTeleportationEnabled;
        public static ConfigEntry<bool> AllowIronScrapTeleportationEnabled;
        public static ConfigEntry<bool> AllowSilverOreTeleportationEnabled;
        public static ConfigEntry<bool> AllowTinOreTeleportationEnabled;
        public static ConfigEntry<bool> AllowBlackMetalScrapTeleportationEnabled;
        public static ConfigEntry<bool> AllowFlametalOreTeleportationEnabled;
        
        public static ConfigEntry<int> CarrotSoupAmount;
        public static ConfigEntry<int> SerpentStewAmount;
        public static ConfigEntry<int> DeerStewAmount;
        public static ConfigEntry<int> BloodPuddingAmount;
        public static ConfigEntry<int> FishWrapsAmount;
        public static ConfigEntry<int> MinceMeatSauceAmount;
        public static ConfigEntry<int> TurnipStewAmount;
        public static ConfigEntry<int> MeadBaseFrostResistAmount;
        public static ConfigEntry<int> MeadBaseHealthMediumAmount;
        public static ConfigEntry<int> MeadBaseHealthMinorAmount;
        public static ConfigEntry<int> MeadBasePoisonResistAmount;
        public static ConfigEntry<int> MeadBaseStaminaMediumAmount;
        public static ConfigEntry<int> MeadBaseStaminaMinorAmount;
        public static ConfigEntry<int> MeadBaseTastyAmount;

        public static ConfigEntry<bool> MasonryChangesEnabled;
        public static ConfigEntry<int> stone_wall_4x2;
        public static ConfigEntry<int> stone_wall_2x1;
        public static ConfigEntry<int> stone_wall_1x1;
        public static ConfigEntry<int> stone_floor_2x2;
        public static ConfigEntry<int> stone_arch;
        public static ConfigEntry<int> stone_pillar;
        public static ConfigEntry<int> stone_stair;

        public static System.Collections.Generic.Dictionary<string, int> stoneWork = new System.Collections.Generic.Dictionary<string, int>();

        public static ConfigEntry<Plugin.LoggingLevel> PluginLoggingLevel;






        // These are the settings that will be saved in the ..\plugins\mofomojo.cfg file
        public static void Init()
        {
            PluginLoggingLevel = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<Plugin.LoggingLevel>("LoggingLevel", "PluginLoggingLevel", Plugin.LoggingLevel.None, "Supported values are None, Normal, Verbose");
            FishingRodRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "FishingRodRecipeEnabled", true, "Enables  a recipe for Fishing Rods");
            LeatherScrapsRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "LeatherScrapsRecipeEnabled", true, "Enables  a recipe for converting Leather to LeatherScraps");
            LeatherRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "LeatherRecipeEnabled", true, "Enables  a recipe for converting LeatherScraps to Leather");
            FineWoodRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "FineWoodRecipeEnabled", true, "Enables  crafting FineWood from 5 Wood with a min Workbench level of 3");

            FishingBaitRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "FishingBaitRecipeEnabled", true, "Enables  a recipe for bait made from Necktails");
            LoxMeatSurpriseRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "LoxMeatSurpriseRecipeEnabled", true, "Enables a recipe and item for Lox Meat Surprise");
            ChainsRecipeEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Recipes", "ChainsRecipeEnabled", true, "Enables a recipe for making chains (4 Iron = 1 chain)");
            RecipeTweaksEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("RecipeTweaks", "RecipeTweaksEnabled", true, "Enabled Various Recipe Tweaks below");
            BronzeTweakEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("RecipeTweaks", "BronzeTweakEnabled", true, "Changes Bronze Recipe from 2 copper+1 tin = 1 bronze to 2+1=3 (and the x5 recipe too)");
            CarrotSoupAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "CarrotSoupAmount", 3, "The amount of carrot soup a single recipe makes");
            SerpentStewAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "SerpentStewAmount", 2, "The amount of Serpent Stew a single recipe makes");
            DeerStewAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "DeerStewAmount", 2, "The amount of Deer Stew a single recipe makes");
            BloodPuddingAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "BloodPuddingAmount", 2, "The amount of Blood Pudding a single recipe makes");
            FishWrapsAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "FishWrapsAmount", 2, "The amount of Fish Wraps a single recipe makes");
            MinceMeatSauceAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "MinceMeatSauceAmount", 2, "The amount of mince meat sauce a single recipe makes");
            TurnipStewAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "TurnipStewAmount", 3, "The amount of Turnip Stew a single recipe makes");
            
            MeadBaseFrostResistAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "MeadBaseFrostResistAmount", 2, "The amount of mead a single recipe makes");
            MeadBaseHealthMediumAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "MeadBaseHealthMediumAmount", 2, "The amount of mead a single recipe makes");
            MeadBaseHealthMinorAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "MeadBaseHealthMinorAmount", 2, "The amount of mead a single recipe makes");
            MeadBasePoisonResistAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "MeadBasePoisonResistAmount", 2, "The amount of mead a single recipe makes");
            MeadBaseStaminaMediumAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "MeadBaseStaminaMediumAmount", 2, "The amount of mead a single recipe makes");
            MeadBaseStaminaMinorAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "MeadBaseStaminaMinorAmount", 2, "The amount of mead a single recipe makes");
            MeadBaseTastyAmount = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("RecipeTweaks", "MeadBaseTastyAmount", 2, "The amount of mead a single recipe makes");

            MasonryChangesEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("Masonry", "MasonryChangesEnabled", true, "Enables  a stone recipe changes");
            stone_wall_4x2 = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Masonry", "stone_wall_4x2", 4, "The amount of stone to make this item");
            stone_wall_2x1 = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Masonry", "stone_wall_2x1", 2, "The amount of stone to make this item");
            stone_wall_1x1 = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Masonry", "stone_wall_1x1", 1, "The amount of stone to make this item");
            stone_floor_2x2 = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Masonry", "stone_floor_2x2", 2, "The amount of stone to make this items");
            stone_arch = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Masonry", "stone_arch", 3, "The amount of stone to make this item");
            stone_pillar = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Masonry", "stone_pillar", 3, "The amount of stone to make this item");
            stone_stair = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<int>("Masonry", "stone_stair", 4, "The amount of stone to make this item");

            AllowPortalOverrides = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PortalOverrides", "AllowPortalOverrides", true, "Enable this to support portal overrides below");
            AllowCopperOreTeleportationEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PortalOverrides", "AllowCopperOreTeleportationEnabled", true, "Enable this metal to go through portals");
            AllowIronOreTeleportationEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PortalOverrides", "AllowIronOreTeleportationEnabled", true, "Enable this metal to go through portals");
            AllowIronScrapTeleportationEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PortalOverrides", "AllowIronScrapTeleportationEnabled", true, "Enable this metal to go through portals");
            AllowSilverOreTeleportationEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PortalOverrides", "AllowSilverOreTeleportationEnabled", true, "Enable this metal to go through portals");
            AllowTinOreTeleportationEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PortalOverrides", "AllowTinOreTeleportationEnabled", true, "Enable this metal to go through portals");
            AllowBlackMetalScrapTeleportationEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PortalOverrides", "AllowBlackMetalScrapTeleportationEnabled", true, "Enable this metal to go through portals");
            AllowFlametalOreTeleportationEnabled = ((BaseUnityPlugin)Plugin.Instance).Config.Bind<bool>("PortalOverrides", "AllowFlametalOreTeleportationEnabled", true, "Enable this metal to go through portals");

            stoneWork.Add("stone_wall_4x2", stone_wall_4x2.Value);
            stoneWork.Add("stone_wall_2x1", stone_wall_2x1.Value);
            stoneWork.Add("stone_wall_1x1", stone_wall_1x1.Value);
            stoneWork.Add("stone_floor_2x2", stone_floor_2x2.Value);
            stoneWork.Add("stone_arch", stone_arch.Value);
            stoneWork.Add("stone_pillar", stone_pillar.Value);
            stoneWork.Add("stone_stair", stone_stair.Value); 

        }

    }
}
