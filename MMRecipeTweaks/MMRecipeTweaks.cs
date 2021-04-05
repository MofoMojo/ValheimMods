using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using JotunnLib.Entities;
using JotunnLib.Managers;

namespace MMRecipeTweaks
{
        public class MMRecipes : BaseUnityPlugin
        {

            #region BronzeTweak
            // Modify the Awake method of ObjectDB
            [HarmonyPatch(typeof(ObjectDB), "Awake")]
            private static class MMEnableBronzeTweak
            {
                // check to see if it's enabled and if not, it won't patch for this mod
                [HarmonyPrepare]
                static bool IsRemeberLastConnectedIpEnabled()
                {
                    bool enabled = Settings.BronzeTweakEnabled.Value;
                    Plugin.Log($"EnableBronzeTweak: {enabled}");

                    return enabled;
                }

                // postfix attach to Awake method of ObjectDB
                [HarmonyPostfix]
                public static void ObjectDB_BronzeTweak(ref ObjectDB __instance)
                {
                    //Plugin.Log($"Looking for Bronze Recipes");
                    foreach (Recipe recipe in __instance.m_recipes)
                    {
                        //Plugin.Log($"Looking at {recipe.name}");

                        // Plugin.Log($"Checking {recipe.name}");
                        if (recipe.name == "Recipe_Bronze")
                        {
                            // Plugin.Log($"Patching {recipe.name}");
                            recipe.m_amount = 3;
                        }
                        else if (recipe.name == "Recipe_Bronze5")
                        {
                            //Plugin.Log($"Patching {recipe.name}");
                            recipe.m_amount = 15;
                        }
                    }
                }
            }

            #endregion

            private void Awake()
            {
                // Register Prefabs First
                PrefabManager.Instance.PrefabRegister += registerPrefabs;

                // Register Objects that use Prefabs after
                ObjectManager.Instance.ObjectRegister += registerObjects;
            }

            // Register new prefabs
            private void registerPrefabs(object sender, EventArgs e)
            {
                // Registering LoxMeatSurprise for Recipe
                if (Settings.LoxMeatSurpriseRecipeEnabled.Value) PrefabManager.Instance.RegisterPrefab(new MMLoxMeatSurprise());
            }

            private void registerObjects(object sender, EventArgs e)
            {
                if (Settings.LoxMeatSurpriseRecipeEnabled.Value) registerLoxMeatSurprise();
                if (Settings.FishingRodRecipeEnabled.Value) registerFishingRod();
                if (Settings.FishingBaitRecipeEnabled.Value) registerFishingBait();
                if (Settings.ChainsRecipeEnabled.Value) registerChainsRecipe();

            }

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

            #region FishingRodRecipe
            private void registerFishingRod()
            {

                ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
                {
                    // Name of the recipe (defaults to "Recipe_YourItem")
                    Name = "Recipe_MMFishingRod",

                    // Name of the prefab for the crafted item
                    Item = "FishingRod",

                    // The minimum station level to craft it
                    MinStationLevel = 2,

                    // The number of items to craft
                    Amount = 1,

                    // Name of the prefab for the crafting station we wish to use
                    // Can set this to null or leave out if you want your recipe to be craftable in your inventory
                    CraftingStation = "piece_workbench",

                    // List of requirements to craft your item
                    Requirements = new PieceRequirementConfig[]
                    {
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "Wood",

                        // Amount required
                        Amount = 3
                    },
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "LinenThread",

                        // Amount required
                        Amount = 2
                    }
                    }
                });
            }
            #endregion
            private void registerFishingBait()
            {
                ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
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
                    Requirements = new PieceRequirementConfig[]
                                {
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "NeckTail",

                        // Amount required
                        Amount = 1
                    }
                    }
                });
            }

            private void registerChainsRecipe()
            {
                ObjectManager.Instance.RegisterRecipe(new RecipeConfig()
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
                    Requirements = new PieceRequirementConfig[]
                                {
                    new PieceRequirementConfig()
                    {
                        // Prefab name of requirement
                        Item = "Iron",

                        // Amount required
                        Amount = 4
                    }
                    }
                });
            }

        }

    }
