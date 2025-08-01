using UnityEngine;
using CaminosDeLaFe.Data;
using CaminosDeLaFe.Entities;
using System.Collections.Generic;

namespace CaminosDeLaFe.Systems
{
    /// <summary>
    /// Auto combat system for calculating battle outcomes
    /// </summary>
    [System.Serializable]
    public class AutoCombatSystem
    {
        private Player player;
        
        public AutoCombatSystem(Player player)
        {
            this.player = player;
        }
        
        /// <summary>
        /// Calculate the outcome of a battle
        /// </summary>
        public BattleResult CalculateBattleOutcome(EnemyPower enemyPower)
        {
            float playerPower = CalculatePlayerPower();
            float factionModifier = GetFactionModifier(enemyPower.faction);
            playerPower *= factionModifier;
            
            float luckFactor = CalculateLuckFactor();
            float effectivePlayerPower = playerPower * luckFactor;
            float effectiveEnemyPower = enemyPower.totalPower * Random.Range(0.8f, 1.2f);
            
            bool victory = effectivePlayerPower > effectiveEnemyPower;
            float damageTaken = CalculateDamageTaken(effectiveEnemyPower);
            
            return new BattleResult
            {
                victory = victory,
                playerPower = effectivePlayerPower,
                enemyPower = effectiveEnemyPower,
                damageTaken = damageTaken,
                experienceGained = victory ? CalculateExperienceGain(enemyPower) : 0,
                goldGained = victory ? CalculateGoldGain(enemyPower) : 0
            };
        }
        
        /// <summary>
        /// Calculate player's total combat power
        /// </summary>
        public float CalculatePlayerPower()
        {
            float basePower = player.level * 10 +
                             player.stats.GetTotalStat("fuerza") * 2 +
                             player.stats.GetTotalStat("defensa") * 1.5f +
                             player.stats.GetTotalStat("tecnica") * 1.5f +
                             player.stats.GetTotalStat("destreza") * 1.2f;
            
            // Add equipment power (to be implemented when equipment system is added)
            // foreach (var item in player.equipment.Values)
            // {
            //     if (item != null)
            //         basePower += item.powerRating;
            // }
            
            return basePower;
        }
        
        /// <summary>
        /// Calculate luck factor for battle randomness
        /// </summary>
        private float CalculateLuckFactor()
        {
            // Base luck affected by player's intelligence and level
            float baseLuck = 1f + (player.stats.GetTotalStat("inteligencia") * 0.01f);
            float levelBonus = player.level * 0.005f;
            
            float minLuck = AutoCombatConfig.LUCK_FACTOR_MIN + levelBonus;
            float maxLuck = AutoCombatConfig.LUCK_FACTOR_MAX + levelBonus;
            
            return Random.Range(minLuck, maxLuck) * baseLuck;
        }
        
        /// <summary>
        /// Get faction modifier based on enemy faction
        /// </summary>
        private float GetFactionModifier(string enemyFaction)
        {
            if (string.IsNullOrEmpty(enemyFaction) || string.IsNullOrEmpty(player.factionName))
                return 1f;
            
            // Check if factions are enemies (bonus damage)
            var playerFaction = Factions.GetFaction(player.factionName);
            if (playerFaction != null)
            {
                foreach (string enemy in playerFaction.enemies)
                {
                    if (enemy == enemyFaction)
                        return 1.2f; // 20% bonus against enemy factions
                }
                
                foreach (string ally in playerFaction.allies)
                {
                    if (ally == enemyFaction)
                        return 0.8f; // 20% penalty against ally factions
                }
            }
            
            return 1f; // Neutral
        }
        
        /// <summary>
        /// Calculate damage taken by player
        /// </summary>
        private float CalculateDamageTaken(float effectiveEnemyPower)
        {
            float defense = player.stats.GetTotalStat("defensa");
            float resistance = player.stats.GetTotalStat("resistencia");
            
            // Damage reduction based on defense and resistance
            float totalDefense = defense + (resistance * 0.5f);
            float damageReduction = totalDefense / (totalDefense + 100f); // Diminishing returns
            damageReduction = Mathf.Clamp(damageReduction, AutoCombatConfig.BASE_DAMAGE_REDUCTION, AutoCombatConfig.MAX_DAMAGE_REDUCTION);
            
            float baseDamage = effectiveEnemyPower * 0.1f; // Base damage is 10% of enemy power
            float actualDamage = baseDamage * (1f - damageReduction);
            
            return Mathf.Max(1f, actualDamage); // Minimum 1 damage
        }
        
        /// <summary>
        /// Calculate experience gained from victory
        /// </summary>
        private int CalculateExperienceGain(EnemyPower enemyPower)
        {
            float baseExp = enemyPower.totalPower * 0.1f;
            float levelDifference = enemyPower.level - player.level;
            
            // Bonus/penalty based on level difference
            float levelModifier = 1f + (levelDifference * 0.1f);
            levelModifier = Mathf.Clamp(levelModifier, 0.5f, 2f);
            
            return Mathf.RoundToInt(baseExp * levelModifier);
        }
        
        /// <summary>
        /// Calculate gold gained from victory
        /// </summary>
        private int CalculateGoldGain(EnemyPower enemyPower)
        {
            float baseGold = 5f + (enemyPower.level * 2f);
            float randomFactor = Random.Range(0.5f, 1.5f);
            
            return Mathf.RoundToInt(baseGold * randomFactor);
        }
    }
    
    /// <summary>
    /// Represents enemy power for combat calculations
    /// </summary>
    [System.Serializable]
    public class EnemyPower
    {
        public string faction;
        public int level;
        public float totalPower;
        public Dictionary<string, float> stats;
        
        public EnemyPower(string faction, int level, float totalPower)
        {
            this.faction = faction;
            this.level = level;
            this.totalPower = totalPower;
            this.stats = new Dictionary<string, float>();
        }
        
        /// <summary>
        /// Create enemy power from an Enemy component
        /// </summary>
        public static EnemyPower FromEnemy(Enemy enemy)
        {
            float estimatedPower = enemy.maxHealth + (enemy.attackDamage * 5f) + (enemy.moveSpeed * 2f);
            int estimatedLevel = Mathf.RoundToInt(estimatedPower / 20f);
            
            return new EnemyPower(enemy.factionName, estimatedLevel, estimatedPower);
        }
    }
    
    /// <summary>
    /// Result of a battle calculation
    /// </summary>
    [System.Serializable]
    public class BattleResult
    {
        public bool victory;
        public float playerPower;
        public float enemyPower;
        public float damageTaken;
        public int experienceGained;
        public int goldGained;
        
        public override string ToString()
        {
            return $"Victory: {victory}, Player Power: {playerPower:F1}, Enemy Power: {enemyPower:F1}, " +
                   $"Damage Taken: {damageTaken:F1}, XP: {experienceGained}, Gold: {goldGained}";
        }
    }
}
