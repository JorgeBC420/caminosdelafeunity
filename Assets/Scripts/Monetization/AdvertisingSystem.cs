using System;
using System.Collections;
using UnityEngine;
using CaminosDeLaFe.Data;

namespace CaminosDeLaFe.Monetization
{
    /// <summary>
    /// 📺 ADVERTISING SYSTEM - Critical Revenue Stream
    /// Manages banner ads, interstitial ads, and rewarded video ads
    /// Integrates with Faith Pass for ad-free experience
    /// </summary>
    public class AdvertisingSystem : MonoBehaviour
    {
        [Header("Ad Configuration")]
        [SerializeField] private bool adSystemEnabled = true;
        [SerializeField] private float gameSessionStartTime;
        [SerializeField] private float lastAdTime;
        [SerializeField] private int adsWatchedToday;
        [SerializeField] private bool bannerVisible = true;
        
        [Header("Ad Placement IDs (Configure with Ad Network)")]
        [SerializeField] private string bannerAdID = "ca-app-pub-XXXXXXXX/XXXXXXXX";
        [SerializeField] private string interstitialAdID = "ca-app-pub-XXXXXXXX/XXXXXXXX";
        [SerializeField] private string rewardedAdID = "ca-app-pub-XXXXXXXX/XXXXXXXX";
        
        [Header("Revenue Tracking")]
        [SerializeField] private float totalRevenueToday;
        [SerializeField] private float totalRevenueAllTime;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private bool simulateAds = true;
        
        // Components
        private FaithPassSystem faithPassSystem;
        private Player player;
        
        // Events
        public static event System.Action OnAdWatched;
        public static event System.Action<float> OnRewardedAdCompleted;
        public static event System.Action<bool> OnBannerVisibilityChanged;
        
        // Ad tracking
        private Coroutine adTimerCoroutine;
        private bool isAdPlaying = false;
        
        private void Start()
        {
            InitializeAdSystem();
            StartAdTimer();
        }
        
        private void InitializeAdSystem()
        {
            faithPassSystem = FindObjectOfType<FaithPassSystem>();
            player = FindObjectOfType<Player>();
            
            gameSessionStartTime = Time.time;
            lastAdTime = Time.time;
            
            LoadAdData();
            
            if (adSystemEnabled)
            {
                InitializeAdNetwork();
                ShowBannerAd();
            }
            
            Debug.Log("📺 Advertising System initialized");
        }
        
        #region Ad Network Integration
        
        private void InitializeAdNetwork()
        {
            // In real implementation, initialize ad networks:
            // - Google AdMob
            // - Unity Ads
            // - AppLovin MAX
            // - Facebook Audience Network
            
            if (debugMode)
            {
                Debug.Log("📺 Initializing Ad Networks...");
                Debug.Log($"📺 Banner ID: {bannerAdID}");
                Debug.Log($"📺 Interstitial ID: {interstitialAdID}");
                Debug.Log($"📺 Rewarded ID: {rewardedAdID}");
            }
            
            // Mock initialization
            StartCoroutine(MockAdNetworkInit());
        }
        
        private IEnumerator MockAdNetworkInit()
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("✅ Ad Networks initialized successfully");
        }
        
        #endregion
        
        #region Banner Ads
        
        public void ShowBannerAd()
        {
            // Don't show banner if Faith Pass user has ad-free
            if (faithPassSystem != null && faithPassSystem.IsAdFreeActive())
            {
                HideBannerAd();
                return;
            }
            
            if (!bannerVisible)
            {
                bannerVisible = true;
                OnBannerVisibilityChanged?.Invoke(true);
                
                if (debugMode)
                {
                    Debug.Log("📺 Banner Ad shown");
                }
                
                // Track revenue (estimated)
                RecordAdRevenue(0.02f); // $0.02 per banner impression
            }
        }
        
        public void HideBannerAd()
        {
            if (bannerVisible)
            {
                bannerVisible = false;
                OnBannerVisibilityChanged?.Invoke(false);
                
                if (debugMode)
                {
                    Debug.Log("🚫 Banner Ad hidden");
                }
            }
        }
        
        #endregion
        
        #region Interstitial Ads
        
        private void StartAdTimer()
        {
            if (adTimerCoroutine != null)
            {
                StopCoroutine(adTimerCoroutine);
            }
            
            adTimerCoroutine = StartCoroutine(AdTimerCoroutine());
        }
        
        private IEnumerator AdTimerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(60f); // Check every minute
                
                // Check if 15 minutes have passed since last ad
                float timeSinceLastAd = Time.time - lastAdTime;
                
