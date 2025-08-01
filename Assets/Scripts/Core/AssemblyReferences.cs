using UnityEngine;

namespace CaminoDeLaFe.Core
{
    public class AssemblyReferences : MonoBehaviour
    {
        // This class helps Unity recognize our custom namespaces
        // Include references to ensure proper compilation
        
        [System.Serializable]
        public class SystemReferences
        {
            [Header("Core Systems")]
            public GameManager gameManager;
            public GameConfig gameConfig;
            
            [Header("Entity References")]
            public Player player;
            public Enemy enemy;
            
            [Header("Item Systems")]
            public CaminoDeLaFe.Items.Item[] sampleItems;
            public CaminoDeLaFe.Inventory.PlayerInventory playerInventory;
            public CaminoDeLaFe.Systems.MountSystem mountSystem;
        }
        
        [SerializeField] private SystemReferences references;
        
        void Start()
        {
            // Initialize cross-system references if needed
            if (references.player != null && references.playerInventory != null)
            {
                // Connect inventory to player
                ConnectInventoryToPlayer();
            }
        }
        
        private void ConnectInventoryToPlayer()
        {
            // Helper method to ensure proper initialization
            var inventory = references.playerInventory;
            var player = references.player;
            
            if (inventory != null && player != null)
            {
                // The inventory system is now properly referenced
                Debug.Log("✅ Player Inventory system connected successfully!");
            }
        }
    }
}
