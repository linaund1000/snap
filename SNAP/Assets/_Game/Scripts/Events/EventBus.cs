using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPOyun.Events
{
    /// <summary>
    /// Decoupled event bus. Systems publish/subscribe without direct references.
    /// Key events: NewsPublished, NPCEmotionChanged, PhotoCaptured.
    /// </summary>
    public class EventBus : MonoBehaviour
    {
        public static EventBus Instance { get; private set; }

        private readonly Dictionary<Type, List<Delegate>> _listeners = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Subscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (!_listeners.ContainsKey(type)) _listeners[type] = new List<Delegate>();
            _listeners[type].Add(listener);
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (_listeners.ContainsKey(type)) _listeners[type].Remove(listener);
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (!_listeners.ContainsKey(type)) return;
            foreach (var listener in _listeners[type])
                (listener as Action<T>)?.Invoke(eventData);
        }
    }

    // ---------- Event Payloads ----------

    public struct NewsPublishedEvent
    {
        public NewsStory FrontPage;    // Slot 1: 100% reach
        public NewsStory SecondStory;  // Slot 2: 70% reach
        public NewsStory SmallStory;   // Slot 3: 30% reach
        public int DayIndex;
    }

    [Serializable]
    public class NewsStory
    {
        public string Headline;
        public Texture2D Photo;
        public NewsCategory Category;
    }

    public struct NPCEmotionChangedEvent
    {
        public int NpcId;
        public EmotionType NewEmotion;
        public float Intensity; // 0-1
    }

    public struct PhotoCapturedEvent
    {
        public Texture2D CapturedTexture;
        public Vector3 WorldPosition;
        public GPOyun.Environment.PhotoSubject PrimarySubject;
    }

    public struct CoreInitializedEvent { }

    public struct DayPhaseChangedEvent
    {
        public DayPhase NewPhase;
    }

    // --- PIPELINE TELEMETRY ---
    public struct MovementMetricsEvent
    {
        public string EntityId;
        public Vector3 Velocity;
        public float Speed;
        public Vector3 Position;
    }

    public struct InputTelemetryEvent
    {
        public float Horizontal;
        public float Vertical;
        public bool IsMoving;
    }

    public enum NewsCategory { Local, Global, Scandal, Celebration, Disaster }
    public enum EmotionType   { Neutral, Happy, Sad, Angry, Fearful, Surprised, Disgusted }
    public enum DayPhase      { Morning, Midday, Afternoon, Evening, Night }
}
