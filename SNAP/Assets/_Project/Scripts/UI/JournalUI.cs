using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GPOyun.Core;

namespace GPOyun.UI
{
    /// <summary>
    /// Observational Journal UI. 
    /// Visualizes relationships, global feed, and targeted NPC focus.
    /// Built procedurally.
    /// </summary>
    public class JournalUI : MonoBehaviour
    {
        private static JournalUI _instance;
        public static JournalUI Instance
        {
            get
            {
                if (_instance == null) _instance = Object.FindAnyObjectByType<JournalUI>();
                return _instance;
            }
        }

        public CanvasGroup journalCanvasGroup;
        private List<GameObject> _cellPool = new List<GameObject>();
        
        private NPC.NPCController _focusedTarget;

        private void Awake()
        {
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this;
        }

        private void Start()
        {
            SetupUI();
            Hide();
        }

        private float _updateTimer = 0f;
        private void Update()
        {
            if (journalCanvasGroup.alpha <= 0) return;
            
            _updateTimer += Time.deltaTime;
            if (_updateTimer >= 1.0f) // Refresh UI every second
            {
                _updateTimer = 0f;
                if (journalCanvasGroup != null && journalCanvasGroup.alpha > 0)
                {
                    // Find which one is active
                    string state = "Matrix";
                    Transform canvasTransform = transform.Find("JOURNAL_CANVAS");
                    if (canvasTransform != null)
                    {
                        var feed = canvasTransform.Find("FeedContainer");
                        var focus = canvasTransform.Find("FocusContainer");
                        if (feed != null && feed.gameObject.activeSelf) state = "Feed";
                        if (focus != null && focus.gameObject.activeSelf) state = "Focus";
                    }
                    RebuildUIForCurrentState(state);
                }
            }
        }

        public void ShowMatrix()
        {
            _focusedTarget = null;
            RebuildUIForCurrentState("Matrix");
            GPOyun.UI.UIManager.Instance?.PushMenu(gameObject);
            Apply(1f, true, true);
        }

        public void ShowFeed()
        {
            _focusedTarget = null;
            RebuildUIForCurrentState("Feed");
            GPOyun.UI.UIManager.Instance?.PushMenu(gameObject);
            Apply(1f, true, true);
        }

        public void ShowFocus(NPC.NPCController target)
        {
            _focusedTarget = target;
            RebuildUIForCurrentState("Focus");
            GPOyun.UI.UIManager.Instance?.PushMenu(gameObject);
            Apply(1f, true, true);
        }

        public void Hide()
        {
            _focusedTarget = null;
            GPOyun.UI.UIManager.Instance?.PopMenu(gameObject);
            Apply(0f, false, false);
        }

        private void RebuildUIForCurrentState(string stateName)
        {
            ClearPool();
            SetupUI(); // Ensure canvas exists

            // Hide/Show sub-panels based on state
            Transform canvasTransform = transform.Find("JOURNAL_CANVAS");
            if (canvasTransform != null)
            {
                var matrixContainer = canvasTransform.Find("MatrixContainer");
                if (matrixContainer != null) matrixContainer.gameObject.SetActive(stateName == "Matrix");

                var feedContainer = canvasTransform.Find("FeedContainer");
                if (feedContainer != null) feedContainer.gameObject.SetActive(stateName == "Feed");

                var focusContainer = canvasTransform.Find("FocusContainer");
                if (focusContainer != null) focusContainer.gameObject.SetActive(stateName == "Focus");
            }

            if (stateName == "Matrix")
            {
                BuildMatrix();
            }
            else if (stateName == "Feed")
            {
                BuildFeed();
            }
            else if (stateName == "Focus")
            {
                BuildFocus(_focusedTarget);
            }
        }



        private Coroutine _fadeCoroutine;

        private void Apply(float targetAlpha, bool blocks, bool interact)
        {
            if (journalCanvasGroup == null) return;
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

            if (Application.isBatchMode)
            {
                journalCanvasGroup.alpha = targetAlpha;
                journalCanvasGroup.blocksRaycasts = blocks;
                journalCanvasGroup.interactable = interact;
            }
            else
            {
                _fadeCoroutine = StartCoroutine(VisualUtils.FadeGroupCoroutine(journalCanvasGroup, targetAlpha, 0.15f, blocks, interact));
            }
        }

        private void ClearPool()
        {
            foreach (var cell in _cellPool) Destroy(cell);
            _cellPool.Clear();
        }

