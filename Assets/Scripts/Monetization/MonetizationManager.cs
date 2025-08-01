using UnityEngine;
using CaminosDeLaFe.Data;

namespace CaminosDeLaFe.Monetization
{
    /// <summary>
    /// 🎯 GAMES LAB MONETIZATION MANAGER
    /// Central hub for all monetization systems
    /// Implements the complete Games Lab revenue model from OpenNexus
    /// </summary>
    public class MonetizationManager : MonoBehaviour
    {
        [Header("Monetization Systems")]
        [SerializeField] private FaithPassSystem faithPassSystem;
        [SerializeField] private AdvertisingSystem advertisingSystem;
        [SerializeField] private EconomySystem economySystem;
        [SerializeField] private DailyLimitsSystem dailyLimitsSystem;
        
        [Header("Monetization Status")]
        [SerializeField] private bool monetizationEnabled = true;
        [SerializeField] private float totalRevenueToday;
        [SerializeField] private float totalRevenueProjected;
        [SerializeField] private int activePayingUsers;
        [SerializeField] private float averageRevenuePerUser;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        
        // Components
        private Player player;
        
        // Events
        public static event System.Action<float> OnRevenueUpdated;
        public static event System.Action<MonetizationEvent> OnMonetizationEvent;
        
        private void Start()
        {
            InitializeMonetization();
            SetupEventListeners();
        }
        
        private void InitializeMonetization()
        {
            player = FindObjectOfType<Player>();
            
            // Initialize or find monetization systems
            if (faithPassSystem == null)
                faithPassSystem = FindObjectOfType<FaithPassSystem>();
            if (advertisingSystem == null)
                advertisingSystem = FindObjectOfType<AdvertisingSystem>();
            if (economySystem == null)
                economySystem = FindObjectOfType<EconomySystem>();
            if (dailyLimitsSystem == null)
                dailyLimitsSystem = FindObjectOfType<DailyLimitsSystem>();
            
            // Create systems if they don't exist
            CreateMissingMonetizationSystems();
            
            Debug.Log("🎯 Games Lab Monetization Manager initialized");
            
            if (debugMode)
            {
                LogSystemStatus();
            }
        }
        
        private void CreateMissingMonetizationSystems()
        {
            if (faithPassSystem == null)
            {
                GameObject faithPassGO = new GameObject("FaithPassSystem");
                faithPassGO.transform.SetParent(transform);
                faithPassSystem = faithPassGO.AddComponent<FaithPassSystem>();
            }
            
            if (advertisingSystem == null)
            {
                GameObject adSystemGO = new GameObject("AdvertisingSystem");
                adSystemGO.transform.SetParent(transform);
                advertisingSystem = adSystemGO.AddComponent<AdvertisingSystem>();
            }
            
            if (economySystem == null)
            {
                GameObject economyGO = new GameObject("EconomySystem");
                economyGO.transform.SetParent(transform);
                economySystem = economyGO.AddComponent<EconomySystem>();
            }
            
            if (dailyLimitsSystem == null)
            {
                GameObject limitsGO = new GameObject("DailyLimitsSystem");
                limitsGO.transform.SetParent(transform);
                dailyLimitsSystem = limitsGO.AddComponent<DailyLimitsSystem>();
            }
        }
        
        #region Event System Setup
        
