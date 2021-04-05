using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace MMGuardStoneMod
{
    class MMGuardWardTweaks
    {
        // this hooks into PrivateArea type's Awake method
        [HarmonyPatch(typeof(PrivateArea), "Awake")]
        public class HarmonyPatch_AdjustGuardStoneRadius
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Debug.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this is a prefix so it will overwrite the m_radius of the instance before Awake is called
            [HarmonyPrefix]
            // Prefix indicates that your code will execute before the actual code
            private static void Prefix(PrivateArea __instance)
            {
                Debug.Log($"GuardStone/PrivateArea Radius: {__instance.m_radius}");
                __instance.m_radius = Settings.GuardStoneRadius.Value;
                Debug.Log($"Modified GuardStone/PrivateArea Radius: {__instance.m_radius}");

            }

            // this is a postfix to Awake method which checks to see if the instance is already enabled and if so, add's the no monster area... area.
            [HarmonyPostfix]
            private static void PostFix(PrivateArea __instance)
            {
                if (__instance.IsEnabled())
                {
                    AddNoMonsterArea(__instance);
                }
                else
                {
                    DestroyNoMonsterArea(__instance);
                }
            }

        }



        [HarmonyPatch(typeof(PrivateArea), "SetEnabled")]
        public class HarmonyPatch_SetEnabled
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Debug.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this is a postfix so it will enable the proper EventArea when the ward is activated and remove it when deactivated
            [HarmonyPostfix]
            private static void SetEnabled_PostFix(PrivateArea __instance, bool enabled)
            {
                try
                {
                    if (__instance.IsEnabled())
                    {
                        AddNoMonsterArea(__instance);
                    }
                    else
                    {
                        // try and get the noMonsters Effect Area
                        DestroyNoMonsterArea(__instance);
                    }

                }
                catch (Exception ex)
                {
                    Debug.Log($"Exception GuardStoneTweakEnabled: {ex.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(PrivateArea), "Interact")]
        public class HarmonyPatch_Interact
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Debug.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this is a postfix so it will enable the proper EventArea when the ward is activated and remove it when deactivated
            [HarmonyPrefix]
            private static bool Interact_PreFix(Humanoid human, bool hold, PrivateArea __instance, bool __result)
            {
                if (hold)
                {
                    __result = false;
                    return false;
                }
                Player player = human as Player;
                if (__instance.m_piece.IsCreator())
                {
                    __instance.m_nview.InvokeRPC("ToggleEnabled", player.GetPlayerID());
                    __result = true;
                    return false;
                }
                __instance.m_nview.InvokeRPC("TogglePermitted", player.GetPlayerID(), player.GetPlayerName());
                __result = true;

                return false;
            }
        }

        private static void AddNoMonsterArea(PrivateArea pa)
        {

            try
            {
                Debug.Log("AddNoMonsterArea");
                EffectArea oldMonsters = pa.gameObject.GetComponent<EffectArea>();


                if (null != oldMonsters)
                {
                    Debug.Log("AddNoMonsterArea - Destroying Original");
                    GameObject.Destroy(oldMonsters);
                }

                EffectArea NewNoMonsters = pa.gameObject.AddComponent<EffectArea>();

                if (null != NewNoMonsters)
                {
                    Debug.Log("AddNoMonsterArea - Added New No Monster Area");
                    // set the effect area name, and types
                    // for some reason, the name MUST be NoMonsterArea
                    NewNoMonsters.name = "NoMonsterArea";

                    if (pa.IsEnabled())
                    {
                        Debug.Log("AddNoMonsterArea - EffectArea.Type.NoMonsters & EffectArea.Type.PlayerBase");
                        NewNoMonsters.m_type = EffectArea.Type.NoMonsters & EffectArea.Type.PlayerBase;
                    }
                    else
                    {
                        Debug.Log("AddNoMonsterArea - EffectArea.Type.None");
                        NewNoMonsters.m_type = EffectArea.Type.None;
                    }

                    // you need a sphere collider attached to the EffectArea to define the radius
                    SphereCollider sphereCollider = NewNoMonsters.gameObject.GetComponent<SphereCollider>();

                    if (null == sphereCollider)
                    {
                        sphereCollider = NewNoMonsters.gameObject.AddComponent<SphereCollider>();
                    }

                    // set the radius to the radius of the private area
                    sphereCollider.radius = pa.m_radius;

                    // make sure IsTrigger is set so physics don't apply. 
                    sphereCollider.isTrigger = true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"AddNoMonsterAreaException - {ex.Message}");
            }


        }

        private static void DestroyNoMonsterArea(PrivateArea pa)
        {
            try
            {
                Debug.Log("DestroyNoMonsterArea");
                EffectArea noMonsters = pa.gameObject.GetComponent<EffectArea>();

                if (null != noMonsters)
                {
                    // when destroying, for some reason need to destroy the SphereCollider or monsters will come back into the zone.
                    SphereCollider collider = noMonsters.GetComponent<SphereCollider>();
                    if (null != collider) GameObject.Destroy(collider);
                    GameObject.Destroy(noMonsters);
                    Debug.Log("Destroyed NoMonsterArea");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"DestroyNoMonsterArea - Exception {ex.Message}");
            }


        }

    }
}