        private void BuildMatrix()
        {
            var npcs = FindObjectsByType<NPC.NPCController>(FindObjectsInactive.Include);
            int count = npcs.Length;
            if (count == 0)
            {
                Debug.LogWarning("[JournalUI] BuildMatrix found 0 NPCs! Matrix will be empty.");
                return;
            }

            // Sort NPCs by ID to have a consistent matrix
            System.Array.Sort(npcs, (a, b) => a.NpcId.CompareTo(b.NpcId));

            var rm = ServiceLocator.Get<RelationshipMatrix>();

            Transform canvasTransform = transform.Find("JOURNAL_CANVAS");
            if (canvasTransform == null) return;
            Transform matrixContainer = canvasTransform.Find("MatrixContainer");
            if (matrixContainer == null) return;

            // Setup Grid Layout
            var grid = matrixContainer.GetComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = count + 1; // +1 for the header column
            grid.cellSize = new Vector2(22, 22);
            grid.spacing = new Vector2(2, 2);

            // 1. Top-Left Empty Corner
            CreateTextCell("", VisualUtils.StuccoWhite, true, matrixContainer);

            // 2. Top Header Row (Target NPCs)
            for (int i = 0; i < count; i++)
            {
                CreateTextCell(FormatName(npcs[i].NpcName), VisualUtils.StuccoWhite, true, matrixContainer);
            }

            // 3. Rows
            for (int row = 0; row < count; row++)
            {
                // Row Header (Source NPC)
                CreateTextCell(FormatName(npcs[row].NpcName), VisualUtils.StuccoWhite, true, matrixContainer);

                for (int col = 0; col < count; col++)
                {
                    if (row == col)
                    {
                        CreateTextCell("-", Color.gray, false, matrixContainer);
                    }
                    else
                    {
                        int rel = rm != null ? rm.GetRelationship(npcs[row].NpcId, npcs[col].NpcId) : 0;
                        int trust = rm != null ? rm.GetTrust(npcs[row].NpcId, npcs[col].NpcId) : 50;

                        string text = rel.ToString();
                        Color bgColor = Color.gray;

                        if (rel > 50) bgColor = VisualUtils.PineGreen;
                        else if (rel < -50) bgColor = VisualUtils.Terracotta;
                        else if (rel > 0) bgColor = new Color(0.3f, 0.5f, 0.3f);
                        else if (rel < 0) bgColor = new Color(0.5f, 0.3f, 0.3f);

                        // Trust indicators
                        if (trust <= -50) text += " 🐍";
                        if (trust >= 80 && rel >= 80) text += " ❤️";

                        CreateColorCell(text, bgColor, matrixContainer);
                    }
                }
            }
        }

        private void BuildFeed()
        {
            Transform canvasTransform = transform.Find("JOURNAL_CANVAS");
            if (canvasTransform == null) return;
            Transform feedContainer = canvasTransform.Find("FeedContainer");
            if (feedContainer == null) return;

            // Clear previous feed items
            foreach (Transform child in feedContainer) Destroy(child.gameObject);

            var events = GPOyun.Core.GlobalEventLogger.GetRecentEvents();

            // Create title
            CreateTextCell("GLOBAL EVENT FEED (Last 20 Events)", VisualUtils.StuccoWhite, true, feedContainer);

            int displayCount = Mathf.Min(20, events.Count);
            if (displayCount == 0)
            {
                CreateTextCell("No events yet...", new Color(0.6f, 0.6f, 0.6f), false, feedContainer);
                return;
            }

            for (int i = 0; i < displayCount; i++)
            {
                CreateTextCell(events[i], new Color(0.8f, 0.8f, 0.8f), false, feedContainer);
            }
        }

        private void BuildFocus(NPC.NPCController target)
        {
            Transform canvasTransform = transform.Find("JOURNAL_CANVAS");
            if (canvasTransform == null) return;
            Transform focusContainer = canvasTransform.Find("FocusContainer");
            if (focusContainer == null) return;

            // Clear previous items
            foreach (Transform child in focusContainer) Destroy(child.gameObject);

            if (target == null)
            {
                CreateTextCell("No Target Selected", VisualUtils.Terracotta, true, focusContainer);
                return;
            }

            CreateTextCell($"FOCUS: {target.NpcName.ToUpper()}", VisualUtils.StuccoWhite, true, focusContainer);

            // Left side logic / Simple list for now
            CreateTextCell("--- RECENT MEMORIES ---", VisualUtils.StuccoWhite, true, focusContainer);
            var memStream = target.GetComponent<NPC.Memory.NPCMemoryStream>();
            if (memStream != null)
            {
                var memories = memStream.GetMemorySnapshot();
                memories.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp)); // Newest first