        private void SetupEventListeners()
        {
            // Faith Pass events
            if (faithPassSystem != null)
            {
                // Subscribe to Faith Pass events
            }
            
            // Advertising events
            if (advertisingSystem != null)
            {
                AdvertisingSystem.OnRewardedAdCompleted += OnAdRewardCompleted;
                AdvertisingSystem.OnAdWatched += OnAdWatched;
            }
            
            // Economy events
            if (economySystem != null)
            {
                EconomySystem.OnGoldPurchased += OnGoldPurchased;
                EconomySystem.OnRevenueGenerated += OnRevenueGenerated;
            }
            
            // Daily limits events
            if (dailyLimitsSystem != null)
            {
                DailyLimitsSystem.OnLimitReached += OnDailyLimitReached;
                DailyLimitsSystem.OnEventCurrencyEarned += OnEventCurrencyEarned;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (advertisingSystem != null)
            {
                AdvertisingSystem.OnRewardedAdCompleted -= OnAdRewardCompleted;
                AdvertisingSystem.OnAdWatched -= OnAdWatched;
            }
            
            if (economySystem != null)
            {
                EconomySystem.OnGoldPurchased -= OnGoldPurchased;
                EconomySystem.OnRevenueGenerated -= OnRevenueGenerated;
            }
            
            if (dailyLimitsSystem != null)
            {
                DailyLimitsSystem.OnLimitReached -= OnDailyLimitReached;
                DailyLimitsSystem.OnEventCurrencyEarned -= OnEventCurrencyEarned;
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnAdRewardCompleted(float goldReward)
        {
            TriggerMonetizationEvent(new MonetizationEvent
            {
                type = MonetizationEventType.AdRewardClaimed,
                value = goldReward,
                source = "rewarded_video",
                timestamp = System.DateTime.Now
            });
        }
        
        private void OnAdWatched()
        {
            TriggerMonetizationEvent(new MonetizationEvent
            {
                type = MonetizationEventType.AdWatched,
                value = 0.5f, // Estimated ad revenue
                source = "interstitial",
                timestamp = System.DateTime.Now
            });
        }
        
        private void OnGoldPurchased(int goldAmount, float usdAmount)
        {
            TriggerMonetizationEvent(new MonetizationEvent
            {
                type = MonetizationEventType.GoldPurchased,
                value = usdAmount,
                source = "gold_package",
                timestamp = System.DateTime.Now,
                metadata = $"Gold: {goldAmount}"
            });
        }
        
        private void OnRevenueGenerated(float amount)
        {
            totalRevenueToday += amount;
            OnRevenueUpdated?.Invoke(totalRevenueToday);
            
            CalculateProjectedRevenue();
        }
        
        private void OnDailyLimitReached(string limitType)
        {
            // When F2P players hit limits, show monetization options
            ShowMonetizationOptions(limitType);
        }
        
        private void OnEventCurrencyEarned(int amount)
        {
            TriggerMonetizationEvent(new MonetizationEvent
            {
                type = MonetizationEventType.EventCurrencyEarned,
                value = amount,
                source = "daily_limit_compensation",
                timestamp = System.DateTime.Now
            });
        }
        
        #endregion
        
        #region Monetization Flow Management
        
        /// <summary>
        /// Show appropriate monetization options when limits are reached
        /// </summary>
        private void ShowMonetizationOptions(string limitType)
        {
            if (!monetizationEnabled) return;
            
            // Check if player already has Faith Pass
            bool hasFaithPass = faithPassSystem?.IsFaithPassActive() ?? false;
            
            if (!hasFaithPass)
            {
                // Show Faith Pass promotion
                ShowFaithPassPromotion(limitType);
            }
            else
            {
                // Show alternative options (limit increases, etc.)
                ShowPremiumOptions(limitType);
            }
        }
        
        private void ShowFaithPassPromotion(string limitType)
        {
            string message = GetFaithPassPromotionMessage(limitType);
            
            if (debugMode)
            {
                Debug.Log($"💰 FAITH PASS PROMOTION: {message}");
            }
            
            // In real implementation, show UI popup
            // UI: "Daily limit reached! Get Faith Pass for unlimited farming + ad-free experience!"
            // Button: "Get Faith Pass ($10/month)"
            // Button: "Watch Ad for bonus"
            // Button: "Continue with Faith Tokens"
        }
        
        private void ShowPremiumOptions(string limitType)
        {
            if (debugMode)
            {
                Debug.Log($"💎 PREMIUM OPTIONS: Showing additional options for {limitType}");
            }
            
            // Show premium options for existing Faith Pass users
            // - Instant limit reset ($2.99)
            // - Permanent limit increase ($4.99)
            // - Watch rewarded ad for temporary boost
        }
        
        private string GetFaithPassPromotionMessage(string limitType)
        {
            switch (limitType)
            {
                case "daily_gold":
                    return "Daily gold limit reached! Faith Pass gives +50% gold limits and +30% gold bonus!";
                case "daily_missions":
                    return "Mission limit reached! Faith Pass unlocks 8 daily missions instead of 5!";
                case "daily_boss_kills":
                    return "Boss kill limit reached! Faith Pass gives +50% boss kill limits!";
                default:
                    return "Daily limit reached! Faith Pass removes most limits and gives amazing bonuses!";
            }
        }
        
        #endregion
        
        #region Revenue Analytics
        
        private void CalculateProjectedRevenue()
        {
            // Simple projection based on current day performance
            float hoursInDay = 24f;
            float currentHour = System.DateTime.Now.Hour + (System.DateTime.Now.Minute / 60f);
            
            if (currentHour > 0)
            {
                totalRevenueProjected = totalRevenueToday * (hoursInDay / currentHour);
            }
        }
        
        public MonetizationStats GetMonetizationStats()
        {
            var adStats = advertisingSystem?.GetRevenueToday() ?? 0f;
            var economyReport = economySystem?.GenerateEconomyReport();
            
            return new MonetizationStats
            {
                totalRevenueToday = totalRevenueToday,
                projectedDailyRevenue = totalRevenueProjected,
                adRevenue = adStats,
                goldPurchaseRevenue = economyReport?.totalRevenueToday ?? 0f,
                faithPassRevenue = CalculateFaithPassRevenue(),
                activePayingUsers = activePayingUsers,
                averageRevenuePerUser = averageRevenuePerUser
            };
        }
        
        private float CalculateFaithPassRevenue()
        {
            // Estimate Faith Pass revenue based on active subscriptions
            // In real implementation, this would come from payment processor
            return 0f; // Placeholder
        }
        
        #endregion
        
        #region Player Conversion Tracking
        
        /// <summary>
        /// Track player progression through monetization funnel
        /// </summary>
        public void TrackPlayerConversion(ConversionStep step)
        {
            if (debugMode)
            {
                Debug.Log($"📊 CONVERSION TRACKING: Player reached {step}");
            }
            
            // Send to analytics
            SendConversionAnalytics(step);
        }
        
        public enum ConversionStep
        {
            FirstLimitReached,
            ViewedFaithPassPromotion,
            StartedFaithPassPurchase,
            CompletedFaithPassPurchase,
            FirstGoldPurchase,
            BecameWhale, // High spender
            Churned      // Stopped playing
        }
        
        private void SendConversionAnalytics(ConversionStep step)
        {
            // Integration with analytics platforms
            // - Google Analytics 4
            // - Unity Analytics
            // - Custom analytics API
            
            Debug.Log($"📊 ANALYTICS: Conversion step '{step}' recorded");
        }
        
        #endregion
        
        #region Cross-Platform Integration
        
        /// <summary>
        /// Integrate with other OpenNexus products
        /// </summary>
        public void IntegrateWithEcosystem()
        {
            // Check for CajaCentralPOS integration
            bool hasCajaCentral = CheckCajaCentralIntegration();
            
            // Check for CounterCoreHazardAV integration
            bool hasCounterCore = CheckCounterCoreIntegration();
            
            if (hasCajaCentral || hasCounterCore)
            {
                ApplyEcosystemBonuses();
            }
        }
        
        private bool CheckCajaCentralIntegration()
        {
            // Mock check - in real implementation, call CajaCentral API
            // GET /api/user/{userId}/status
            return PlayerPrefs.GetInt("HasUsedCajaCentral", 0) == 1;
        }
        
        private bool CheckCounterCoreIntegration()
        {
            // Mock check - in real implementation, call CounterCore API
            return PlayerPrefs.GetInt("HasUsedCounterCore", 0) == 1;
        }
        
        private void ApplyEcosystemBonuses()
        {
            float bonus = GameConfig.CROSS_PRODUCT_REWARD_BONUS;
            
            if (debugMode)
            {
                Debug.Log($"🌐 ECOSYSTEM BONUS: +{(bonus - 1f) * 100}% for using other OpenNexus products");
            }
            
            // Apply bonuses to current session
            // This would integrate with the stats and economy systems
        }
        
        #endregion
        
        #region Debug & Admin Tools
        
        [ContextMenu("Debug: Show Monetization Status")]
        private void DebugShowStatus()
        {
            var stats = GetMonetizationStats();
            
            Debug.Log("🎯 MONETIZATION STATUS:");
            Debug.Log($"  Revenue Today: ${stats.totalRevenueToday:F2}");
            Debug.Log($"  Projected Daily: ${stats.projectedDailyRevenue:F2}");
            Debug.Log($"  Ad Revenue: ${stats.adRevenue:F2}");
            Debug.Log($"  Gold Purchase Revenue: ${stats.goldPurchaseRevenue:F2}");
            Debug.Log($"  Faith Pass Active: {faithPassSystem?.IsFaithPassActive()}");
            Debug.Log($"  Ad-Free Active: {advertisingSystem?.IsAdFreeForUser()}");
        }
        
        [ContextMenu("Debug: Simulate Conversion Funnel")]
        private void DebugSimulateConversion()
        {
            TrackPlayerConversion(ConversionStep.FirstLimitReached);
            TrackPlayerConversion(ConversionStep.ViewedFaithPassPromotion);
            TrackPlayerConversion(ConversionStep.StartedFaithPassPurchase);
        }
        
        [ContextMenu("Debug: Force Limit Reached")]
        private void DebugForceLimitReached()
        {
            ShowMonetizationOptions("daily_gold");
        }
        
        private void LogSystemStatus()
        {
            Debug.Log("🎯 GAMES LAB MONETIZATION SYSTEMS:");
            Debug.Log($"  ✅ Faith Pass System: {faithPassSystem != null}");
            Debug.Log($"  ✅ Advertising System: {advertisingSystem != null}");
            Debug.Log($"  ✅ Economy System: {economySystem != null}");
            Debug.Log($"  ✅ Daily Limits System: {dailyLimitsSystem != null}");
            Debug.Log($"  🎯 Monetization Enabled: {monetizationEnabled}");
        }
        
        #endregion
        
        #region Public API
        
        public bool IsFaithPassActive() => faithPassSystem?.IsFaithPassActive() ?? false;
        public bool IsAdFreeActive() => advertisingSystem?.IsAdFreeForUser() ?? false;
        public float GetTotalRevenue() => totalRevenueToday;
        public MonetizationStats GetStats() => GetMonetizationStats();
        
        /// <summary>
        /// Manually trigger monetization event
        /// </summary>
        public void TriggerMonetizationEvent(MonetizationEvent eventData)
        {
            OnMonetizationEvent?.Invoke(eventData);
            
            if (debugMode)
            {
                Debug.Log($"🎯 MONETIZATION EVENT: {eventData.type} | ${eventData.value:F2} | {eventData.source}");
            }
        }
        
        #endregion
    }
    
    // Supporting classes
    [System.Serializable]
    public class MonetizationEvent
    {
        public MonetizationEventType type;
        public float value;
        public string source;
        public System.DateTime timestamp;
        public string metadata;
    }
    
    public enum MonetizationEventType
    {
        FaithPassPurchased,
        GoldPurchased,
        AdWatched,
        AdRewardClaimed,
        LimitReached,
        EventCurrencyEarned,
        ConversionStep
    }
    
    [System.Serializable]
    public class MonetizationStats
    {
        public float totalRevenueToday;
        public float projectedDailyRevenue;
        public float adRevenue;
        public float goldPurchaseRevenue;
        public float faithPassRevenue;
        public int activePayingUsers;
        public float averageRevenuePerUser;
    }
}
