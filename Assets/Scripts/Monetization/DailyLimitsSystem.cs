using System;
using System.Collections.Generic;
using UnityEngine;
using CaminoDeLaFe.Data;

namespace CaminoDeLaFe.Monetization
{
    /// <summary>
    /// 📅 DAILY LIMITS & EVENT CURRENCY SYSTEM
    /// Implements farming limits and F2P-friendly event currency
    /// Critical for economy balance and player retention
    /// </summary>
    [System.Serializable]
    public class DailyLimits
    {
        public int goldEarned;
        public int goldLimit;
        public int missionsCompleted;
        public int missionLimit;
        public int bossKills;
        public int bossKillLimit;
        public int eventCurrencyEarned;
        public DateTime lastResetDate;
        
        public DailyLimits()
        {
            lastResetDate = DateTime.Now.Date;
            ResetLimits();
        }
        
        public void ResetLimits()
        {
            goldEarned = 0;
            missionsCompleted = 0;
            bossKills = 0;
            eventCurrencyEarned = 0;
        }
    }
    
    [System.Serializable]
    public class EventCurrency
    {
        public int faithTokens;
        public int faithTokensSpent;
        public int faithTokensEarnedToday;
        public DateTime lastDailyReward;
        
        public EventCurrency()
        {
            faithTokens = 0;
            faithTokensSpent = 0;
            faithTokensEarnedToday = 0;
            lastDailyReward = DateTime.MinValue;
        }
    }
    
    public class DailyLimitsSystem : MonoBehaviour
    {
        [Header("Daily Limits Configuration")]
        [SerializeField] private DailyLimits dailyLimits;
        [SerializeField] private EventCurrency eventCurrency;
        [SerializeField] private bool limitsEnabled = true;
        
        [Header("Premium Overrides")]
        [SerializeField] private bool faithPassActive = false;
        [SerializeField] private float faithPassLimitMultiplier = 1.5f; // +50% limits with Faith Pass
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        
        // Components
        private Player player;
        private FaithPassSystem faithPassSystem;
        
        // Events
        public static event System.Action<int, int> OnGoldLimitChanged;
        public static event System.Action<int> OnEventCurrencyEarned;
        public static event System.Action OnDailyLimitsReset;
        public static event System.Action<string> OnLimitReached;
        
        private void Start()
        {
            InitializeLimitsSystem();
            LoadLimitsData();
            CheckDailyReset();
        }
        
        private void InitializeLimitsSystem()
        {
            player = FindObjectOfType<Player>();
            faithPassSystem = FindObjectOfType<FaithPassSystem>();
            
            if (dailyLimits == null)
            {
                dailyLimits = new DailyLimits();
            }
            
            if (eventCurrency == null)
            {
                eventCurrency = new EventCurrency();
            }
            
            UpdateLimitsBasedOnLevel();
            Debug.Log("📅 Daily Limits System initialized");
        }
        
        #region Daily Limits Management
        
        /// <summary>
        /// Update daily limits based on player level and Faith Pass status
        /// </summary>
        public void UpdateLimitsBasedOnLevel()
        {
            if (player == null) return;
            
            int playerLevel = player.Stats.Level;
            faithPassActive = faithPassSystem?.IsFaithPassActive() ?? false;
            
            // Calculate base limits
            int baseGoldLimit = GameConfig.BASE_DAILY_GOLD_LIMIT + (playerLevel * GameConfig.DAILY_GOLD_LIMIT_PER_LEVEL);
            int baseMissionLimit = faithPassActive ? GameConfig.DAILY_MISSIONS_FAITH_PASS : GameConfig.DAILY_MISSIONS_BASE;
            int baseBossLimit = GameConfig.DAILY_BOSS_KILLS_LIMIT;
            
            // Apply Faith Pass multiplier
            if (faithPassActive)
            {
                baseGoldLimit = Mathf.RoundToInt(baseGoldLimit * faithPassLimitMultiplier);
                baseBossLimit = Mathf.RoundToInt(baseBossLimit * faithPassLimitMultiplier);
            }
            
            dailyLimits.goldLimit = baseGoldLimit;
            dailyLimits.missionLimit = baseMissionLimit;
            dailyLimits.bossKillLimit = baseBossLimit;
            
            OnGoldLimitChanged?.Invoke(dailyLimits.goldEarned, dailyLimits.goldLimit);
            
            if (debugMode)
            {
                Debug.Log($"📅 Daily limits updated for Level {playerLevel} (Faith Pass: {faithPassActive}):");
                Debug.Log($"  Gold Limit: {dailyLimits.goldLimit}");
                Debug.Log($"  Mission Limit: {dailyLimits.missionLimit}");
                Debug.Log($"  Boss Kill Limit: {dailyLimits.bossKillLimit}");
            }
        }
        
