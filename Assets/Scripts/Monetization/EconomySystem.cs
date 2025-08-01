using System;
using System.Collections.Generic;
using UnityEngine;
using CaminoDeLaFe.Data;

namespace CaminoDeLaFe.Monetization
{
    /// <summary>
    /// 💰 USD-GOLD ECONOMY SYSTEM
    /// Manages the critical USD to Gold conversion system:
    /// - Level 1-10: $1 = 1000 Gold
    /// - Level 11+: $1 = 100 × Player Level Gold
    /// Core monetization mechanic for Games Lab revenue model
    /// </summary>
    [System.Serializable]
    public class GoldTransaction
    {
        public DateTime timestamp;
        public float usdAmount;
        public int goldAmount;
        public int playerLevel;
        public string transactionId;
        public TransactionType type;
        
        public enum TransactionType
        {
            Purchase,    // Player buys gold with USD
            Withdrawal,  // Player converts gold to USD (future feature)
            Reward,      // Gold from ads/Faith Pass
            Burn         // Gold removed (anti-inflation)
        }
    }
    
    public class EconomySystem : MonoBehaviour
    {
        [Header("Economy Configuration")]
        [SerializeField] private bool economyEnabled = true;
        [SerializeField] private float totalRevenueToday;
        [SerializeField] private float totalRevenueAllTime;
        [SerializeField] private int totalGoldSold;
        [SerializeField] private List<GoldTransaction> todaysTransactions;
        
        [Header("Anti-Inflation Tracking")]
        [SerializeField] private int totalGoldBurned;
        [SerializeField] private float averagePlayerLevel;
        [SerializeField] private float inflationRate;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        
        // Components
        private Player player;
        private FaithPassSystem faithPassSystem;
        
        // Events
        public static event System.Action<int, float> OnGoldPurchased;
        public static event System.Action<int> OnGoldBurned;
        public static event System.Action<float> OnRevenueGenerated;
        
        private void Start()
        {
            InitializeEconomy();
            LoadEconomyData();
        }
        
        private void InitializeEconomy()
        {
            player = FindObjectOfType<Player>();
            faithPassSystem = FindObjectOfType<FaithPassSystem>();
            
            if (todaysTransactions == null)
            {
                todaysTransactions = new List<GoldTransaction>();
            }
            
            Debug.Log("💰 Economy System initialized");
        }
        
        #region USD to Gold Conversion
        
        /// <summary>
        /// Calculate how much gold a player gets for USD based on their level
        /// Level 1-10: $1 = 1000 Gold
        /// Level 11+: $1 = 100 × Level Gold
        /// </summary>
        public int CalculateGoldForUSD(float usdAmount, int playerLevel)
        {
            int goldPerDollar;
            
            if (playerLevel <= 10)
            {
                goldPerDollar = GameConfig.USD_TO_GOLD_BASE; // 1000 gold per dollar
            }
            else
            {
                goldPerDollar = GameConfig.USD_TO_GOLD_MULTIPLIER * playerLevel; // 100 × Level
            }
            
            return Mathf.RoundToInt(usdAmount * goldPerDollar);
        }
        
        /// <summary>
        /// Calculate USD value of gold (for potential withdrawal system)
        /// </summary>
        public float CalculateUSDForGold(int goldAmount, int playerLevel)
        {
            int goldPerDollar = CalculateGoldForUSD(1f, playerLevel);
            float baseValue = (float)goldAmount / goldPerDollar;
            
            // Apply platform fee
            float afterFee = baseValue * (1f - GameConfig.GOLD_TO_USD_CONVERSION_FEE);
            
            return afterFee;
        }
        
        #endregion
        
        #region Gold Purchase System
        
        /// <summary>
        /// Process gold purchase with real money
        /// </summary>
        public void PurchaseGold(float usdAmount, System.Action<bool, int> callback = null)
        {
            if (!economyEnabled)
            {
                callback?.Invoke(false, 0);
                return;
            }
            
            if (player == null)
            {
                Debug.LogError("❌ Cannot purchase gold - Player not found!");
                callback?.Invoke(false, 0);
                return;
            }
            
            int playerLevel = player.Stats.Level;
            int goldAmount = CalculateGoldForUSD(usdAmount, playerLevel);
            
            if (debugMode)
            {
                Debug.Log($"💳 Processing gold purchase: ${usdAmount} → {goldAmount} gold (Level {playerLevel})");
            }
            
            // Process payment
            ProcessGoldPayment(usdAmount, goldAmount, playerLevel, callback);
        }
        
