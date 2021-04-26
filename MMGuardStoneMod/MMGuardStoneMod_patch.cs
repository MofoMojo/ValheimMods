using System;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace MofoMojo.MMGuardStoneMod
{
    class MMGuardWardTweaks
    {

        private static List<GameObject> m_NoMonsterSpheres = new List<GameObject>();


        // this hooks into PrivateArea type's Awake method
        [HarmonyPatch(typeof(PrivateArea), "Awake")]
        public class HarmonyPatch_Awake
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this is a prefix so it will overwrite the m_radius of the instance before Awake is called
            [HarmonyPrefix]
            // Prefix indicates that your code will execute before the actual code
            private static void Prefix(PrivateArea __instance)
            {
                Plugin.Log($"GuardStone/PrivateArea Radius: {__instance.m_radius}");
                __instance.m_radius = Settings.GuardStoneRadius.Value;
                FixupRadii(__instance, Settings.GuardStoneRadius.Value);

                Plugin.Log($"Modified GuardStone/PrivateArea Radius: {__instance.m_radius}");

            }

            // this is a postfix to Awake method which checks to see if the instance is already enabled and if so, add's the no monster area... area.
            [HarmonyPostfix]
            private static void PostFix(PrivateArea __instance)
            {
                if(Settings.WardBehavior.Value == Plugin.WardBehavior.Original)
                {
                    Plugin.Log("WardBehavior.Original set, Attempting to remove any monster areas");
                    RemoveNoMonsterArea(__instance);
                    return;
                }
                SetNoMonsterArea(__instance, __instance.IsEnabled());
            }

            private static void FixupRadii(PrivateArea pa, float radius)
            {
                Plugin.Log($"FixupRaddi called - {radius}");
                foreach (SphereCollider sphere in pa.GetComponents<SphereCollider>())
                {
                    Plugin.Log("sphere adjusted");
                    sphere.radius = radius;
                    sphere.isTrigger = true;
                    sphere.material = null;
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
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this is a postfix so it will enable the proper EventArea when the ward is activated and remove it when deactivated
            [HarmonyPostfix]
            private static void SetEnabled_PostFix(PrivateArea __instance, bool enabled)
            {
                Plugin.Log($"GuardStone SetEnabled Called with {enabled}");

                try
                {
                    if (Settings.WardBehavior.Value == Plugin.WardBehavior.Original)
                    {
                        Plugin.LogVerbose("WardBehavior.Original set, Attempting to remove any monster areas");
                        RemoveNoMonsterArea(__instance);
                        return;
                    }

                    SetNoMonsterArea(__instance, enabled);

                }
                catch (Exception ex)
                {
                    Plugin.LogError($"Exception GuardStoneTweakEnabled: {ex.Message}");
                }
            }
        }

        private static void ToOpaqueMode(ref Material material)
        {
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }

        private static void ToFadeMode(ref Material material)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        private static void SetNoMonsterArea(PrivateArea pa, bool enabled)
        {
            Plugin.Log($"SetNoMonsterArea called: {enabled}");

            GameObject sphere = GetNoMonsterArea(pa);
            EffectArea NoMonsters = sphere.GetComponent<EffectArea>();

            if (null != NoMonsters)
            {
                Plugin.Log($"SetNoMonsterArea Got NoMonsterArea");

                // NoMonsters keeps monsters from "attempting" to navigate inside
                // Playerbase ensures that creatures don't spawn within the radius
                // however you can't build when it's enabled. 

                if(enabled)
                {
                    NoMonsters.enabled = enabled;
                    
                    // layer modelled after the ForceField in the Vendor_BlackForest scene.
                    NoMonsters.gameObject.layer = 14;
                    SphereCollider collider = NoMonsters.GetComponent<SphereCollider>();
                    if (null != collider)
                    {
                        collider.enabled = enabled;
                        collider.radius = pa.m_radius;
                    }
                }
                else
                {
                    RemoveNoMonsterArea(pa);
                }
                NoMonsters.m_type = enabled ? (EffectArea.Type.NoMonsters | EffectArea.Type.PlayerBase) : EffectArea.Type.None;
            }

        }

        private static GameObject[] GetSpheresInRange(Vector3 point, float radius)
        {
            List<GameObject> spheres = new List<GameObject>();
            foreach(GameObject gameObject in m_NoMonsterSpheres)
            {
                if(Vector3.Distance(point, gameObject.transform.position) < radius)
                {
                    spheres.Add(gameObject);
                }
            }

            return spheres.ToArray();
        }

        private static GameObject GetSphereInRange(Vector3 point, float radius)
        {
            foreach (GameObject gameObject in m_NoMonsterSpheres)
            {
                if (Vector3.Distance(point, gameObject.transform.position) < radius)
                {
                    return gameObject;
                }
            }

            return null;
        }

        private static GameObject GetNoMonsterArea(PrivateArea pa)
        {
            Plugin.Log("GetNoMonsterArea called");
            GameObject sphere = null;

            EffectArea NoMonstersEffectArea = null;
            try
            {
                sphere = GetSphereInRange(pa.transform.position, 1);

                // if we didn't find one, make one
                if (null == sphere)
                {
                    Plugin.Log("GetNoMonsterArea Creating NoMonsterArea");
                    sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.name = "MMForceField";
                    sphere.transform.position = pa.transform.position;
                    sphere.transform.localScale = pa.transform.localScale;

                    // https://forum.unity.com/threads/change-rendering-mode-via-script.476437/
                    Material temp = sphere.GetComponent<Renderer>().material;

                    // set a transparent color
                    temp.color = new Color(0, 0, 0, 0.25f);

                    // if you don't do this, it'll still be opaque.
                    temp.SetOverrideTag("RenderType", "Transparent");
                    temp.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    temp.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    temp.SetInt("_ZWrite", 0);
                    temp.DisableKeyword("_ALPHATEST_ON");
                    temp.EnableKeyword("_ALPHABLEND_ON");
                    temp.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    temp.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                    SphereCollider sphereCollider = sphere.GetComponent<SphereCollider>();

                    NoMonstersEffectArea = sphere.gameObject.AddComponent<EffectArea>();

                    NoMonstersEffectArea.name = Plugin.NoMonsterEffectAreaName;

                    // initialize this with no affect at first...
                    NoMonstersEffectArea.m_type = EffectArea.Type.None;

                    // you need a sphere collider attached to the EffectArea to define the radius
                    //SphereCollider sphereCollider = NoMonstersEffectArea.gameObject.AddComponent<SphereCollider>();

                    // set this off by default if we're making it
                    sphereCollider.enabled = false;

                    // make sure IsTrigger is set so physics don't apply. 
                    sphereCollider.isTrigger = true;

                    // set the radius to the radius of the private area
                    Plugin.Log("GetNoMonsterArea - adjusting radius of SphereCollider");
                    sphereCollider.radius = pa.m_radius;

                    m_NoMonsterSpheres.Add(sphere);

                }


            }
            catch (Exception ex)
            {
                Plugin.LogError($"AddNoMonsterArea - {ex.Message}");
            }

            return sphere;

        }

        //no need for this but will keep around in case code changes and requires it
        private static void RemoveNoMonsterArea(PrivateArea pa)
        {
            Plugin.Log("RemoveNoMonsterArea");
            GameObject[] spheres = null;
            EffectArea noMonstersArea = null;

            spheres = GetSpheresInRange(pa.transform.position, 1);

            foreach(GameObject sphere in spheres)
            {
                noMonstersArea = sphere.GetComponent<EffectArea>();

                if (null != noMonstersArea)
                {
                    // destroy the colliders
                    SphereCollider[] colliders = noMonstersArea.GetComponents<SphereCollider>();
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        Plugin.Log("RemoveNoMonsterArea - Destroying SphereCollider");
                        GameObject.Destroy(colliders[i]);
                    }

                    // destroy the object
                    Plugin.Log("RemoveNoMonsterArea - Destroying EffectArea");
                    GameObject.Destroy(noMonstersArea);
                }
                m_NoMonsterSpheres.Remove(sphere);
                GameObject.Destroy(sphere);
            }
        }

        [HarmonyPatch(typeof(PrivateArea), "Interact")]
        public class HarmonyPatch_Interact
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            [HarmonyPrefix]
            public static bool Interact(Humanoid human, bool hold, PrivateArea __instance, ref bool __result)
            {
                // if using the original interaction behavior then just return true and run the original code
                if (Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.Original) return true;

                // default to a false just in case.
                __result = false;

                Plugin.Log("Patched Interact");

                // if not allowing permitted players to activate/deactivate, just run the standard function
                if (Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.OwnerOnly) return true;

                if (hold)
                {
                    Plugin.Log("HOLDING");
                    __result = false;

                    //don't execute.. what's the point?
                    return false;
                }
                Player player = human as Player;

                // If player is creator
                // or if ward mode is ownerandpermitted and is permitted or behavior is all AND modifier key is not being held....
                if (__instance.m_piece.IsCreator()
                    ||
                    ((Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.OwnerAndPermitted && __instance.IsPermitted(player.GetPlayerID())  ||  (Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.All))
                    && !UtilityClass.CheckKeyHeld(Settings.InteractModifier.Value))
                    )
                {
                    __instance.m_nview.InvokeRPC("ToggleEnabled", player.GetPlayerID());
                    __result = true;

                    // don't execute the original code
                    return false;
                }
                Plugin.Log("Wasn't normal activation");

                // if OwnerOnly then don't allow anyone to do anything else
                if (Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.OwnerOnly)
                {
                    __result = false;
                    return false;
                }

                // if the player is using the modifier key and gets this far, then toggle permitted
                // can only do this when the peice isn't active
                if (UtilityClass.CheckKeyHeld(Settings.InteractModifier.Value) && !__instance.IsEnabled())
                {
                    __instance.m_nview.InvokeRPC("TogglePermitted", player.GetPlayerID(), player.GetPlayerName());
                    __result = true;

                    // don't execute original code
                    return false;
                }

                __result = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(PrivateArea), "RPC_ToggleEnabled")]
        public class HarmonyPatch_RPC_ToggleEnabled
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this patch allows permitted players to toggle the ward on and off
            [HarmonyPrefix]
            public static bool RPC_ToggleEnabled(long uid, long playerID, PrivateArea __instance)
            {
                // if using the original interaction behavior then just return true and run the original code
                if (Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.Original) return true;

                ZLog.Log((object)("Toggle enabled from " + playerID + "  creator is " + __instance.m_piece.GetCreator()));
                if (__instance.m_nview.IsOwner() && ((__instance.IsPermitted(playerID) && Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.OwnerAndPermitted) || (Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.All)))
                {
                    __instance.SetEnabled(!__instance.IsEnabled());
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(PrivateArea), "GetHoverText")]
        public class HarmonyPatch_GetHoverText
        {
            [HarmonyPrepare]
            static bool IsGuardStoneTweakEnabled()
            {
                bool enabled = Settings.MMGuardStoneModEnabled.Value;
                Plugin.Log($"EnableGuardStoneTweak {enabled}");

                return enabled;
            }

            // this patch allows permitted players to toggle the ward on and off
            [HarmonyPrefix]
            public static bool GetHoverText(PrivateArea __instance, ref string __result)
            {
                // if using the original interaction behavior then just return true and run the original code
                if (Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.Original) return true;

                Plugin.LogDebug("Executing Patched GetHoverText");
                if (!__instance.m_nview.IsValid())
                {
                    __result = "";
                    return false;
                }
                if (Player.m_localPlayer == null)
                {
                    __result = "";
                    return false;
                }
                __instance.ShowAreaMarker();
                StringBuilder stringBuilder = new StringBuilder(256);

                // Player or owner hovering over so display activate/dectivate options
                if (__instance.m_piece.IsCreator() || (Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.OwnerAndPermitted && __instance.IsPermitted(Player.m_localPlayer.GetPlayerID())) || Settings.WardInteractBehavior.Value == Plugin.WardInteractBehavior.All)
                {
                    if (__instance.IsEnabled())
                    {
                        stringBuilder.Append(__instance.m_name + " ( $piece_guardstone_active )");
                        stringBuilder.Append("\n$piece_guardstone_owner:" + __instance.GetCreatorName());
                        stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_deactivate");
                    }
                    else
                    {
                        stringBuilder.Append(__instance.m_name + " ($piece_guardstone_inactive )");
                        stringBuilder.Append("\n$piece_guardstone_owner:" + __instance.GetCreatorName());
                        stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_activate");
                        if (Settings.WardInteractBehavior.Value != Plugin.WardInteractBehavior.OwnerOnly)
                        {
                            if(__instance.IsPermitted(Player.m_localPlayer.GetPlayerID()))
                            {
                                stringBuilder.Append("\n[<color=yellow><b>" + Settings.InteractModifier.Value.ToString() + " - $KEY_Use</b></color>] $piece_guardstone_remove");
                            }
                            else
                            {
                                stringBuilder.Append("\n[<color=yellow><b>" + Settings.InteractModifier.Value.ToString() + " - $KEY_Use</b></color>] $piece_guardstone_add");
                            }
                        }

                    }
                }
                else if (__instance.IsEnabled())
                {
                    // Player is not permitted and is not the owner and ward is active... do nothing
                    stringBuilder.Append(__instance.m_name + " ( $piece_guardstone_active )");
                    stringBuilder.Append("\n$piece_guardstone_owner:" + __instance.GetCreatorName());
                    if (Settings.WardInteractBehavior.Value != Plugin.WardInteractBehavior.OwnerOnly) stringBuilder.Append("\n[<color=yellow><b>" + Settings.InteractModifier.Value.ToString() + " - $KEY_Use</b></color>] $piece_guardstone_remove");
                }
                else 
                {
                    // player is not permitted and is not the owner, but ward is inactive.... show modifier
                    stringBuilder.Append(__instance.m_name + " ( $piece_guardstone_inactive )");
                    stringBuilder.Append("\n$piece_guardstone_owner:" + __instance.GetCreatorName());
                    if (Settings.WardInteractBehavior.Value != Plugin.WardInteractBehavior.OwnerOnly) stringBuilder.Append("\n[<color=yellow><b>" + Settings.InteractModifier.Value.ToString() + " - $KEY_Use</b></color>] $piece_guardstone_add");
                }
                __instance.AddUserList(stringBuilder);
                __result = Localization.instance.Localize(stringBuilder.ToString());
                Plugin.LogDebug($"Result {__result}");
                return false;
            }
        }
    }
}