                if (timeSinceLastAd >= GameConfig.AD_INTERVAL_MINUTES * 60f)
                {
                    ShowInterstitialAd();
                }
            }
        }
        
        public void ShowInterstitialAd()
        {
            // Don't show ads if Faith Pass user has ad-free
            if (faithPassSystem != null && faithPassSystem.IsAdFreeActive())
            {
                if (debugMode)
                {
                    Debug.Log("🚫 Interstitial blocked - Faith Pass ad-free active");
                }
                return;
            }
            
            // Don't show ad if one is already playing
            if (isAdPlaying) return;
            
            StartCoroutine(PlayInterstitialAd());
        }
        
        private IEnumerator PlayInterstitialAd()
        {
            isAdPlaying = true;
            lastAdTime = Time.time;
            
            if (debugMode)
            {
                Debug.Log("📺 Showing Interstitial Ad...");
            }
            
            // Pause game
            Time.timeScale = 0f;
            
            if (simulateAds)
            {
                // Simulate ad duration (30 seconds)
                float adDuration = 30f;
                float startTime = Time.unscaledTime;
                
                while (Time.unscaledTime - startTime < adDuration)
                {
                    yield return null;
                }
            }
            else
            {
                // Real ad implementation would go here
                yield return new WaitForSecondsRealtime(30f);
            }
            
            // Resume game
            Time.timeScale = 1f;
            isAdPlaying = false;
            
            adsWatchedToday++;
            RecordAdRevenue(0.50f); // $0.50 per interstitial
            OnAdWatched?.Invoke();
            
            SaveAdData();
            
            if (debugMode)
            {
                Debug.Log($"✅ Interstitial Ad completed. Revenue: +$0.50");
            }
        }
        
        #endregion
        
        #region Rewarded Video Ads
        
        public void ShowRewardedAd(System.Action<bool> callback = null)
        {
            StartCoroutine(PlayRewardedAd(callback));
        }
        
        private IEnumerator PlayRewardedAd(System.Action<bool> callback)
        {
            isAdPlaying = true;
            
            if (debugMode)
            {
                Debug.Log("📺 Showing Rewarded Video Ad...");
            }
            
            // Pause game
            Time.timeScale = 0f;
            
            bool adCompleted = true; // Assume success for simulation
            
            if (simulateAds)
            {
                // Simulate ad duration
                float adDuration = 30f;
                float startTime = Time.unscaledTime;
                
                while (Time.unscaledTime - startTime < adDuration)
                {
                    yield return null;
                }
                
                // 95% success rate for testing
                adCompleted = UnityEngine.Random.Range(0f, 1f) < 0.95f;
            }
            else
            {
                // Real rewarded ad implementation
                yield return new WaitForSecondsRealtime(30f);
            }
            
            // Resume game
            Time.timeScale = 1f;
            isAdPlaying = false;
            
            if (adCompleted)
            {
                // Give reward to player
                float goldReward = CalculateRewardedAdGold();
                player?.Stats.AddGold(Mathf.RoundToInt(goldReward));
                
                adsWatchedToday++;
                RecordAdRevenue(1.00f); // $1.00 per rewarded video
                OnRewardedAdCompleted?.Invoke(goldReward);
                
                if (debugMode)
                {
                    Debug.Log($"✅ Rewarded Ad completed! Player earned {goldReward} gold. Revenue: +$1.00");
                }
            }
            else
            {
                if (debugMode)
                {
                    Debug.Log("❌ Rewarded Ad was skipped or failed");
                }
            }
            
            callback?.Invoke(adCompleted);
            SaveAdData();
        }
        
        private float CalculateRewardedAdGold()
        {
            // Base reward
            float baseReward = 100f;
            
            // Level scaling
            if (player != null)
            {
                baseReward *= (1f + player.Stats.Level * 0.1f);
            }
            
            // Faith Pass bonus
            if (faithPassSystem != null && faithPassSystem.IsFaithPassActive())
            {
                baseReward *= GameConfig.FAITH_PASS_GOLD_BONUS;
            }
            
            // Optional ad bonus
            baseReward *= GameConfig.AD_OPTIONAL_GOLD_BONUS;
            
            return baseReward;
        }
        
        #endregion
        
        #region Revenue Analytics
        
        private void RecordAdRevenue(float revenue)
        {
            totalRevenueToday += revenue;
            totalRevenueAllTime += revenue;
            
            // Track platform share (70% to developer)
            float developerShare = revenue * GameConfig.AD_BANNER_REVENUE_SHARE;
            
            if (debugMode)
            {
                Debug.Log($"💰 Ad Revenue: +${revenue:F2} (Developer: ${developerShare:F2})");
                Debug.Log($"💰 Daily Total: ${totalRevenueToday:F2}");
            }
            
            // Send analytics to revenue tracking systems
            SendRevenueAnalytics(revenue, "advertisement");
        }
        
        private void SendRevenueAnalytics(float amount, string source)
        {
            // Integration points for analytics:
            // - Google Analytics 4
            // - Unity Analytics
            // - Facebook Analytics
            // - Custom analytics API
            
            Debug.Log($"📊 REVENUE ANALYTICS: +${amount:F2} from {source}");
            
            // In real implementation, send HTTP request to analytics API
            // POST /api/revenue/track
            // {
            //   "amount": amount,
            //   "source": source,
            //   "timestamp": DateTime.Now,
            //   "playerId": playerId
            // }
        }
        
        #endregion
        
        #region Daily Limits & Reset
        
        public bool CanWatchMoreAds()
        {
            // No limit for rewarded ads, but track for analytics
            return true;
        }
        
        public int GetAdsWatchedToday()
        {
            return adsWatchedToday;
        }
        
        public float GetRevenueToday()
        {
            return totalRevenueToday;
        }
        
        private void ResetDailyCounters()
        {
            adsWatchedToday = 0;
            totalRevenueToday = 0f;
            SaveAdData();
            
            Debug.Log("🔄 Daily ad counters reset");
        }
        
        #endregion
        
        #region Faith Pass Integration
        
        public bool IsAdFreeForUser()
        {
            return faithPassSystem != null && faithPassSystem.IsAdFreeActive();
        }
        
        public void OnFaithPassStatusChanged(bool isActive)
        {
            if (isActive)
            {
                HideBannerAd();
                Debug.Log("🚫 Ads disabled - Faith Pass activated");
            }
            else
            {
                ShowBannerAd();
                Debug.Log("📺 Ads re-enabled - Faith Pass expired");
            }
        }
        
        #endregion
        
        #region Data Persistence
        
        private void SaveAdData()
        {
            PlayerPrefs.SetInt("AdsWatchedToday", adsWatchedToday);
            PlayerPrefs.SetFloat("RevenueToday", totalRevenueToday);
            PlayerPrefs.SetFloat("RevenueAllTime", totalRevenueAllTime);
            PlayerPrefs.SetFloat("LastAdTime", lastAdTime);
            PlayerPrefs.SetString("LastResetDate", DateTime.Now.Date.ToString());
            PlayerPrefs.Save();
        }
        
        private void LoadAdData()
        {
            // Check if we need to reset daily counters
            string lastResetDate = PlayerPrefs.GetString("LastResetDate", "");
            if (lastResetDate != DateTime.Now.Date.ToString())
            {
                ResetDailyCounters();
            }
            else
            {
                adsWatchedToday = PlayerPrefs.GetInt("AdsWatchedToday", 0);
                totalRevenueToday = PlayerPrefs.GetFloat("RevenueToday", 0f);
            }
            
            totalRevenueAllTime = PlayerPrefs.GetFloat("RevenueAllTime", 0f);
            lastAdTime = PlayerPrefs.GetFloat("LastAdTime", Time.time);
        }
        
        #endregion
        
        #region Debug & Testing
        
        [ContextMenu("Debug: Force Interstitial Ad")]
        private void DebugForceInterstitial()
        {
            ShowInterstitialAd();
        }
        
        [ContextMenu("Debug: Show Rewarded Ad")]
        private void DebugShowRewarded()
        {
            ShowRewardedAd((success) => 
            {
                Debug.Log($"Rewarded ad result: {success}");
            });
        }
        
        [ContextMenu("Debug: Toggle Banner")]
        private void DebugToggleBanner()
        {
            if (bannerVisible)
                HideBannerAd();
            else
                ShowBannerAd();
        }
        
        [ContextMenu("Debug: Show Revenue Stats")]
        private void DebugShowRevenue()
        {
            Debug.Log($"📊 REVENUE STATS:");
            Debug.Log($"  Today: ${totalRevenueToday:F2}");
            Debug.Log($"  All-time: ${totalRevenueAllTime:F2}");
            Debug.Log($"  Ads watched today: {adsWatchedToday}");
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Check if an ad is currently playing
        /// </summary>
        public bool IsAdPlaying() => isAdPlaying;
        
        /// <summary>
        /// Get time until next automatic ad
        /// </summary>
        public float GetTimeUntilNextAd()
        {
            float timeSinceLastAd = Time.time - lastAdTime;
            float adInterval = GameConfig.AD_INTERVAL_MINUTES * 60f;
            return Mathf.Max(0f, adInterval - timeSinceLastAd);
        }
        
        /// <summary>
        /// Manually trigger rewarded ad for bonus gold
        /// </summary>
        public void RequestBonusGoldAd()
        {
            if (!isAdPlaying)
            {
                ShowRewardedAd();
            }
        }
        
        #endregion
    }
}
