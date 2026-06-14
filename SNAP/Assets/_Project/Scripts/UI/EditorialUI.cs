using UnityEngine;
using UnityEngine.UI;
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
        public static EditorialUI Instance { get; private set; }

        [Header("References")]
        public CanvasGroup editorialCanvasGroup;
        public RectTransform rollContainer;
        public Text statusText;

        private bool _isExplicitlyOpen = false;
        private bool _wasAutoOpenedToday = false;
        private string _typedHeadline = "";

        private void OnEnable()
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard != null) keyboard.onTextInput += OnTextInput;
        }

        private void OnDisable()
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard != null) keyboard.onTextInput -= OnTextInput;
        }

        private void OnTextInput(char c)
        {
            if (!_isExplicitlyOpen) return;

            if (c == '\b') // Backspace
            {
                if (_typedHeadline.Length > 0)
                    _typedHeadline = _typedHeadline.Substring(0, _typedHeadline.Length - 1);
            }
            else if (c == '\n' || c == '\r') { /* Ignore */ }
            else if (char.IsControl(c)) { /* Ignore other controls */ }
            else
            {
                if (_typedHeadline.Length < 40) // Limit length
                    _typedHeadline += c;
            }

            if (statusText != null)
                statusText.text = "HEADLINE: " + (_typedHeadline.Length > 0 ? _typedHeadline : "Type something...");
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
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
            if (!GPOyun.Core.ServiceLocator.TryGet<GPOyun.Managers.TimeManager>(out var timeManager)) return;

            float hour = timeManager.GetCurrentHour();
            
            if (hour >= 22f && !_wasAutoOpenedToday && !_isExplicitlyOpen)
            {
                _wasAutoOpenedToday = true;
                Show();
                if (statusText != null) statusText.text = "NIGHT SHIFT: SELECT TOMORROW'S HEADLINES";
            }

            if (hour >= 6f && hour < 7f) _wasAutoOpenedToday = false;
        }

        public void Show()
        {
            if (editorialCanvasGroup == null) return;
            
            SettingsController.Instance?.Hide();
            PhotoGalleryUI.Instance?.Hide();
            JournalUI.Instance?.Hide();

            GPOyun.UI.UIManager.Instance?.PushMenu(gameObject);

            
            

            editorialCanvasGroup.alpha = 1f;
            editorialCanvasGroup.blocksRaycasts = true;
            editorialCanvasGroup.interactable = true;
            _isExplicitlyOpen = true;
            _typedHeadline = "";
            
            if (statusText != null) statusText.text = "HEADLINE: Type something...";
            Debug.Log("[EditorialUI] Workspace active.");
        }

        public void Hide()
        {
            if (!_isExplicitlyOpen) return;
            _isExplicitlyOpen = false;

            GPOyun.UI.UIManager.Instance?.PopMenu(gameObject);

            
            

            if (editorialCanvasGroup != null)
            {
                editorialCanvasGroup.alpha = 0f;
                editorialCanvasGroup.blocksRaycasts = false;
                editorialCanvasGroup.interactable = false;
            }
        }

        // Must be wired in Inspector to a button
        public void OnPublishClicked()
        {
            Debug.Log("[EditorialUI] Selection confirmed.");
            
            var photos = NewspaperManager.Instance.GetTodaysPhotos();
            NewsCategory finalCat = NewsCategory.Local;
            
            // Use user-typed headline if available, otherwise fallback to standard
            string headline = string.IsNullOrWhiteSpace(_typedHeadline) ? "A Quiet Day" : _typedHeadline;

            if (photos != null && photos.Count > 0)
            {
                var lastPhoto = photos[photos.Count - 1];
                if (lastPhoto.PrimarySubject != null)
                {
                    finalCat = lastPhoto.PrimarySubject.PrimaryCategory;
                    if (string.IsNullOrWhiteSpace(_typedHeadline))
                        headline = $"New {finalCat} Event Captured!";
                }
            }

            NewsStory front = new NewsStory { Headline = headline, Category = finalCat };
            NewspaperManager.Instance.PublishEdition(front);
            
            Hide();
        }
    }
}
