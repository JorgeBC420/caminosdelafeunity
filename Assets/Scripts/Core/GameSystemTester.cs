using UnityEngine;
using CaminoDeLaFe.Data;
using CaminoDeLaFe.Items;
using CaminoDeLaFe.Inventory;
using CaminoDeLaFe.Systems;

namespace CaminoDeLaFe.Core
{
    /// <summary>
    /// Sistema de testing y debug para verificar que todos los sistemas funcionan correctamente
    /// </summary>
    public class GameSystemTester : MonoBehaviour
    {
        [Header("Testing Configuration")]
        [SerializeField] private bool runTestsOnStart = true;
        [SerializeField] private bool enableDebugLog = true;
        [SerializeField] private KeyCode testAllSystemsKey = KeyCode.F12;
        [SerializeField] private KeyCode giveTestItemsKey = KeyCode.F11;
        [SerializeField] private KeyCode testMountSystemKey = KeyCode.F10;
        
        [Header("System References")]
        [SerializeField] private Player player;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private MountSystem mountSystem;
        
        private void Start()
        {
            if (runTestsOnStart)
            {
                Invoke(nameof(RunAllTests), 1f); // Wait 1 second for systems to initialize
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(testAllSystemsKey))
            {
                RunAllTests();
            }
            
            if (Input.GetKeyDown(giveTestItemsKey))
            {
                GiveTestItems();
            }
            
            if (Input.GetKeyDown(testMountSystemKey))
            {
                TestMountSystem();
            }
        }
        
        [ContextMenu("Run All Tests")]
        public void RunAllTests()
        {
            LogDebug("🧪 Starting Caminos de la Fe - System Tests");
            
            TestFactionSystem();
            TestPlayerStats();
            TestItemSystem();
            TestInventorySystem();
            TestMountSystem();
            TestCombatSystem();
            
            LogDebug("✅ All system tests completed!");
        }
        
        private void TestFactionSystem()
        {
            LogDebug("Testing Faction System...");
            
            // Test all factions
            foreach (Faction faction in System.Enum.GetValues(typeof(Faction)))
            {
                if (faction != Faction.None)
                {
                    Color factionColor = faction.GetColor();
                    LogDebug($"  ✅ {faction}: Color = {factionColor}");
                }
            }
        }
        
        private void TestPlayerStats()
        {
            if (player == null)
            {
                LogDebug("  ❌ Player reference not set!");
                return;
            }
            
            LogDebug("Testing Player Stats System...");
            PlayerStats stats = player.Stats;
            
            LogDebug($"  ✅ Health: {stats.CurrentHealth}/{stats.MaxHealth}");
            LogDebug($"  ✅ Mana: {stats.CurrentMana}/{stats.MaxMana}");
            LogDebug($"  ✅ Level: {stats.Level} (XP: {stats.Experience}/{stats.ExperienceToNextLevel})");
            LogDebug($"  ✅ Attack Power: {stats.AttackPower}");
            LogDebug($"  ✅ Defense: {stats.Defense}");
        }
        
        private void TestItemSystem()
        {
            LogDebug("Testing Item System...");
            
            // Test creating different item types
            try
            {
                // Test Weapon
                var sword = new Weapon("Espada del Cruzado", "Una espada forjada para la guerra santa",
                    ItemRarity.Common, 100, 50, 10, 25, Faction.Cruzados);
                LogDebug($"  ✅ Weapon created: {sword.Name} (Power: {sword.AttackPower})");
                
                // Test Consumable
                var potion = new HealthPotion();
                LogDebug($"  ✅ Consumable created: {potion.Name} (Healing: {potion.HealingAmount})");
                
                // Test Legendary Item
                var legendary = LegendaryItem.CreateExcalibur();
                LogDebug($"  ✅ Legendary created: {legendary.Name} (Owner: {legendary.CurrentOwner})");
            }
            catch (System.Exception e)
            {
                LogDebug($"  ❌ Item system error: {e.Message}");
            }
        }
        
        private void TestInventorySystem()
        {
            if (playerInventory == null)
            {
                LogDebug("  ⚠️ PlayerInventory reference not set - creating temporary one");
                playerInventory = gameObject.AddComponent<PlayerInventory>();
            }
            
            LogDebug("Testing Inventory System...");
            
            try
            {
                // Test adding items
                var sword = new Weapon("Test Sword", "Test weapon", ItemRarity.Common, 
                    100, 50, 10, 25, Faction.Cruzados);
                
                bool added = playerInventory.AddItem(sword, 1);
                LogDebug($"  ✅ Add item result: {added}");
                
                LogDebug($"  ✅ Inventory slots used: {playerInventory.GetUsedSlots()}/{GameConfig.InventoryMaxSlots}");
            }
            catch (System.Exception e)
            {
                LogDebug($"  ❌ Inventory system error: {e.Message}");
            }
        }
        
