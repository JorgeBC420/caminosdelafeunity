using UnityEngine;
using System.Collections;

namespace CaminosDeLaFe.Items
{
    /// <summary>
    /// Consumable items like potions, food, and scrolls
    /// </summary>
    [System.Serializable]
    public class Consumable : Item
    {
        [Header("Consumable Properties")]
        public ConsumableType consumableType;
        public float effectValue;
        public float duration = 0f; // 0 = instant effect
        public bool canUseInCombat = true;
        public float cooldown = 0f;
        
        [Header("Effects")]
        public string[] statusEffects; // Buffs/debuffs to apply
        public AudioClip useSound;
        public GameObject useEffect; // Particle effect when used
        
        public Consumable(string name, string description, ConsumableType type, float value, ItemRarity rarity = ItemRarity.Common)
            : base(name, description, rarity, (int)(value * 5))
        {
            this.consumableType = type;
            this.effectValue = value;
            this.maxStackSize = 99; // Consumables can stack
            this.statusEffects = new string[0];
        }
        
        /// <summary>
        /// Use the consumable item
        /// </summary>
        public override bool Use(CaminosDeLaFe.Entities.Player player)
        {
            if (!CanUse(player))
                return false;
                
            // Apply immediate effects based on type
            switch (consumableType)
            {
                case ConsumableType.HealthPotion:
                    return UseHealthPotion(player);
                    
                case ConsumableType.ManaPotion:
                    return UseManaPotion(player);
                    
                case ConsumableType.StaminaPotion:
                    return UseStaminaPotion(player);
                    
                case ConsumableType.StatBoost:
                    return UseStatBoost(player);
                    
                case ConsumableType.Food:
                    return UseFood(player);
                    
                case ConsumableType.Herb:
                    return UseHerb(player);
                    
                case ConsumableType.Scroll:
                    return UseScroll(player);
                    
                default:
                    Debug.LogWarning($"Unhandled consumable type: {consumableType}");
                    return false;
            }
        }
        
        private bool UseHealthPotion(CaminosDeLaFe.Entities.Player player)
        {
            if (player.currentHealth >= player.stats.maxHealth)
            {
                Debug.Log("Health is already full!");
                return false;
            }
            
            int healAmount = Mathf.RoundToInt(effectValue);
            player.currentHealth = Mathf.Min(player.stats.maxHealth, player.currentHealth + healAmount);
            player.OnHealthChanged?.Invoke(player.currentHealth);
            
            Debug.Log($"Restored {healAmount} health! Current: {player.currentHealth}/{player.stats.maxHealth}");
            PlayUseEffects(player);
            return true;
        }
        
        private bool UseManaPotion(CaminosDeLaFe.Entities.Player player)
        {
            if (player.currentMana >= player.stats.maxMana)
            {
                Debug.Log("Mana is already full!");
                return false;
            }
            
            int manaAmount = Mathf.RoundToInt(effectValue);
            player.currentMana = Mathf.Min(player.stats.maxMana, player.currentMana + manaAmount);
            player.OnManaChanged?.Invoke(player.currentMana);
            
            Debug.Log($"Restored {manaAmount} mana! Current: {player.currentMana}/{player.stats.maxMana}");
            PlayUseEffects(player);
            return true;
        }
        
        private bool UseStaminaPotion(CaminosDeLaFe.Entities.Player player)
        {
            // For now, just boost movement speed temporarily
            if (duration > 0)
            {
                player.StartCoroutine(ApplyTemporarySpeedBoost(player, effectValue, duration));
                Debug.Log($"Speed boost applied! +{effectValue} for {duration} seconds");
                PlayUseEffects(player);
                return true;
            }
            return false;
        }
        
        private bool UseStatBoost(CaminosDeLaFe.Entities.Player player)
        {
            // Temporary stat boost - would need a buff system to implement properly
            Debug.Log($"Stat boost effect: +{effectValue} for {duration} seconds");
            PlayUseEffects(player);
            return true;
        }
        
        private bool UseFood(CaminosDeLaFe.Entities.Player player)
        {
            // Food heals slowly over time
            if (duration > 0)
            {
                player.StartCoroutine(ApplyHealOverTime(player, effectValue, duration));
                Debug.Log($"Food consumed! Healing {effectValue} over {duration} seconds");
                PlayUseEffects(player);
                return true;
            }
            return UseHealthPotion(player); // Instant food acts like health potion
        }
        
