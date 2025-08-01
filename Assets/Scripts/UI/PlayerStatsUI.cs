using UnityEngine;
using UnityEngine.UI;
using CaminosDeLaFe.Entities;
using System.Collections.Generic;

namespace CaminosDeLaFe.UI
{
    /// <summary>
    /// Player statistics UI panel
    /// </summary>
    public class PlayerStatsUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Image backgroundPanel;
        public Text titleText;
        public ScrollRect scrollRect;
        public Transform contentParent;
        
        private Player player;
        private Dictionary<string, Text> statLabels = new Dictionary<string, Text>();
        private Dictionary<string, Button> upgradeButtons = new Dictionary<string, Button>();
        private Text healthText;
        private Text manaText;
        private Text goldText;
        
        public void Initialize(Player player)
        {
            this.player = player;
            CreateStatsUI();
            RefreshUI();
        }
        
        private void CreateStatsUI()
        {
            // Background panel
            GameObject bgObject = new GameObject("Background");
            bgObject.transform.SetParent(transform, false);
            
            backgroundPanel = bgObject.AddComponent<Image>();
            backgroundPanel.color = new Color(0, 0, 0, 0.8f);
            
            RectTransform bgRect = bgObject.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.3f, 0.2f);
            bgRect.anchorMax = new Vector2(0.7f, 0.8f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Title
            CreateTitle();
            
            // Create scroll view
            CreateScrollView();
            
            // Create stat entries
            CreateStatEntries();
            
            // Create derived stats
            CreateDerivedStats();
        }
        
        private void CreateTitle()
        {
            GameObject titleObject = new GameObject("Title");
            titleObject.transform.SetParent(backgroundPanel.transform, false);
            
            titleText = titleObject.AddComponent<Text>();
            titleText.text = "Estadísticas del Personaje";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 20;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform titleRect = titleObject.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0.9f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
        }
        
        private void CreateScrollView()
        {
            GameObject scrollObject = new GameObject("ScrollView");
            scrollObject.transform.SetParent(backgroundPanel.transform, false);
            
            scrollRect = scrollObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            
            RectTransform scrollRectTransform = scrollObject.GetComponent<RectTransform>();
            scrollRectTransform.anchorMin = new Vector2(0.05f, 0.1f);
            scrollRectTransform.anchorMax = new Vector2(0.95f, 0.85f);
            scrollRectTransform.offsetMin = Vector2.zero;
            scrollRectTransform.offsetMax = Vector2.zero;
            
            // Viewport
            GameObject viewportObject = new GameObject("Viewport");
            viewportObject.transform.SetParent(scrollObject.transform, false);
            
            Image viewportImage = viewportObject.AddComponent<Image>();
            viewportImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            
            Mask viewportMask = viewportObject.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            
            RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            
            scrollRect.viewport = viewportRect;
            
            // Content
            GameObject contentObject = new GameObject("Content");
            contentObject.transform.SetParent(viewportObject.transform, false);
            
            contentParent = contentObject.transform;
            
            RectTransform contentRect = contentObject.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            
            scrollRect.content = contentRect;
            
            // Add ContentSizeFitter
            ContentSizeFitter contentSizeFitter = contentObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Add VerticalLayoutGroup
            VerticalLayoutGroup layoutGroup = contentObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 10f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
        }
        
        private void CreateStatEntries()
        {
            string[] statNames = player.stats.GetStatNames();
            
            foreach (string statName in statNames)
            {
                CreateStatEntry(statName);
            }
        }
        
        private void CreateStatEntry(string statName)
        {
            GameObject entryObject = new GameObject($"{statName}Entry");
            entryObject.transform.SetParent(contentParent, false);
            
            // Add LayoutElement
            LayoutElement layoutElement = entryObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 30f;
            
            // Stat label
            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(entryObject.transform, false);
            
            Text statLabel = labelObject.AddComponent<Text>();
            statLabel.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            statLabel.fontSize = 14;
            statLabel.color = Color.white;
            
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0.7f, 1f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            
            statLabels[statName] = statLabel;
            
            // Upgrade button
            GameObject buttonObject = new GameObject("UpgradeButton");
            buttonObject.transform.SetParent(entryObject.transform, false);
            
            Button upgradeButton = buttonObject.AddComponent<Button>();
            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 1f);
            
            // Button text
            GameObject buttonTextObject = new GameObject("Text");
            buttonTextObject.transform.SetParent(buttonObject.transform, false);
            
