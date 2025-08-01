using UnityEngine;
using System.Collections.Generic;

namespace CaminoDeLaFe.Items
{
    /// <summary>
    /// Equipment items that can be worn by the player
    /// </summary>
    [System.Serializable]
    public class Equipment : Item
    {
        [Header("Equipment Stats")]
        public EquipmentSlot equipmentSlot;
        public Dictionary<string, int> statBonuses;
        public float powerRating;
        
        [Header("Visual")]
        public GameObject visualPrefab; // 3D model when equipped
        public bool hideHair = false;
        public bool hideFacialHair = false;
        
        [Header("Special Effects")]
        public string[] specialEffects; // Names of special abilities
        public float durability = 100f;
        public float maxDurability = 100f;
        
        public Equipment(string name, string description, EquipmentSlot slot, ItemRarity rarity = ItemRarity.Common, int goldValue = 50)
            : base(name, description, rarity, goldValue)
        {
            this.equipmentSlot = slot;
            this.statBonuses = new Dictionary<string, int>();
            this.specialEffects = new string[0];
            this.maxStackSize = 1; // Equipment can't stack
        }
        
        /// <summary>
        /// Add a stat bonus to this equipment
        /// </summary>
        public void AddStatBonus(string statName, int bonus)
        {
            if (statBonuses.ContainsKey(statName))
                statBonuses[statName] += bonus;
            else
                statBonuses[statName] = bonus;
                
            RecalculatePowerRating();
        }
        
        /// <summary>
        /// Calculate total power rating based on all bonuses
        /// </summary>
        private void RecalculatePowerRating()
        {
            powerRating = 0f;
            foreach (var bonus in statBonuses)
            {
                powerRating += bonus.Value * GetStatWeight(bonus.Key);
            }
            
            // Rarity multiplier
            powerRating *= GetRarityMultiplier();
        }
        
        private float GetStatWeight(string statName)
        {
            switch (statName.ToLower())
            {
                case "fuerza": return 2.0f;
                case "defensa": return 1.8f;
                case "tecnica": return 1.5f;
                case "destreza": return 1.5f;
                case "velocidad": return 1.2f;
                case "agilidad": return 1.2f;
                case "resistencia": return 1.0f;
                case "inteligencia": return 1.0f;
                default: return 1.0f;
            }
        }
        
        private float GetRarityMultiplier()
        {
            switch (rarity)
            {
                case ItemRarity.Common: return 1.0f;
                case ItemRarity.Uncommon: return 1.2f;
                case ItemRarity.Rare: return 1.5f;
                case ItemRarity.Epic: return 2.0f;
                case ItemRarity.Legendary: return 3.0f;
                case ItemRarity.Unique: return 5.0f;
                default: return 1.0f;
            }
        }
        
        /// <summary>
        /// Check if this equipment can be equipped by the player
        /// </summary>
        public override bool CanUse(CaminoDeLaFe.Entities.Player player)
        {
            if (!base.CanUse(player))
                return false;
                
            // Check if equipment is broken
            if (durability <= 0)
                return false;
                
            return true;
        }
        
        /// <summary>
        /// Repair the equipment
        /// </summary>
        public void Repair(float amount)
        {
            durability = Mathf.Min(maxDurability, durability + amount);
        }
        
        /// <summary>
        /// Damage the equipment
        /// </summary>
        public void TakeDamage(float damage)
        {
            durability = Mathf.Max(0, durability - damage);
        }
        
        /// <summary>
        /// Get durability percentage
        /// </summary>
        public float GetDurabilityPercent()
        {
            return durability / maxDurability;
        }
        
        /// <summary>
        /// Check if equipment is broken
        /// </summary>
        public bool IsBroken()
        {
            return durability <= 0;
        }
    }
    
    /// <summary>
    /// Weapon-specific equipment
    /// </summary>
    [System.Serializable]
    public class Weapon : Equipment
    {
        [Header("Weapon Stats")]
        public WeaponType weaponType;
        public float minDamage;
        public float maxDamage;
        public float attackSpeed = 1.0f;
        public float criticalChance = 0.05f;
        public float criticalMultiplier = 2.0f;
        
        public Weapon(string name, string description, WeaponType type, float minDamage, float maxDamage, ItemRarity rarity = ItemRarity.Common)
            : base(name, description, EquipmentSlot.Weapon, rarity, (int)(minDamage + maxDamage) * 10)
        {
            this.weaponType = type;
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
        }
        
        /// <summary>
        /// Calculate damage for an attack
        /// </summary>
        public float CalculateDamage(bool forceCritical = false)
        {
            float baseDamage = Random.Range(minDamage, maxDamage);
            
            // Check for critical hit
            bool isCritical = forceCritical || Random.Range(0f, 1f) < criticalChance;
            
            if (isCritical)
                baseDamage *= criticalMultiplier;
                
            return baseDamage;
        }
    }
    
    /// <summary>
    /// Armor-specific equipment
    /// </summary>
    [System.Serializable]
    public class Armor : Equipment
    {
        [Header("Armor Stats")]
        public ArmorType armorType;
        public float physicalDefense;
        public float magicalDefense;
        public float movementPenalty = 0f; // 0 = no penalty, 1 = can't move
        
        public Armor(string name, string description, EquipmentSlot slot, ArmorType type, float physicalDef, ItemRarity rarity = ItemRarity.Common)
            : base(name, description, slot, rarity, (int)(physicalDef * 20))
        {
            this.armorType = type;
            this.physicalDefense = physicalDef;
        }
    }
    
    /// <summary>
    /// Accessory equipment (rings, necklaces)
    /// </summary>
    [System.Serializable]
    public class Accessory : Equipment
    {
        [Header("Accessory")]
        public AccessoryType accessoryType;
        public string[] magicalEffects;
        
        public Accessory(string name, string description, EquipmentSlot slot, AccessoryType type, ItemRarity rarity = ItemRarity.Uncommon)
            : base(name, description, slot, rarity, 100)
        {
            this.accessoryType = type;
            this.magicalEffects = new string[0];
        }
    }
    
    /// <summary>
    /// Weapon types
    /// </summary>
    public enum WeaponType
    {
        Sword,      // Espada
        Mace,       // Maza
        Axe,        // Hacha
        Spear,      // Lanza
        Bow,        // Arco
        Crossbow,   // Ballesta
        Dagger,     // Daga
        Staff       // Bastón
    }
    
    /// <summary>
    /// Armor types
    /// </summary>
    public enum ArmorType
    {
        Cloth,      // Tela (magos)
        Leather,    // Cuero (scouts)
        Mail,       // Cota de malla
        Plate       // Placas (caballeros pesados)
    }
    
    /// <summary>
    /// Accessory types
    /// </summary>
    public enum AccessoryType
    {
        Ring,       // Anillo
        Necklace,   // Collar
        Amulet,     // Amuleto
        Bracelet    // Brazalete
    }
}
