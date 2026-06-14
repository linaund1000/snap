using UnityEngine;
using UnityEngine.InputSystem;
using GPOyun.Core;
using GPOyun.UI;

namespace GPOyun.Player
{
    /// <summary>
    /// A1 Level Player Controller — uses the New Input System package.
    /// WASD / Arrow keys: move & turn. 
    /// Mouse X: also turns the player (for smoother feel).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Controls")]
        public float moveSpeed    = 5.0f;
        public float rotationSpeed = 120.0f;
        public float mouseSensitivity = 0.15f;
        public float gravity       = -9.8f;

        private CharacterController _cc;
        private float _verticalVelocity;

        private bool _isForcedToLook = false;
        private Vector3 _forcedTargetPos;
        private float _forceLookTimer = 0f;
        private float _forceLookDuration = 0f;

        public void ForceLookAt(Vector3 targetPos, float duration)
        {
            _isForcedToLook = true;
            _forcedTargetPos = targetPos;
            _forceLookDuration = duration;
            _forceLookTimer = 0f;
            Debug.Log($"[PlayerController] Forced to look at target for {duration} seconds.");
        }

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            CreateLabel();
        }

        private void CreateLabel()
        {
            var labelGo = new GameObject("ID_Label");
            labelGo.transform.SetParent(transform);
            labelGo.transform.localPosition = new Vector3(0, 2.2f, 0);
            var text = labelGo.AddComponent<TextMesh>();
            text.text          = "PLAYER";
            text.characterSize = 0.2f;
            text.anchor        = TextAnchor.MiddleCenter;
            text.alignment     = TextAlignment.Center;
            text.color         = Color.white;
            text.fontStyle     = FontStyle.Bold;

            var emojiGo = new GameObject("Emoji_Label");
            emojiGo.transform.SetParent(transform);
            emojiGo.transform.localPosition = new Vector3(0, 2.6f, 0);
            _emojiLabel = emojiGo.AddComponent<TextMesh>();
            _emojiLabel.characterSize = 0.5f;
            _emojiLabel.anchor        = TextAnchor.MiddleCenter;
            _emojiLabel.alignment     = TextAlignment.Center;
            _emojiLabel.text          = "";
        }

        private TextMesh _emojiLabel;
        private float _emojiTimer;

        private void EmitPlayerEmoji(string emoji, GPOyun.NPC.EmotionType emotion)
        {
            if (_emojiLabel != null)
            {
                _emojiLabel.text = emoji;
                _emojiTimer = 2f;
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, 15f);
            foreach (var col in colliders)
            {
                var npc = col.GetComponentInParent<GPOyun.NPC.NPCController>();
                if (npc != null)
                {
                    if (emotion == GPOyun.NPC.EmotionType.Happy)
                        npc.relationshipWithPlayer += 5;
                    else if (emotion == GPOyun.NPC.EmotionType.Angry)
                        npc.relationshipWithPlayer -= 10;

                    npc.relationshipWithPlayer = Mathf.Clamp(npc.relationshipWithPlayer, -100, 100);
                    npc.TriggerReaction(emotion == GPOyun.NPC.EmotionType.Happy ? "😊" : "💢", Color.white);
                }
            }
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse    = Mouse.current;
            if (keyboard == null) return;

            // ── TAB RELATIONSHIP OVERLAY ──────────────────────────────────
            if (keyboard.tabKey.wasPressedThisFrame && GPOyun.Core.ServiceLocator.TryGet<GPOyun.Core.RelationshipMatrix>(out var rm))
            {
                // Removed legacy Tab scoreboard. J (Journal) handles matrix now.
            }

            // ── FSM STATE PAUSED CHECK ────────────────────────────────────
            if (GPOyun.Core.ServiceLocator.TryGet<GPOyun.Core.GameManager>(out var gm) && gm.CurrentState == GameManager.GameState.Paused)
            {
                _verticalVelocity = -1f;
                return;
            }

            // ── PLAYER EMOJIS ─────────────────────────────────────────────
            if (_emojiTimer > 0)
            {
                _emojiTimer -= Time.deltaTime;
                if (_emojiTimer <= 0 && _emojiLabel != null) _emojiLabel.text = "";
            }

            if (keyboard.digit1Key.wasPressedThisFrame) EmitPlayerEmoji("👋", GPOyun.NPC.EmotionType.Happy);
            if (keyboard.digit2Key.wasPressedThisFrame) EmitPlayerEmoji("😡", GPOyun.NPC.EmotionType.Angry);

            // ── MOVEMENT ──────────────────────────────────────────────────
            float moveZ = 0f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)   moveZ =  1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)  moveZ = -1f;

            // ── ROTATION ──────────────────────────────────────────────────
            if (_isForcedToLook)
            {
                _forceLookTimer += Time.deltaTime;
                if (_forceLookTimer >= _forceLookDuration)
                {
                    _isForcedToLook = false;
                }
                else
                {
                    Vector3 dir = (_forcedTargetPos - transform.position).normalized;
                    dir.y = 0; // Horizontal only
                    if (dir != Vector3.zero)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
                    }
                }
            }
            else
            {
                float rotY = 0f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)  rotY = -1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) rotY =  1f;

                // Mouse horizontal also rotates (only when mouse is locked)
                if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
                    rotY += mouse.delta.ReadValue().x * mouseSensitivity;

                transform.Rotate(Vector3.up, rotY * rotationSpeed * Time.deltaTime);
            }

            // ── GRAVITY ───────────────────────────────────────────────────
            if (_cc.isGrounded)
                _verticalVelocity = -1f;
            else
                _verticalVelocity += gravity * Time.deltaTime;

            Vector3 move = transform.forward * moveZ * moveSpeed;
            move.y = _verticalVelocity;
            _cc.Move(move * Time.deltaTime);

            // ── CURSOR LOCK ────────────────────────────────────────────────
            // Click -> lock only allowed if no menus are open!
            bool isMenuOpen = GPOyun.Core.ServiceLocator.TryGet<GPOyun.UI.UIManager>(out var uiMgr) && uiMgr.IsAnyMenuOpen();
            if (!isMenuOpen && mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