        private void ProcessGoldPayment(float usdAmount, int goldAmount, int playerLevel, System.Action<bool, int> callback)
        {
            // Mock payment processing
            // In real implementation, integrate with:
            // - Google Play Billing
            // - Apple App Store
            // - PayPal
            // - Stripe
            // - CajaCentralPOS integration
            
            StartCoroutine(MockPaymentProcess(usdAmount, goldAmount, playerLevel, callback));
        }
        
        private System.Collections.IEnumerator MockPaymentProcess(float usdAmount, int goldAmount, int playerLevel, System.Action<bool, int> callback)
        {
            Debug.Log($"💳 Processing payment: ${usdAmount}...");
            
            // Simulate payment processing time
            yield return new WaitForSeconds(2f);
            
            // 95% success rate for testing
            bool paymentSuccess = UnityEngine.Random.Range(0f, 1f) < 0.95f;
            
            if (paymentSuccess)
            {
                // Give gold to player
                player.Stats.AddGold(goldAmount);
                
                // Record transaction
                RecordTransaction(new GoldTransaction
                {
                    timestamp = DateTime.Now,
                    usdAmount = usdAmount,
                    goldAmount = goldAmount,
                    playerLevel = playerLevel,
                    transactionId = GenerateTransactionId(),
                    type = GoldTransaction.TransactionType.Purchase
                });
                
                // Update totals
                totalRevenueToday += usdAmount;
                totalRevenueAllTime += usdAmount;
                totalGoldSold += goldAmount;
                
                // Trigger events
                OnGoldPurchased?.Invoke(goldAmount, usdAmount);
                OnRevenueGenerated?.Invoke(usdAmount);
                
                SaveEconomyData();
                
                Debug.Log($"✅ Gold purchase successful! Player received {goldAmount} gold for ${usdAmount}");
                Debug.Log($"💰 Total revenue today: ${totalRevenueToday:F2}");
            }
            else
            {
                Debug.Log("❌ Payment failed!");
            }
            
            callback?.Invoke(paymentSuccess, paymentSuccess ? goldAmount : 0);
        }
        
        #endregion
        
        #region Pre-made Gold Packages
        
        [System.Serializable]
        public class GoldPackage
        {
            public string name;
            public float usdPrice;
            public string description;
            public bool isPopular;
            public float bonusMultiplier; // Extra gold bonus
        }
        
        public List<GoldPackage> GetGoldPackages()
        {
            return new List<GoldPackage>
            {
                new GoldPackage
                {
                    name = "Starter Pack",
                    usdPrice = 0.99f,
                    description = "Perfect for new crusaders",
                    isPopular = false,
                    bonusMultiplier = 1.0f
                },
                new GoldPackage
                {
                    name = "Knight's Purse",
                    usdPrice = 4.99f,
                    description = "Equip yourself for battle",
                    isPopular = true,
                    bonusMultiplier = 1.1f // +10% bonus
                },
                new GoldPackage
                {
                    name = "Lord's Treasury",
                    usdPrice = 9.99f,
                    description = "For the wealthy crusader",
                    isPopular = false,
                    bonusMultiplier = 1.2f // +20% bonus
                },
                new GoldPackage
                {
                    name = "King's Fortune",
                    usdPrice = 19.99f,
                    description = "Royal wealth awaits",
                    isPopular = false,
                    bonusMultiplier = 1.3f // +30% bonus
                },
                new GoldPackage
                {
                    name = "Emperor's Vault",
                    usdPrice = 49.99f,
                    description = "Ultimate gold package",
                    isPopular = false,
                    bonusMultiplier = 1.5f // +50% bonus
                }
            };
        }
        
        public void PurchaseGoldPackage(GoldPackage package, System.Action<bool, int> callback = null)
        {
            if (player == null) 
            {
                callback?.Invoke(false, 0);
                return;
            }
            
            int baseGold = CalculateGoldForUSD(package.usdPrice, player.Stats.Level);
            int bonusGold = Mathf.RoundToInt(baseGold * package.bonusMultiplier);
            
            Debug.Log($"🎁 Purchasing {package.name}: ${package.usdPrice} → {bonusGold} gold (includes bonus)");
            
            ProcessGoldPayment(package.usdPrice, bonusGold, player.Stats.Level, callback);
        }
        
        #endregion
        
        #region Anti-Inflation System
        
