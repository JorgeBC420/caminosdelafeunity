using System;
using System.Collections.Generic;
using UnityEngine;
using CaminoDeLaFe.Data;

namespace CaminoDeLaFe.Monetization
{
    /// <summary>
    /// 🎯 FAITH PASS SYSTEM - $10/month Battle Pass
    /// Core monetization system providing premium benefits and ad-free experience
    /// </summary>
    [System.Serializable]
    public class FaithPassData
    {
        public bool isActive;
        public DateTime purchaseDate;
        public DateTime expirationDate;
        public int currentTier;
        public int totalXPEarned;
        public bool[] tierRewardsClaimed;
        public bool adFreeActive;
        public DateTime adFreeExpiration;
        
        public FaithPassData()
        {
            isActive = false;
            currentTier = 0;
            totalXPEarned = 0;
            tierRewardsClaimed = new bool[50]; // 50 tiers
            adFreeActive = false;
        }
    }
    
    [System.Serializable]
    public class FaithPassReward
    {
        public int tier;
        public string rewardName;
        public RewardType type;
        public int quantity;
        public string description;
        public bool isPremiumOnly;
        
        public enum RewardType
        {
            Gold,
            EventCurrency,
            Equipment,
            Mount,
            Cosmetic,
            MissionSlot,
            XPBoost,
            GoldBoost
        }
    }
    
    public class FaithPassSystem : MonoBehaviour
    {
        [Header("Faith Pass Configuration")]
        [SerializeField] private FaithPassData currentPass;
        [SerializeField] private List<FaithPassReward> passRewards;
        [SerializeField] private int xpPerTier = 1000;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        
        // Events
        public static event System.Action<int> OnTierReached;
        public static event System.Action<FaithPassReward> OnRewardClaimed;
        public static event System.Action<bool> OnFaithPassStatusChanged;
        
        private void Start()
        {
            InitializeFaithPass();
            LoadFaithPassData();
        }
        
        private void InitializeFaithPass()
        {
            if (currentPass == null)
            {
                currentPass = new FaithPassData();
            }
            
            if (passRewards == null || passRewards.Count == 0)
            {
                CreateDefaultRewards();
            }
        }
        
        #region Faith Pass Management
        
        /// <summary>
        /// Purchase Faith Pass - $10/month
        /// </summary>
        public bool PurchaseFaithPass()
        {
            // In real implementation, this would connect to payment processor
            if (debugMode)
            {
                Debug.Log("🎯 FAITH PASS: Processing $10 payment...");
            }
            
            // Simulate successful purchase
            bool paymentSuccess = ProcessPayment(GameConfig.FAITH_PASS_PRICE_USD);
            
            if (paymentSuccess)
            {
                ActivateFaithPass();
                return true;
            }
            
            return false;
        }
        
        private void ActivateFaithPass()
        {
            currentPass.isActive = true;
            currentPass.purchaseDate = DateTime.Now;
            currentPass.expirationDate = DateTime.Now.AddDays(GameConfig.FAITH_PASS_DURATION_DAYS);
            
            // Activate ad-free experience for 6 months
            currentPass.adFreeActive = true;
            currentPass.adFreeExpiration = DateTime.Now.AddDays(GameConfig.FAITH_PASS_AD_FREE_MONTHS * 30);
            
            SaveFaithPassData();
            OnFaithPassStatusChanged?.Invoke(true);
            
            Debug.Log($"✅ FAITH PASS ACTIVATED! Expires: {currentPass.expirationDate}");
            Debug.Log($"🚫 AD-FREE until: {currentPass.adFreeExpiration}");
        }
        
        public bool IsFaithPassActive()
        {
            if (!currentPass.isActive) return false;
            
            // Check if expired
            if (DateTime.Now > currentPass.expirationDate)
            {
                currentPass.isActive = false;
                SaveFaithPassData();
                OnFaithPassStatusChanged?.Invoke(false);
                return false;
            }
            
            return true;
        }
        
        public bool IsAdFreeActive()
        {
            if (!currentPass.adFreeActive) return false;
            
            // Check if ad-free period expired
            if (DateTime.Now > currentPass.adFreeExpiration)
            {
                currentPass.adFreeActive = false;
                SaveFaithPassData();
                return false;
            }
            
            return true;
        }
        
        #endregion
        