                int displayCount = Mathf.Min(10, memories.Count);
                for (int i = 0; i < displayCount; i++)
                {
                    var evt = memories[i];
                    string timeStr = System.TimeSpan.FromSeconds(evt.Timestamp).ToString(@"mm\:ss");
                    CreateTextCell($"[{timeStr}] {evt.Trigger} -> {evt.FeltEmotion}", new Color(0.8f, 0.8f, 0.8f), false, focusContainer);
                }
            }

            CreateTextCell("--- RELATIONSHIPS ---", VisualUtils.StuccoWhite, true, focusContainer);
            var rm = ServiceLocator.Get<RelationshipMatrix>();
            if (rm != null)
            {
                var npcs = FindObjectsByType<NPC.NPCController>(FindObjectsInactive.Include);
                foreach (var other in npcs)
                {
                    if (other.NpcId == target.NpcId) continue;
                    int rel = rm.GetRelationship(target.NpcId, other.NpcId);
                    int trust = rm.GetTrust(target.NpcId, other.NpcId);
                    
                    Color color = Color.gray;
                    if (rel > 50) color = VisualUtils.PineGreen;
                    else if (rel < -50) color = VisualUtils.Terracotta;

                    string relText = $"{other.NpcName}: Rel({rel}) Trust({trust})";
                    CreateTextCell(relText, color, false, focusContainer);
                }
            }
        }

        private string FormatName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "?";
            var parts = fullName.Split(' ');
            return parts[0].Length > 6 ? parts[0].Substring(0, 6) + "." : parts[0];
        }

        private Color GetRelationshipColor(int score)
        {
            // Range: -100 to 100
            if (score > 50) return new Color(0.2f, 0.6f, 0.3f, 0.8f); // Friendly Green
            if (score > 10) return new Color(0.4f, 0.6f, 0.4f, 0.5f); // Mild Green
            if (score < -50) return new Color(0.8f, 0.2f, 0.2f, 0.8f); // Enemy Red
            if (score < -10) return new Color(0.6f, 0.4f, 0.4f, 0.5f); // Mild Red
            return new Color(0.3f, 0.3f, 0.35f, 0.5f); // Neutral Gray
        }

        private string GetTrustIcon(int trust)
        {
            if (trust < 30) return "🐍"; // Snake / Untrustworthy
            if (trust > 80) return "🤝"; // Handshake / High Trust
            return "";
        }

        private void CreateTextCell(string text, Color textColor, bool isHeader, Transform parent)
        {
            GameObject cell = new GameObject("Cell_Text");
            cell.transform.SetParent(parent, false);
            
            var bg = cell.AddComponent<Image>();
            bg.color = isHeader ? new Color(0.1f, 0.1f, 0.15f, 0.8f) : new Color(0,0,0,0);

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(cell.transform, false);
            var txt = txtGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 9;
            txt.color = textColor;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.text = text;

            var rect = txtGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;

            _cellPool.Add(cell);
        }

        private void CreateColorCell(string text, Color bgColor, Transform parent)
        {
            GameObject cell = new GameObject("Cell_Color");
            cell.transform.SetParent(parent, false);
            
            var bg = cell.AddComponent<Image>();
            bg.color = bgColor;

            if (!string.IsNullOrEmpty(text))
            {
                GameObject txtGo = new GameObject("Icon");
                txtGo.transform.SetParent(cell.transform, false);
                var txt = txtGo.AddComponent<Text>();
                txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                txt.fontSize = 10;
                txt.color = Color.white;
                txt.alignment = TextAnchor.MiddleCenter;
                txt.text = text;

                var rect = txtGo.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }

            _cellPool.Add(cell);
        }

        private void SetupUI()
        {
            if (journalCanvasGroup != null) return;
            
            Canvas canvas = VisualUtils.CreateBaseCanvas("JOURNAL_CANVAS", 800, transform);
            journalCanvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            journalCanvasGroup.alpha = 0f;
            journalCanvasGroup.blocksRaycasts = false;
            journalCanvasGroup.interactable = false;

            // Blur/Frosted Glass background
            GameObject bg = new GameObject("BG");
            bg.transform.SetParent(canvas.transform, false);
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.05f, 0.05f, 0.08f, 0.98f);
            var bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;

            // Title
            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(canvas.transform, false);
            var titleTxt = titleGo.AddComponent<Text>();
            titleTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleTxt.fontSize = 40; titleTxt.color = VisualUtils.StuccoWhite;
            titleTxt.text = "SOCIAL NETWORK MATRIX  [J]";
            titleTxt.alignment = TextAnchor.MiddleCenter;
            var titleRect = titleGo.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1); titleRect.anchorMax = new Vector2(1, 1);
            titleRect.anchoredPosition = new Vector2(0, -60); titleRect.sizeDelta = new Vector2(0, 60);

            // Subtitle instructions
            GameObject subGo = new GameObject("Subtitle");
            subGo.transform.SetParent(canvas.transform, false);
            var subTxt = subGo.AddComponent<Text>();
            subTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            subTxt.fontSize = 16; subTxt.color = new Color(0.6f, 0.6f, 0.6f);
            subTxt.text = "Rows = Source NPC | Columns = Target NPC | Green = Friendly, Red = Hostile | 🐍 = Untrustworthy, ❤️ = Love";
            subTxt.alignment = TextAnchor.MiddleCenter;
            var subRect = subGo.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0, 1); subRect.anchorMax = new Vector2(1, 1);
            subRect.anchoredPosition = new Vector2(0, -100); subRect.sizeDelta = new Vector2(0, 30);

            // Matrix Container
            Transform existingMatrix = canvas.transform.Find("MatrixContainer");
            GameObject matrixGo;
            if (existingMatrix == null)
            {
                matrixGo = new GameObject("MatrixContainer");
                matrixGo.transform.SetParent(canvas.transform, false);
                
                var matrixRect = matrixGo.AddComponent<RectTransform>();
                // Bounds the matrix to center 70% of screen so blurred background is visible
                matrixRect.anchorMin = new Vector2(0.15f, 0.15f);
                matrixRect.anchorMax = new Vector2(0.85f, 0.85f);
                matrixRect.offsetMin = Vector2.zero;
                matrixRect.offsetMax = Vector2.zero;

                var grid = matrixGo.AddComponent<GridLayoutGroup>();
                grid.childAlignment = TextAnchor.MiddleCenter;
            }
            else
            {
                matrixGo = existingMatrix.gameObject;
            }

            // Feed Container
            Transform existingFeed = canvas.transform.Find("FeedContainer");
            if (existingFeed == null)
            {
                var feedGo = new GameObject("FeedContainer");
                feedGo.transform.SetParent(canvas.transform, false);
                var feedRect = feedGo.AddComponent<RectTransform>();
                feedRect.anchorMin = new Vector2(0.2f, 0.1f);
                feedRect.anchorMax = new Vector2(0.8f, 0.9f);
                feedRect.offsetMin = Vector2.zero; feedRect.offsetMax = Vector2.zero;

                var vLayout = feedGo.AddComponent<VerticalLayoutGroup>();
                vLayout.childAlignment = TextAnchor.UpperCenter;
                vLayout.spacing = 8f;
                
                var feedBgImg = feedGo.AddComponent<Image>();
                feedBgImg.color = new Color(0.05f, 0.05f, 0.08f, 0.95f);
                
                feedGo.SetActive(false);
            }

            // Focus Container
            Transform existingFocus = canvas.transform.Find("FocusContainer");
            if (existingFocus == null)
            {
                var focusGo = new GameObject("FocusContainer");
                focusGo.transform.SetParent(canvas.transform, false);
                var focusRect = focusGo.AddComponent<RectTransform>();
                focusRect.anchorMin = new Vector2(0.2f, 0.1f);
                focusRect.anchorMax = new Vector2(0.8f, 0.9f);
                focusRect.offsetMin = Vector2.zero; focusRect.offsetMax = Vector2.zero;

                var vLayout = focusGo.AddComponent<VerticalLayoutGroup>();
                vLayout.childAlignment = TextAnchor.UpperCenter;
                vLayout.spacing = 8f;
                focusGo.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                
                var focusBgImg = focusGo.AddComponent<Image>();
                focusBgImg.color = new Color(0.05f, 0.05f, 0.08f, 0.95f);
                
                focusGo.SetActive(false);
            }

            VisualUtils.EnsureCanvasRenderers(transform);
        }
    }
}
