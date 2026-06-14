using UnityEngine;
using UnityEngine.UI;
using GPOyun.Newspaper;

namespace GPOyun.UI
{
    /// <summary>
    /// A1 Level HUD Manager
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("UI Overlays")]
        public GameObject viewfinderGroup;
        public Text photoCountText;
        public Text clockText;

        [Header("Animation Settings")]
        public float pulseSpeed = 2f;
        public float pulseAmount = 0.1f;

        public static HUDManager Instance { get; private set; }

        [Header("Overlay Settings")]
        public bool relationshipOverlayActive = false;

        private int _totalPhotos = 5;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Initialize(GameObject viewfinder, Text photoText, Text clock)
        {
            viewfinderGroup = viewfinder;
            photoCountText = photoText;
            clockText = clock;
        }

        private void Start()
        {
            if (viewfinderGroup != null) viewfinderGroup.SetActive(false);
            UpdatePhotoUI();

            // Spawn the Live Ticker HUD
            if (LiveTickerUI.Instance == null)
            {
                var tickerGo = new GameObject("LiveTickerUI");
                tickerGo.AddComponent<LiveTickerUI>();
            }
        }

        private readonly System.Collections.Generic.List<EmojiReaction> _activeReactions = new();

        public void SpawnEmojiReaction(Transform targetNpc, string symbol, Color color)
        {
            string mappedSymbol = symbol;
            
            // Map bracket labels to rich Unicode Emojis
            if (symbol == "[FLAME]") mappedSymbol = "🔥 Rivalry!";
            else if (symbol == "[STAR]") mappedSymbol = "⭐ Hero!";
            else if (symbol == "[GIFT]") mappedSymbol = "🎁 Gift!";
            else if (symbol == "[PIZZA]") mappedSymbol = "🍕 Pizza!";
            else if (symbol == "[ZZZ]") mappedSymbol = "💤 Snoozing";
            else if (symbol == "[SHH]") mappedSymbol = "🤫 Gossip!";
            else if (symbol == "[DRAMA]") mappedSymbol = "🎭 Drama!";
            else if (symbol == "[CELEB]") mappedSymbol = "🎉 Celebration!";
            else if (symbol == "[BROKEN]") mappedSymbol = "💔 Betrayal";
            else if (symbol == "[ALLY]") mappedSymbol = "🤝 Alliance";
            else if (symbol == "[CHILL]") mappedSymbol = "🕶️ Chilling";
            else if (symbol == "[TRAVEL]") mappedSymbol = "🎒 Traveling";
            else if (symbol == "[TALK]") mappedSymbol = "💬 Chatting";
            else if (symbol == "[FLEE]") mappedSymbol = "🏃 Fleeing";
            else if (symbol == "[HAPPY]") mappedSymbol = "😊 Happy";
            else if (symbol == "[SAD]") mappedSymbol = "😢 Sad";
            else if (symbol == "[ANGRY]") mappedSymbol = "😡 Angry";

            // Physical AoE Emoji Spell Cast!
            var casterController = targetNpc.GetComponent<NPC.NPCController>();
            if (casterController != null)
            {
                GPOyun.Core.GlobalEventLogger.Log($"{casterController.NpcName} reacted with: {mappedSymbol}");
                
                Collider[] hitColliders = Physics.OverlapSphere(targetNpc.position, 5.0f); // 5 meter emotional radius
                foreach (var hitCollider in hitColliders)
                {
                    var hitBrain = hitCollider.GetComponent<NPC.UtilityAI.NPCBrain>();
                    if (hitBrain != null)
                    {
                        hitBrain.ProcessEmojiStimulus(symbol, casterController.NpcId);
                    }
                }
            }

            _activeReactions.Add(new EmojiReaction
            {
                target = targetNpc,
                symbol = mappedSymbol,
                color = color,
                timeStarted = Time.time,
                driftOffset = new Vector2(Random.Range(-30f, 30f), Random.Range(30f, 60f))
            });
        }

        public void SpawnTargetedEmoji(Transform sourceNpc, Transform targetNpc, string symbol, Color color)
        {
            // Same visual as a normal emoji on the source NPC...
            SpawnEmojiReaction(sourceNpc, symbol, color);

            // ...But it specifically sends a Handshake Request to the target's Brain instead of an AoE blast!
            var casterController = sourceNpc.GetComponent<NPC.NPCController>();
            var targetBrain = targetNpc.GetComponent<NPC.UtilityAI.NPCBrain>();
            
            if (casterController != null && targetBrain != null)
            {
                targetBrain.ReceiveHandshakeRequest(symbol, casterController.NpcId);
            }
        }

        private void DrawActiveEmojiReactions()
        {
            var mainCam = Camera.main;
            if (mainCam == null) return;

            for (int i = _activeReactions.Count - 1; i >= 0; i--)
            {
                var rx = _activeReactions[i];
                if (rx.target == null) { _activeReactions.RemoveAt(i); continue; }

                float elapsed = Time.time - rx.timeStarted;
                if (elapsed >= rx.duration)
                {
                    _activeReactions.RemoveAt(i);
                    continue;
                }

                // Project head to screen
                Vector3 worldPos = rx.target.position + Vector3.up * 2.5f;
                Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

                if (screenPos.z <= 0) continue;

                // Float upward and drift
                float progress = elapsed / rx.duration;
                float x = screenPos.x;
                float y = (Screen.height - screenPos.y) - rx.driftOffset.y * progress - (progress * 100f);

                GUIStyle emojiStyle = new GUIStyle();
                emojiStyle.fontSize = (int)(32f * (1.5f - progress)); // Balanced drift size
                emojiStyle.fontStyle = FontStyle.Bold;
                emojiStyle.alignment = TextAnchor.MiddleCenter;

                // Dynamic fade opacity
                Color fontColor = rx.color;
                fontColor.a = 1f - progress;

                // Draw text shadow
                emojiStyle.normal.textColor = new Color(0, 0, 0, 1f - progress);
                GUI.Label(new Rect(x - 150 + 1, y - 20 + 1, 300, 40), rx.symbol, emojiStyle);

                // Draw text
                emojiStyle.normal.textColor = fontColor;
                GUI.Label(new Rect(x - 150, y - 20, 300, 40), rx.symbol, emojiStyle);
            }
        }


        private void OnGUI()
        {
            DrawActiveEmojiReactions();
        }
        private void Update()
        {
            HandleAimingVisuals();
            UpdateClockUI();
            UpdatePhotoUI();
        }

        private void UpdateClockUI()
        {
            if (clockText != null && GPOyun.Core.ServiceLocator.TryGet<GPOyun.Managers.TimeManager>(out var tm))
            {
                clockText.text = tm.GetFormattedTime();
            }
        }

        private void HandleAimingVisuals()
        {
            bool isAiming = false;
            var camCtrl = Object.FindAnyObjectByType<CameraSystem.CameraController>();
            if (camCtrl != null)
            {
                isAiming = camCtrl.IsViewfinderActive();
            }
            else
            {
                var keyboard = UnityEngine.InputSystem.Keyboard.current;
                isAiming = keyboard != null && keyboard.cKey.isPressed;
            }
            
            if (viewfinderGroup != null)
            {
                viewfinderGroup.SetActive(isAiming);
                
                if (isAiming)
                {
                    float scale = 1.0f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
                    viewfinderGroup.transform.localScale = Vector3.one * scale;
                }
            }
        }

        private void UpdatePhotoUI()
        {
            if (photoCountText != null && NewspaperManager.Instance != null)
            {
                int photosTaken = NewspaperManager.Instance.GetTodaysPhotos().Count;
                photoCountText.text = $"PHOTOS: {photosTaken} / {_totalPhotos}";
            }
        }
    }

    public class EmojiReaction
    {
        public Transform target;
        public string symbol;
        public Color color;
        public float timeStarted;
        public float duration = 1.5f;
        public Vector2 driftOffset;
    }
}
