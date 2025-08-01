using UnityEngine;
using System.Collections.Generic;
using System;

namespace CaminoDeLaFe.Systems
{
    /// <summary>
    /// Manages player statistics and bonuses
    /// </summary>
    [System.Serializable]
    public class PlayerStats
    {
        [Header("Base Stats")]
        public Dictionary<string, int> baseStats;
        public Dictionary<string, int> itemBonuses;
        public Dictionary<string, int> horseBonuses;
        
        [Header("Derived Stats")]
        public int maxHealth;
        public int maxMana;
        
        // Events for stat changes
        public event Action<string, int> OnStatChanged;
        public event Action<int> OnHealthChanged;
        public event Action<int> OnManaChanged;
        
        public PlayerStats(Dictionary<string, int> initialStats = null, Dictionary<string, int> itemBonuses = null, Dictionary<string, int> horseBonuses = null)
        {
            // Initialize with default stats if none provided
            this.baseStats = initialStats ?? GetDefaultStats();
            this.itemBonuses = itemBonuses ?? new Dictionary<string, int>();
            this.horseBonuses = horseBonuses ?? new Dictionary<string, int>();
            
            RecalculateDerivedStats();
        }
        
        private Dictionary<string, int> GetDefaultStats()
        {
            return new Dictionary<string, int>
            {
                { "nivel", 1 },
                { "fuerza", 10 },
                { "tecnica", 10 },
                { "destreza", 10 },
                { "defensa", 10 },
                { "resistencia", 10 },
                { "velocidad", 10 },
                { "agilidad", 10 },
                { "inteligencia", 10 }
            };
        }
        
        /// <summary>
        /// Get the total value of a stat including all bonuses
        /// </summary>
        public int GetTotalStat(string statName)
        {
            int baseStat = baseStats.ContainsKey(statName) ? baseStats[statName] : 0;
            int itemBonus = itemBonuses.ContainsKey(statName) ? itemBonuses[statName] : 0;
            int horseBonus = horseBonuses.ContainsKey(statName) ? horseBonuses[statName] : 0;
            
            return baseStat + itemBonus + horseBonus;
        }
        
        /// <summary>
        /// Get only the bonus value (items + horse) for a stat
        /// </summary>
        public int GetBonusValue(string statName)
        {
            int itemBonus = itemBonuses.ContainsKey(statName) ? itemBonuses[statName] : 0;
            int horseBonus = horseBonuses.ContainsKey(statName) ? horseBonuses[statName] : 0;
            
            return itemBonus + horseBonus;
        }
        
        /// <summary>
        /// Improve a base stat by the specified amount
        /// </summary>
        public bool ImproveStat(string statName, int amount = 1)
        {
            if (!baseStats.ContainsKey(statName))
                return false;
                
            baseStats[statName] += amount;
            RecalculateDerivedStats();
            OnStatChanged?.Invoke(statName, baseStats[statName]);
            
            return true;
        }
        
        /// <summary>
        /// Update item bonuses (called when equipment changes)
        /// </summary>
        public void UpdateItemBonuses(Dictionary<string, int> newItemBonuses)
        {
            itemBonuses = newItemBonuses ?? new Dictionary<string, int>();
            RecalculateDerivedStats();
        }
        
        /// <summary>
        /// Update horse bonuses (called when mount changes)
        /// </summary>
        public void UpdateHorseBonuses(Dictionary<string, int> newHorseBonuses)
        {
            horseBonuses = newHorseBonuses ?? new Dictionary<string, int>();
            RecalculateDerivedStats();
        }
        
        /// <summary>
        /// Recalculate derived stats like health and mana
        /// </summary>
        private void RecalculateDerivedStats()
        {
            int oldMaxHealth = maxHealth;
            int oldMaxMana = maxMana;
            
            // Health calculation: base + (resistance * 5) + (strength * 2)
            maxHealth = 100 + (GetTotalStat("resistencia") * 5) + (GetTotalStat("fuerza") * 2);
            
            // Mana calculation: base + (intelligence * 10)
            maxMana = 50 + (GetTotalStat("inteligencia") * 10);
            
            // Fire events if values changed
            if (oldMaxHealth != maxHealth)
                OnHealthChanged?.Invoke(maxHealth);
                
            if (oldMaxMana != maxMana)
                OnManaChanged?.Invoke(maxMana);
        }
        
        /// <summary>
        /// Get the cost to improve a stat (quadratic scaling)
        /// </summary>
        public int GetStatUpgradeCost(string statName)
        {
            if (!baseStats.ContainsKey(statName))
                return int.MaxValue;
                
            int currentLevel = baseStats[statName];
            return (currentLevel + 1) * (currentLevel + 1); // Quadratic cost
        }
        
        /// <summary>
        /// Get all stat names
        /// </summary>
        public string[] GetStatNames()
        {
            var statNames = new List<string>();
            foreach (var kvp in baseStats)
            {
                if (kvp.Key != "nivel") // Skip level as it's special
                    statNames.Add(kvp.Key);
            }
            return statNames.ToArray();
        }
        
        /// <summary>
        /// Calculate total power rating for combat
        /// </summary>
        public float CalculatePowerRating()
        {
            return GetTotalStat("nivel") * 10 +
                   GetTotalStat("fuerza") * 2 +
                   GetTotalStat("tecnica") * 1.5f +
                   GetTotalStat("destreza") * 1.5f +
                   GetTotalStat("defensa") * 1.5f;
        }
    }
}
