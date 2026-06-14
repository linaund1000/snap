using UnityEngine;
using UnityEngine.UI;
using GPOyun.Core;
using GPOyun.NPC;

namespace GPOyun.UI
{
    /// <summary>
    /// Target Inspector UI (State 3: Inspecting).
    /// Raycasts from the camera center. If an NPC is looked at, it shows a sleek floating card.
    /// Uses Moore Machine design: Output purely depends on the active inspected target.
    /// </summary>
    public class TargetInspectorUI : MonoBehaviour
    {
        private static TargetInspectorUI _instance;
        public static TargetInspectorUI Instance
        {
            get
            {
                if (_instance == null) _instance = Object.FindAnyObjectByType<TargetInspectorUI>();
                return _instance;
            }
        }

        [Header("Settings")]
        public float maxRaycastDistance = 25f;
        public float cardLerpSpeed = 10f;
        public Vector2 cardScreenOffset = new Vector2(100f, 0f);

        [Header("UI Elements")]
        private CanvasGroup _canvasGroup;
        private RectTransform _cardRect;
        private Text _nameText;
        private Text _emotionText;
        private Text _trustText;

        private NPCController _currentTarget;
        public NPCController CurrentTarget => _currentTarget;

        private void Awake()
        {
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this;
            SetupProceduralUI();
        }

        private void Update()
        {
            if (Camera.main == null || UIManager.Instance.IsAnyMenuOpen())
            {
                SetTarget(null);
                UpdateCardFade(false);
                return;
            }

            bool needsSetup = false;
            try
            {
                if (_cardRect == null || _cardRect.gameObject == null)
                {
                    needsSetup = true;
                }
            }
            catch (System.Exception)
            {
                needsSetup = true;
            }

            if (needsSetup)
            {
                SetupProceduralUI();
            }

            // 1. Raycast to find NPC
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance))
            {
                NPCController npc = hit.collider.GetComponentInParent<NPCController>();
                SetTarget(npc);
            }
            else
            {
                SetTarget(null);
            }

            // 2. Update UI Position and Fade
            bool hasTarget = _currentTarget != null;
            UpdateCardFade(hasTarget);

            if (hasTarget)
            {
                UpdateCardPosition();
                UpdateCardData();
            }
        }

        private void SetTarget(NPCController newTarget)
        {
            if (_currentTarget != newTarget)
            {
                _currentTarget = newTarget;
            }
        }

        private void UpdateCardFade(bool isVisible)
        {
            if (_canvasGroup == null) return;
            float targetAlpha = isVisible ? 1f : 0f;
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, targetAlpha, Time.deltaTime * cardLerpSpeed);
        }

        private void UpdateCardPosition()
        {
            if (_cardRect == null || Camera.main == null || _currentTarget == null) return;

            // Project NPC head to screen space
            Vector3 worldPos = _currentTarget.transform.position + Vector3.up * 1.8f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0)
            {
                // Smooth follow
                Vector2 targetPos = new Vector2(screenPos.x, screenPos.y) + cardScreenOffset;
                _cardRect.position = Vector3.Lerp(_cardRect.position, targetPos, Time.deltaTime * cardLerpSpeed);
            }
            else
            {
                _canvasGroup.alpha = 0f; // Hide if behind camera
            }
        }

        private void UpdateCardData()
        {
            if (_currentTarget == null) return;

            _nameText.text = _currentTarget.NpcName;
            _emotionText.text = "Mood: " + _currentTarget.currentEmotion.ToString().ToUpper();
            
            string trustString = "Opinion of You: Neutral";
            Color trustColor = new Color(0.8f, 0.8f, 0.8f);

            int playerTrust = _currentTarget.relationshipWithPlayer;
            if (playerTrust >= 50)
            {
                trustString = "Opinion of You: ALLY";
                trustColor = new Color(0.3f, 0.8f, 0.4f);
            }
            else if (playerTrust <= -50)
            {
                trustString = "Opinion of You: RIVAL";
                trustColor = new Color(0.9f, 0.3f, 0.3f);
            }
            else
            {
                trustString = $"Opinion of You: {playerTrust}";
            }

            _trustText.text = trustString;
            _trustText.color = trustColor;
        }

        private void SetupProceduralUI()
        {
            Canvas canvas = VisualUtils.CreateBaseCanvas("TARGET_INSPECTOR_CANVAS", 100, transform);
            // This needs to be overlay so it floats above the world easily
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            _canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            // Card Background (Frosted Glass style)
            GameObject cardBg = new GameObject("CardBG");
            cardBg.transform.SetParent(canvas.transform, false);
            var img = cardBg.AddComponent<Image>();
            img.color = new Color(0.05f, 0.05f, 0.08f, 0.85f); // Dark translucent
            _cardRect = cardBg.GetComponent<RectTransform>();
            _cardRect.sizeDelta = new Vector2(240, 100);
            _cardRect.pivot = new Vector2(0f, 0.5f); // Left-center pivot so it expands to the right
            
            // Name Text
            GameObject nameGo = new GameObject("NameText");
            nameGo.transform.SetParent(_cardRect, false);
            _nameText = nameGo.AddComponent<Text>();
            _nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _nameText.fontSize = 20;
            _nameText.fontStyle = FontStyle.Bold;
            _nameText.color = Color.white;
            _nameText.alignment = TextAnchor.MiddleLeft;
            var nameRect = nameGo.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 1); nameRect.anchorMax = new Vector2(1, 1);
            nameRect.pivot = new Vector2(0, 1);
            nameRect.anchoredPosition = new Vector2(15, -15);
            nameRect.sizeDelta = new Vector2(-30, 25);

            // Emotion Text
            GameObject emoGo = new GameObject("EmotionText");
            emoGo.transform.SetParent(_cardRect, false);
            _emotionText = emoGo.AddComponent<Text>();
            _emotionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _emotionText.fontSize = 14;
            _emotionText.color = new Color(0.7f, 0.7f, 0.7f);
            _emotionText.alignment = TextAnchor.MiddleLeft;
            var emoRect = emoGo.GetComponent<RectTransform>();
            emoRect.anchorMin = new Vector2(0, 1); emoRect.anchorMax = new Vector2(1, 1);
            emoRect.pivot = new Vector2(0, 1);
            emoRect.anchoredPosition = new Vector2(15, -45);
            emoRect.sizeDelta = new Vector2(-30, 20);

            // Trust Text
            GameObject trustGo = new GameObject("TrustText");
            trustGo.transform.SetParent(_cardRect, false);
            _trustText = trustGo.AddComponent<Text>();
            _trustText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _trustText.fontSize = 14;
            _trustText.fontStyle = FontStyle.Bold;
            _trustText.alignment = TextAnchor.MiddleLeft;
            var trustRect = trustGo.GetComponent<RectTransform>();
            trustRect.anchorMin = new Vector2(0, 1); trustRect.anchorMax = new Vector2(1, 1);
            trustRect.pivot = new Vector2(0, 1);
            trustRect.anchoredPosition = new Vector2(15, -65);
            trustRect.sizeDelta = new Vector2(-30, 20);
        }
    }
}
