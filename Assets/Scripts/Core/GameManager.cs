using UnityEngine;
using UnityEngine.SceneManagement;
using CaminosDeLaFe.Data;
using CaminosDeLaFe.Entities;
using CaminosDeLaFe.UI;
using System.Collections.Generic;

namespace CaminosDeLaFe.Core
{
    /// <summary>
    /// Main game controller that manages game states and scene transitions
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        public bool debugMode = true;
        
        [Header("Scene References")]
        public string factionSelectionScene = "FactionSelection";
        public string mainGameScene = "MainGame";
        public string combatScene = "Combat";
        
        [Header("Prefabs")]
        public GameObject playerPrefab;
        public GameObject[] enemyPrefabs;
        public GameObject groundPrefab;
        
        // Game State
        public static GameManager Instance { get; private set; }
        public GameData gameData { get; private set; }
        
        // Current Scene Objects
        private Player currentPlayer;
        private List<Enemy> currentEnemies = new List<Enemy>();
        private GameObject currentGround;
        
        // UI References
        private FactionSelectionUI factionSelectionUI;
        private GameUI gameUI;
        
        // Events
        public System.Action<string> OnFactionSelected;
        public System.Action OnGameStarted;
        public System.Action OnGameEnded;
        
        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            // Start with faction selection
            StartFactionSelection();
        }
        
        private void InitializeGame()
        {
            gameData = new GameData();
            
            if (debugMode)
                Debug.Log("Game Manager initialized");
        }
        
        #region Scene Management
        
        public void StartFactionSelection()
        {
            if (debugMode)
                Debug.Log("Starting faction selection...");
            
            CleanupCurrentScene();
            Camera.main.backgroundColor = Color.gray;
            
            // Create faction selection UI
            if (factionSelectionUI == null)
            {
                GameObject uiObject = new GameObject("FactionSelectionUI");
                factionSelectionUI = uiObject.AddComponent<FactionSelectionUI>();
                factionSelectionUI.Initialize(this);
            }
            else
            {
                factionSelectionUI.gameObject.SetActive(true);
            }
        }
        
        public void StartCombatScene(string selectedFaction)
        {
            if (debugMode)
                Debug.Log($"Starting combat scene with faction: {selectedFaction}");
            
            gameData.selectedFaction = selectedFaction;
            CleanupCurrentScene();
            
            // Hide faction selection UI
            if (factionSelectionUI != null)
                factionSelectionUI.gameObject.SetActive(false);
            
            // Setup combat environment
            SetupCombatScene();
            
            OnGameStarted?.Invoke();
        }
        
        private void SetupCombatScene()
        {
            // Set background color
            Camera.main.backgroundColor = new Color(0.5f, 0.75f, 1f); // Sky blue
            
            // Create ground
            if (groundPrefab != null)
            {
                currentGround = Instantiate(groundPrefab);
            }
            else
            {
                // Create basic ground plane
                currentGround = GameObject.CreatePrimitive(PrimitiveType.Plane);
                currentGround.name = "Ground";
                currentGround.transform.localScale = new Vector3(10, 1, 10);
                currentGround.GetComponent<Renderer>().material.color = new Color(0.4f, 0.6f, 0.25f); // Grass green
            }
            
            // Create player
            CreatePlayer();
            
            // Create enemies
            CreateEnemies();
            
            // Create game UI
            CreateGameUI();
        }
        
        private void CreatePlayer()
        {
            GameObject playerObject;
            
            if (playerPrefab != null)
            {
                playerObject = Instantiate(playerPrefab);
            }
            else
            {
                // Create basic player cube
                playerObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                playerObject.name = "Player";
                playerObject.tag = "Player";
                
                // Add CharacterController
                var collider = playerObject.GetComponent<Collider>();
                if (collider != null)
                    DestroyImmediate(collider);
                    
                playerObject.AddComponent<CharacterController>();
            }
            
            // Add Player component
            currentPlayer = playerObject.GetComponent<Player>();
            if (currentPlayer == null)
                currentPlayer = playerObject.AddComponent<Player>();
            
            // Set player faction
            currentPlayer.SetFaction(gameData.selectedFaction);
            
            // Position player
            playerObject.transform.position = new Vector3(0, 1, 0);
            
            // Setup camera target
            GameObject cameraTarget = new GameObject("CameraTarget");
            cameraTarget.transform.SetParent(playerObject.transform);
            cameraTarget.transform.localPosition = Vector3.zero;
            currentPlayer.cameraTarget = cameraTarget.transform;
        }
        
        private void CreateEnemies()
        {
            // Create a few enemies around the player
            Vector3[] enemyPositions = {
                new Vector3(10, 1, 5),
                new Vector3(-5, 1, 10),
                new Vector3(8, 1, -8),
                new Vector3(-12, 1, -3)
            };
            
            foreach (var position in enemyPositions)
            {
                CreateEnemyAtPosition(position);
            }
        }
        
        private void CreateEnemyAtPosition(Vector3 position)
        {
            GameObject enemyObject;
            
            if (enemyPrefabs != null && enemyPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                enemyObject = Instantiate(enemyPrefabs[randomIndex]);
            }
            else
            {
                // Create basic enemy sphere
                enemyObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                enemyObject.name = "Enemy";
                enemyObject.tag = "Enemy";
                
                // Add CharacterController
                var collider = enemyObject.GetComponent<Collider>();
                if (collider != null)
                    DestroyImmediate(collider);
                    
                enemyObject.AddComponent<CharacterController>();
            }
            
            // Add Enemy component
            var enemy = enemyObject.GetComponent<Enemy>();
            if (enemy == null)
                enemy = enemyObject.AddComponent<Enemy>();
            
            // Set enemy faction (opposite of player)
            string enemyFaction = gameData.selectedFaction == "Cruzados" ? "Sarracenos" : "Cruzados";
            enemy.factionName = enemyFaction;
            
            // Set position and target
            enemyObject.transform.position = position;
            enemy.target = currentPlayer.transform;
            
            currentEnemies.Add(enemy);
        }
        
        private void CreateGameUI()
        {
            GameObject uiObject = new GameObject("GameUI");
            gameUI = uiObject.AddComponent<GameUI>();
            gameUI.Initialize(currentPlayer);
        }
        
        private void CleanupCurrentScene()
        {
            // Destroy current player
            if (currentPlayer != null)
                Destroy(currentPlayer.gameObject);
            
            // Destroy current enemies
            foreach (var enemy in currentEnemies)
            {
                if (enemy != null)
                    Destroy(enemy.gameObject);
            }
            currentEnemies.Clear();
            
            // Destroy ground
            if (currentGround != null)
                Destroy(currentGround);
            
            // Destroy game UI
            if (gameUI != null)
                Destroy(gameUI.gameObject);
        }
        
        #endregion
        
        #region Event Handlers
        
        public void OnFactionButtonClicked(string factionName)
        {
            if (debugMode)
                Debug.Log($"Faction selected: {factionName}");
            
            OnFactionSelected?.Invoke(factionName);
            StartCombatScene(factionName);
        }
        
        public void OnPlayerDied()
        {
            if (debugMode)
                Debug.Log("Player died!");
            
            // Handle player death (restart, respawn, etc.)
            RestartGame();
        }
        
        public void RestartGame()
        {
            StartFactionSelection();
        }
        
        public void QuitGame()
        {
            if (debugMode)
                Debug.Log("Quitting game...");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        #endregion
        
        #region Utility Methods
        
        public void PauseGame()
        {
            Time.timeScale = 0f;
        }
        
        public void ResumeGame()
        {
            Time.timeScale = 1f;
        }
        
        public void SetTimeScale(float scale)
        {
            Time.timeScale = Mathf.Clamp(scale, 0f, 3f);
        }
        
        #endregion
    }
    
    /// <summary>
    /// Game data container
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        public string selectedFaction = "Cruzados";
        public int playerLevel = 1;
        public int playerGold = 100;
        public int playerExperience = 0;
        public float gameTime = 0f;
        public bool gameCompleted = false;
        
        public Dictionary<string, object> customData = new Dictionary<string, object>();
    }
}