        #region XP and Tier Progression
        
        public void AddXP(int xp)
        {
            int originalTier = currentPass.currentTier;
            
            // Apply Faith Pass XP bonus
            if (IsFaithPassActive())
            {
                xp = Mathf.RoundToInt(xp * GameConfig.FAITH_PASS_XP_BONUS);
            }
            
            currentPass.totalXPEarned += xp;
            
            // Calculate new tier
            int newTier = currentPass.totalXPEarned / xpPerTier;
            newTier = Mathf.Min(newTier, passRewards.Count - 1);
            
            if (newTier > originalTier)
            {
                currentPass.currentTier = newTier;
                OnTierReached?.Invoke(newTier);
                
                // Auto-claim free rewards
                for (int i = originalTier + 1; i <= newTier; i++)
                {
                    var reward = GetRewardForTier(i);
                    if (reward != null && !reward.isPremiumOnly)
                    {
                        ClaimReward(i);
                    }
                }
            }
            
            SaveFaithPassData();
        }
        
        public bool ClaimReward(int tier)
        {
            if (tier >= currentPass.tierRewardsClaimed.Length) return false;
            if (currentPass.tierRewardsClaimed[tier]) return false; // Already claimed
            if (tier > currentPass.currentTier) return false; // Tier not reached
            
            var reward = GetRewardForTier(tier);
            if (reward == null) return false;
            
            // Check if premium reward and player has Faith Pass
            if (reward.isPremiumOnly && !IsFaithPassActive())
            {
                Debug.Log($"❌ Premium reward requires active Faith Pass: {reward.rewardName}");
                return false;
            }
            
            // Give reward to player
            GiveRewardToPlayer(reward);
            
            currentPass.tierRewardsClaimed[tier] = true;
            OnRewardClaimed?.Invoke(reward);
            SaveFaithPassData();
            
            Debug.Log($"🎁 FAITH PASS REWARD CLAIMED: {reward.rewardName} (Tier {tier})");
            return true;
        }
        
        private FaithPassReward GetRewardForTier(int tier)
        {
            return passRewards.Find(r => r.tier == tier);
        }
        
        #endregion
        
        #region Reward Distribution
        
        private void GiveRewardToPlayer(FaithPassReward reward)
        {
            // Get player components
            var player = FindObjectOfType<Player>();
            if (player == null)
            {
                Debug.LogError("Player not found for reward distribution!");
                return;
            }
            
            switch (reward.type)
            {
                case FaithPassReward.RewardType.Gold:
                    player.Stats.AddGold(reward.quantity);
                    break;
                    
                case FaithPassReward.RewardType.EventCurrency:
                    // Add event currency (to be implemented)
                    Debug.Log($"Added {reward.quantity} Event Currency");
                    break;
                    
                case FaithPassReward.RewardType.Equipment:
                    // Give equipment item (to be implemented with inventory)
                    Debug.Log($"Received equipment: {reward.rewardName}");
                    break;
                    
                case FaithPassReward.RewardType.Mount:
                    // Unlock mount (to be implemented)
                    Debug.Log($"Mount unlocked: {reward.rewardName}");
                    break;
                    
                case FaithPassReward.RewardType.MissionSlot:
                    // Add mission slot (to be implemented)
                    Debug.Log($"Mission slot unlocked!");
                    break;
                    
                case FaithPassReward.RewardType.XPBoost:
                case FaithPassReward.RewardType.GoldBoost:
                    // Apply temporary boost (to be implemented)
                    Debug.Log($"Boost activated: {reward.rewardName}");
                    break;
            }
        }
        
        #endregion
        
        #region Default Rewards Setup
        
