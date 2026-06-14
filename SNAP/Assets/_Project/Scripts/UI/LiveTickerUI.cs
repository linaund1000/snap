using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GPOyun.Core;

namespace GPOyun.UI
{
    public class LiveTickerUI : MonoBehaviour
    {
        public static LiveTickerUI Instance { get; private set; }

        private const int MAX_LINES = 15;
        private const float LINE_LIFETIME = 15f;

        private class TickerLine
        {
            public GameObject go;
            public Text textComponent;
            public float timeCreated;
        }

        private List<TickerLine> _lines = new List<TickerLine>();
        private Transform _container;
        private Font _font;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            SetupUI();
        }

        private void SetupUI()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Create Canvas if it doesn't exist on this object
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Above HUD, below Menus
            
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Container for vertical layout
            var containerGo = new GameObject("TickerContainer");
            _container = containerGo.transform;
            _container.SetParent(transform, false);

            var containerRect = containerGo.AddComponent<RectTransform>();
            // Anchor to bottom left, but with some safe margin
            containerRect.anchorMin = new Vector2(0, 0);
            containerRect.anchorMax = new Vector2(0, 0);
            containerRect.pivot = new Vector2(0, 0);
            containerRect.anchoredPosition = new Vector2(30, 30);
            containerRect.sizeDelta = new Vector2(600, 400); // larger width

            var vlg = containerGo.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.LowerLeft;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.spacing = 2;

            AddLine("<i>Live Ticker initialized... Waiting for events.</i>");
        }

        private void OnEnable()
        {
            GlobalEventLogger.OnEventLogged += AddLine;
        }

        private void OnDisable()
        {
            GlobalEventLogger.OnEventLogged -= AddLine;
        }

        public void AddLine(string message)
        {
            if (_container == null) return;

            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            string formattedMessage = $"<color=#888888>[{timestamp}]</color> {message}";

            var lineGo = new GameObject("TickerLine");
            lineGo.transform.SetParent(_container, false);

            var txt = lineGo.AddComponent<Text>();
            txt.font = _font != null ? _font : Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = 16;
            txt.color = Color.white;
            txt.raycastTarget = false;
            txt.alignment = TextAnchor.MiddleLeft;
            
            var outline = lineGo.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(0, 0, 0, 0.8f);
            outline.effectDistance = new Vector2(1, -1);
            txt.supportRichText = true;
            txt.text = formattedMessage;

            outline.effectDistance = new Vector2(1, -1);

            _lines.Add(new TickerLine
            {
                go = lineGo,
                textComponent = txt,
                timeCreated = Time.time
            });

            // Enforce max lines
            if (_lines.Count > MAX_LINES)
            {
                Destroy(_lines[0].go);
                _lines.RemoveAt(0);
            }
        }

        private void Update()
        {
            float currentTime = Time.time;
            for (int i = _lines.Count - 1; i >= 0; i--)
            {
                var line = _lines[i];
                if (line.go == null)
                {
                    _lines.RemoveAt(i);
                    continue;
                }

                float age = currentTime - line.timeCreated;
                if (age > LINE_LIFETIME)
                {
                    Destroy(line.go);
                    _lines.RemoveAt(i);
                }
                else if (age > LINE_LIFETIME - 2f) // Fade out in last 2 seconds
                {
                    float alpha = (LINE_LIFETIME - age) / 2f;
                    Color c = line.textComponent.color;
                    c.a = alpha;
                    line.textComponent.color = c;
                }
            }
        }
    }
}