        private void TestMountSystem()
        {
            if (mountSystem == null)
            {
                LogDebug("  ⚠️ MountSystem reference not set - creating temporary one");
                mountSystem = gameObject.AddComponent<MountSystem>();
            }
            
            LogDebug("Testing Mount System...");
            
            try
            {
                // Test mount creation
                var warhorse = MountDatabase.GetAvailableMounts(Faction.Cruzados)[0];
                LogDebug($"  ✅ Mount loaded: {warhorse.Name} (Speed: +{warhorse.SpeedBonus})");
                
                // Test mount equipping
                mountSystem.EquipMount(warhorse);
                bool hasMountEquipped = mountSystem.HasMountEquipped();
                LogDebug($"  ✅ Mount equipped: {hasMountEquipped}");
                
                if (hasMountEquipped)
                {
                    var currentMount = mountSystem.GetCurrentMount();
                    LogDebug($"  ✅ Current mount: {currentMount.Name}");
                }
            }
            catch (System.Exception e)
            {
                LogDebug($"  ❌ Mount system error: {e.Message}");
            }
        }
        
        private void TestCombatSystem()
        {
            if (player == null)
            {
                LogDebug("  ❌ Cannot test combat - Player reference not set!");
                return;
            }
            
            LogDebug("Testing Combat System...");
            
            try
            {
                // Test damage calculation
                int baseDamage = player.Stats.AttackPower;
                float damageWithModifiers = player.CalculateDamageOutput(baseDamage);
                
                LogDebug($"  ✅ Base damage: {baseDamage}");
                LogDebug($"  ✅ Modified damage: {damageWithModifiers}");
                
                // Test faction bonus calculation
                float factionBonus = player.GetFactionDamageBonus();
                LogDebug($"  ✅ Faction damage bonus: +{factionBonus * 100}%");
            }
            catch (System.Exception e)
            {
                LogDebug($"  ❌ Combat system error: {e.Message}");
            }
        }
        
        [ContextMenu("Give Test Items")]
        public void GiveTestItems()
        {
            if (playerInventory == null)
            {
                LogDebug("❌ Cannot give items - PlayerInventory not found!");
                return;
            }
            
            LogDebug("🎁 Giving test items to player...");
            
            try
            {
                // Give some basic items
                var sword = new Weapon("Espada de Prueba", "Espada para testing", 
                    ItemRarity.Uncommon, 150, 75, 15, 35, player.GetFaction());
                var armor = new Armor("Armadura de Prueba", "Armadura para testing",
                    ItemRarity.Common, 100, 50, 0, 20, player.GetFaction());
                var potion = new HealthPotion();
                
                playerInventory.AddItem(sword, 1);
                playerInventory.AddItem(armor, 1);
                playerInventory.AddItem(potion, 5);
                
                LogDebug("✅ Test items added to inventory!");
            }
            catch (System.Exception e)
            {
                LogDebug($"❌ Error giving test items: {e.Message}");
            }
        }
        
        private void LogDebug(string message)
        {
            if (enableDebugLog)
            {
                Debug.Log($"[GameSystemTester] {message}");
            }
        }
        
        private void OnGUI()
        {
            if (!enableDebugLog) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("🧪 Caminos de la Fe - Debug Tools", GUI.skin.box);
            
            if (GUILayout.Button($"Run All Tests ({testAllSystemsKey})"))
            {
                RunAllTests();
            }
            
            if (GUILayout.Button($"Give Test Items ({giveTestItemsKey})"))
            {
                GiveTestItems();
            }
            
            if (GUILayout.Button($"Test Mount System ({testMountSystemKey})"))
            {
                TestMountSystem();
            }
            
            GUILayout.Space(10);
            
            if (player != null)
            {
                GUILayout.Label($"Level: {player.Stats.Level}");
                GUILayout.Label($"Health: {player.Stats.CurrentHealth}/{player.Stats.MaxHealth}");
                GUILayout.Label($"Gold: {player.Stats.Gold}");
            }
            
            GUILayout.EndArea();
        }
    }
}
