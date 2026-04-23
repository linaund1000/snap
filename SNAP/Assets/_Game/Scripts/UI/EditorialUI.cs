using UnityEngine;
using UnityEngine.UI;
using GPOyun.Events;
using GPOyun.Newspaper;
using GPOyun.Core;
using GPOyun.Managers;
using System.Collections.Generic;

namespace GPOyun.UI
{
    /// <summary>
    /// The "Tomorrow's Edition" interface.
    /// Appears in the Evening to let the player choose headlines.
    /// </summary>
    public class EditorialUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup editorialCanvasGroup;
        [SerializeField] private RectTransform rollContainer;
        [SerializeField] private Text statusText;

        private bool _isExplicitlyOpen = false;
        private bool _wasAutoOpenedToday = false;

        private void Start()
        {
            if (EventBus.Instance != null)
            {
                EventBus.Instance.Subscribe<DayPhaseChangedEvent>(OnPhaseChanged);
            }
            
            Hide();
        }

        private void Update()
        {
            HandleInputs();
            CheckTimedAutoOpen();
        }

        private void HandleInputs()
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard != null && keyboard.nKey.wasPressedThisFrame)
            {
                if (_isExplicitlyOpen) Hide();
                else Show();
            }
        }

        private void CheckTimedAutoOpen()
        {
            if (TimeManager.Instance == null) return;

            float hour = TimeManager.Instance.GetCurrentHour();
            
            // Auto open at 10 PM (22:00) if not already opened
            if (hour >= 22f && !_wasAutoOpenedToday && !_isExplicitlyOpen)
            {
                _wasAutoOpenedToday = true;
                Show();
                statusText.text = "NIGHT SHIFT: SELECT TOMORROW'S HEADLINES";
            }

            // Reset flag at 6 AM
            if (hour >= 6f && hour < 7f) _wasAutoOpenedToday = false;
        }

        private void OnPhaseChanged(DayPhaseChangedEvent ev)
        {
            // Transitions handled by time check mostly, but phases can trigger specific logic
        }

        public void Show()
        {
            if (editorialCanvasGroup == null) SetupMockUI();
            
            editorialCanvasGroup.alpha = 1f;
            editorialCanvasGroup.blocksRaycasts = true;
            editorialCanvasGroup.interactable = true;
            _isExplicitlyOpen = true;
            
            statusText.text = "EDITORIAL DESK";
            Debug.Log("[EditorialUI] Workspace active.");
        }

        public void Hide()
        {
            if (editorialCanvasGroup != null)
            {
                editorialCanvasGroup.alpha = 0f;
                editorialCanvasGroup.blocksRaycasts = false;
                editorialCanvasGroup.interactable = false;
            }
            _isExplicitlyOpen = false;
        }

        private void SetupMockUI()
        {
            Canvas canvas = VisualUtils.CreateBaseCanvas("EDITORIAL_UI", 500, transform);
            editorialCanvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            
            // Background (Mediterranean Stucco)
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform);
            var img = bg.AddComponent<Image>();
            img.color = new Color(0.98f, 0.95f, 0.92f, 0.98f);
            var rt = bg.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(50, 50); rt.offsetMax = new Vector2(-50, -50);

            // Title
            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(canvas.transform);
            statusText = titleGo.AddComponent<Text>();
            statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            statusText.fontSize = 32;
            statusText.color = Color.black;
            statusText.alignment = TextAnchor.UpperCenter;
            var titleRt = titleGo.GetComponent<RectTransform>();
            titleRt.anchoredPosition = new Vector2(0, 300);
            titleRt.sizeDelta = new Vector2(800, 100);
            titleRt.anchorMin = new Vector2(0.5f, 0.5f); titleRt.anchorMax = new Vector2(0.5f, 0.5f);

            // --- 3-SLOT LAYOUT (6:3:1 Proportions) ---
            float panelWidth = 1000f; // Mock canvas width-ish
            float spacing = 20f;
            float totalWidth = panelWidth - (spacing * 4);
            
            float w6 = totalWidth * 0.6f;
            float w3 = totalWidth * 0.3f;
            float w1 = totalWidth * 0.1f;

            CreateSlot("MainSlot", new Vector2(-(w3 + w1) / 2 - spacing, 0), new Vector2(w6, 400), "PRIMARY STORY (SIZE 6)", VisualUtils.CobaltBlue);
            CreateSlot("SecondSlot", new Vector2((w6 - w1) / 2 + spacing, 50), new Vector2(w3, 300), "SND STORY (3)", VisualUtils.Terracotta);
            CreateSlot("MinorSlot", new Vector2((w6 + w3) / 2 + spacing*2, -100), new Vector2(w1, 150), "MINOR (1)", Color.gray);

            // PUBLISH BUTTON (Sticky Bottom)
            GameObject btnGo = new GameObject("PublishButton");
            btnGo.transform.SetParent(canvas.transform);
            var btnImg = btnGo.AddComponent<Image>();
            btnImg.color = VisualUtils.CobaltBlue;
            var btn = btnGo.AddComponent<Button>();
            var btnRt = btnGo.GetComponent<RectTransform>();
            btnRt.anchoredPosition = new Vector2(0, -350);
            btnRt.sizeDelta = new Vector2(250, 50);
            btnRt.anchorMin = new Vector2(0.5f, 0.5f); btnRt.anchorMax = new Vector2(0.5f, 0.5f);
            
            GameObject btnTextGo = new GameObject("Text");
            btnTextGo.transform.SetParent(btnGo.transform);
            var bt = btnTextGo.AddComponent<Text>();
            bt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            bt.text = "PUBLISH EDITION";
            bt.color = Color.white;
            bt.alignment = TextAnchor.MiddleCenter;
            btnTextGo.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 50);

            btn.onClick.AddListener(OnPublishClicked);
        }

        private void CreateSlot(string name, Vector2 pos, Vector2 size, string label, Color accent)
        {
            GameObject slot = new GameObject(name);
            slot.transform.SetParent(editorialCanvasGroup.transform);
            RectTransform rt = slot.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);

            // Frame
            Image img = slot.AddComponent<Image>();
            img.color = new Color(0,0,0,0.05f);

            // Label
            GameObject labelGo = new GameObject("Label");
            labelGo.transform.SetParent(slot.transform);
            Text t = labelGo.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.text = label;
            t.fontSize = (int)(size.x / 10f);
            t.color = accent;
            t.alignment = TextAnchor.LowerCenter;
            RectTransform lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchoredPosition = new Vector2(0, -size.y/2 - 20);
            lrt.sizeDelta = new Vector2(size.x, 40);
        }

        private void OnPublishClicked()
        {
            Debug.Log("[EditorialUI] Selection confirmed.");
            
            NewsStory front = new NewsStory { Headline = "A Quiet Day", Category = NewsCategory.Local };
            NewspaperManager.Instance.PublishEdition(front);
            
            Hide();
        }
    }
}
