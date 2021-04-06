using JotunnLib.Entities;


namespace MofoMojo.MMRecipeTweaks
{
    public class MMLoxMeatSurprise : PrefabConfig
    {
        // Note, if you inherit froma base type of food, you can't eat that base type and the new type at the same time
        public MMLoxMeatSurprise() : base("MMLoxMeatSurprise", "LoxPie")
        {

        }

        public override void Register()
        {
            // Configure item drop
            ItemDrop item = Prefab.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "Lox Meat Surprise";
            item.m_itemData.m_shared.m_description = "Better Than Lox Pie, Stamina Regen for 1 minute";
            item.m_itemData.m_dropPrefab = Prefab;
            item.m_itemData.m_shared.m_weight = 1f;
            item.m_itemData.m_shared.m_maxStackSize = 10;
            item.m_itemData.m_shared.m_variants = 1;

            // This is how much health bonus it adds
            item.m_itemData.m_shared.m_food = 100f;

            // This is the amount of additional stamina it gives
            item.m_itemData.m_shared.m_foodStamina = 140f;

            // This is the healing rate 
            item.m_itemData.m_shared.m_foodRegen = 6f;

            // This is how long the food will last
            item.m_itemData.m_shared.m_foodBurnTime = 3200f;

            // Colors the food? Doesn't seem to have an impact on Lox Meat Image
            item.m_itemData.m_shared.m_foodColor = new UnityEngine.Color(1.0f, 0.5f, 0f);

            // Set a status Affect on the Food
            item.m_itemData.m_shared.m_consumeStatusEffect = new SE_Stats()
            {
                // A 2 indicates a +100% increase in stamina regen in the UI
                m_staminaRegenMultiplier = 2f,

                // This is 1.5 by default. Should this be changed to 0? 
                m_healthOverTimeInterval = 0f,

                // This might be how long the status affect lives? 30 seconds?
                m_ttl = 60f
            };


            /*
                MINOR: {"m_name":"$item_mead_stamina_minor","m_name_EN":"Minor stamina mead","m_category":"staminapotion","m_flashIcon":false,"m_cooldownIcon":true,"m_tooltip":"$se_potion_staminaminor_tooltip","m_tooltip_EN":"Regenerate stamina fast.","m_attributes":"None","m_eMessage":"$se_potion_start","m_startMessage_EN":"You drink the potion","m_startMessageType":"TopLeft","m_stopMessage":"","m_stopMessage_EN":"","m_stopMessageType":"TopLeft","m_repeatMessage":"","m_repeatMessage_EN":"","m_repeatMessageType":"TopLeft","m_repeatInterval":0,"m_ttl":120,"m_cooldown":0,"m_activationAnimation":"gpower"}
                MEDIU: {"m_name":"$item_mead_stamina_medium","m_name_EN":"Medium stamina mead","m_category":"staminapotion","m_flashIcon":false,"m_cooldownIcon":true,"m_tooltip":"$se_potion_staminamedium_tooltip","m_tooltip_EN":"Regenerate stamina fast.","m_attributes":"None","m_startMessage":"$se_potion_start","m_startMessage_EN":"You drink the potion","m_startMessageType":"TopLeft","m_stopMessage":"","m_stopMessage_EN":"","m_stopMessageType":"TopLeft","m_repeatMessage":"","m_repeatMessage_EN":"","m_repeatMessageType":"TopLeft","m_repeatInterval":0,"m_ttl":120,"m_cooldown":0,"m_activationAnimation":"gpower"}
                */

        }
    }
}
