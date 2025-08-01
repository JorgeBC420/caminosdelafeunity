using UnityEngine;
using UnityEngine.UI;
using CaminoDeLaFe.UI;
using CaminoDeLaFe.Data;
using CaminoDeLaFe.Systems;
using CaminoDeLaFe.Inventory;

namespace CaminoDeLaFe.Core
{
    /// <summary>
    /// Configuración automática de la escena principal del juego
    /// Este script crea y configura todos los GameObjects necesarios
    /// </summary>
    public class AutoSceneSetup : MonoBehaviour
    {
        [Header("Auto Setup Configuration")]
        [SerializeField] private bool setupOnStart = true;
        [SerializeField] private bool createBasicLighting = true;
        [SerializeField] private bool createGroundPlane = true;
        [SerializeField] private bool setupCamera = true;
        [SerializeField] private bool createUI = true;
        
        [Header("Scene References (Auto-populated)")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Player player;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Light mainLight;
        
        private void Start()
        {
            if (setupOnStart)
            {
                SetupCompleteScene();
            }
        }
        
        [ContextMenu("Setup Complete Scene")]
        public void SetupCompleteScene()
        {
            Debug.Log("🏗️ Setting up Caminos de la Fe scene...");
            
            SetupGameManager();
            SetupLighting();
            SetupCamera();
            SetupGroundPlane();
            SetupPlayer();
            SetupUI();
            SetupTestingSystems();
            
            Debug.Log("✅ Scene setup completed successfully!");
        }
        
        private void SetupGameManager()
        {
            if (gameManager == null)
            {
                GameObject gmObject = GameObject.Find("GameManager");
                if (gmObject == null)
                {
                    gmObject = new GameObject("GameManager");
                    gameManager = gmObject.AddComponent<GameManager>();
                    Debug.Log("✅ GameManager created");
                }
                else
                {
                    gameManager = gmObject.GetComponent<GameManager>();
                    if (gameManager == null)
                    {
                        gameManager = gmObject.AddComponent<GameManager>();
                    }
                }
            }
        }
        
        private void SetupLighting()
        {
            if (!createBasicLighting) return;
            
            if (mainLight == null)
            {
                GameObject lightObject = GameObject.Find("Main Light");
                if (lightObject == null)
                {
                    lightObject = new GameObject("Main Light");
                    mainLight = lightObject.AddComponent<Light>();
                    
                    // Configure directional light for medieval atmosphere
                    mainLight.type = LightType.Directional;
                    mainLight.color = new Color(1f, 0.95f, 0.8f); // Warm sunlight
                    mainLight.intensity = 1.2f;
                    mainLight.shadows = LightShadows.Soft;
                    
                    // Position for late afternoon sun
                    lightObject.transform.rotation = Quaternion.Euler(45f, 30f, 0f);
                    
                    Debug.Log("✅ Main lighting created");
                }
                else
                {
                    mainLight = lightObject.GetComponent<Light>();
                }
            }
            
            // Configure ambient lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.5f, 0.7f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.4f, 0.4f, 0.4f);
            RenderSettings.ambientGroundColor = new Color(0.2f, 0.2f, 0.2f);
        }
        
        private void SetupCamera()
        {
            if (!setupCamera) return;
            
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    GameObject cameraObject = new GameObject("Main Camera");
                    mainCamera = cameraObject.AddComponent<Camera>();
                    cameraObject.AddComponent<AudioListener>();
                    cameraObject.tag = "MainCamera";
                }
            }
            
            // Configure camera for 3D RPG
            mainCamera.fieldOfView = 60f;
            mainCamera.nearClipPlane = 0.1f;
            mainCamera.farClipPlane = 1000f;
            
            // Position camera for third-person view
            mainCamera.transform.position = new Vector3(0, 8, -12);
            mainCamera.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
            
            // Add camera controller for following player
            var cameraController = mainCamera.GetComponent<CameraController>();
            if (cameraController == null)
            {
                cameraController = mainCamera.gameObject.AddComponent<CameraController>();
            }
            
            Debug.Log("✅ Camera configured");
        }
        
