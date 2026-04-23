using UnityEngine;
using UnityEngine.UI;

namespace GPOyun.Core
{
    /// <summary>
    /// Centralized aesthetic utility for the Mediterranean Minimalist 'Zero-Asset' project.
    /// Handles URP material generation and provides the curated color palette.
    /// </summary>
    public static class VisualUtils
    {
        // --- Color Tokens (Mediterranean Palette) ---
        public static readonly Color StuccoWhite   = new Color(0.98f, 0.95f, 0.92f); // Primary walls
        public static readonly Color Terracotta     = new Color(0.85f, 0.35f, 0.2f); // Roofs and trims
        public static readonly Color CobaltBlue     = new Color(0.05f, 0.3f, 0.7f);  // Player and accent doors
        public static readonly Color CrimsonRed     = new Color(0.8f, 0.1f, 0.1f);   // Main Player (Distinct)
        public static readonly Color SlateGrey      = new Color(0.35f, 0.35f, 0.4f); // Ground
        public static readonly Color FountainBlue   = new Color(0.4f, 0.7f, 0.95f);  // Water
        public static readonly Color WoodBrown      = new Color(0.4f, 0.25f, 0.15f); // Info boards
        public static readonly Color PineGreen      = new Color(0.1f, 0.35f, 0.2f);  // Mediterranean Trees
        public static readonly Color OliveGreen     = new Color(0.35f, 0.45f, 0.25f); // Bushes

        private static Shader _urpShader;

        /// <summary>
        /// Applies a specific color and ensures a URP-compatible material is used.
        /// This is the 'Nuclear Cure' for pink objects.
        /// </summary>
        public static void ApplyAesthetic(GameObject obj, Color color, float smoothness = 0.2f)
        {
            if (_urpShader == null)
            {
                _urpShader = Shader.Find("Universal Render Pipeline/Lit");
                if (_urpShader == null) _urpShader = Shader.Find("Standard"); // Fallback
            }

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) return;

            // Create a new unique material to avoid affecting other objects
            Material mat = new Material(_urpShader);
            mat.color = color;
            
            // URP specific properties
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);

