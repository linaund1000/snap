using UnityEngine;
using UnityEngine.UI;
using GPOyun.Events;

namespace GPOyun.UI
{
    /// <summary>
    /// Manages the on-screen HUD (Heads-Up Display) for the Snap mechanic.
    /// Displays the viewfinder corners and photo count.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("UI Overlays")]
        [SerializeField] private GameObject viewfinderGroup;
        [SerializeField] private Text photoCountText;
        [SerializeField] private Text clockText;

        public void Initialize(GameObject viewfinder, Text photoText, Text timeText)
        {
            viewfinderGroup = viewfinder;
            photoCountText = photoText;
            clockText = timeText;
        }
        
        [Header("Animation Settings")]
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseAmount = 0.1f;

        private int _totalPhotos = 5; // Configurable
        private int _photosTaken = 0;

        private void Start()
        {
            if (viewfinderGroup != null) viewfinderGroup.SetActive(false);
            UpdatePhotoUI();

            // Subscribe to game events
            if (EventBus.Instance != null)
            {
                EventBus.Instance.Subscribe<PhotoCapturedEvent>(OnPhotoCaptured);
            }
        }

        private void OnDestroy()
        {
            if (EventBus.Instance != null)
            {
                EventBus.Instance.Unsubscribe<PhotoCapturedEvent>(OnPhotoCaptured);
            }
        }

        private void Update()
        {
            HandleAimingVisuals();
            UpdateClockUI();
        }

        private void UpdateClockUI()
        {
            if (clockText != null && GPOyun.Managers.TimeManager.Instance != null)
            {
                clockText.text = GPOyun.Managers.TimeManager.Instance.GetFormattedTime();
            }
        }

        private void HandleAimingVisuals()
        {
            // Simple logic: Viewfinder appears when 'C' (Aim) is held
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            bool isAiming = keyboard != null && keyboard.cKey.isPressed;
            
            if (viewfinderGroup != null)
            {
                viewfinderGroup.SetActive(isAiming);
                
                if (isAiming)
                {
                    // Subtle pulse effect
                    float scale = 1.0f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
                    viewfinderGroup.transform.localScale = Vector3.one * scale;
                }
            }
        }

        private void OnPhotoCaptured(PhotoCapturedEvent evt)
        {
            _photosTaken++;
            UpdatePhotoUI();
            
            // Visual feedback could be added here (e.g., a flash)
            Debug.Log($"[HUD] Photo captured! Slots: {_photosTaken}/{_totalPhotos}");
        }

        private void UpdatePhotoUI()
        {
            if (photoCountText != null)
            {
                photoCountText.text = $"PHOTOS: {_photosTaken} / {_totalPhotos}";
            }
        }
    }
}