        /// <summary>
        /// Burn gold from high-value transactions to prevent inflation
        /// </summary>
        public void BurnGoldFromTransaction(int transactionAmount)
        {
            if (transactionAmount < GameConfig.GOLD_BURN_THRESHOLD) return;
            
            int burnAmount = Mathf.RoundToInt(transactionAmount * GameConfig.GOLD_BURN_TAX_RATE);
            
            // Record burn transaction
            RecordTransaction(new GoldTransaction
            {
                timestamp = DateTime.Now,
                usdAmount = 0f,
                goldAmount = -burnAmount,
                playerLevel = player?.Stats.Level ?? 0,
                transactionId = GenerateTransactionId(),
                type = GoldTransaction.TransactionType.Burn
            });
            
            totalGoldBurned += burnAmount;
            OnGoldBurned?.Invoke(burnAmount);
            
            if (debugMode)
            {
                Debug.Log($"🔥 Gold burned for anti-inflation: {burnAmount} (from {transactionAmount} transaction)");
            }
        }
        
        /// <summary>
        /// Calculate current inflation rate based on gold supply
        /// </summary>
        public float CalculateInflationRate()
        {
            if (totalGoldSold == 0) return 0f;
            
            float goldSupply = totalGoldSold - totalGoldBurned;
            float burnRate = (float)totalGoldBurned / totalGoldSold;
            
            // Healthy economy should have 5-10% burn rate
            if (burnRate < 0.05f)
            {
                inflationRate = (0.05f - burnRate) * 100f; // High inflation
            }
            else if (burnRate > 0.15f)
            {
                inflationRate = -(burnRate - 0.10f) * 100f; // Deflation
            }
            else
            {
                inflationRate = 0f; // Stable
            }
            
            return inflationRate;
        }
        
        #endregion
        
        #region Cross-Ecosystem Integration
        
        /// <summary>
        /// Apply bonus for using other OpenNexus products
        /// </summary>
        public float GetCrossEcosystemBonus()
        {
            // Check if player has used other OpenNexus products
            // - CounterCoreHazardAV
            // - CajaCentralPOS
            // - Other Games Lab titles
            
            bool hasUsedOtherProducts = CheckOtherProductUsage();
            
            return hasUsedOtherProducts ? GameConfig.CROSS_PRODUCT_REWARD_BONUS : 1.0f;
        }
        
        private bool CheckOtherProductUsage()
        {
            // Mock check - in real implementation, query OpenNexus API
            // GET /api/user/{userId}/products
            
            bool hasCajaCentral = PlayerPrefs.GetInt("HasUsedCajaCentral", 0) == 1;
            bool hasCounterCore = PlayerPrefs.GetInt("HasUsedCounterCore", 0) == 1;
            
            return hasCajaCentral || hasCounterCore;
        }
        
        /// <summary>
        /// Award loyalty points for ecosystem engagement
        /// </summary>
        public void AwardLoyaltyPoints(float usdSpent)
        {
            int points = Mathf.RoundToInt(usdSpent * GameConfig.ECOSYSTEM_LOYALTY_POINTS_PER_DOLLAR);
            
            // Store loyalty points
            int currentPoints = PlayerPrefs.GetInt("EcosystemLoyaltyPoints", 0);
            PlayerPrefs.SetInt("EcosystemLoyaltyPoints", currentPoints + points);
            
            Debug.Log($"🏆 Loyalty points awarded: +{points} (Total: {currentPoints + points})");
        }
        
        #endregion
        
        #region Analytics & Reporting
        
        public EconomyReport GenerateEconomyReport()
        {
            return new EconomyReport
            {
                totalRevenueToday = totalRevenueToday,
                totalRevenueAllTime = totalRevenueAllTime,
                totalGoldSold = totalGoldSold,
                totalGoldBurned = totalGoldBurned,
                inflationRate = CalculateInflationRate(),
                averageTransactionSize = CalculateAverageTransactionSize(),
                transactionCount = todaysTransactions.Count,
                topSpendingLevel = GetTopSpendingLevel()
            };
        }
        
        private float CalculateAverageTransactionSize()
        {
            if (todaysTransactions.Count == 0) return 0f;
            
            float total = 0f;
            foreach (var transaction in todaysTransactions)
            {
                if (transaction.type == GoldTransaction.TransactionType.Purchase)
                {
                    total += transaction.usdAmount;
                }
            }
            
            return total / todaysTransactions.Count;
        }
        
        private int GetTopSpendingLevel()
        {
            Dictionary<int, float> spendingByLevel = new Dictionary<int, float>();
            
            foreach (var transaction in todaysTransactions)
            {
                if (transaction.type == GoldTransaction.TransactionType.Purchase)
                {
                    if (!spendingByLevel.ContainsKey(transaction.playerLevel))
                    {
                        spendingByLevel[transaction.playerLevel] = 0f;
                    }
                    spendingByLevel[transaction.playerLevel] += transaction.usdAmount;
                }
            }
            
            int topLevel = 0;
            float maxSpending = 0f;
            
            foreach (var kvp in spendingByLevel)
            {
                if (kvp.Value > maxSpending)
                {
                    maxSpending = kvp.Value;
                    topLevel = kvp.Key;
                }
            }
            
            return topLevel;
        }
        