        /// <summary>
        /// Try to earn gold, respecting daily limits
        /// </summary>
        public bool TryEarnGold(int amount, out int actualAmount)
        {
            actualAmount = amount;
            
            if (!limitsEnabled)
            {
                return true;
            }
            
            int remainingLimit = dailyLimits.goldLimit - dailyLimits.goldEarned;
            
            if (remainingLimit <= 0)
            {
                actualAmount = 0;
                OnLimitReached?.Invoke("daily_gold");
                
                if (debugMode)
                {
                    Debug.Log($"❌ Daily gold limit reached! ({dailyLimits.goldEarned}/{dailyLimits.goldLimit})");
                }
                
                return false;
            }
            
            // Cap the amount to remaining limit
            actualAmount = Mathf.Min(amount, remainingLimit);
            dailyLimits.goldEarned += actualAmount;
            
            // Award Faith Tokens for F2P players when hitting limits
            if (actualAmount < amount && !faithPassActive)
            {
                int tokensAwarded = Mathf.RoundToInt((amount - actualAmount) * 0.1f);
                AwardEventCurrency(tokensAwarded, "Gold limit compensation");
            }
            
            OnGoldLimitChanged?.Invoke(dailyLimits.goldEarned, dailyLimits.goldLimit);
            SaveLimitsData();
            
            if (debugMode && actualAmount < amount)
            {
                Debug.Log($"⚠️ Gold earning capped: {amount} → {actualAmount} (Limit: {dailyLimits.goldEarned}/{dailyLimits.goldLimit})");
            }
            
            return actualAmount > 0;
        }
        
        /// <summary>
        /// Try to complete a mission, respecting daily limits
        /// </summary>
        public bool TryCompleteMission()
        {
            if (!limitsEnabled)
            {
                return true;
            }
            
            if (dailyLimits.missionsCompleted >= dailyLimits.missionLimit)
            {
                OnLimitReached?.Invoke("daily_missions");
                
                // Award Faith Tokens for F2P players
                if (!faithPassActive)
                {
                    AwardEventCurrency(GameConfig.EVENT_CURRENCY_MISSION_REWARD, "Mission limit reached");
                }
                
                if (debugMode)
                {
                    Debug.Log($"❌ Daily mission limit reached! ({dailyLimits.missionsCompleted}/{dailyLimits.missionLimit})");
                }
                
                return false;
            }
            
            dailyLimits.missionsCompleted++;
            SaveLimitsData();
            
            if (debugMode)
            {
                Debug.Log($"✅ Mission completed ({dailyLimits.missionsCompleted}/{dailyLimits.missionLimit})");
            }
            
            return true;
        }
        
        /// <summary>
        /// Try to kill a boss, respecting daily limits
        /// </summary>
        public bool TryKillBoss()
        {
            if (!limitsEnabled)
            {
                return true;
            }
            
            if (dailyLimits.bossKills >= dailyLimits.bossKillLimit)
            {
                OnLimitReached?.Invoke("daily_boss_kills");
                
                // Award Faith Tokens for F2P players
                if (!faithPassActive)
                {
                    AwardEventCurrency(25, "Boss kill limit reached");
                }
                
                if (debugMode)
                {
                    Debug.Log($"❌ Daily boss kill limit reached! ({dailyLimits.bossKills}/{dailyLimits.bossKillLimit})");
                }
                
                return false;
            }
            
            dailyLimits.bossKills++;
            SaveLimitsData();
            
            if (debugMode)
            {
                Debug.Log($"💀 Boss killed ({dailyLimits.bossKills}/{dailyLimits.bossKillLimit})");
            }
            
            return true;
        }
        
        #endregion
        
        #region Event Currency (Faith Tokens)
        
        /// <summary>
        /// Award Faith Tokens (F2P currency)
        /// </summary>
        public void AwardEventCurrency(int amount, string reason = "")
        {
            eventCurrency.faithTokens += amount;
            eventCurrency.faithTokensEarnedToday += amount;
            
            OnEventCurrencyEarned?.Invoke(amount);
            SaveLimitsData();
            
            if (debugMode)
            {
                Debug.Log($"🪙 Faith Tokens awarded: +{amount} ({reason}) - Total: {eventCurrency.faithTokens}");
            }
        }
        
