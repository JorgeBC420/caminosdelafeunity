using UnityEngine;
using CaminosDeLaFe.Data;
using CaminosDeLaFe.Systems;
// using CaminosDeLaFe.Monetization; // TODO: Fix assembly references
using System;
using System.Collections;

namespace CaminosDeLaFe.Entities
{
    /// <summary>
    /// Main player controller and data management
    /// Integrated with Games Lab monetization systems
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Header("Player Data")]
        public string playerName = "Player";
        public string factionName = "Cruzados";
        public int gold = 100;
        public int experience = 0;
        public int level = 1;
        
        [Header("Combat")]
        public int currentHealth;
        public int currentMana;
        public float attackDamage = 15f;
        public float attackRange = 2.5f;
        public bool canAttack = true;
        
        [Header("Movement")]
        public float moveSpeed = 5f;
        public CharacterController characterController;
        
        [Header("References")]
        public Transform cameraTarget;
        
        [Header("Monetization Integration")]
        // TODO: Uncomment when assembly references are fixed
        // [SerializeField] private MonetizationManager monetizationManager;
        // [SerializeField] private DailyLimitsSystem dailyLimitsSystem;
        // [SerializeField] private FaithPassSystem faithPassSystem;
        
        // Core Systems
        public PlayerStats stats { get; private set; }
        public Faction faction { get; private set; }
        
        // TODO: Inventory System (to be added after Unity assembly references are fixed)
        // private CaminosDeLaFe.Inventory.PlayerInventory _inventory;
        
        // Events
        public event Action<int> OnHealthChanged;
        public event Action<int> OnManaChanged;
        public event Action<int> OnGoldChanged;
        public event Action<int> OnExperienceChanged;
        public event Action<int> OnLevelChanged;
        public event Action<string> OnFactionChanged;
        
        // Auto Combat System
        public AutoCombatSystem autoCombat { get; private set; }
        
        // Faction War
        public int factionWarContribution = 0;
        public DateTime lastContributionDate;
        public string authToken = "example_token";
        
        // Input
        private Vector3 moveDirection;
        private bool isMoving;
        
        void Awake()
        {
            // Get or add CharacterController
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
            if (characterController == null)
                characterController = gameObject.AddComponent<CharacterController>();
            
            // Initialize stats system
            InitializeStats();
            
            // Set faction
            SetFaction(factionName);
            
            // Initialize auto combat
            autoCombat = new AutoCombatSystem(this);
        }
        
        void Start()
        {
            // Set initial health and mana
            currentHealth = stats.maxHealth;
            currentMana = stats.maxMana;
            
            // Setup camera if target is set
            if (cameraTarget != null)
            {
                Camera.main.transform.SetParent(cameraTarget);
                Camera.main.transform.localPosition = new Vector3(0, GameConfig.CAMERA_HEIGHT, -GameConfig.CAMERA_DISTANCE);
                Camera.main.transform.localRotation = Quaternion.Euler(GameConfig.CAMERA_ANGLE, 0, 0);
            }
        }
        
        void Update()
        {
            HandleInput();
            HandleMovement();
        }
        
        private void InitializeStats()
        {
            var baseStats = new System.Collections.Generic.Dictionary<string, int>
            {
                { "nivel", level },
                { "fuerza", 12 },
                { "tecnica", 10 },
                { "destreza", 10 },
                { "defensa", 8 },
                { "resistencia", 10 },
                { "velocidad", 10 },
                { "agilidad", 10 },
                { "inteligencia", 5 }
            };
            
            var itemBonuses = new System.Collections.Generic.Dictionary<string, int>
            {
                { "fuerza", 2 },
                { "defensa", 1 }
            };
            
            var horseBonuses = new System.Collections.Generic.Dictionary<string, int>
            {
                { "fuerza", 1 },
                { "velocidad", 3 }
            };
            
            stats = new PlayerStats(baseStats, itemBonuses, horseBonuses);
            
            // Subscribe to stat change events
            stats.OnHealthChanged += (newMaxHealth) => {
                // Adjust current health proportionally
                float healthRatio = currentHealth / (float)stats.maxHealth;
                currentHealth = Mathf.RoundToInt(newMaxHealth * healthRatio);
                OnHealthChanged?.Invoke(currentHealth);
            };
            
            stats.OnManaChanged += (newMaxMana) => {
                // Adjust current mana proportionally
                float manaRatio = currentMana / (float)stats.maxMana;
                currentMana = Mathf.RoundToInt(newMaxMana * manaRatio);
                OnManaChanged?.Invoke(currentMana);
            };
        }
        
        private void HandleInput()
        {
            // Get input from WASD or arrow keys
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            isMoving = moveDirection.magnitude > 0.1f;
            
            // Attack input
            if (Input.GetMouseButtonDown(0) && canAttack)
            {
                Attack();
            }
            
            // Stats UI toggle
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleStatsUI();
            }
        }
        
        private void HandleMovement()
        {
            if (isMoving)
            {
                // Move the character
                Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
                characterController.Move(move);
                
                // Rotate to face movement direction
                if (moveDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, 
                        Quaternion.LookRotation(moveDirection), 
                        Time.deltaTime * 10f);
                }
            }
        }
        
        public void SetFaction(string newFactionName)
        {
            faction = Factions.GetFaction(newFactionName);
            if (faction != null)
            {
                factionName = newFactionName;
                // Update visual appearance
                var renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = faction.color;
                }
                OnFactionChanged?.Invoke(factionName);
            }
        }
        
        public void Attack()
        {
            if (!canAttack) return;
            
            canAttack = false;
            StartCoroutine(AttackCooldown());
            
            // Simple attack animation (move forward and back)
            StartCoroutine(AttackAnimation());
            
            // Check for enemies in range
            Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    var enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        float damage = attackDamage + stats.GetTotalStat("fuerza") * 0.5f;
                        enemyComponent.TakeDamage(damage);
                        Debug.Log($"Attacked enemy for {damage} damage!");
                    }
                }
            }
        }
        
        private IEnumerator AttackCooldown()
        {
            yield return new WaitForSeconds(GameConfig.PLAYER_ATTACK_COOLDOWN);
            canAttack = true;
        }
        
        private IEnumerator AttackAnimation()
        {
            Vector3 originalPosition = transform.position;
            Vector3 attackPosition = originalPosition + transform.forward;
            
            // Move forward
            float elapsed = 0;
            while (elapsed < 0.1f)
            {
                transform.position = Vector3.Lerp(originalPosition, attackPosition, elapsed / 0.1f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Move back
            elapsed = 0;
            while (elapsed < 0.1f)
            {
                transform.position = Vector3.Lerp(attackPosition, originalPosition, elapsed / 0.1f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.position = originalPosition;
        }
        
        public void TakeDamage(float damage)
        {
            // Calculate damage reduction based on defense
            float defense = stats.GetTotalStat("defensa");
            float reduction = defense / (defense + 100f); // Diminishing returns formula
            float actualDamage = damage * (1f - reduction);
            
            currentHealth = Mathf.Max(0, currentHealth - Mathf.RoundToInt(actualDamage));
            OnHealthChanged?.Invoke(currentHealth);
            
            Debug.Log($"Player took {actualDamage} damage. Health: {currentHealth}/{stats.maxHealth}");
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            Debug.Log("Player died!");
            // Handle player death (respawn, game over, etc.)
        }
        
        public bool SpendGold(int amount)
        {
            if (gold >= amount)
            {
                gold -= amount;
                OnGoldChanged?.Invoke(gold);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Add gold to player, respecting daily limits from Games Lab monetization
        /// TODO: Uncomment when assembly references are fixed
        /// </summary>
        public void AddGold(int amount)
        {
            // 🎯 GAMES LAB INTEGRATION: Check daily limits
            int actualAmount = amount;
            
            /* TODO: Uncomment when assembly references are fixed
            // Try to find daily limits system
            var limitsSystem = FindFirstObjectByType<CaminosDeLaFe.Monetization.DailyLimitsSystem>();
            if (limitsSystem != null)
            {
                // This uses the daily limits system we just created
                bool canEarn = limitsSystem.TryEarnGold(amount, out actualAmount);
                
                if (!canEarn && amount > 0)
                {
                    Debug.Log($"💰 Daily gold limit reached! Only earned {actualAmount}/{amount} gold");
                    
                    // Show monetization options when limit is reached
                    var monetizationManager = FindFirstObjectByType<CaminosDeLaFe.Monetization.MonetizationManager>();
                    if (monetizationManager != null)
                    {
                        Debug.Log("🎯 Showing monetization options for exceeded gold limit...");
                    }
                }
            }
            
            // Apply Faith Pass gold bonus if active
            var faithPassSystem = FindFirstObjectByType<CaminosDeLaFe.Monetization.FaithPassSystem>();
            if (faithPassSystem != null && faithPassSystem.IsFaithPassActive())
            {
                actualAmount = Mathf.RoundToInt(actualAmount * faithPassSystem.GetGoldMultiplier());
                Debug.Log($"🎯 Faith Pass gold bonus applied: {amount} → {actualAmount}");
            }
            */
            
            gold += actualAmount;
            OnGoldChanged?.Invoke(gold);
            
            // Update stats system (gold is managed separately for now)
            // TODO: Integrate with PlayerStats when available
        }
        
        /// <summary>
        /// Add experience to player with Faith Pass bonuses
        /// TODO: Uncomment when assembly references are fixed
        /// </summary>
        public void AddExperience(int amount)
        {
            int actualAmount = amount;
            
            /* TODO: Uncomment when assembly references are fixed
            // Apply Faith Pass XP bonus if active
            var faithPassSystem = FindFirstObjectByType<CaminosDeLaFe.Monetization.FaithPassSystem>();
            if (faithPassSystem != null && faithPassSystem.IsFaithPassActive())
            {
                actualAmount = Mathf.RoundToInt(actualAmount * faithPassSystem.GetXPMultiplier());
                Debug.Log($"🎯 Faith Pass XP bonus applied: {amount} → {actualAmount}");
            }
            */
            
            experience += actualAmount;
            OnExperienceChanged?.Invoke(experience);
            
            /* TODO: Uncomment when assembly references are fixed
            // Also add XP to Faith Pass system
            if (faithPassSystem != null)
            {
                faithPassSystem.AddXP(actualAmount);
            }
            */
            
            // Check for level up
            int newLevel = CalculateLevel(experience);
            if (newLevel > level)
            {
                LevelUp(newLevel);
                
                /* TODO: Uncomment when assembly references are fixed
                // Update daily limits when leveling up
                var limitsSystem = FindFirstObjectByType<CaminosDeLaFe.Monetization.DailyLimitsSystem>();
                if (limitsSystem != null)
                {
                    limitsSystem.UpdateLimitsBasedOnLevel();
                }
                */
            }
        }
        
        private int CalculateLevel(int exp)
        {
            // Simple level calculation: every 100 XP = 1 level
            return 1 + (exp / 100);
        }
        
        private void LevelUp(int newLevel)
        {
            level = newLevel;
            stats.ImproveStat("nivel", newLevel - level);
            OnLevelChanged?.Invoke(level);
            
            Debug.Log($"Level up! New level: {level}");
        }
        
        private void ToggleStatsUI()
        {
            // This will be implemented when we create the UI system
            Debug.Log("Toggle Stats UI (to be implemented)");
        }
        
        // Faction War Methods
        public float CalculateWarPower()
        {
            float basePower = stats.CalculatePowerRating();
            float loyaltyBonus = 1 + (factionWarContribution * GameConfig.LOYALTY_BONUS_MULTIPLIER);
            return basePower * loyaltyBonus;
        }
        
        public bool CanContributeToWar()
        {
            return lastContributionDate.Date != DateTime.Now.Date;
        }
        
        // Debug methods
        void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
