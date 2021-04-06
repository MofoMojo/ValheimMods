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
                foreach(SphereCollider sphere in pa.gameObject.GetComponents<SphereCollider>())
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

            if (enabled)
            {
                AddNoMonsterArea(pa);
            }
            else
            {
                // changing the effect area does seem to be enough to "fix"
                RemoveNoMonsterArea(pa);
            }
        }

        private static EffectArea AddNoMonsterArea(PrivateArea pa)
        {
            Plugin.Log("AddNoMonsterArea called");
            EffectArea NewNoMonsters = null;
            try
            {
                NewNoMonsters = pa.gameObject.AddComponent<EffectArea>();
                NewNoMonsters.name = Plugin.MonsterEffectArea;
                NewNoMonsters.m_type = EffectArea.Type.NoMonsters | EffectArea.Type.PlayerBase;
                Plugin.Log($"AddNoMonsterArea - type set to {NewNoMonsters.m_type }");
                // you need a sphere collider attached to the EffectArea to define the radius
                SphereCollider sphereCollider = NewNoMonsters.gameObject.AddComponent<SphereCollider>();
                        
                // make sure IsTrigger is set so physics don't apply. 
                sphereCollider.isTrigger = true;

                // set the radius to the radius of the private area
                Plugin.Log("AddNoMonsterArea - adjusting radius of SphereCollider");
                sphereCollider.radius = pa.m_radius;
               
            }
            catch (Exception ex)
            {
                Plugin.LogError($"AddNoMonsterArea - {ex.Message}");
            }

            return NewNoMonsters;

        }
        private static void RemoveNoMonsterArea(PrivateArea pa)
        {
            Plugin.Log("RemoveNoMonsterArea");
            EffectArea[] effectAreas = pa.GetComponents<EffectArea>();
            EffectArea noMonstersArea = null;
            foreach(EffectArea effectArea in effectAreas)
            {
                if(effectArea.name == Plugin.MonsterEffectArea)
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
