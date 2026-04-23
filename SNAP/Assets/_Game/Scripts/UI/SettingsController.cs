using UnityEngine;
using UnityEngine.UI;

namespace GPOyun.UI
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup settingsCanvasGroup;
        private bool _isOpen = false;

        public void Initialize(CanvasGroup cg)
        {
            settingsCanvasGroup = cg;
            SetupListeners();
        }

        private void SetupListeners()
        {
            if (settingsCanvasGroup == null) return;
            
            Slider[] sliders = settingsCanvasGroup.GetComponentsInChildren<UnityEngine.UI.Slider>(true);
            foreach (var s in sliders)
            {
                if (s.gameObject.name.Contains("Sensitivity"))
                {
                    s.onValueChanged.AddListener(val => {
                        var player = UnityEngine.Object.FindObjectOfType<GPOyun.Player.PlayerController>();
                        if (player != null) player.SetSensitivity(val * 2f); 
                    });
                }
                else if (s.gameObject.name.Contains("Volume"))
                {
                    s.onValueChanged.AddListener(val => {
                        UnityEngine.AudioListener.volume = val;
                    });
                }
            }
        }

        public void ToggleSettings()
        {
            _isOpen = !_isOpen;
            if (settingsCanvasGroup != null)
            {
                settingsCanvasGroup.alpha = _isOpen ? 1 : 0;
                settingsCanvasGroup.blocksRaycasts = _isOpen;
                settingsCanvasGroup.interactable = _isOpen;
            }
            Debug.Log($"[Settings] {(_isOpen ? "Opened" : "Closed")}");
        }

        private void Update()
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                ToggleSettings();
            }
        }
    }
}
