using UnityEngine;

namespace CaminosDeLaFe.Data
{
    /// <summary>
    /// Configuration settings for the game
    /// </summary>
    [System.Serializable]
    public static class GameConfig
    {
        // Player Settings
        public const float PLAYER_SPEED = 5f;
        public const int PLAYER_HEALTH = 100;
        public const float PLAYER_ATTACK_DAMAGE = 15f;
        public const float PLAYER_ATTACK_RANGE = 2.5f;
        public const float PLAYER_ATTACK_COOLDOWN = 0.8f;

        // Enemy Settings
        public const float ENEMY_SPEED = 3f;
        public const int ENEMY_HEALTH = 50;
        
        // Camera Settings
        public const float CAMERA_HEIGHT = 10f;
        public const float CAMERA_DISTANCE = 12f;
        public const float CAMERA_ANGLE = 30f;
        
        // Combat Settings
        public const float COMBAT_DETECTION_RANGE = 15f;
        public const float MIN_ATTACK_DISTANCE = 1.5f;
        
        // UI Settings
        public const float UI_FADE_DURATION = 0.3f;
        public const float HEALTHBAR_UPDATE_SPEED = 2f;
        
        // Stats System
        public const int BASE_STAT_VALUE = 10;
        public const int STARTING_GOLD = 100;
        public const int STAT_COST_MULTIPLIER = 2; // Cost = (current_level + 1)^2
        
        // Faction War Settings
        public const int DAILY_CONTRIBUTION_LIMIT = 1;
        public const float LOYALTY_BONUS_MULTIPLIER = 0.01f;
        
        // Mount System Settings
        public const float MOUNT_ANIMATION_TIME = 2f;
        public const float GALLOP_STAMINA_COST = 20f;
        public const float STAMINA_REGEN_RATE = 10f;
        public const float MOUNT_TRAMPLE_RANGE = 3f;
        
        // Inventory Settings
        public const int DEFAULT_INVENTORY_SIZE = 30;
        public const int MAX_STACK_SIZE_DEFAULT = 99;
        public const float ITEM_PICKUP_RANGE = 2f;
        
        // Legendary Items Settings
        public const float LEGENDARY_STEAL_BASE_CHANCE = 0.1f;
        public const float MAX_STEAL_CHANCE = 0.5f;
        public const int PROTECTION_UPGRADE_COST = 1000;
        public const float PURIFICATION_SUCCESS_RATE = 0.8f;
        
        // ==============================================
        // 🚨 GAMES LAB ECONOMICS - MONETIZATION SYSTEM
        // ==============================================
        
        // USD-GOLD ECONOMY (Critical Revenue System)
        public const int USD_TO_GOLD_BASE = 1000; // $1 = 1000 gold (Level 1-10)
        public const int USD_TO_GOLD_SCALING_START = 11; // Level when scaling starts
        public const int USD_TO_GOLD_MULTIPLIER = 100; // $1 = 100 × Level gold (Level >10)
        public const float GOLD_TO_USD_CONVERSION_FEE = 0.15f; // 15% platform fee
        
        // FAITH PASS (Battle Pass System) - $10/month
        public const float FAITH_PASS_PRICE_USD = 10f;
        public const int FAITH_PASS_DURATION_DAYS = 30;
        public const int FAITH_PASS_AD_FREE_MONTHS = 6;
        public const float FAITH_PASS_XP_BONUS = 1.5f; // +50% XP
        public const float FAITH_PASS_GOLD_BONUS = 1.3f; // +30% Gold
        public const int FAITH_PASS_DAILY_MISSIONS_BONUS = 3; // +3 daily missions
        
        // ADVERTISING SYSTEM (Critical Revenue Stream)
        public const float AD_INTERVAL_MINUTES = 15f; // Ad every 15 minutes
        public const float AD_OPTIONAL_GOLD_BONUS = 1.5f; // +50% gold for watching ad
        public const float AD_BANNER_REVENUE_SHARE = 0.7f; // 70% to developer
        public const int AD_FREE_DAYS_FAITH_PASS = 180; // 6 months ad-free
        
        // DAILY FARMING LIMITS (Anti-Inflation)
        public const int BASE_DAILY_GOLD_LIMIT = 500;
        public const int DAILY_GOLD_LIMIT_PER_LEVEL = 50; // +50 gold limit per level
        public const int DAILY_MISSIONS_BASE = 5;
        public const int DAILY_MISSIONS_FAITH_PASS = 8; // +3 with Faith Pass
        public const int DAILY_BOSS_KILLS_LIMIT = 10;
        public const float FARMING_LIMIT_RESET_HOUR = 0f; // Midnight UTC
        
        // EVENT CURRENCY (F2P Accessibility)
        public const string EVENT_CURRENCY_NAME = "Faith Tokens";
        public const int EVENT_CURRENCY_DAILY_EARN = 25;
        public const int EVENT_CURRENCY_MISSION_REWARD = 15;
        public const float EVENT_CURRENCY_TO_PREMIUM_RATIO = 0.1f; // 10 Event = 1 Premium
        
        // ANTI-INFLATION MECHANICS
        public const float GOLD_BURN_TAX_RATE = 0.05f; // 5% tax on high-value transactions
        public const int GOLD_BURN_THRESHOLD = 10000; // Transactions >10k gold get taxed
        public const float EQUIPMENT_REPAIR_GOLD_SINK = 0.1f; // 10% of item value
        public const float MOUNT_MAINTENANCE_COST = 0.02f; // 2% of mount value daily
        
        // MISSION SLOT SYSTEM
        public const int BASE_MISSION_SLOTS = 3;
        public const int FAITH_PASS_MISSION_SLOTS = 6; // +3 slots with Faith Pass
        public const int MAX_MISSION_SLOTS = 8;
        public const float MISSION_SLOT_PURCHASE_COST_USD = 2f; // $2 per additional slot
        
        // CROSS-ECOSYSTEM INTEGRATION (OpenNexus)
        public const float CROSS_PRODUCT_REWARD_BONUS = 1.2f; // +20% rewards for using other products
        public const int ECOSYSTEM_LOYALTY_POINTS_PER_DOLLAR = 10;
        public const float CajaCentralPOS_INTEGRATION_DISCOUNT = 0.15f; // 15% discount
        
        // NFT INTEGRATION (Polygon Blockchain)
        public const string NFT_BLOCKCHAIN = "Polygon";
        public const float NFT_MINTING_COST_USD = 5f;
        public const float NFT_MARKETPLACE_FEE = 0.025f; // 2.5% marketplace fee
        public const int NFT_UTILITY_BONUS_PERCENT = 25; // +25% stats for NFT items
        
        // REAL MONEY SHOP
        public const float STARTER_PACK_USD = 4.99f;
        public const float PREMIUM_GEAR_PACK_USD = 9.99f;
        public const float LEGENDARY_MOUNT_USD = 19.99f;
        public const float INSTANT_LEVEL_BOOST_USD = 7.99f;
    }

    /// <summary>
    /// Auto Combat specific configuration
    /// </summary>
    [System.Serializable]
    public static class AutoCombatConfig
    {
        public const float LUCK_FACTOR_MIN = 0.8f;
        public const float LUCK_FACTOR_MAX = 1.3f;
        public const float ENEMY_POWER_VARIANCE = 0.2f;
        public const float BASE_DAMAGE_REDUCTION = 0.1f;
        public const float MAX_DAMAGE_REDUCTION = 0.8f;
    }
}
