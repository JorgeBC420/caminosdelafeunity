using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CaminoDeLaFe.Items
{
    /// <summary>
    /// Legendary weapons with unique abilities and faction-specific powers
    /// </summary>
    [System.Serializable]
    public class LegendaryItem : Equipment
    {
        [Header("Legendary Properties")]
        public bool isUnique = true; // Only one per server
        public string currentOwner = ""; // Player who owns this unique item
        public string lore; // Rich backstory
        public LegendaryType legendaryType;
        
        [Header("Unique Abilities")]
        public LegendaryAbility[] abilities;
        public float abilityPower = 100f;
        public float abilityCooldown = 30f;
        public bool requiresQuestToObtain = true;
        
        [Header("Purification System")]
        public bool canBePurified = true;
        public bool isPurified = false;
        public string[] purificationRequirements;
        public Dictionary<string, int> purifiedStatBonuses;
        
        [Header("Theft Protection")]
        public bool canBeStolen = true;
        public float stealDifficulty = 0.8f; // 0-1, higher = harder to steal
        public int stealProtectionLevel = 0; // Increases with upgrades
        
        public LegendaryItem(string name, string description, EquipmentSlot slot, LegendaryType type, string faction = "")
            : base(name, description, slot, ItemRarity.Legendary, 50000)
        {
            this.legendaryType = type;
            this.lore = description;
            this.abilities = new LegendaryAbility[0];
            this.purificationRequirements = new string[0];
            this.purifiedStatBonuses = new Dictionary<string, int>();
            
            if (!string.IsNullOrEmpty(faction))
            {
                this.factionRequirements = new string[] { faction };
            }
            
            InitializeLegendaryPowers();
        }
        
        private void InitializeLegendaryPowers()
        {
            // Base legendary stats are much higher
            foreach (var stat in statBonuses.Keys.ToArray())
            {
                statBonuses[stat] *= 3; // Triple the base stats
            }
            
            // Add special effects based on type
            switch (legendaryType)
            {
                case LegendaryType.Weapon:
                    AddWeaponAbilities();
                    break;
                case LegendaryType.Armor:
                    AddArmorAbilities();
                    break;
                case LegendaryType.Accessory:
                    AddAccessoryAbilities();
                    break;
                case LegendaryType.UniqueArtifact:
                    AddArtifactAbilities();
                    break;
            }
        }
        
        private void AddWeaponAbilities()
        {
            // Legendary weapons get powerful combat abilities
            var abilities = new List<LegendaryAbility>();
            
            if (factionRequirements.Length > 0)
            {
                string faction = factionRequirements[0];
                switch (faction)
                {
                    case "Cruzados":
                        abilities.Add(new LegendaryAbility("Juicio Divino", "Deals massive holy damage to enemies", 200f, 45f));
                        abilities.Add(new LegendaryAbility("Luz Purificadora", "Heals allies and damages undead", 150f, 30f));
                        break;
                        
                    case "Sarracenos":
                        abilities.Add(new LegendaryAbility("Tormenta de Arena", "Creates a sandstorm that blinds enemies", 180f, 40f));
                        abilities.Add(new LegendaryAbility("Viento del Desierto", "Increases movement and attack speed", 100f, 25f));
                        break;
                        
                    case "Antiguos":
                        abilities.Add(new LegendaryAbility("Olvido Eterno", "Drains enemy memories and mana", 220f, 50f));
                        abilities.Add(new LegendaryAbility("Sabiduría Ancestral", "Reveals enemy weaknesses", 80f, 20f));
                        break;
                }
            }
            
            this.abilities = abilities.ToArray();
        }
        
        private void AddArmorAbilities()
        {
            // Legendary armor provides defensive abilities
            var abilities = new List<LegendaryAbility>
            {
                new LegendaryAbility("Escudo Legendario", "Absorbs a percentage of incoming damage", 0f, 60f),
                new LegendaryAbility("Regeneración", "Slowly heals the wearer over time", 50f, 0f)
            };
            
            this.abilities = abilities.ToArray();
        }
        
        private void AddAccessoryAbilities()
        {
            // Legendary accessories provide utility abilities
            var abilities = new List<LegendaryAbility>
            {
                new LegendaryAbility("Teleportación", "Instantly move to a target location", 0f, 20f),
                new LegendaryAbility("Visión Verdadera", "See through illusions and detect hidden enemies", 0f, 45f)
            };
            
            this.abilities = abilities.ToArray();
        }
        
        private void AddArtifactAbilities()
        {
            // Unique artifacts have server-wide effects
            var abilities = new List<LegendaryAbility>
            {
                new LegendaryAbility("Bendición Global", "Affects all faction members on the server", 300f, 300f),
                new LegendaryAbility("Poder Ancestral", "Unlocks hidden areas or secrets", 0f, 0f)
            };
            
            this.abilities = abilities.ToArray();
        }
        
        /// <summary>
        /// Use a legendary ability
        /// </summary>
        public bool UseAbility(int abilityIndex, CaminoDeLaFe.Entities.Player user)
        {
            if (abilityIndex < 0 || abilityIndex >= abilities.Length)
                return false;
                
            LegendaryAbility ability = abilities[abilityIndex];
            
            // Check cooldown (would need a cooldown system to implement properly)
            Debug.Log($"Using legendary ability: {ability.name} - {ability.description}");
            Debug.Log($"Power: {ability.power}, Cooldown: {ability.cooldown}s");
            
            // Apply ability effects based on the specific ability
            ApplyAbilityEffect(ability, user);
            
            return true;
        }
        
        private void ApplyAbilityEffect(LegendaryAbility ability, CaminoDeLaFe.Entities.Player user)
        {
            switch (ability.name)
            {
                case "Juicio Divino":
                    // Massive damage to all enemies in range
                    Collider[] enemies = Physics.OverlapSphere(user.transform.position, 10f);
                    foreach (var enemy in enemies)
                    {
                        if (enemy.CompareTag("Enemy"))
                        {
                            var enemyComponent = enemy.GetComponent<CaminoDeLaFe.Entities.Enemy>();
                            if (enemyComponent != null)
                            {
                                enemyComponent.TakeDamage(ability.power);
                            }
                        }
                    }
                    break;
                    
                case "Luz Purificadora":
                    // Heal user
                    user.currentHealth = Mathf.Min(user.stats.maxHealth, user.currentHealth + (int)ability.power);
                    user.OnHealthChanged?.Invoke(user.currentHealth);
                    break;
                    
                case "Tormenta de Arena":
                    Debug.Log("Sandstorm created! Enemies are blinded and slowed.");
                    break;
                    
                case "Olvido Eterno":
                    Debug.Log("Enemy memories drained! Mana absorbed.");
                    break;
                    
                default:
                    Debug.Log($"Applied generic legendary effect: {ability.name}");
                    break;
            }
        }
        
        /// <summary>
        /// Attempt to purify the legendary item
        /// </summary>
        public bool TryPurify(CaminoDeLaFe.Entities.Player purifier, Dictionary<string, int> materials)
        {
            if (!canBePurified || isPurified)
                return false;
                
            // Check if player has required materials
            foreach (string requirement in purificationRequirements)
            {
                Debug.Log($"Checking purification requirement: {requirement}");
                // Would check if player has the required items/materials
            }
            
            // Perform purification
            isPurified = true;
            
            // Add purified bonuses
            foreach (var bonus in purifiedStatBonuses)
            {
                AddStatBonus(bonus.Key, bonus.Value);
            }
            
            // Enhance abilities
            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i].power *= 1.5f; // 50% power increase
                abilities[i].cooldown *= 0.8f; // 20% cooldown reduction
            }
            
            Debug.Log($"{itemName} has been purified! Stats and abilities enhanced.");
            return true;
        }
        
        /// <summary>
        /// Attempt to steal this legendary item
        /// </summary>
        public bool TrySteal(CaminoDeLaFe.Entities.Player thief, CaminoDeLaFe.Entities.Player victim)
        {
            if (!canBeStolen || isUnique)
                return false;
                
            // Calculate steal chance based on multiple factors
            float stealChance = CalculateStealChance(thief, victim);
            
            if (Random.Range(0f, 1f) < stealChance)
            {
                // Successful theft
                currentOwner = thief.playerName;
                Debug.Log($"{thief.playerName} successfully stole {itemName} from {victim.playerName}!");
                return true;
            }
            else
            {
                // Failed theft
                Debug.Log($"{thief.playerName} failed to steal {itemName} from {victim.playerName}!");
                return false;
            }
        }
        
        private float CalculateStealChance(CaminoDeLaFe.Entities.Player thief, CaminoDeLaFe.Entities.Player victim)
        {
            float baseChance = 0.1f; // 10% base chance
            
            // Thief advantages
            float thiefLevel = thief.level * 0.01f;
            float thiefDexterity = thief.stats.GetTotalStat("destreza") * 0.005f;
            
            // Victim protections
            float victimLevel = victim.level * 0.01f;
            float victimAwareness = victim.stats.GetTotalStat("inteligencia") * 0.005f;
            float itemProtection = stealProtectionLevel * 0.1f;
            
            float finalChance = baseChance + thiefLevel + thiefDexterity - victimLevel - victimAwareness - itemProtection;
            
            return Mathf.Clamp(finalChance * (1f - stealDifficulty), 0.01f, 0.5f); // Max 50% chance
        }
        
        /// <summary>
        /// Upgrade the item's theft protection
        /// </summary>
        public void UpgradeProtection(int cost)
        {
            stealProtectionLevel++;
            stealDifficulty = Mathf.Min(0.95f, stealDifficulty + 0.05f);
            Debug.Log($"{itemName} protection upgraded to level {stealProtectionLevel}");
        }
    }
    
    /// <summary>
    /// Legendary item abilities
    /// </summary>
    [System.Serializable]
    public class LegendaryAbility
    {
        public string name;
        public string description;
        public float power;
        public float cooldown;
        public bool isPassive;
        
        public LegendaryAbility(string name, string description, float power, float cooldown, bool passive = false)
        {
            this.name = name;
            this.description = description;
            this.power = power;
            this.cooldown = cooldown;
            this.isPassive = passive;
        }
    }
    
    /// <summary>
    /// Types of legendary items
    /// </summary>
    public enum LegendaryType
    {
        Weapon,
        Armor,
        Accessory,
        UniqueArtifact
    }
    
    /// <summary>
    /// Database of legendary items
    /// </summary>
    public static class LegendaryItemDatabase
    {
        /// <summary>
        /// Create the legendary Crusader sword
        /// </summary>
        public static LegendaryItem CreateEspadaDelJuicioFinal()
        {
            var sword = new LegendaryItem(
                "Espada del Juicio Final",
                "La espada sagrada que decidirá el destino de Tierra Santa. Forjada con acero bendito y bañada en luz divina, solo el más puro de corazón puede empuñarla sin ser consumido por su poder.",
                EquipmentSlot.Weapon,
                LegendaryType.Weapon,
                "Cruzados"
            );
            
            // Massive stat bonuses
            sword.AddStatBonus("fuerza", 15);
            sword.AddStatBonus("tecnica", 10);
            sword.AddStatBonus("defensa", 8);
            sword.AddStatBonus("inteligencia", 5);
            
            // Purification bonuses
            sword.purifiedStatBonuses["fuerza"] = 5;
            sword.purifiedStatBonuses["inteligencia"] = 10;
            sword.purificationRequirements = new string[] { "Agua Bendita", "Fragmento de la Vera Cruz", "Oración de 100 Templarios" };
            
            sword.stealDifficulty = 0.9f;
            sword.requiresQuestToObtain = true;
            sword.isUnique = true;
            
            return sword;
        }
        
        /// <summary>
        /// Create the legendary Saracen scimitar
        /// </summary>
        public static LegendaryItem CreateCimitarraDeLosVientos()
        {
            var scimitar = new LegendaryItem(
                "Cimitarra de los Cuatro Vientos",
                "Una hoja curvada que susurra los secretos del desierto. Dice la leyenda que fue forjada por los djinn del viento y solo obedece a quien comprende el lenguaje de las tormentas de arena.",
                EquipmentSlot.Weapon,
                LegendaryType.Weapon,
                "Sarracenos"
            );
            
            // Speed and technique focused
            scimitar.AddStatBonus("velocidad", 12);
            scimitar.AddStatBonus("agilidad", 12);
            scimitar.AddStatBonus("tecnica", 10);
            scimitar.AddStatBonus("destreza", 8);
            
            // Purification bonuses
            scimitar.purifiedStatBonuses["velocidad"] = 8;
            scimitar.purifiedStatBonuses["agilidad"] = 8;
            scimitar.purificationRequirements = new string[] { "Arena del Desierto Sagrado", "Lágrima de Djinn", "Bendición del Viento del Este" };
            
            scimitar.stealDifficulty = 0.85f;
            scimitar.requiresQuestToObtain = true;
            scimitar.isUnique = true;
            
            return scimitar;
        }
        
        /// <summary>
        /// Create the legendary Ancient scythe
        /// </summary>
        public static LegendaryItem CreateGuadanaDelOlvido()
        {
            var scythe = new LegendaryItem(
                "Guadaña del Olvido",
                "Un arma ancestral que corta no solo la carne, sino también los recuerdos. Sus víctimas olvidan quien fueron, convirtiéndose en sombras de su yo anterior. Solo los Antiguos conocen el secreto para resistir su poder.",
                EquipmentSlot.Weapon,
                LegendaryType.Weapon,
                "Antiguos"
            );
            
            // Intelligence and mystical power focused
            scythe.AddStatBonus("inteligencia", 20);
            scythe.AddStatBonus("tecnica", 8);
            scythe.AddStatBonus("resistencia", 8);
            scythe.AddStatBonus("defensa", 6);
            
            // Purification bonuses
            scythe.purifiedStatBonuses["inteligencia"] = 10;
            scythe.purifiedStatBonuses["resistencia"] = 5;
            scythe.purificationRequirements = new string[] { "Fragmento de Memoria Ancestral", "Esencia de Olvido", "Ritual de los Antiguos Maestros" };
            
            scythe.stealDifficulty = 0.95f;
            scythe.requiresQuestToObtain = true;
            scythe.isUnique = true;
            
            return scythe;
        }
        
        /// <summary>
        /// Create unique server artifacts
        /// </summary>
        public static LegendaryItem CreateCaliceDeLaVida()
        {
            var chalice = new LegendaryItem(
                "Cáliz de la Vida Eterna",
                "El Santo Grial perdido, capaz de otorgar vida eterna a quien beba de él. Su poder afecta a todos los miembros de la facción que lo posea, otorgando regeneración y resistencia a la muerte.",
                EquipmentSlot.Necklace,
                LegendaryType.UniqueArtifact
            );
            
            // Server-wide bonuses
            chalice.AddStatBonus("resistencia", 25);
            chalice.AddStatBonus("inteligencia", 15);
            
            chalice.canBePurified = false; // Already pure
            chalice.canBeStolen = true;
            chalice.stealDifficulty = 0.99f; // Nearly impossible to steal
            chalice.isUnique = true;
            
            return chalice;
        }
        
        public static LegendaryItem CreateEspejoDelDestino()
        {
            var mirror = new LegendaryItem(
                "Espejo del Destino",
                "Un espejo ancestral que muestra no el reflejo físico, sino el destino del alma. Quien lo posea puede ver el futuro de las batallas y anticipar los movimientos enemigos.",
                EquipmentSlot.Ring1,
                LegendaryType.UniqueArtifact
            );
            
            // Prophetic abilities
            mirror.AddStatBonus("inteligencia", 20);
            mirror.AddStatBonus("agilidad", 15);
            mirror.AddStatBonus("tecnica", 10);
            
            mirror.canBePurified = false;
            mirror.canBeStolen = true;
            mirror.stealDifficulty = 0.98f;
            mirror.isUnique = true;
            
            return mirror;
        }
        
        public static LegendaryItem CreateMascaraDeLaMuerte()
        {
            var mask = new LegendaryItem(
                "Máscara de la Muerte Silenciosa",
                "Una máscara que otorga poder sobre los muertos y los moribundos. Su portador puede comunicarse con los espíritus caídos y drenar la vida de sus enemigos para sanar a sus aliados.",
                EquipmentSlot.Helmet,
                LegendaryType.UniqueArtifact
            );
            
            // Necromantic powers
            mask.AddStatBonus("inteligencia", 18);
            mask.AddStatBonus("resistencia", 12);
            mask.AddStatBonus("defensa", 10);
            
            mask.canBePurified = false; // Dark artifact
            mask.canBeStolen = true;
            mask.stealDifficulty = 0.97f;
            mask.isUnique = true;
            
            return mask;
        }
        
        /// <summary>
        /// Get all legendary items for a specific faction
        /// </summary>
        public static List<LegendaryItem> GetFactionLegendaries(string factionName)
        {
            var legendaries = new List<LegendaryItem>();
            
            switch (factionName)
            {
                case "Cruzados":
                    legendaries.Add(CreateEspadaDelJuicioFinal());
                    break;
                case "Sarracenos":
                    legendaries.Add(CreateCimitarraDeLosVientos());
                    break;
                case "Antiguos":
                    legendaries.Add(CreateGuadanaDelOlvido());
                    break;
            }
            
            return legendaries;
        }
        
        /// <summary>
        /// Get all unique server artifacts
        /// </summary>
        public static List<LegendaryItem> GetUniqueArtifacts()
        {
            var artifacts = new List<LegendaryItem>
            {
                CreateCaliceDeLaVida(),
                CreateEspejoDelDestino(),
                CreateMascaraDeLaMuerte()
            };
            
            return artifacts;
        }
    }
}
