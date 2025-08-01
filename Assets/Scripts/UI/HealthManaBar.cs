using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CaminoDeLaFe.UI
{
    /// <summary>
    /// Health bar UI component
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("Health Bar")]
        public Image backgroundImage;
        public Image fillImage;
        public Text healthText;
        
        private int maxValue;
        private int currentValue;
        private Coroutine updateCoroutine;
        
        public void Initialize(int maxHealth)
        {
            maxValue = maxHealth;
            currentValue = maxHealth;
            CreateHealthBar();
            UpdateDisplay();
        }
        
        private void CreateHealthBar()
        {
            // Background
            GameObject bgObject = new GameObject("Background");
            bgObject.transform.SetParent(transform, false);
            
            backgroundImage = bgObject.AddComponent<Image>();
            backgroundImage.color = Color.black;
            
            RectTransform bgRect = bgObject.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Fill
            GameObject fillObject = new GameObject("Fill");
            fillObject.transform.SetParent(transform, false);
            
            fillImage = fillObject.AddComponent<Image>();
            fillImage.color = Color.red;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            
            RectTransform fillRect = fillObject.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            
            // Text
            GameObject textObject = new GameObject("HealthText");
            textObject.transform.SetParent(transform, false);
            
            healthText = textObject.AddComponent<Text>();
            healthText.text = $"{currentValue}/{maxValue}";
            healthText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            healthText.fontSize = 12;
            healthText.color = Color.white;
            healthText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        
        public void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, maxValue);
            
            if (updateCoroutine != null)
                StopCoroutine(updateCoroutine);
                
            updateCoroutine = StartCoroutine(AnimateToValue(newValue));
        }
        
        public void SetMaxValue(int newMaxValue)
        {
            float ratio = (float)currentValue / maxValue;
            maxValue = newMaxValue;
            currentValue = Mathf.RoundToInt(maxValue * ratio);
            UpdateDisplay();
        }
        
        private IEnumerator AnimateToValue(int targetValue)
        {
            int startValue = currentValue;
            float elapsed = 0f;
            float duration = 0.3f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
                UpdateDisplay();
                yield return null;
            }
            
            currentValue = targetValue;
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = (float)currentValue / maxValue;
                
                // Change color based on health percentage
                float healthPercent = fillImage.fillAmount;
                if (healthPercent > 0.6f)
                    fillImage.color = Color.green;
                else if (healthPercent > 0.3f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.red;
            }
            
            if (healthText != null)
                healthText.text = $"{currentValue}/{maxValue}";
        }
    }
    
    /// <summary>
    /// Mana bar UI component
    /// </summary>
    public class ManaBar : MonoBehaviour
    {
        [Header("Mana Bar")]
        public Image backgroundImage;
        public Image fillImage;
        public Text manaText;
        
        private int maxValue;
        private int currentValue;
        private Coroutine updateCoroutine;
        
        public void Initialize(int maxMana)
        {
            maxValue = maxMana;
            currentValue = maxMana;
            CreateManaBar();
            UpdateDisplay();
        }
        
        private void CreateManaBar()
        {
            // Background
            GameObject bgObject = new GameObject("Background");
            bgObject.transform.SetParent(transform, false);
            
            backgroundImage = bgObject.AddComponent<Image>();
            backgroundImage.color = Color.black;
            
            RectTransform bgRect = bgObject.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Fill
            GameObject fillObject = new GameObject("Fill");
            fillObject.transform.SetParent(transform, false);
            
            fillImage = fillObject.AddComponent<Image>();
            fillImage.color = Color.blue;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            
            RectTransform fillRect = fillObject.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            
            // Text
            GameObject textObject = new GameObject("ManaText");
            textObject.transform.SetParent(transform, false);
            
            manaText = textObject.AddComponent<Text>();
            manaText.text = $"{currentValue}/{maxValue}";
            manaText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            manaText.fontSize = 12;
            manaText.color = Color.white;
            manaText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        
        public void SetValue(int newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, maxValue);
            
            if (updateCoroutine != null)
                StopCoroutine(updateCoroutine);
                
            updateCoroutine = StartCoroutine(AnimateToValue(newValue));
        }
        
        public void SetMaxValue(int newMaxValue)
        {
            float ratio = (float)currentValue / maxValue;
            maxValue = newMaxValue;
            currentValue = Mathf.RoundToInt(maxValue * ratio);
            UpdateDisplay();
        }
        
        private IEnumerator AnimateToValue(int targetValue)
        {
            int startValue = currentValue;
            float elapsed = 0f;
            float duration = 0.3f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
                UpdateDisplay();
                yield return null;
            }
            
            currentValue = targetValue;
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = (float)currentValue / maxValue;
                
                // Keep blue color for mana
                fillImage.color = Color.blue;
            }
            
            if (manaText != null)
                manaText.text = $"{currentValue}/{maxValue}";
        }
    }
}
