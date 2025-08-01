using UnityEngine;
using CaminosDeLaFe.Data;
using System.Collections;

namespace CaminosDeLaFe.Entities
{
    /// <summary>
    /// Basic enemy AI and behavior
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Data")]
        public string enemyName = "Enemy";
        public string factionName = "Sarracenos";
        public int maxHealth = 50;
        public int currentHealth;
        public float moveSpeed = 3f;
        public float attackDamage = 10f;
        public float attackRange = 2f;
        public float detectionRange = 10f;
        
        [Header("AI Behavior")]
        public float attackCooldown = 1.5f;
        public float pursuitDistance = 15f;
        public float returnToStartDistance = 20f;
        
        [Header("References")]
        public Transform target;
        
        // Private variables
        private Vector3 startPosition;
        private bool canAttack = true;
        private bool isChasing = false;
        private bool isDead = false;
        private Renderer enemyRenderer;
        private CharacterController characterController;
        
        // AI States
        private enum EnemyState
        {
            Idle,
            Chasing,
            Attacking,
            Returning,
            Dead
        }
        
        private EnemyState currentState = EnemyState.Idle;
        
        void Awake()
        {
            enemyRenderer = GetComponent<Renderer>();
            characterController = GetComponent<CharacterController>();
            
            if (characterController == null)
                characterController = gameObject.AddComponent<CharacterController>();
        }
        
        void Start()
        {
            // Initialize health
            currentHealth = maxHealth;
            startPosition = transform.position;
            
            // Set faction color
            SetFactionAppearance();
            
            // Find player if no target assigned
            if (target == null)
            {
                var player = FindObjectOfType<Player>();
                if (player != null)
                    target = player.transform;
            }
        }
        
        void Update()
        {
            if (isDead) return;
            
            switch (currentState)
            {
                case EnemyState.Idle:
                    UpdateIdle();
                    break;
                case EnemyState.Chasing:
                    UpdateChasing();
                    break;
                case EnemyState.Attacking:
                    UpdateAttacking();
                    break;
                case EnemyState.Returning:
                    UpdateReturning();
                    break;
            }
        }
        
        private void SetFactionAppearance()
        {
            var faction = Factions.GetFaction(factionName);
            if (faction != null && enemyRenderer != null)
            {
                enemyRenderer.material.color = faction.color;
            }
        }
        
        private void UpdateIdle()
        {
            if (target == null) return;
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget <= detectionRange)
            {
                currentState = EnemyState.Chasing;
                isChasing = true;
            }
        }
        
        private void UpdateChasing()
        {
            if (target == null)
            {
                currentState = EnemyState.Returning;
                return;
            }
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            float distanceToStart = Vector3.Distance(transform.position, startPosition);
            
            // Return to start if too far from spawn point
            if (distanceToStart > returnToStartDistance)
            {
                currentState = EnemyState.Returning;
                isChasing = false;
                return;
            }
            
            // Attack if in range
            if (distanceToTarget <= attackRange)
            {
                currentState = EnemyState.Attacking;
                return;
            }
            
            // Stop chasing if target is too far
            if (distanceToTarget > pursuitDistance)
            {
                currentState = EnemyState.Returning;
                isChasing = false;
                return;
            }
            
            // Move towards target
            MoveTowards(target.position);
        }
        
        private void UpdateAttacking()
        {
            if (target == null)
            {
                currentState = EnemyState.Returning;
                return;
            }
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            // If target moved out of attack range, chase again
            if (distanceToTarget > attackRange)
            {
                currentState = EnemyState.Chasing;
                return;
            }
            
            // Look at target
            LookAtTarget(target.position);
            
            // Attack if cooldown is ready
            if (canAttack)
            {
                Attack();
            }
        }
        
        private void UpdateReturning()
        {
            float distanceToStart = Vector3.Distance(transform.position, startPosition);
            
            if (distanceToStart < 1f)
            {
                currentState = EnemyState.Idle;
                isChasing = false;
                return;
            }
            
            // Move towards start position
            MoveTowards(startPosition);
            
            // Check if player comes back in range
            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (distanceToTarget <= detectionRange)
                {
                    currentState = EnemyState.Chasing;
                    isChasing = true;
                }
            }
        }
        
        private void MoveTowards(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Keep movement horizontal
            
            if (direction.magnitude > 0.1f)
            {
                // Move the character
                Vector3 move = direction * moveSpeed * Time.deltaTime;
                characterController.Move(move);
                
                // Rotate to face movement direction
                LookAtTarget(targetPosition);
            }
        }
        
        private void LookAtTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Keep rotation horizontal
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        
        private void Attack()
        {
            if (!canAttack || target == null) return;
            
            canAttack = false;
            StartCoroutine(AttackCooldownCoroutine());
            
            // Simple attack animation
            StartCoroutine(AttackAnimation());
            
            // Deal damage to player
            var player = target.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
                Debug.Log($"{enemyName} attacked player for {attackDamage} damage!");
            }
        }
        
        private IEnumerator AttackCooldownCoroutine()
        {
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
        }
        
        private IEnumerator AttackAnimation()
        {
            Vector3 originalPosition = transform.position;
            Vector3 attackPosition = originalPosition + transform.forward * 0.5f;
            
            // Move forward
            float elapsed = 0;
            while (elapsed < 0.15f)
            {
                transform.position = Vector3.Lerp(originalPosition, attackPosition, elapsed / 0.15f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Move back
            elapsed = 0;
            while (elapsed < 0.15f)
            {
                transform.position = Vector3.Lerp(attackPosition, originalPosition, elapsed / 0.15f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.position = originalPosition;
        }
        
        public void TakeDamage(float damage)
        {
            if (isDead) return;
            
            currentHealth = Mathf.Max(0, currentHealth - Mathf.RoundToInt(damage));
            
            // Blink effect
            if (enemyRenderer != null)
            {
                StartCoroutine(DamageFlash());
            }
            
            Debug.Log($"{enemyName} took {damage} damage. Health: {currentHealth}/{maxHealth}");
            
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                // If not already chasing and has a target, start chasing
                if (currentState == EnemyState.Idle && target != null)
                {
                    currentState = EnemyState.Chasing;
                    isChasing = true;
                }
            }
        }
        
        private IEnumerator DamageFlash()
        {
            Color originalColor = enemyRenderer.material.color;
            enemyRenderer.material.color = Color.white;
            
            yield return new WaitForSeconds(0.1f);
            
            enemyRenderer.material.color = originalColor;
        }
        
        private void Die()
        {
            isDead = true;
            currentState = EnemyState.Dead;
            
            Debug.Log($"{enemyName} died!");
            
            // Give experience to player if killed by player
            var player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.AddExperience(25); // Give XP for kill
                player.AddGold(Random.Range(5, 15)); // Random gold drop
            }
            
            // Destroy after a short delay
            StartCoroutine(DestroyAfterDelay(2f));
        }
        
        private IEnumerator DestroyAfterDelay(float delay)
        {
            // Fade out effect
            if (enemyRenderer != null)
            {
                Color originalColor = enemyRenderer.material.color;
                float elapsed = 0;
                
                while (elapsed < delay)
                {
                    float alpha = Mathf.Lerp(1f, 0f, elapsed / delay);
                    Color newColor = originalColor;
                    newColor.a = alpha;
                    enemyRenderer.material.color = newColor;
                    
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            
            Destroy(gameObject);
        }
        
        // Debug visualization
        void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Pursuit distance
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, pursuitDistance);
            
            // Return to start distance
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(startPosition, returnToStartDistance);
            
            // Line to target
            if (target != null && isChasing)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}
