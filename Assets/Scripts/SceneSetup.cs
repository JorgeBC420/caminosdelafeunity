using UnityEngine;
using CaminoDeLaFe.Core;

namespace CaminoDeLaFe
{
    /// <summary>
    /// Scene setup script - attach this to an empty GameObject in your main scene
    /// </summary>
    public class SceneSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public bool setupOnStart = true;
        public bool createGameManager = true;
        public bool setupEventSystem = true;
        
        void Start()
        {
            if (setupOnStart)
            {
                SetupScene();
            }
        }
        
        [ContextMenu("Setup Scene")]
        public void SetupScene()
        {
            Debug.Log("Setting up Caminos de la Fe scene...");
            
            // Create Game Manager if it doesn't exist
            if (createGameManager && GameManager.Instance == null)
            {
                GameObject gameManagerObject = new GameObject("GameManager");
                gameManagerObject.AddComponent<GameManager>();
                Debug.Log("GameManager created");
            }
            
            // Setup EventSystem for UI if it doesn't exist
            if (setupEventSystem && FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("EventSystem created");
            }
            
            Debug.Log("Scene setup complete!");
        }
    }
}
