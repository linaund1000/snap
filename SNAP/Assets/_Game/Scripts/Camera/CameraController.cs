using UnityEngine;
using UnityEngine.InputSystem;
using GPOyun.Events;

namespace GPOyun.CameraSystem
{
    /// <summary>
    /// Handles the Viewfinder, Photo Capture, and Cooldown logic.
    /// Follows the Camera FSM: READY -> VIEWFINDER -> SHUTTER -> COOLDOWN.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Keys")]
        [SerializeField] private KeyCode viewfinderKey = KeyCode.C;
        [SerializeField] private KeyCode captureKey = KeyCode.Space;
        
        [Header("Settings")]
        [SerializeField] private float captureCooldown = 4f;

        private float _cooldownTimer;
        private bool _isViewfinderActive;

        private void Update()
        {
            HandleInputs();
            if (_cooldownTimer > 0) _cooldownTimer -= Time.deltaTime;
        }

        private void HandleInputs()
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (keyboard == null || mouse == null) return;

            // VIEWFINDER State
            if (keyboard.cKey.wasPressedThisFrame) _isViewfinderActive = true;
            if (keyboard.cKey.wasReleasedThisFrame) _isViewfinderActive = false;

            // SHUTTER State
            if (_isViewfinderActive && (keyboard.spaceKey.wasPressedThisFrame || mouse.leftButton.wasPressedThisFrame))
            {
                TryCapture();
            }
        }

        private void TryCapture()
        {
            if (_cooldownTimer > 0)
            {
                Debug.Log($"[Camera] Cooldown: {_cooldownTimer:F1}s");
                return;
            }

            Debug.Log("[Camera] SHUTTER: Click!");
            _cooldownTimer = captureCooldown;

            // Detect Subject
            GPOyun.Environment.PhotoSubject subject = DetectSubject();

            // Publish Capture Event
            if (EventBus.Instance != null)
            {
                EventBus.Instance.Publish(new PhotoCapturedEvent
                {
                    CapturedTexture = null, 
                    WorldPosition   = transform.position,
                    PrimarySubject  = subject
                });
            }
        }

        private GPOyun.Environment.PhotoSubject DetectSubject()
        {
            // Simple Raycast from center of screen (Viewport space)
            Ray ray = UnityEngine.Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                var subject = hit.collider.GetComponentInParent<GPOyun.Environment.PhotoSubject>();
                if (subject != null)
                {
                    Debug.Log($"[Camera] Captured subject: {subject.SubjectName}");
                    return subject;
                }
            }

            // Fallback: Sphere check at a distance
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 5f, 3f);
            foreach (var h in hits)
            {
                var s = h.GetComponentInParent<GPOyun.Environment.PhotoSubject>();
                if (s != null) return s;
            }

            return null;
        }

        public bool IsViewfinderActive() => _isViewfinderActive;
    }
}
