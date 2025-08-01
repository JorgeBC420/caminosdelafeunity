using UnityEngine;
using System.Collections.Generic;

namespace CaminoDeLaFe.Systems
{
    /// <summary>
    /// Mount/Horse system for player transportation and combat bonuses
    /// </summary>
    [System.Serializable]
    public class Mount
    {
        [Header("Basic Info")]
        public string mountName;
        public string description;
        public MountType mountType;
        public MountRarity rarity;
        public Sprite icon;
        
        [Header("Stats")]
        public Dictionary<string, int> statBonuses;
        public float speedBonus;
        public float jumpPower;
        public int carryCapacity; // Extra inventory slots
        public float stamina;
        public float maxStamina;
        
        [Header("Combat")]
        public float trampleDamage; // Damage when charging through enemies
        public float mountedAttackBonus; // Attack damage bonus when mounted
        public bool canFightMounted; // Can attack while mounted
        
        [Header("Visual")]
        public GameObject mountPrefab; // 3D model
        public AudioClip[] neighSounds;
        public AudioClip gallopSound;
        
        [Header("Requirements")]
        public int levelRequirement = 1;
        public string[] factionRequirements;
        public int goldCost = 1000;
        
        public Mount(string name, MountType type, MountRarity rarity = MountRarity.Common)
        {
            this.mountName = name;
            this.mountType = type;
            this.rarity = rarity;
            this.statBonuses = new Dictionary<string, int>();
            this.factionRequirements = new string[0];
            this.maxStamina = 100f;
            this.stamina = maxStamina;
            
            InitializeBaseStats();
        }
        
        private void InitializeBaseStats()
        {
            // Base stats based on mount type
            switch (mountType)
            {
                case MountType.WarHorse:
                    speedBonus = 8f;
                    trampleDamage = 25f;
                    mountedAttackBonus = 1.5f;
                    canFightMounted = true;
                    AddStatBonus("fuerza", 3);
                    AddStatBonus("defensa", 2);
                    break;
                    
                case MountType.FastHorse:
                    speedBonus = 12f;
                    trampleDamage = 15f;
                    mountedAttackBonus = 1.2f;
                    canFightMounted = true;
                    AddStatBonus("velocidad", 5);
                    AddStatBonus("agilidad", 3);
                    break;
                    
                case MountType.HeavyHorse:
                    speedBonus = 6f;
                    trampleDamage = 35f;
                    mountedAttackBonus = 1.8f;
                    canFightMounted = true;
                    carryCapacity = 10;
                    AddStatBonus("fuerza", 4);
                    AddStatBonus("defensa", 4);
                    AddStatBonus("resistencia", 2);
                    break;
                    
                case MountType.ScoutHorse:
                    speedBonus = 10f;
                    trampleDamage = 10f;
                    mountedAttackBonus = 1.1f;
                    canFightMounted = false;
                    AddStatBonus("velocidad", 4);
                    AddStatBonus("agilidad", 4);
                    AddStatBonus("tecnica", 2);
                    break;
                    
                case MountType.Camel:
                    speedBonus = 7f;
                    trampleDamage = 20f;
                    mountedAttackBonus = 1.3f;
                    canFightMounted = true;
                    AddStatBonus("resistencia", 5);
                    AddStatBonus("inteligencia", 2);
                    break;
                    
                case MountType.Donkey:
                    speedBonus = 4f;
                    trampleDamage = 5f;
                    mountedAttackBonus = 1.0f;
                    canFightMounted = false;
                    carryCapacity = 15;
                    AddStatBonus("resistencia", 3);
                    break;
            }
            
            // Apply rarity multipliers
            ApplyRarityBonuses();
        }
        
        private void ApplyRarityBonuses()
        {
            float multiplier = GetRarityMultiplier();
            
            speedBonus *= multiplier;
            trampleDamage *= multiplier;
            mountedAttackBonus *= multiplier;
            maxStamina *= multiplier;
            stamina = maxStamina;
            
            // Increase stat bonuses based on rarity
            var bonusKeys = new List<string>(statBonuses.Keys);
            foreach (string stat in bonusKeys)
            {
                statBonuses[stat] = Mathf.RoundToInt(statBonuses[stat] * multiplier);
            }
        }
        