        #endregion
        
        #region Utility Methods
        
        private string GenerateTransactionId()
        {
            return $"TX_{DateTime.Now.Ticks}_{UnityEngine.Random.Range(1000, 9999)}";
        }
        
        private void RecordTransaction(GoldTransaction transaction)
        {
            todaysTransactions.Add(transaction);
            
            // Send to analytics
            SendTransactionAnalytics(transaction);
        }
        
        private void SendTransactionAnalytics(GoldTransaction transaction)
        {
            Debug.Log($"📊 TRANSACTION ANALYTICS: {transaction.type} | ${transaction.usdAmount} | {transaction.goldAmount} gold");
            
            // In real implementation, send to analytics APIs
        }
        
        #endregion
        
        #region Data Persistence
        
        private void SaveEconomyData()
        {
            PlayerPrefs.SetFloat("TotalRevenueToday", totalRevenueToday);
            PlayerPrefs.SetFloat("TotalRevenueAllTime", totalRevenueAllTime);
            PlayerPrefs.SetInt("TotalGoldSold", totalGoldSold);
            PlayerPrefs.SetInt("TotalGoldBurned", totalGoldBurned);
            PlayerPrefs.SetString("LastEconomyResetDate", DateTime.Now.Date.ToString());
            
            // Save today's transactions
            string transactionsJson = JsonUtility.ToJson(new TransactionList { transactions = todaysTransactions });
            PlayerPrefs.SetString("TodaysTransactions", transactionsJson);
            
            PlayerPrefs.Save();
        }
        
        private void LoadEconomyData()
        {
            // Check if we need to reset daily counters
            string lastResetDate = PlayerPrefs.GetString("LastEconomyResetDate", "");
            if (lastResetDate != DateTime.Now.Date.ToString())
            {
                ResetDailyCounters();
            }
            else
            {
                totalRevenueToday = PlayerPrefs.GetFloat("TotalRevenueToday", 0f);
                
                // Load today's transactions
                if (PlayerPrefs.HasKey("TodaysTransactions"))
                {
                    string json = PlayerPrefs.GetString("TodaysTransactions");
                    var transactionList = JsonUtility.FromJson<TransactionList>(json);
                    todaysTransactions = transactionList.transactions ?? new List<GoldTransaction>();
                }
            }
            
            totalRevenueAllTime = PlayerPrefs.GetFloat("TotalRevenueAllTime", 0f);
            totalGoldSold = PlayerPrefs.GetInt("TotalGoldSold", 0);
            totalGoldBurned = PlayerPrefs.GetInt("TotalGoldBurned", 0);
        }
        
        private void ResetDailyCounters()
        {
            totalRevenueToday = 0f;
            todaysTransactions.Clear();
            SaveEconomyData();
            Debug.Log("🔄 Daily economy counters reset");
        }
        
        #endregion
        
        #region Debug & Testing
        
        [ContextMenu("Debug: Buy $1 Gold")]
        private void DebugBuyGold1()
        {
            PurchaseGold(1f, (success, gold) => 
            {
                Debug.Log($"Purchase result: {success}, Gold received: {gold}");
            });
        }
        
        [ContextMenu("Debug: Buy $10 Gold")]
        private void DebugBuyGold10()
        {
            PurchaseGold(10f, (success, gold) => 
            {
                Debug.Log($"Purchase result: {success}, Gold received: {gold}");
            });
        }
        
        [ContextMenu("Debug: Show Economy Report")]
        private void DebugShowEconomyReport()
        {
            var report = GenerateEconomyReport();
            Debug.Log($"📊 ECONOMY REPORT:");
            Debug.Log($"  Revenue Today: ${report.totalRevenueToday:F2}");
            Debug.Log($"  Revenue All-Time: ${report.totalRevenueAllTime:F2}");
            Debug.Log($"  Gold Sold: {report.totalGoldSold}");
            Debug.Log($"  Gold Burned: {report.totalGoldBurned}");
            Debug.Log($"  Inflation Rate: {report.inflationRate:F2}%");
            Debug.Log($"  Transactions Today: {report.transactionCount}");
        }
        
        #endregion
    }
    
    // Supporting classes
    [System.Serializable]
    public class TransactionList
    {
        public List<GoldTransaction> transactions;
    }
    
    [System.Serializable]
    public class EconomyReport
    {
        public float totalRevenueToday;
        public float totalRevenueAllTime;
        public int totalGoldSold;
        public int totalGoldBurned;
        public float inflationRate;
        public float averageTransactionSize;
        public int transactionCount;
        public int topSpendingLevel;
    }
}