        private void SetupGroundPlane()
        {
            if (!createGroundPlane) return;
            
            GameObject ground = GameObject.Find("Ground");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
                ground.transform.localScale = new Vector3(10, 1, 10); // 100x100 units
                
                // Create basic ground material
                var renderer = ground.GetComponent<Renderer>();
                var material = new Material(Shader.Find("Standard"));
                material.color = new Color(0.4f, 0.6f, 0.3f); // Grass green
                renderer.material = material;
                
                Debug.Log("✅ Ground plane created");
            }
        }
        
        private void SetupPlayer()
        {
            if (player == null)
            {
                GameObject playerObject = GameObject.Find("Player");
                if (playerObject == null)
                {
                    playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    playerObject.name = "Player";
                    playerObject.transform.position = new Vector3(0, 1, 0);
                    
                    // Remove the primitive collider and add character controller
                    DestroyImmediate(playerObject.GetComponent<Collider>());
                    playerObject.AddComponent<CharacterController>();
                    
                    // Add player components
                    player = playerObject.AddComponent<Player>();
                    
                    // Add inventory system
                    var inventory = playerObject.AddComponent<PlayerInventory>();
                    
                    // Add mount system
                    var mountSystem = playerObject.AddComponent<MountSystem>();
                    
                    // Configure player material
                    var renderer = playerObject.GetComponent<Renderer>();
                    var material = new Material(Shader.Find("Standard"));
                    material.color = Color.blue; // Will be set by faction selection
                    renderer.material = material;
                    
                    Debug.Log("✅ Player created with all systems");
                }
                else
                {
                    player = playerObject.GetComponent<Player>();
                    if (player == null)
                    {
                        player = playerObject.AddComponent<Player>();
                    }
                }
            }
            
            // Ensure camera follows player
            if (mainCamera != null)
            {
                var cameraController = mainCamera.GetComponent<CameraController>();
                if (cameraController != null)
                {
                    cameraController.SetTarget(player.transform);
                }
            }
        }
        
        private void SetupUI()
        {
            if (!createUI) return;
            
            if (mainCanvas == null)
            {
                GameObject canvasObject = GameObject.Find("Main Canvas");
                if (canvasObject == null)
                {
                    canvasObject = new GameObject("Main Canvas");
                    mainCanvas = canvasObject.AddComponent<Canvas>();
                    canvasObject.AddComponent<CanvasScaler>();
                    canvasObject.AddComponent<GraphicRaycaster>();
                    
                    // Configure canvas
                    mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    mainCanvas.sortingOrder = 0;
                    
                    var scaler = mainCanvas.GetComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                }
                else
                {
                    mainCanvas = canvasObject.GetComponent<Canvas>();
                }
            }
            
            SetupGameUI();
            SetupFactionSelectionUI();
            
            Debug.Log("✅ UI system created");
        }
        
        private void SetupGameUI()
        {
            GameObject gameUIObject = mainCanvas.transform.Find("GameUI")?.gameObject;
            if (gameUIObject == null)
            {
                gameUIObject = new GameObject("GameUI");
                gameUIObject.transform.SetParent(mainCanvas.transform, false);
                var gameUI = gameUIObject.AddComponent<GameUI>();
                
                // Create health/mana bars
                CreateHealthManaBar(gameUIObject);
                
                // Create stats panel
                CreateStatsPanel(gameUIObject);
            }
        }
        
        private void CreateHealthManaBar(GameObject parent)
        {
            GameObject barContainer = new GameObject("HealthManaContainer");
            barContainer.transform.SetParent(parent.transform, false);
            
            var rectTransform = barContainer.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.02f, 0.85f);
            rectTransform.anchorMax = new Vector2(0.4f, 0.98f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            var healthManaBar = barContainer.AddComponent<HealthManaBar>();
            
            // Create health bar
            CreateBar(barContainer, "HealthBar", Color.red, new Vector2(0, 0.6f), new Vector2(1, 1));
            
            // Create mana bar  
            CreateBar(barContainer, "ManaBar", Color.blue, new Vector2(0, 0), new Vector2(1, 0.4f));
        }
        
        private void CreateBar(GameObject parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject bar = new GameObject(name);
            bar.transform.SetParent(parent.transform, false);
            
            var image = bar.AddComponent<Image>();
            image.color = color;
            
            var rectTransform = bar.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
        
        private void CreateStatsPanel(GameObject parent)
        {
            GameObject statsPanel = new GameObject("StatsPanel");
            statsPanel.transform.SetParent(parent.transform, false);
            
            var rectTransform = statsPanel.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.7f, 0.02f);
            rectTransform.anchorMax = new Vector2(0.98f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            var image = statsPanel.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.7f);
            
            var playerStatsUI = statsPanel.AddComponent<PlayerStatsUI>();
        }
        
        private void SetupFactionSelectionUI()
        {
            GameObject factionUIObject = mainCanvas.transform.Find("FactionSelectionUI")?.gameObject;
            if (factionUIObject == null)
            {
                factionUIObject = new GameObject("FactionSelectionUI");
                factionUIObject.transform.SetParent(mainCanvas.transform, false);
                
                var rectTransform = factionUIObject.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                
                var factionUI = factionUIObject.AddComponent<FactionSelectionUI>();
                
                // Initially active for faction selection
                factionUIObject.SetActive(true);
            }
        }
        
        private void SetupTestingSystems()
        {
            // Add testing system to GameManager
            if (gameManager != null)
            {
                var tester = gameManager.GetComponent<GameSystemTester>();
                if (tester == null)
                {
                    tester = gameManager.gameObject.AddComponent<GameSystemTester>();
                    Debug.Log("✅ Testing system added");
                }
                
                // 🎯 ADD GAMES LAB MONETIZATION SYSTEMS
                SetupMonetizationSystems();
            }
            
            // Add assembly references helper
            var assemblyRef = gameManager.GetComponent<AssemblyReferences>();
            if (assemblyRef == null)
            {
                assemblyRef = gameManager.gameObject.AddComponent<AssemblyReferences>();
                Debug.Log("✅ Assembly references helper added");
            }
        }
        
        /// <summary>
        /// Setup all Games Lab monetization systems
        /// </summary>
        private void SetupMonetizationSystems()
        {
            Debug.Log("🎯 Setting up Games Lab Monetization Systems...");
            
            // Create monetization parent object
            GameObject monetizationRoot = GameObject.Find("MonetizationSystems");
            if (monetizationRoot == null)
            {
                monetizationRoot = new GameObject("MonetizationSystems");
            }
            
            // 1. Faith Pass System
            if (monetizationRoot.transform.Find("FaithPassSystem") == null)
            {
                GameObject faithPassGO = new GameObject("FaithPassSystem");
                faithPassGO.transform.SetParent(monetizationRoot.transform);
                
                // Note: Will need to uncomment when assembly references are fixed
                // faithPassGO.AddComponent<CaminoDeLaFe.Monetization.FaithPassSystem>();
                Debug.Log("🎯 Faith Pass System placeholder created");
            }
            
            // 2. Advertising System
            if (monetizationRoot.transform.Find("AdvertisingSystem") == null)
            {
                GameObject adSystemGO = new GameObject("AdvertisingSystem");
                adSystemGO.transform.SetParent(monetizationRoot.transform);
                
                // Note: Will need to uncomment when assembly references are fixed
                // adSystemGO.AddComponent<CaminoDeLaFe.Monetization.AdvertisingSystem>();
                Debug.Log("🎯 Advertising System placeholder created");
            }
            
            // 3. Economy System
            if (monetizationRoot.transform.Find("EconomySystem") == null)
            {
                GameObject economyGO = new GameObject("EconomySystem");
                economyGO.transform.SetParent(monetizationRoot.transform);
                
                // Note: Will need to uncomment when assembly references are fixed
                // economyGO.AddComponent<CaminoDeLaFe.Monetization.EconomySystem>();
                Debug.Log("🎯 Economy System placeholder created");
            }
            
            // 4. Daily Limits System
            if (monetizationRoot.transform.Find("DailyLimitsSystem") == null)
            {
                GameObject limitsGO = new GameObject("DailyLimitsSystem");
                limitsGO.transform.SetParent(monetizationRoot.transform);
                
                // Note: Will need to uncomment when assembly references are fixed
                // limitsGO.AddComponent<CaminoDeLaFe.Monetization.DailyLimitsSystem>();
                Debug.Log("🎯 Daily Limits System placeholder created");
            }
            
            // 5. Monetization Manager (Main Controller)
            if (monetizationRoot.GetComponent<MonoBehaviour>() == null)
            {
                // Note: Will need to uncomment when assembly references are fixed
                // monetizationRoot.AddComponent<CaminoDeLaFe.Monetization.MonetizationManager>();
                Debug.Log("🎯 Monetization Manager placeholder created");
            }
            
            Debug.Log("🎯 Games Lab Monetization Systems setup complete!");
            Debug.Log("⚠️  Remember to uncomment components when assembly references are fixed");
        }
        
        [ContextMenu("Create Test Enemy")]
        public void CreateTestEnemy()
        {
            GameObject enemyObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemyObject.name = "Test Enemy";
            enemyObject.transform.position = new Vector3(5, 1, 5);
            
            // Configure enemy material
            var renderer = enemyObject.GetComponent<Renderer>();
            var material = new Material(Shader.Find("Standard"));
            material.color = Color.red;
            renderer.material = material;
            
            // Add enemy component
            var enemy = enemyObject.AddComponent<Enemy>();
            
            // Add NavMeshAgent for movement
            var navAgent = enemyObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            
            Debug.Log("✅ Test enemy created");
        }
        
        [ContextMenu("Setup EventSystem")]
        public void SetupEventSystem()
        {
            if (GameObject.Find("EventSystem") == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                
                Debug.Log("✅ EventSystem created");
            }
        }
    }
}
