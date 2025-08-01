using UnityEngine;
using UnityEngine.UI;
using CaminosDeLaFe.Entities;
using CaminosDeLaFe.Systems;

namespace CaminosDeLaFe.UI
{
    /// <summary>
    /// Main game UI controller
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Canvas canvas;
        public HealthBar healthBar;
        public ManaBar manaBar;
        public Text goldText;
        public Text levelText;
        public Text factionText;
        public PlayerStatsUI statsUI;
        
        private Player player;
        private bool statsUIVisible = false;
        
        public void Initialize(Player player)
        {
            this.player = player;
            CreateUI();
            SubscribeToEvents();
        }
        
        private void CreateUI()
        {
            // Create Canvas
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("GameUICanvas");
                canvasObject.transform.SetParent(transform);
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10;
                
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }
            
            CreateHealthBar();
            CreateManaBar();
            CreateInfoTexts();
            CreateStatsUI();
        }
        
        private void CreateHealthBar()
        {
            GameObject healthBarObject = new GameObject("HealthBar");
            healthBarObject.transform.SetParent(canvas.transform, false);
            
            healthBar = healthBarObject.AddComponent<HealthBar>();
            healthBar.Initialize(player.stats.maxHealth);
            
            // Position at bottom left
            RectTransform healthRect = healthBarObject.GetComponent<RectTransform>();
            healthRect.anchorMin = new Vector2(0.02f, 0.05f);
            healthRect.anchorMax = new Vector2(0.02f, 0.05f);
            healthRect.anchoredPosition = Vector2.zero;
            healthRect.sizeDelta = new Vector2(200, 20);
        }
        
        private void CreateManaBar()
        {
            GameObject manaBarObject = new GameObject("ManaBar");
            manaBarObject.transform.SetParent(canvas.transform, false);
            
            manaBar = manaBarObject.AddComponent<ManaBar>();
            manaBar.Initialize(player.stats.maxMana);
            
            // Position below health bar
            RectTransform manaRect = manaBarObject.GetComponent<RectTransform>();
            manaRect.anchorMin = new Vector2(0.02f, 0.02f);
            manaRect.anchorMax = new Vector2(0.02f, 0.02f);
            manaRect.anchoredPosition = Vector2.zero;
            manaRect.sizeDelta = new Vector2(200, 20);
        }
        
        private void CreateInfoTexts()
        {
            // Faction text (top left)
            GameObject factionObject = new GameObject("FactionText");
            factionObject.transform.SetParent(canvas.transform, false);
            
            factionText = factionObject.AddComponent<Text>();
            factionText.text = $"Facción: {player.factionName}";
            factionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            factionText.fontSize = 16;
            factionText.color = Color.white;
            
            RectTransform factionRect = factionObject.GetComponent<RectTransform>();
            factionRect.anchorMin = new Vector2(0.02f, 0.95f);
            factionRect.anchorMax = new Vector2(0.02f, 0.95f);
            factionRect.anchoredPosition = Vector2.zero;
            factionRect.sizeDelta = new Vector2(200, 30);
            
            // Level text (top right)
            GameObject levelObject = new GameObject("LevelText");
            levelObject.transform.SetParent(canvas.transform, false);
            
            levelText = levelObject.AddComponent<Text>();
            levelText.text = $"Nivel: {player.level}";
            levelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            levelText.fontSize = 16;
            levelText.color = Color.white;
            levelText.alignment = TextAnchor.MiddleRight;
            
            RectTransform levelRect = levelObject.GetComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0.8f, 0.95f);
            levelRect.anchorMax = new Vector2(0.8f, 0.95f);
            levelRect.anchoredPosition = Vector2.zero;
            levelRect.sizeDelta = new Vector2(180, 30);
            
            // Gold text (top right, below level)
            GameObject goldObject = new GameObject("GoldText");
            goldObject.transform.SetParent(canvas.transform, false);
            
            goldText = goldObject.AddComponent<Text>();
            goldText.text = $"Oro: {player.gold}";
            goldText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            goldText.fontSize = 16;
            goldText.color = Color.yellow;
            goldText.alignment = TextAnchor.MiddleRight;
            
            RectTransform goldRect = goldObject.GetComponent<RectTransform>();
            goldRect.anchorMin = new Vector2(0.8f, 0.9f);
            goldRect.anchorMax = new Vector2(0.8f, 0.9f);
            goldRect.anchoredPosition = Vector2.zero;
            goldRect.sizeDelta = new Vector2(180, 30);
        }
        
        private void CreateStatsUI()
        {
            GameObject statsObject = new GameObject("PlayerStatsUI");
            statsObject.transform.SetParent(canvas.transform, false);
            
            statsUI = statsObject.AddComponent<PlayerStatsUI>();
            statsUI.Initialize(player);
            statsUI.gameObject.SetActive(false); // Hidden by default
        }
        
        private void SubscribeToEvents()
        {
            if (player != null)
            {
                player.OnHealthChanged += UpdateHealthBar;
                player.OnManaChanged += UpdateManaBar;
                player.OnGoldChanged += UpdateGoldText;
                player.OnLevelChanged += UpdateLevelText;
                player.OnFactionChanged += UpdateFactionText;
            }
        }
        
        void Update()
        {
            // Toggle stats UI with Tab key
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleStatsUI();
            }
        }
        
        private void UpdateHealthBar(int newHealth)
        {
            if (healthBar != null)
                healthBar.SetValue(newHealth);
        }
        
        private void UpdateManaBar(int newMana)
        {
            if (manaBar != null)
                manaBar.SetValue(newMana);
        }
        
        private void UpdateGoldText(int newGold)
        {
            if (goldText != null)
                goldText.text = $"Oro: {newGold}";
        }
        
        private void UpdateLevelText(int newLevel)
        {
            if (levelText != null)
                levelText.text = $"Nivel: {newLevel}";
        }
        
        private void UpdateFactionText(string newFaction)
        {
            if (factionText != null)
                factionText.text = $"Facción: {newFaction}";
        }
        
        private void ToggleStatsUI()
        {
            if (statsUI != null)
            {
                statsUIVisible = !statsUIVisible;
                statsUI.gameObject.SetActive(statsUIVisible);
                
                if (statsUIVisible)
                {
                    statsUI.RefreshUI();
                }
            }
        }
        
        void OnDestroy()
        {
            // Unsubscribe from events
            if (player != null)
            {
                player.OnHealthChanged -= UpdateHealthBar;
                player.OnManaChanged -= UpdateManaBar;
                player.OnGoldChanged -= UpdateGoldText;
                player.OnLevelChanged -= UpdateLevelText;
                player.OnFactionChanged -= UpdateFactionText;
            }
        }
    }
}
