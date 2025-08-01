using UnityEngine;
using UnityEngine.UI;
using CaminosDeLaFe.Data;
using CaminosDeLaFe.Core;

namespace CaminosDeLaFe.UI
{
    /// <summary>
    /// UI for faction selection screen
    /// </summary>
    public class FactionSelectionUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Canvas canvas;
        public Text titleText;
        public Text subtitleText;
        public Button cruzadosButton;
        public Button sarracenosButton;
        public Button antiguosButton;
        
        private GameManager gameManager;
        
        public void Initialize(GameManager gameManager)
        {
            this.gameManager = gameManager;
            CreateUI();
        }
        
        private void CreateUI()
        {
            // Create Canvas
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("FactionSelectionCanvas");
                canvasObject.transform.SetParent(transform);
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }
            
            // Create title
            CreateTitleText();
            
            // Create subtitle
            CreateSubtitleText();
            
            // Create faction buttons
            CreateFactionButtons();
        }
        
        private void CreateTitleText()
        {
            GameObject titleObject = new GameObject("TitleText");
            titleObject.transform.SetParent(canvas.transform, false);
            
            titleText = titleObject.AddComponent<Text>();
            titleText.text = "Caminos de la Fe: Cruzada y Conquista";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform titleRect = titleObject.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.7f);
            titleRect.anchorMax = new Vector2(0.5f, 0.7f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(800, 100);
        }
        
        private void CreateSubtitleText()
        {
            GameObject subtitleObject = new GameObject("SubtitleText");
            subtitleObject.transform.SetParent(canvas.transform, false);
            
            subtitleText = subtitleObject.AddComponent<Text>();
            subtitleText.text = "Elige tu facción";
            subtitleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            subtitleText.fontSize = 24;
            subtitleText.color = Color.white;
            subtitleText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform subtitleRect = subtitleObject.GetComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0.5f, 0.6f);
            subtitleRect.anchorMax = new Vector2(0.5f, 0.6f);
            subtitleRect.anchoredPosition = Vector2.zero;
            subtitleRect.sizeDelta = new Vector2(400, 50);
        }
        
        private void CreateFactionButtons()
        {
            string[] factionNames = Factions.GetFactionNames();
            float startY = 0.5f;
            float buttonSpacing = 0.1f;
            
            for (int i = 0; i < factionNames.Length; i++)
            {
                string factionName = factionNames[i];
                Faction faction = Factions.GetFaction(factionName);
                
                GameObject buttonObject = new GameObject($"{factionName}Button");
                buttonObject.transform.SetParent(canvas.transform, false);
                
                Button button = buttonObject.AddComponent<Button>();
                Image buttonImage = buttonObject.AddComponent<Image>();
                buttonImage.color = faction.color;
                
                // Add button text
                GameObject textObject = new GameObject("Text");
                textObject.transform.SetParent(buttonObject.transform, false);
                
                Text buttonText = textObject.AddComponent<Text>();
                buttonText.text = factionName;
                buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                buttonText.fontSize = 18;
                buttonText.color = Color.white;
                buttonText.alignment = TextAnchor.MiddleCenter;
                
                RectTransform textRect = textObject.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                // Position button
                RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, startY - (i * buttonSpacing));
                buttonRect.anchorMax = new Vector2(0.5f, startY - (i * buttonSpacing));
                buttonRect.anchoredPosition = Vector2.zero;
                buttonRect.sizeDelta = new Vector2(200, 60);
                
                // Add button listener
                string factionNameCopy = factionName; // Capture for closure
                button.onClick.AddListener(() => OnFactionButtonClicked(factionNameCopy));
                
                // Store button references
                switch (factionName)
                {
                    case "Cruzados":
                        cruzadosButton = button;
                        break;
                    case "Sarracenos":
                        sarracenosButton = button;
                        break;
                    case "Antiguos":
                        antiguosButton = button;
                        break;
                }
            }
        }
        
        private void OnFactionButtonClicked(string factionName)
        {
            Debug.Log($"Faction button clicked: {factionName}");
            
            // Add button press effect
            StartCoroutine(ButtonPressEffect(GetButtonByFaction(factionName)));
            
            // Notify game manager
            if (gameManager != null)
            {
                gameManager.OnFactionButtonClicked(factionName);
            }
        }
        
        private Button GetButtonByFaction(string factionName)
        {
            switch (factionName)
            {
                case "Cruzados":
                    return cruzadosButton;
                case "Sarracenos":
                    return sarracenosButton;
                case "Antiguos":
                    return antiguosButton;
                default:
                    return null;
            }
        }
        
        private System.Collections.IEnumerator ButtonPressEffect(Button button)
        {
            if (button == null) yield break;
            
            Vector3 originalScale = button.transform.localScale;
            Vector3 pressedScale = originalScale * 0.9f;
            
            // Scale down
            float elapsed = 0f;
            float duration = 0.1f;
            
            while (elapsed < duration)
            {
                button.transform.localScale = Vector3.Lerp(originalScale, pressedScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Scale back up
            elapsed = 0f;
            while (elapsed < duration)
            {
                button.transform.localScale = Vector3.Lerp(pressedScale, originalScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            button.transform.localScale = originalScale;
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