        private bool UseHerb(CaminosDeLaFe.Entities.Player player)
        {
            // Herbs can cure status effects or provide minor boosts
            Debug.Log($"Herb used! Effect: {effectValue}");
            PlayUseEffects(player);
            return true;
        }
        
        private bool UseScroll(CaminosDeLaFe.Entities.Player player)
        {
            // Scrolls have special magical effects
            Debug.Log($"Scroll activated! Magical effect: {effectValue}");
            PlayUseEffects(player);
            return true;
        }
        
        private void PlayUseEffects(CaminosDeLaFe.Entities.Player player)
        {
            // Play sound effect
            if (useSound != null)
            {
                AudioSource.PlayClipAtPoint(useSound, player.transform.position);
            }
            
            // Spawn particle effect
            if (useEffect != null)
            {
                GameObject effect = Object.Instantiate(useEffect, player.transform.position, Quaternion.identity);
                Object.Destroy(effect, 3f);
            }
        }
        
        private IEnumerator ApplyTemporarySpeedBoost(CaminosDeLaFe.Entities.Player player, float speedBoost, float duration)
        {
            float originalSpeed = player.moveSpeed;
            player.moveSpeed += speedBoost;
            
            yield return new WaitForSeconds(duration);
            
            player.moveSpeed = originalSpeed;
            Debug.Log("Speed boost effect ended.");
        }
        
        private IEnumerator ApplyHealOverTime(CaminosDeLaFe.Entities.Player player, float totalHealing, float duration)
        {
            float healPerSecond = totalHealing / duration;
            float elapsed = 0f;
            
            while (elapsed < duration && player.currentHealth < player.stats.maxHealth)
            {
                yield return new WaitForSeconds(1f);
                elapsed += 1f;
                
                int healAmount = Mathf.RoundToInt(healPerSecond);
                player.currentHealth = Mathf.Min(player.stats.maxHealth, player.currentHealth + healAmount);
                player.OnHealthChanged?.Invoke(player.currentHealth);
                
                Debug.Log($"Food healing tick: +{healAmount} HP");
            }
        }
    }
    
    /// <summary>
    /// Predefined consumable items
    /// </summary>
    public static class ConsumableDatabase
    {
        public static Consumable CreateHealthPotion(ItemRarity rarity = ItemRarity.Common)
        {
            float healAmount = GetHealAmountByRarity(rarity);
            string name = GetPotionNameByRarity("Poción de Vida", rarity);
            
            var potion = new Consumable(name, $"Restaura {healAmount} puntos de vida", ConsumableType.HealthPotion, healAmount, rarity);
            return potion;
        }
        
        public static Consumable CreateManaPotion(ItemRarity rarity = ItemRarity.Common)
        {
            float manaAmount = GetManaAmountByRarity(rarity);
            string name = GetPotionNameByRarity("Poción de Maná", rarity);
            
            var potion = new Consumable(name, $"Restaura {manaAmount} puntos de maná", ConsumableType.ManaPotion, manaAmount, rarity);
            return potion;
        }
        
        public static Consumable CreateFood(string name, float healAmount, float duration = 10f)
        {
            var food = new Consumable(name, $"Restaura {healAmount} vida durante {duration} segundos", ConsumableType.Food, healAmount, ItemRarity.Common);
            food.duration = duration;
            return food;
        }
        
        private static float GetHealAmountByRarity(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return 25f;
                case ItemRarity.Uncommon: return 50f;
                case ItemRarity.Rare: return 100f;
                case ItemRarity.Epic: return 200f;
                case ItemRarity.Legendary: return 500f;
                default: return 25f;
            }
        }
        
        private static float GetManaAmountByRarity(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return 20f;
                case ItemRarity.Uncommon: return 40f;
                case ItemRarity.Rare: return 80f;
                case ItemRarity.Epic: return 160f;
                case ItemRarity.Legendary: return 400f;
                default: return 20f;
            }
        }
        
        private static string GetPotionNameByRarity(string baseName, ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return baseName + " Menor";
                case ItemRarity.Uncommon: return baseName;
                case ItemRarity.Rare: return baseName + " Mayor";
                case ItemRarity.Epic: return baseName + " Épica";
                case ItemRarity.Legendary: return baseName + " Legendaria";
                default: return baseName;
            }
        }
    }
}
