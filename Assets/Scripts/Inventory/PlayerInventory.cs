using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CaminoDeLaFe.Inventory
{
    /// <summary>
    /// Represents a stack of items in the inventory
    /// </summary>
    [System.Serializable]
    public class ItemStack
    {
        public CaminoDeLaFe.Items.Item item;
        public int quantity;
        
        public ItemStack(CaminoDeLaFe.Items.Item item, int quantity = 1)
        {
            this.item = item;
            this.quantity = Mathf.Clamp(quantity, 0, item.maxStackSize);
        }
        
        /// <summary>
        /// Check if this stack can accept more of the same item
        /// </summary>
        public bool CanAddMore(int amount = 1)
        {
            return quantity + amount <= item.maxStackSize;
        }
        
        /// <summary>
        /// Add items to this stack
        /// </summary>
        public int AddItems(int amount)
        {
            int canAdd = Mathf.Min(amount, item.maxStackSize - quantity);
            quantity += canAdd;
            return amount - canAdd; // Return overflow
        }
        
        /// <summary>
        /// Remove items from this stack
        /// </summary>
        public int RemoveItems(int amount)
        {
            int removed = Mathf.Min(amount, quantity);
            quantity -= removed;
            return removed;
        }
        
        /// <summary>
        /// Check if stack is empty
        /// </summary>
        public bool IsEmpty()
        {
            return quantity <= 0;
        }
        
        /// <summary>
        /// Check if stack is full
        /// </summary>
        public bool IsFull()
        {
            return quantity >= item.maxStackSize;
        }
    }
    
    /// <summary>
    /// Player inventory management system
    /// </summary>
    public class PlayerInventory
    {
        [Header("Inventory Settings")]
        public int maxSlots = 30;
        public Dictionary<CaminoDeLaFe.Items.EquipmentSlot, CaminoDeLaFe.Items.Equipment> equippedItems;
        public List<ItemStack> inventorySlots;
        
        // Events
        public System.Action<ItemStack, int> OnItemAdded;
        public System.Action<ItemStack, int> OnItemRemoved;
        public System.Action<CaminoDeLaFe.Items.Equipment, CaminoDeLaFe.Items.EquipmentSlot> OnItemEquipped;
        public System.Action<CaminoDeLaFe.Items.Equipment, CaminoDeLaFe.Items.EquipmentSlot> OnItemUnequipped;
        public System.Action OnInventoryChanged;
        
        private CaminoDeLaFe.Entities.Player owner;
        
        public PlayerInventory(CaminoDeLaFe.Entities.Player owner, int maxSlots = 30)
        {
            this.owner = owner;
            this.maxSlots = maxSlots;
            this.equippedItems = new Dictionary<CaminoDeLaFe.Items.EquipmentSlot, CaminoDeLaFe.Items.Equipment>();
            this.inventorySlots = new List<ItemStack>();
            
            // Initialize inventory slots
            for (int i = 0; i < maxSlots; i++)
            {
                inventorySlots.Add(null);
            }
        }
        
        #region Item Management
        
        /// <summary>
        /// Add an item to the inventory
        /// </summary>
        public bool AddItem(CaminoDeLaFe.Items.Item item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
                return false;
            
            int remainingQuantity = quantity;
            
            // First, try to add to existing stacks of the same item
            if (item.maxStackSize > 1)
            {
                for (int i = 0; i < inventorySlots.Count && remainingQuantity > 0; i++)
                {
                    ItemStack stack = inventorySlots[i];
                    if (stack != null && stack.item.itemName == item.itemName && !stack.IsFull())
                    {
                        int added = stack.AddItems(remainingQuantity);
                        remainingQuantity -= added;
                        OnItemAdded?.Invoke(stack, added);
                    }
                }
            }
            
            // Then, create new stacks for remaining items
            while (remainingQuantity > 0)
            {
                int emptySlot = FindEmptySlot();
                if (emptySlot == -1)
                {
                    Debug.LogWarning("Inventory is full! Cannot add more items.");
                    break;
                }
                
                int stackSize = Mathf.Min(remainingQuantity, item.maxStackSize);
                ItemStack newStack = new ItemStack(item, stackSize);
                inventorySlots[emptySlot] = newStack;
                remainingQuantity -= stackSize;
                
                OnItemAdded?.Invoke(newStack, stackSize);
            }
            
            if (remainingQuantity < quantity)
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Remove an item from the inventory
        /// </summary>
        public bool RemoveItem(CaminoDeLaFe.Items.Item item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
                return false;
            
            int remainingToRemove = quantity;
            
            // Remove from stacks (starting from the end to avoid index issues)
            for (int i = inventorySlots.Count - 1; i >= 0 && remainingToRemove > 0; i--)
            {
                ItemStack stack = inventorySlots[i];
                if (stack != null && stack.item.itemName == item.itemName)
                {
                    int removed = stack.RemoveItems(remainingToRemove);
                    remainingToRemove -= removed;
                    OnItemRemoved?.Invoke(stack, removed);
                    
                    // Remove empty stacks
                    if (stack.IsEmpty())
                    {
                        inventorySlots[i] = null;
                    }
                }
            }
            
            if (remainingToRemove < quantity)
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Use an item from the inventory
        /// </summary>
        public bool UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
                return false;
            
            ItemStack stack = inventorySlots[slotIndex];
            if (stack == null || stack.IsEmpty())
                return false;
            
            CaminoDeLaFe.Items.Item item = stack.item;
            
            // Try to use the item
            if (item.Use(owner))
            {
                // If it's a consumable, remove one from the stack
                if (item is CaminoDeLaFe.Items.Consumable)
                {
                    stack.RemoveItems(1);
                    OnItemRemoved?.Invoke(stack, 1);
                    
                    if (stack.IsEmpty())
                    {
                        inventorySlots[slotIndex] = null;
                    }
                    
                    OnInventoryChanged?.Invoke();
                }
                // If it's equipment, try to equip it
                else if (item is CaminoDeLaFe.Items.Equipment equipment)
                {
                    return EquipItem(equipment, slotIndex);
                }
                
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Get the total quantity of a specific item
        /// </summary>
        public int GetItemCount(CaminoDeLaFe.Items.Item item)
        {
            int total = 0;
            foreach (ItemStack stack in inventorySlots)
            {
                if (stack != null && stack.item.itemName == item.itemName)
                {
                    total += stack.quantity;
                }
            }
            return total;
        }
        
        /// <summary>
        /// Check if inventory has enough of an item
        /// </summary>
        public bool HasItem(CaminoDeLaFe.Items.Item item, int quantity = 1)
        {
            return GetItemCount(item) >= quantity;
        }
        
        /// <summary>
        /// Find the first empty slot
        /// </summary>
        private int FindEmptySlot()
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i] == null)
                    return i;
            }
            return -1;
        }
        
        /// <summary>
        /// Get all items of a specific type
        /// </summary>
        public List<ItemStack> GetItemsByType<T>() where T : CaminoDeLaFe.Items.Item
        {
            List<ItemStack> result = new List<ItemStack>();
            foreach (ItemStack stack in inventorySlots)
            {
                if (stack != null && stack.item is T)
                {
                    result.Add(stack);
                }
            }
            return result;
        }
        
        #endregion
        
        #region Equipment Management
        
        /// <summary>
        /// Equip an item
        /// </summary>
        public bool EquipItem(CaminoDeLaFe.Items.Equipment equipment, int inventorySlot = -1)
        {
            if (equipment == null || !equipment.CanUse(owner))
                return false;
            
            CaminoDeLaFe.Items.EquipmentSlot slot = equipment.equipmentSlot;
            
            // Unequip current item in that slot
            if (equippedItems.ContainsKey(slot) && equippedItems[slot] != null)
            {
                UnequipItem(slot);
            }
            
            // Equip the new item
            equippedItems[slot] = equipment;
            
            // Remove from inventory if specified
            if (inventorySlot >= 0 && inventorySlot < inventorySlots.Count)
            {
                ItemStack stack = inventorySlots[inventorySlot];
                if (stack != null && stack.item == equipment)
                {
                    stack.RemoveItems(1);
                    if (stack.IsEmpty())
                    {
                        inventorySlots[inventorySlot] = null;
                    }
                }
            }
            
            // Apply stat bonuses
            ApplyEquipmentBonuses();
            
            OnItemEquipped?.Invoke(equipment, slot);
            OnInventoryChanged?.Invoke();
            
            Debug.Log($"Equipped {equipment.itemName} in {slot} slot");
            return true;
        }
        
        /// <summary>
        /// Unequip an item
        /// </summary>
        public bool UnequipItem(CaminoDeLaFe.Items.EquipmentSlot slot)
        {
            if (!equippedItems.ContainsKey(slot) || equippedItems[slot] == null)
                return false;
            
            CaminoDeLaFe.Items.Equipment equipment = equippedItems[slot];
            
            // Try to add back to inventory
            if (!AddItem(equipment))
            {
                Debug.LogWarning("Cannot unequip item - inventory is full!");
                return false;
            }
            
            // Remove from equipped items
            equippedItems[slot] = null;
            
            // Recalculate stat bonuses
            ApplyEquipmentBonuses();
            
            OnItemUnequipped?.Invoke(equipment, slot);
            OnInventoryChanged?.Invoke();
            
            Debug.Log($"Unequipped {equipment.itemName} from {slot} slot");
            return true;
        }
        
        /// <summary>
        /// Get equipped item in a specific slot
        /// </summary>
        public CaminoDeLaFe.Items.Equipment GetEquippedItem(CaminoDeLaFe.Items.EquipmentSlot slot)
        {
            equippedItems.TryGetValue(slot, out CaminoDeLaFe.Items.Equipment equipment);
            return equipment;
        }
        
        /// <summary>
        /// Apply stat bonuses from all equipped items
        /// </summary>
        private void ApplyEquipmentBonuses()
        {
            // Calculate total bonuses from all equipped items
            Dictionary<string, int> totalBonuses = new Dictionary<string, int>();
            
            foreach (var kvp in equippedItems)
            {
                CaminoDeLaFe.Items.Equipment equipment = kvp.Value;
                if (equipment != null)
                {
                    foreach (var statBonus in equipment.statBonuses)
                    {
                        if (totalBonuses.ContainsKey(statBonus.Key))
                            totalBonuses[statBonus.Key] += statBonus.Value;
                        else
                            totalBonuses[statBonus.Key] = statBonus.Value;
                    }
                }
            }
            
            // Update player stats
            owner.stats.UpdateItemBonuses(totalBonuses);
        }
        
        /// <summary>
        /// Calculate total power rating from equipped items
        /// </summary>
        public float GetTotalEquipmentPower()
        {
            float totalPower = 0f;
            foreach (var kvp in equippedItems)
            {
                if (kvp.Value != null)
                    totalPower += kvp.Value.powerRating;
            }
            return totalPower;
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Get inventory usage percentage
        /// </summary>
        public float GetInventoryUsagePercent()
        {
            int usedSlots = inventorySlots.Count(slot => slot != null);
            return (float)usedSlots / maxSlots;
        }
        
        /// <summary>
        /// Sort inventory by item type and rarity
        /// </summary>
        public void SortInventory()
        {
            List<ItemStack> nonEmptyStacks = inventorySlots.Where(stack => stack != null).ToList();
            
            // Sort by item type, then by rarity, then by name
            nonEmptyStacks.Sort((a, b) => 
            {
                int typeComparison = a.item.GetType().Name.CompareTo(b.item.GetType().Name);
                if (typeComparison != 0) return typeComparison;
                
                int rarityComparison = a.item.rarity.CompareTo(b.item.rarity);
                if (rarityComparison != 0) return rarityComparison;
                
                return a.item.itemName.CompareTo(b.item.itemName);
            });
            
            // Clear inventory and re-add sorted items
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i] = null;
            }
            
            for (int i = 0; i < nonEmptyStacks.Count && i < maxSlots; i++)
            {
                inventorySlots[i] = nonEmptyStacks[i];
            }
            
            OnInventoryChanged?.Invoke();
        }
        
        /// <summary>
        /// Get inventory summary for saving
        /// </summary>
        public InventorySaveData GetSaveData()
        {
            return new InventorySaveData
            {
                inventorySlots = this.inventorySlots,
                equippedItems = this.equippedItems
            };
        }
        
        /// <summary>
        /// Load inventory from save data
        /// </summary>
        public void LoadSaveData(InventorySaveData saveData)
        {
            if (saveData != null)
            {
                this.inventorySlots = saveData.inventorySlots ?? new List<ItemStack>();
                this.equippedItems = saveData.equippedItems ?? new Dictionary<CaminoDeLaFe.Items.EquipmentSlot, CaminoDeLaFe.Items.Equipment>();
                
                // Ensure correct size
                while (inventorySlots.Count < maxSlots)
                {
                    inventorySlots.Add(null);
                }
                
                ApplyEquipmentBonuses();
                OnInventoryChanged?.Invoke();
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// Save data structure for inventory
    /// </summary>
    [System.Serializable]
    public class InventorySaveData
    {
        public List<ItemStack> inventorySlots;
        public Dictionary<CaminoDeLaFe.Items.EquipmentSlot, CaminoDeLaFe.Items.Equipment> equippedItems;
    }
}
