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
                SetNoMonsterArea(__instance, __instance.IsEnabled());
            }

            private static void FixupRadii(PrivateArea pa, float radius)
            {
                Plugin.Log($"FixupRaddi called - {radius}");
                foreach(SphereCollider sphere in pa.GetComponents<SphereCollider>())
                {
                    Plugin.Log("sphere adjusted");
                    sphere.radius = radius;
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
                    SetNoMonsterArea(__instance, enabled);

                }
                catch (Exception ex)
                {
                    Plugin.LogError($"Exception GuardStoneTweakEnabled: {ex.Message}");
                }
            }
        }

        private static void SetNoMonsterArea(PrivateArea pa, bool enabled)
        {
            Plugin.Log($"SetNoMonsterArea called: {enabled}");

            EffectArea NoMonsters = GetNoMonsterArea(pa);

            if (null != NoMonsters)
            {
                Plugin.Log($"SetNoMonsterArea Got NoMonsterArea");

                    // NoMonsters keeps monsters from "attempting" to navigate inside
                    // Playerbase ensures that creatures don't spawn within the radius
                    // however you can't build when it's enabled. 
                    NoMonsters.m_type = enabled ? (EffectArea.Type.NoMonsters | EffectArea.Type.PlayerBase) : EffectArea.Type.None;


                Plugin.Log($"SetEnabled - type set to {NoMonsters.m_type }");
                
                NoMonsters.enabled = enabled;

                SphereCollider collider = NoMonsters.GetComponent<SphereCollider>();
                if(null != collider)
                {
                    collider.enabled = enabled;
                    collider.radius = pa.m_radius;
                }

            }


        }

        private static EffectArea GetNoMonsterArea(PrivateArea pa)
        {
            Plugin.Log("GetNoMonsterArea called");
            EffectArea NoMonstersEffectArea = null;
            try
            {
                EffectArea[] effectAreas = pa.GetComponents<EffectArea>();
                foreach(EffectArea effectArea in effectAreas)
                {
                    if (effectArea.name == Plugin.NoMonsterEffectAreaName)
                    {
                        Plugin.Log("GetNoMonsterArea found NoMonsterArea");
                        NoMonstersEffectArea = effectArea;
                        break;
                    }
                }
                
                // if we didn't find one, make one
                if(null == NoMonstersEffectArea)
                {
                    Plugin.Log("GetNoMonsterArea Creating NoMonsterArea");
                    NoMonstersEffectArea = pa.gameObject.AddComponent<EffectArea>();

                    NoMonstersEffectArea.name = Plugin.NoMonsterEffectAreaName;

                    // initialize this with no affect at first...
                    NoMonstersEffectArea.m_type = EffectArea.Type.None;

                    // you need a sphere collider attached to the EffectArea to define the radius
                    SphereCollider sphereCollider = NoMonstersEffectArea.gameObject.AddComponent<SphereCollider>();

                    // set this off by default if we're making it
                    sphereCollider.enabled = false;

                    // make sure IsTrigger is set so physics don't apply. 
                    sphereCollider.isTrigger = true;

                    // set the radius to the radius of the private area
                    Plugin.Log("GetNoMonsterArea - adjusting radius of SphereCollider");
                    sphereCollider.radius = pa.m_radius;
                }
                
               
            }
            catch (Exception ex)
            {
                Plugin.LogError($"AddNoMonsterArea - {ex.Message}");
            }

            return NoMonstersEffectArea;

        }
        private static void RemoveNoMonsterArea(PrivateArea pa)
        {
            Plugin.Log("RemoveNoMonsterArea");
            EffectArea[] effectAreas = pa.GetComponents<EffectArea>();
            EffectArea noMonstersArea = null;
            foreach(EffectArea effectArea in effectAreas)
            {
                if(effectArea.name == Plugin.NoMonsterEffectAreaName)
                {
                    Plugin.Log("RemoveNoMonsterArea - Found effect area");
                    noMonstersArea = effectArea;
                    break;
                }
            }

            
            if(null != noMonstersArea)
            {
                // destroy the colliders
                SphereCollider[] colliders = noMonstersArea.GetComponents<SphereCollider>();
                for(int i = 0; i < colliders.Length; i++)
                {
                    Plugin.Log("RemoveNoMonsterArea - Destroying SphereCollider");
                    GameObject.Destroy(colliders[i]);
                }

                // destroy the object
                Plugin.Log("RemoveNoMonsterArea - Destroying EffectArea");
                GameObject.Destroy(noMonstersArea);
            }




        }
    }
}