        /// <summary>
        /// Spend Faith Tokens
        /// </summary>
        public bool SpendEventCurrency(int amount, string reason = "")
        {
            if (eventCurrency.faithTokens < amount)
            {
                if (debugMode)
                {
                    Debug.Log($"❌ Not enough Faith Tokens: {eventCurrency.faithTokens} < {amount}");
                }
                return false;
            }
            
            eventCurrency.faithTokens -= amount;
            eventCurrency.faithTokensSpent += amount;
            SaveLimitsData();
            
            if (debugMode)
            {
                Debug.Log($"💸 Faith Tokens spent: -{amount} ({reason}) - Remaining: {eventCurrency.faithTokens}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Convert Faith Tokens to premium items (limited conversion)
        /// </summary>
        public bool ConvertTokensToPremium(int tokens, out int premiumValue)
        {
            premiumValue = Mathf.RoundToInt(tokens * GameConfig.EVENT_CURRENCY_TO_PREMIUM_RATIO);
            
            if (SpendEventCurrency(tokens, "Premium conversion"))
            {
                // Give premium currency/items equivalent
                Debug.Log($"🔄 Converted {tokens} Faith Tokens → {premiumValue} premium value");
                return true;
            }
            
            premiumValue = 0;
            return false;
        }
        
        /// <summary>
        /// Daily Faith Tokens reward for F2P players
        /// </summary>
        public void ClaimDailyEventCurrency()
        {
            DateTime today = DateTime.Now.Date;
            
            if (eventCurrency.lastDailyReward.Date >= today)
            {
                if (debugMode)
                {
                    Debug.Log("❌ Daily Faith Tokens already claimed today");
                }
                return;
            }
            
            AwardEventCurrency(GameConfig.EVENT_CURRENCY_DAILY_EARN, "Daily reward");
            eventCurrency.lastDailyReward = today;
            
            Debug.Log($"🎁 Daily Faith Tokens claimed: {GameConfig.EVENT_CURRENCY_DAILY_EARN}");
        }
        
        #endregion
        
        #region Limit Removal & Premium Features
        
        /// <summary>
        /// Purchase additional limit increases (for premium players)
        /// </summary>
        public bool PurchaseLimitIncrease(LimitType limitType, float usdCost)
        {
            // Mock purchase - in real implementation, process payment
            bool paymentSuccess = true; // Assume success for demo
            
            if (paymentSuccess)
            {
                switch (limitType)
                {
                    case LimitType.Gold:
                        dailyLimits.goldLimit += 1000;
                        break;
                    case LimitType.Missions:
                        dailyLimits.missionLimit += 2;
                        break;
                    case LimitType.BossKills:
                        dailyLimits.bossKillLimit += 5;
                        break;
                }
                
                SaveLimitsData();
                Debug.Log($"💰 Limit increase purchased: {limitType} (+cost: ${usdCost})");
                return true;
            }
            
            return false;
        }
        
        public enum LimitType
        {
            Gold,
            Missions,
            BossKills
        }
        
        /// <summary>
        /// Emergency limit reset (premium feature)
        /// </summary>
        public bool PurchaseLimitReset(float usdCost = 2.99f)
        {
            // Process payment for limit reset
            bool paymentSuccess = true; // Mock success
            
            if (paymentSuccess)
            {
                dailyLimits.ResetLimits();
                UpdateLimitsBasedOnLevel();
                SaveLimitsData();
                
                Debug.Log($"🔄 Daily limits reset purchased (${usdCost})");
                return true;
            }
            
            return false;
        }
        
        #endregion
        
        #region Time Management
        
        private void CheckDailyReset()
        {
            DateTime today = DateTime.Now.Date;
            
            if (dailyLimits.lastResetDate.Date < today)
            {
                PerformDailyReset();
            }
            
            // Schedule next check
            float timeUntilMidnight = GetTimeUntilMidnight();
            Invoke(nameof(PerformDailyReset), timeUntilMidnight);
        }
        
        private void PerformDailyReset()
        {
            dailyLimits.ResetLimits();
            dailyLimits.lastResetDate = DateTime.Now.Date;
            
            // Reset event currency daily counter
            eventCurrency.faithTokensEarnedToday = 0;
            
            UpdateLimitsBasedOnLevel();
            OnDailyLimitsReset?.Invoke();
            SaveLimitsData();
            
            Debug.Log("🔄 Daily limits reset at midnight");
            
            // Schedule next reset
            float timeUntilMidnight = GetTimeUntilMidnight();
            Invoke(nameof(PerformDailyReset), timeUntilMidnight);
        }
        
        private float GetTimeUntilMidnight()
        {
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.Date.AddDays(1);
            return (float)(tomorrow - now).TotalSeconds;
        }
        
        #endregion
        
        #region Limit Notifications & UI
        
        public string GetLimitStatusText(LimitType limitType)
        {
            switch (limitType)
            {
                case LimitType.Gold:
                    return $"Gold: {dailyLimits.goldEarned}/{dailyLimits.goldLimit}";
                case LimitType.Missions:
                    return $"Missions: {dailyLimits.missionsCompleted}/{dailyLimits.missionLimit}";
                case LimitType.BossKills:
                    return $"Boss Kills: {dailyLimits.bossKills}/{dailyLimits.bossKillLimit}";
                default:
                    return "";
            }
        }
        
        public float GetLimitProgress(LimitType limitType)
        {
            switch (limitType)
            {
                case LimitType.Gold:
                    return (float)dailyLimits.goldEarned / dailyLimits.goldLimit;
                case LimitType.Missions:
                    return (float)dailyLimits.missionsCompleted / dailyLimits.missionLimit;
                case LimitType.BossKills:
                    return (float)dailyLimits.bossKills / dailyLimits.bossKillLimit;
                default:
                    return 0f;
            }
        }
        
        public bool IsLimitReached(LimitType limitType)
        {
            switch (limitType)
            {
                case LimitType.Gold:
                    return dailyLimits.goldEarned >= dailyLimits.goldLimit;
                case LimitType.Missions:
                    return dailyLimits.missionsCompleted >= dailyLimits.missionLimit;
                case LimitType.BossKills:
                    return dailyLimits.bossKills >= dailyLimits.bossKillLimit;
                default:
                    return false;
            }
        }
        
        #endregion
        
        #region Data Persistence
        
        private void SaveLimitsData()
        {
            string limitsJson = JsonUtility.ToJson(dailyLimits);
            string currencyJson = JsonUtility.ToJson(eventCurrency);
            
            PlayerPrefs.SetString("DailyLimits", limitsJson);
            PlayerPrefs.SetString("EventCurrency", currencyJson);
            PlayerPrefs.Save();
        }
        
        private void LoadLimitsData()
        {
            if (PlayerPrefs.HasKey("DailyLimits"))
            {
                string limitsJson = PlayerPrefs.GetString("DailyLimits");
                dailyLimits = JsonUtility.FromJson<DailyLimits>(limitsJson);
            }
            
            if (PlayerPrefs.HasKey("EventCurrency"))
            {
                string currencyJson = PlayerPrefs.GetString("EventCurrency");
                eventCurrency = JsonUtility.FromJson<EventCurrency>(currencyJson);
            }
        }
        
        #endregion
        
        #region Debug & Testing
        
        [ContextMenu("Debug: Reset Daily Limits")]
        private void DebugResetLimits()
        {
            PerformDailyReset();
        }
        
        [ContextMenu("Debug: Add 1000 Faith Tokens")]
        private void DebugAddTokens()
        {
            AwardEventCurrency(1000, "Debug");
        }
        
        [ContextMenu("Debug: Reach Gold Limit")]
        private void DebugReachGoldLimit()
        {
            dailyLimits.goldEarned = dailyLimits.goldLimit;
            OnGoldLimitChanged?.Invoke(dailyLimits.goldEarned, dailyLimits.goldLimit);
        }
        
        [ContextMenu("Debug: Show Limits Status")]
        private void DebugShowStatus()
        {
            Debug.Log("📅 DAILY LIMITS STATUS:");
            Debug.Log($"  {GetLimitStatusText(LimitType.Gold)}");
            Debug.Log($"  {GetLimitStatusText(LimitType.Missions)}");
            Debug.Log($"  {GetLimitStatusText(LimitType.BossKills)}");
            Debug.Log($"  Faith Tokens: {eventCurrency.faithTokens}");
            Debug.Log($"  Faith Pass Active: {faithPassActive}");
        }
        
        #endregion
        
        #region Public API
        
        public DailyLimits GetDailyLimits() => dailyLimits;
        public EventCurrency GetEventCurrency() => eventCurrency;
        public bool IsFaithPassActive() => faithPassActive;
        public float GetTimeUntilReset() => GetTimeUntilMidnight();
        
        #endregion
    }
}
