using System.Collections.Generic;
using UnityEngine;
using GPOyun.Events;
using GPOyun.Managers;

namespace GPOyun.Newspaper
{
    /// <summary>
    /// Manages the collection of photos and the daily publishing process.
    /// </summary>
    public class NewspaperManager : MonoBehaviour
    {
        public static NewspaperManager Instance { get; private set; }

        private readonly List<PhotoCapturedEvent> _capturedPhotos = new();
        private readonly List<NewsPublishedEvent> _history = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            if (EventBus.Instance != null)
            {
                EventBus.Instance.Subscribe<PhotoCapturedEvent>(OnPhotoCaptured);
                EventBus.Instance.Subscribe<DayPhaseChangedEvent>(OnPhaseChanged);
            }
        }

        private void OnDestroy()
        {
            if (EventBus.Instance != null)
            {
                EventBus.Instance.Unsubscribe<PhotoCapturedEvent>(OnPhotoCaptured);
                EventBus.Instance.Unsubscribe<DayPhaseChangedEvent>(OnPhaseChanged);
            }
        }

        private void OnPhotoCaptured(PhotoCapturedEvent photo)
        {
            _capturedPhotos.Add(photo);
            Debug.Log($"[NewspaperManager] Photo stored. Total today: {_capturedPhotos.Count}");
        }

        private void OnPhaseChanged(DayPhaseChangedEvent phaseEvent)
        {
            // Logic for the early morning newspaper arrival (06:00)
            // We check this at the start of Morning phase
            if (phaseEvent.NewPhase == DayPhase.Morning)
            {
                Debug.Log("[NewspaperManager] Morning arrived! Preparing newspaper roll for tomorrow.");
                // photos from 'yesterday' are cleared after they've been used for the edition
                _capturedPhotos.Clear();
            }
        }

        public void PublishEdition(NewsStory front, NewsStory second = null, NewsStory small = null)
        {
            var evt = new NewsPublishedEvent
            {
                FrontPage   = front,
                SecondStory = second,
                SmallStory  = small,
                DayIndex    = _history.Count + 1
            };

            _history.Add(evt);
            Debug.Log($"[NewspaperManager] Published Edition #{evt.DayIndex}!");
            
            if (EventBus.Instance != null) 
                EventBus.Instance.Publish(evt);
        }

        public List<NewsPublishedEvent> GetHistory() => _history;
        public List<PhotoCapturedEvent> GetTodaysPhotos() => _capturedPhotos;
    }
}