            Text buttonText = buttonTextObject.AddComponent<Text>();
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 12;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform buttonTextRect = buttonTextObject.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.72f, 0.1f);
            buttonRect.anchorMax = new Vector2(0.98f, 0.9f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            // Button click event
            string statNameCopy = statName; // Capture for closure
            upgradeButton.onClick.AddListener(() => OnUpgradeButtonClicked(statNameCopy));
            
            upgradeButtons[statName] = upgradeButton;
        }
        
        private void CreateDerivedStats()
        {
            // Health stat
            GameObject healthObject = new GameObject("HealthEntry");
            healthObject.transform.SetParent(contentParent, false);
            
            LayoutElement healthLayout = healthObject.AddComponent<LayoutElement>();
            healthLayout.preferredHeight = 25f;
            
            healthText = healthObject.AddComponent<Text>();
            healthText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            healthText.fontSize = 14;
            healthText.color = Color.red;
            
            RectTransform healthRect = healthObject.GetComponent<RectTransform>();
            healthRect.anchorMin = Vector2.zero;
            healthRect.anchorMax = Vector2.one;
            healthRect.offsetMin = Vector2.zero;
            healthRect.offsetMax = Vector2.zero;
            
            // Mana stat
            GameObject manaObject = new GameObject("ManaEntry");
            manaObject.transform.SetParent(contentParent, false);
            
            LayoutElement manaLayout = manaObject.AddComponent<LayoutElement>();
            manaLayout.preferredHeight = 25f;
            
            manaText = manaObject.AddComponent<Text>();
            manaText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            manaText.fontSize = 14;
            manaText.color = Color.blue;
            
            RectTransform manaRect = manaObject.GetComponent<RectTransform>();
            manaRect.anchorMin = Vector2.zero;
            manaRect.anchorMax = Vector2.one;
            manaRect.offsetMin = Vector2.zero;
            manaRect.offsetMax = Vector2.zero;
            
            // Gold stat
            GameObject goldObject = new GameObject("GoldEntry");
            goldObject.transform.SetParent(contentParent, false);
            
            LayoutElement goldLayout = goldObject.AddComponent<LayoutElement>();
            goldLayout.preferredHeight = 25f;
            
            goldText = goldObject.AddComponent<Text>();
            goldText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            goldText.fontSize = 14;
            goldText.color = Color.yellow;
            
            RectTransform goldRect = goldObject.GetComponent<RectTransform>();
            goldRect.anchorMin = Vector2.zero;
            goldRect.anchorMax = Vector2.one;
            goldRect.offsetMin = Vector2.zero;
            goldRect.offsetMax = Vector2.zero;
        }
        
        public void RefreshUI()
        {
            if (player == null) return;
            
            // Update stat labels and buttons
            foreach (var kvp in statLabels)
            {
                string statName = kvp.Key;
                Text label = kvp.Value;
                
                int baseStat = player.stats.baseStats.ContainsKey(statName) ? player.stats.baseStats[statName] : 0;
                int bonus = player.stats.GetBonusValue(statName);
                int total = player.stats.GetTotalStat(statName);
                
                if (bonus > 0)
                    label.text = $"{CapitalizeFirst(statName)}: {total} ({baseStat} + {bonus})";
                else
                    label.text = $"{CapitalizeFirst(statName)}: {baseStat}";
                
                // Update button
                if (upgradeButtons.ContainsKey(statName))
                {
                    Button button = upgradeButtons[statName];
                    int cost = player.stats.GetStatUpgradeCost(statName);
                    bool canAfford = player.gold >= cost;
                    
                    Text buttonText = button.GetComponentInChildren<Text>();
                    if (buttonText != null)
                        buttonText.text = $"+ ({cost})";
                    
                    button.interactable = canAfford;
                    
                    Image buttonImage = button.GetComponent<Image>();
                    if (buttonImage != null)
                        buttonImage.color = canAfford ? new Color(0.2f, 0.6f, 1f) : Color.gray;
                }
            }
            
            // Update derived stats
            if (healthText != null)
                healthText.text = $"Vida Máxima: {player.stats.maxHealth}";
                
            if (manaText != null)
                manaText.text = $"Maná Máximo: {player.stats.maxMana}";
                
            if (goldText != null)
                goldText.text = $"Oro: {player.gold}";
        }
        
        private void OnUpgradeButtonClicked(string statName)
        {
            int cost = player.stats.GetStatUpgradeCost(statName);
            
            if (player.SpendGold(cost))
            {
                player.stats.ImproveStat(statName);
                RefreshUI();
                Debug.Log($"Improved {statName}! Cost: {cost} gold");
            }
            else
            {
                Debug.Log($"Not enough gold to improve {statName}. Need {cost} gold, have {player.gold}");
            }
        }
        
        private string CapitalizeFirst(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
                
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
