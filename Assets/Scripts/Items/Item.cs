using UnityEngine;
using System.Collections.Generic;

namespace CaminosDeLaFe.Items
{
    /// <summary>
    /// Base class for all items in the game
    /// </summary>
    [System.Serializable]
    public abstract class Item
    {
        [Header("Basic Info")]
        public string itemName;
        public string description;
        public Sprite icon;
        public ItemRarity rarity;
        public int maxStackSize = 1;
        public int goldValue;
        
        [Header("Requirements")]
        public int levelRequirement = 1;
        public string[] factionRequirements; // Empty = all factions
        
        protected Item(string name, string description, ItemRarity rarity = ItemRarity.Common, int goldValue = 10)
        {
            this.itemName = name;
            this.description = description;
            this.rarity = rarity;
            this.goldValue = goldValue;
            this.factionRequirements = new string[0];
        }
        
        /// <summary>
        /// Called when item is used/consumed
        /// </summary>
        public virtual bool Use(CaminosDeLaFe.Entities.Player player)
        {
            return false; // Base items cannot be used
        }
        
        /// <summary>
        /// Check if player can use this item
        /// </summary>
        public virtual bool CanUse(CaminosDeLaFe.Entities.Player player)
        {
            // Level requirement
            if (player.level < levelRequirement)
                return false;
                
            // Faction requirement
            if (factionRequirements.Length > 0)
            {
                bool factionAllowed = false;
                foreach (string faction in factionRequirements)
                {
                    if (player.factionName == faction)
                    {
                        factionAllowed = true;
                        break;
                    }
                }
                if (!factionAllowed)
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Get item color based on rarity
        /// </summary>
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return Color.white;
                case ItemRarity.Uncommon:
                    return Color.green;
                case ItemRarity.Rare:
                    return Color.blue;
                case ItemRarity.Epic:
                    return Color.magenta;
                case ItemRarity.Legendary:
                    return Color.yellow;
                case ItemRarity.Unique:
                    return Color.red;
                default:
                    return Color.white;
            }
        }
    }
    
    /// <summary>
    /// Item rarity levels
    /// </summary>
    public enum ItemRarity
    {
        Common,     // Blanco
        Uncommon,   // Verde
        Rare,       // Azul
        Epic,       // Morado
        Legendary,  // Amarillo
        Unique      // Rojo - Solo uno por servidor
    }
    
    /// <summary>
    /// Equipment slot types
    /// </summary>
    public enum EquipmentSlot
    {
        None,
        Weapon,         // Arma principal
        Shield,         // Escudo
        Helmet,         // Casco
        Chest,          // Pechera
        Legs,           // Grebas
        Boots,          // Botas
        Gloves,         // Guantes
        Ring1,          // Anillo 1
        Ring2,          // Anillo 2
        Necklace,       // Collar
        Mount           // Montura
    }
    
    /// <summary>
    /// Types of consumable items
    /// </summary>
    public enum ConsumableType
    {
        HealthPotion,
        ManaPotion,
        StaminaPotion,
        StatBoost,
        Food,
        Herb,
        Scroll
    }
}