            renderer.sharedMaterial = mat;
        }

        public static Color GetRandomSkinTone()
        {
            Color[] tones = {
                new Color(0.95f, 0.75f, 0.65f),
                new Color(0.85f, 0.65f, 0.55f),
                new Color(0.75f, 0.55f, 0.45f)
            };
            return tones[Random.Range(0, tones.Length)];
        }

        // --- Mock UI Helpers (Fixed Prefab Issue) ---

        public static Canvas CreateBaseCanvas(string name, int sortingOrder, Transform parent = null)
        {
            GameObject go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent);
            
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            go.AddComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return canvas;
        }

        public static void SetupMockHUD(GPOyun.UI.HUDManager hud)
        {
            Canvas canvas = CreateBaseCanvas("MOCK_HUD", 100, hud.transform);
            
            // Viewfinder Group
            GameObject vf = new GameObject("Viewfinder");
            vf.transform.SetParent(canvas.transform);
            RectTransform vfRect = vf.AddComponent<RectTransform>();
            vfRect.sizeDelta = new Vector2(400, 300);
            vfRect.anchoredPosition = Vector2.zero;
            
            // Corners (Simple Colored Panels)
            // Corners removed
            // Photo Count (Sticky Right)
            GameObject txtGo = new GameObject("PhotoText");
            txtGo.transform.SetParent(canvas.transform);
            Text txt = txtGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 32;
            txt.alignment = TextAnchor.UpperRight;
            txt.color = Color.white;
            RectTransform txtRect = txtGo.GetComponent<RectTransform>();
            txtRect.anchoredPosition = new Vector2(-50, -50);
            txtRect.anchorMax = Vector2.one;
            txtRect.anchorMin = Vector2.one;

            // Clock (Sticky Left - Cozy)
            GameObject clockGo = new GameObject("ClockText");
            clockGo.transform.SetParent(canvas.transform);
            Text clockTxt = clockGo.AddComponent<Text>();
            clockTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            clockTxt.fontSize = 28;
            clockTxt.alignment = TextAnchor.UpperLeft;
            clockTxt.color = StuccoWhite;
            RectTransform clockRect = clockGo.GetComponent<RectTransform>();
            clockRect.anchoredPosition = new Vector2(50, -50);
            clockRect.anchorMax = new Vector2(0, 1);
            clockRect.anchorMin = new Vector2(0, 1);
            
            hud.Initialize(vf, txt, clockTxt);
        }

        private static void CreateCorner(Transform parent, Vector2 pos)
        {
            GameObject go = new GameObject("Corner");
            go.transform.SetParent(parent);
            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(1, 1, 1, 0.5f);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(40, 40);
            rt.anchoredPosition = pos;
        }

        public static void SetupMockSplash(GPOyun.UI.SplashController splash)
        {
            Canvas canvas = CreateBaseCanvas("MOCK_SPLASH", 999, splash.transform);
            CanvasGroup cg = canvas.gameObject.AddComponent<CanvasGroup>();
            splash.Initialize(cg, null);
        }

        public static void SetupMockSettings(GPOyun.UI.SettingsController settings)
        {
            Canvas canvas = CreateBaseCanvas("MOCK_SETTINGS", 1000, settings.transform); // Topmost
            canvas.gameObject.SetActive(true); // Always on but hidden by alpha
            
            CanvasGroup cg = canvas.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            // Background (Semi-Transparent)
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform);
            var img = bg.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0, 0, 0, 0.85f);
            bg.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            bg.GetComponent<RectTransform>().anchorMax = Vector2.one;
            bg.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            bg.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            // Title
            GameObject txtGo = new GameObject("SettingsTitle");
            txtGo.transform.SetParent(canvas.transform);
            Text txt = txtGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 60;
            txt.text = "SETTINGS";
            txt.color = StuccoWhite;
            txt.alignment = TextAnchor.MiddleCenter;
            RectTransform titleRect = txtGo.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 200);
            titleRect.sizeDelta = new Vector2(400, 100);

            // Sensitivity Slider (Container)
            GameObject sensGo = CreateSlider(canvas.transform, "SensitivitySlider", new Vector2(0, 50), "LOOK SENSITIVITY");
            
            // Volume Slider (Container)
            GameObject volGo = CreateSlider(canvas.transform, "VolumeSlider", new Vector2(0, -50), "MASTER VOLUME");

            // Instructions
            GameObject hintGo = new GameObject("HintText");
            hintGo.transform.SetParent(canvas.transform);
            Text hint = hintGo.AddComponent<Text>();
            hint.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            hint.fontSize = 24;
            hint.text = "Press [ESC] to Resume\nPress [TAB] to switch Scene\nPress [N] for News Desk";
            hint.color = Color.gray;
            hint.alignment = TextAnchor.MiddleCenter;
            RectTransform hintRect = hintGo.GetComponent<RectTransform>();
            hintRect.anchoredPosition = new Vector2(0, -250);
            hintRect.sizeDelta = new Vector2(400, 100);

            settings.Initialize(cg);
        }

        private static GameObject CreateSlider(Transform parent, string name, Vector2 pos, string label)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent);
            RectTransform rt = root.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(400, 40);

            // Label
            GameObject labelGo = new GameObject("Label");
            labelGo.transform.SetParent(root.transform);
            Text t = labelGo.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.text = label;
            t.fontSize = 18;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleLeft;
            RectTransform lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchoredPosition = new Vector2(-150, 0);
            lrt.sizeDelta = new Vector2(200, 30);

            // Slider Background
            GameObject sliderGo = new GameObject("Slider");
            sliderGo.transform.SetParent(root.transform);
            Slider slider = sliderGo.AddComponent<Slider>();
            RectTransform srt = sliderGo.GetComponent<RectTransform>();
            srt.anchoredPosition = new Vector2(50, 0);
            srt.sizeDelta = new Vector2(200, 20);

            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(sliderGo.transform);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(1, 1, 1, 0.2f);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 10);

            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGo.transform);
            fillArea.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 10);

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform);
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = CobaltBlue;
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.fillRect.anchorMin = Vector2.zero;
            slider.fillRect.anchorMax = new Vector2(0, 1);
            slider.fillRect.offsetMin = Vector2.zero;
            slider.fillRect.offsetMax = Vector2.zero;

            slider.value = 0.5f; // Default
            return root;
        }

        public static void ApplyPlayerVisuals(GameObject player)
        {
            // Find the Head child (if it exists)
            Transform head = player.transform.Find("Head");
            if (head == null) return;

            // Add simple eyes for orientation
            CreateEye(head, new Vector3(0.15f, 0.1f, 0.35f));
            CreateEye(head, new Vector3(-0.15f, 0.1f, 0.35f));
        }

        private static void CreateEye(Transform parent, Vector3 localPos)
        {
            GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.name = "Eye";
            eye.transform.SetParent(parent);
            eye.transform.localPosition = localPos;
            eye.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            ApplyAesthetic(eye, Color.black, 0.8f);
            
            // Remove collider from eyes to avoid physics issues
            var col = eye.GetComponent<Collider>();
            if (col != null)
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying) UnityEngine.Object.DestroyImmediate(col);
                else UnityEngine.Object.Destroy(col);
                #else
                UnityEngine.Object.Destroy(col);
                #endif
            }
        }
    }
}