        private void CreateDefaultRewards()
        {
            passRewards = new List<FaithPassReward>();
            
            // Create 50 tiers of rewards
            for (int i = 0; i < 50; i++)
            {
                // Free rewards (every tier)
                if (i % 1 == 0)
                {
                    passRewards.Add(new FaithPassReward
                    {
                        tier = i,
                        rewardName = $"Gold Reward",
                        type = FaithPassReward.RewardType.Gold,
                        quantity = 100 + (i * 25),
                        description = $"Earn {100 + (i * 25)} gold",
                        isPremiumOnly = false
                    });
                }
                
                // Premium rewards (every 5 tiers)
                if (i % 5 == 4)
                {
                    passRewards.Add(new FaithPassReward
                    {
                        tier = i,
                        rewardName = $"Premium Equipment T{i/5 + 1}",
                        type = FaithPassReward.RewardType.Equipment,
                        quantity = 1,
                        description = $"Exclusive Tier {i/5 + 1} equipment",
                        isPremiumOnly = true
                    });
                }
                
                // Special milestone rewards
                if (i == 10)
                {
                    passRewards.Add(new FaithPassReward
                    {
                        tier = i,
                        rewardName = "Faith Pass Mount",
                        type = FaithPassReward.RewardType.Mount,
                        quantity = 1,
                        description = "Exclusive Faith Pass mount",
                        isPremiumOnly = true
                    });
                }
                
                if (i == 25)
                {
                    passRewards.Add(new FaithPassReward
                    {
                        tier = i,
                        rewardName = "Extra Mission Slot",
                        type = FaithPassReward.RewardType.MissionSlot,
                        quantity = 1,
                        description = "Permanent additional mission slot",
                        isPremiumOnly = true
                    });
                }
            }
        }
        
        #endregion
        
        #region Bonus Systems
        
        public float GetGoldMultiplier()
        {
            return IsFaithPassActive() ? GameConfig.FAITH_PASS_GOLD_BONUS : 1f;
        }
        
        public float GetXPMultiplier()
        {
            return IsFaithPassActive() ? GameConfig.FAITH_PASS_XP_BONUS : 1f;
        }
        
        public int GetDailyMissionCount()
        {
            return IsFaithPassActive() ? 
                GameConfig.DAILY_MISSIONS_FAITH_PASS : 
                GameConfig.DAILY_MISSIONS_BASE;
        }
        
        #endregion
        
        #region Payment Processing (Mock)
        
        private bool ProcessPayment(float amount)
        {
            // Mock payment processing
            // In real implementation, integrate with:
            // - Google Play Billing
            // - Apple App Store
            // - PayPal
            // - Stripe
            
            Debug.Log($"💳 Processing payment: ${amount}");
            
            // Simulate payment success (90% success rate for testing)
            bool success = UnityEngine.Random.Range(0f, 1f) > 0.1f;
            
            if (success)
            {
                Debug.Log("✅ Payment successful!");
                RecordRevenueAnalytics(amount);
            }
            else
            {
                Debug.Log("❌ Payment failed!");
            }
            
            return success;
        }
        
        private void RecordRevenueAnalytics(float amount)
        {
            // Record analytics for revenue tracking
            Debug.Log($"📊 REVENUE ANALYTICS: +${amount} from Faith Pass");
            
            // In real implementation, send to analytics:
            // - Google Analytics
            // - Unity Analytics
            // - Custom analytics API
        }
        
        #endregion
        
        #region Data Persistence
        
        private void SaveFaithPassData()
        {
            string json = JsonUtility.ToJson(currentPass);
            PlayerPrefs.SetString("FaithPassData", json);
            PlayerPrefs.Save();
        }
        
        private void LoadFaithPassData()
        {
            if (PlayerPrefs.HasKey("FaithPassData"))
            {
                string json = PlayerPrefs.GetString("FaithPassData");
                currentPass = JsonUtility.FromJson<FaithPassData>(json);
            }
        }
        
        #endregion
        
        #region Debug & Testing
        
        [ContextMenu("Debug: Purchase Faith Pass")]
        private void DebugPurchaseFaithPass()
        {
            PurchaseFaithPass();
        }
        
        [ContextMenu("Debug: Add 1000 XP")]
        private void DebugAddXP()
        {
            AddXP(1000);
        }
        
        [ContextMenu("Debug: Reset Faith Pass")]
        private void DebugResetFaithPass()
        {
            currentPass = new FaithPassData();
            SaveFaithPassData();
            Debug.Log("🔄 Faith Pass reset!");
        }
        
        // Public getters for UI
        public FaithPassData GetCurrentPassData() => currentPass;
        public List<FaithPassReward> GetAllRewards() => passRewards;
        public int GetCurrentTier() => currentPass.currentTier;
        public int GetTotalXP() => currentPass.totalXPEarned;
        public int GetXPForNextTier() => (currentPass.currentTier + 1) * xpPerTier - currentPass.totalXPEarned;
        
        #endregion
    }
}