        private float GetRarityMultiplier()
        {
            switch (rarity)
            {
                case MountRarity.Common: return 1.0f;
                case MountRarity.Uncommon: return 1.2f;
                case MountRarity.Rare: return 1.5f;
                case MountRarity.Epic: return 2.0f;
                case MountRarity.Legendary: return 3.0f;
                default: return 1.0f;
            }
        }
        
        public void AddStatBonus(string statName, int bonus)
        {
            if (statBonuses.ContainsKey(statName))
                statBonuses[statName] += bonus;
            else
                statBonuses[statName] = bonus;
        }
        
        public bool CanBeUsedBy(CaminoDeLaFe.Entities.Player player)
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
        
        public void UseStamina(float amount)
        {
            stamina = Mathf.Max(0, stamina - amount);
        }
        
        public void RestoreStamina(float amount)
        {
            stamina = Mathf.Min(maxStamina, stamina + amount);
        }
        
        public float GetStaminaPercent()
        {
            return stamina / maxStamina;
        }
        
        public bool IsExhausted()
        {
            return stamina <= 0;
        }
        
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case MountRarity.Common: return Color.white;
                case MountRarity.Uncommon: return Color.green;
                case MountRarity.Rare: return Color.blue;
                case MountRarity.Epic: return Color.magenta;
                case MountRarity.Legendary: return Color.yellow;
                default: return Color.white;
            }
        }
    }
    
    /// <summary>
    /// Mount management system for the player
    /// </summary>
    public class MountSystem : MonoBehaviour
    {
        [Header("Mount Settings")]
        public Mount currentMount;
        public List<Mount> ownedMounts;
        public bool isMounted = false;
        public float mountAnimationTime = 2f;
        
        [Header("Visual")]
        public Transform mountPoint; // Where the mount appears
        public GameObject currentMountObject;
        
        private CaminoDeLaFe.Entities.Player player;
        private float originalMoveSpeed;
        private bool wasMovingWhenMounted = false;
        
        // Events
        public System.Action<Mount> OnMountEquipped;
        public System.Action<Mount> OnMountUnequipped;
        public System.Action<bool> OnMountStateChanged;
        
        void Awake()
        {
            player = GetComponent<CaminoDeLaFe.Entities.Player>();
            ownedMounts = new List<Mount>();
            
            if (player != null)
                originalMoveSpeed = player.moveSpeed;
        }
        
        void Start()
        {
            // Give player a basic mount to start
            AddMount(MountDatabase.CreateBasicHorse());
        }
        
        void Update()
        {
            HandleMountInput();
            UpdateMountStamina();
        }
        
        private void HandleMountInput()
        {
            // Toggle mount with M key
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (isMounted)
                    Dismount();
                else if (currentMount != null)
                    Mount();
            }
            
            // Mount abilities
            if (isMounted && currentMount != null)
            {
                // Gallop with Shift (uses stamina)
                if (Input.GetKey(KeyCode.LeftShift) && !currentMount.IsExhausted())
                {
                    Gallop();
                }
                
                // Mount attack with Space (if mount supports it)
                if (Input.GetKeyDown(KeyCode.Space) && currentMount.canFightMounted)
                {
                    MountAttack();
                }
            }
        }
        
        private void UpdateMountStamina()
        {
            if (currentMount != null)
            {
                // Regenerate stamina when not galloping
                if (!Input.GetKey(KeyCode.LeftShift) || !isMounted)
                {
                    currentMount.RestoreStamina(10f * Time.deltaTime);
                }
            }
        }
        
        public void Mount()
        {
            if (currentMount == null || isMounted || !currentMount.CanBeUsedBy(player))
                return;
                
            StartCoroutine(MountAnimation());
        }
        
        public void Dismount()
        {
            if (!isMounted)
                return;
                
            StartCoroutine(DismountAnimation());
        }
        
        private System.Collections.IEnumerator MountAnimation()
        {
            Debug.Log($"Mounting {currentMount.mountName}...");
            
            // Disable player movement during mounting
            player.enabled = false;
            
            // Play mounting animation (placeholder)
            yield return new WaitForSeconds(mountAnimationTime);
            
            // Apply mount effects
            isMounted = true;
            player.moveSpeed += currentMount.speedBonus;
            
            // Update player stats with mount bonuses
            player.stats.UpdateHorseBonuses(currentMount.statBonuses);
            
            // Spawn mount visual
            SpawnMountVisual();
            
            // Re-enable player movement
            player.enabled = true;
            
            OnMountEquipped?.Invoke(currentMount);
            OnMountStateChanged?.Invoke(true);
            
            Debug.Log($"Mounted on {currentMount.mountName}! Speed bonus: +{currentMount.speedBonus}");
        }
        
        private System.Collections.IEnumerator DismountAnimation()
        {
            Debug.Log($"Dismounting from {currentMount.mountName}...");
            
            // Disable player movement during dismounting
            player.enabled = false;
            
            // Play dismounting animation (placeholder)
            yield return new WaitForSeconds(mountAnimationTime * 0.5f);
            
            // Remove mount effects
            isMounted = false;
            player.moveSpeed -= currentMount.speedBonus;
            
            // Remove mount stat bonuses
            player.stats.UpdateHorseBonuses(new Dictionary<string, int>());
            
            // Remove mount visual
            DestroyMountVisual();
            
            // Re-enable player movement
            player.enabled = true;
            
            OnMountUnequipped?.Invoke(currentMount);
            OnMountStateChanged?.Invoke(false);
            
            Debug.Log($"Dismounted from {currentMount.mountName}");
        }
        
        private void Gallop()
        {
            if (!isMounted || currentMount.IsExhausted())
                return;
                
            // Use stamina for galloping
            currentMount.UseStamina(20f * Time.deltaTime);
            
            // Temporary extra speed boost
            player.moveSpeed = originalMoveSpeed + currentMount.speedBonus * 1.5f;
            
            // Play gallop sound
            if (currentMount.gallopSound != null)
            {
                // Would play gallop sound here
            }
        }
        
        private void MountAttack()
        {
            if (!isMounted || !currentMount.canFightMounted)
                return;
                
            Debug.Log($"Mount attack! Trample damage: {currentMount.trampleDamage}");
            
            // Check for enemies in trample range
            Collider[] enemies = Physics.OverlapSphere(transform.position, 3f);
            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    var enemyComponent = enemy.GetComponent<CaminoDeLaFe.Entities.Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.TakeDamage(currentMount.trampleDamage);
                        Debug.Log($"Trampled enemy for {currentMount.trampleDamage} damage!");
                    }
                }
            }
        }
        
        private void SpawnMountVisual()
        {
            if (currentMount.mountPrefab != null && mountPoint != null)
            {
                currentMountObject = Instantiate(currentMount.mountPrefab, mountPoint.position, mountPoint.rotation);
                currentMountObject.transform.SetParent(transform);
            }
        }
        
        private void DestroyMountVisual()
        {
            if (currentMountObject != null)
            {
                Destroy(currentMountObject);
                currentMountObject = null;
            }
        }
        
        public void AddMount(Mount mount)
        {
            if (mount != null && !ownedMounts.Contains(mount))
            {
                ownedMounts.Add(mount);
                Debug.Log($"Added mount: {mount.mountName}");
                
                // Equip first mount automatically
                if (currentMount == null)
                {
                    EquipMount(mount);
                }
            }
        }
        
        public void EquipMount(Mount mount)
        {
            if (mount != null && ownedMounts.Contains(mount) && mount.CanBeUsedBy(player))
            {
                // Dismount current mount if mounted
                if (isMounted)
                    Dismount();
                    
                currentMount = mount;
                Debug.Log($"Equipped mount: {mount.mountName}");
            }
        }
        
        public List<Mount> GetAvailableMounts()
        {
            return ownedMounts.FindAll(mount => mount.CanBeUsedBy(player));
        }
    }
    
    /// <summary>
    /// Mount types
    /// </summary>
    public enum MountType
    {
        WarHorse,    // Caballo de guerra (balanced)
        FastHorse,   // Caballo rápido (speed focused)
        HeavyHorse,  // Caballo pesado (strength/defense)
        ScoutHorse,  // Caballo explorador (stealth/speed)
        Camel,       // Camello (desert factions)
        Donkey       // Burro (basic transport)
    }
    
    /// <summary>
    /// Mount rarity levels
    /// </summary>
    public enum MountRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    
    /// <summary>
    /// Database of predefined mounts
    /// </summary>
    public static class MountDatabase
    {
        public static Mount CreateBasicHorse()
        {
            var mount = new Mount("Caballo Común", MountType.WarHorse, MountRarity.Common);
            mount.description = "Un caballo básico de guerra, confiable y resistente.";
            mount.levelRequirement = 1;
            mount.goldCost = 500;
            return mount;
        }
        
        public static Mount CreateCrusaderDestrier()
        {
            var mount = new Mount("Destrero Cruzado", MountType.HeavyHorse, MountRarity.Rare);
            mount.description = "Un poderoso caballo de guerra de los Cruzados, entrenado para la batalla.";
            mount.factionRequirements = new string[] { "Cruzados" };
            mount.levelRequirement = 10;
            mount.goldCost = 2000;
            return mount;
        }
        
        public static Mount CreateSaracenArabian()
        {
            var mount = new Mount("Árabe Sarraceno", MountType.FastHorse, MountRarity.Rare);
            mount.description = "Un elegante caballo árabe, rápido como el viento del desierto.";
            mount.factionRequirements = new string[] { "Sarracenos" };
            mount.levelRequirement = 10;
            mount.goldCost = 2000;
            return mount;
        }
        
        public static Mount CreateAncientCamel()
        {
            var mount = new Mount("Camello Ancestral", MountType.Camel, MountRarity.Epic);
            mount.description = "Un camello místico de los Antiguos, adaptado a todos los terrenos.";
            mount.factionRequirements = new string[] { "Antiguos" };
            mount.levelRequirement = 15;
            mount.goldCost = 5000;
            mount.AddStatBonus("inteligencia", 3);
            mount.AddStatBonus("resistencia", 2);
            return mount;
        }
        
        public static Mount CreateLegendaryMount(string factionName)
        {
            switch (factionName)
            {
                case "Cruzados":
                    var crusaderMount = new Mount("Bayard el Legendario", MountType.WarHorse, MountRarity.Legendary);
                    crusaderMount.description = "El legendario corcel que llevó a los héroes cruzados a la victoria.";
                    crusaderMount.factionRequirements = new string[] { "Cruzados" };
                    crusaderMount.levelRequirement = 25;
                    crusaderMount.goldCost = 10000;
                    return crusaderMount;
                    
                case "Sarracenos":
                    var saracenMount = new Mount("Al-Buraq el Veloz", MountType.FastHorse, MountRarity.Legendary);
                    saracenMount.description = "El mítico corcel que cruza el desierto en un suspiro.";
                    saracenMount.factionRequirements = new string[] { "Sarracenos" };
                    saracenMount.levelRequirement = 25;
                    saracenMount.goldCost = 10000;
                    return saracenMount;
                    
                case "Antiguos":
                    var ancientMount = new Mount("Pegaso Ancestral", MountType.ScoutHorse, MountRarity.Legendary);
                    ancientMount.description = "Un ser alado de los tiempos antiguos, portador de sabiduría eterna.";
                    ancientMount.factionRequirements = new string[] { "Antiguos" };
                    ancientMount.levelRequirement = 25;
                    ancientMount.goldCost = 10000;
                    ancientMount.AddStatBonus("inteligencia", 5);
                    ancientMount.AddStatBonus("agilidad", 3);
                    return ancientMount;
                    
                default:
                    return CreateBasicHorse();
            }
        }
    }
}
