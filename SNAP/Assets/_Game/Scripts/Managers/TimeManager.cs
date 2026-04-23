using UnityEngine;
using GPOyun.Events;

namespace GPOyun.Managers
{
    /// <summary>
    /// Handles the Day/Night cycle and phase transitions.
    /// Publishes DayPhaseChangedEvent when phases advance.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float phaseDurationSeconds = 120f; // 2 minutes per phase
        [SerializeField] private bool autoAdvance = true;

        public DayPhase CurrentPhase { get; private set; } = DayPhase.Morning;
        private float _phaseTimer;
        private float _currentHour = 6f; // Starts at 6 AM

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            if (EventBus.Instance != null && TimeManager.Instance == this)
            {
                EventBus.Instance.Publish(new DayPhaseChangedEvent { NewPhase = CurrentPhase });
            }
        }

        private void Update()
        {
            if (!autoAdvance) return;

            _phaseTimer += Time.deltaTime;
            
            // Calculate current hour based on phase progress
            // Each phase is 4 hours in a 5-phase cycle (6, 10, 14, 18, 22, 06)
            // Wait, 6AM to 10PM is 4 phases of 4 hours = 16 hours. 10PM to 6AM is 1 phase of 8 hours.
            // Let's adjust: 
            // Morning: 06-10 (4h)
            // Midday: 10-14 (4h)
            // Afternoon: 14-18 (4h)
            // Evening: 18-22 (4h)
            // Night: 22-06 (8h)
            
            float phaseCompletion = _phaseTimer / phaseDurationSeconds;
            float phaseLengthHours = (CurrentPhase == DayPhase.Night) ? 8f : 4f;
            
            float startHour = GetStartHourForPhase(CurrentPhase);
            _currentHour = (startHour + (phaseCompletion * phaseLengthHours)) % 24f;

            if (_phaseTimer >= phaseDurationSeconds)
            {
                AdvancePhase();
            }
        }

        private float GetStartHourForPhase(DayPhase phase) => phase switch {
            DayPhase.Morning => 6f,
            DayPhase.Midday => 10f,
            DayPhase.Afternoon => 14f,
            DayPhase.Evening => 18f,
            DayPhase.Night => 22f,
            _ => 6f
        };

        public void AdvancePhase()
        {
            if (EventBus.Instance == null) return;

            _phaseTimer = 0f;
            
            int nextPhase = ((int)CurrentPhase + 1) % 5;
            CurrentPhase = (DayPhase)nextPhase;

            Debug.Log($"[TimeManager] Hour: {GetFormattedTime()}, Phase: {CurrentPhase}");

            EventBus.Instance.Publish(new DayPhaseChangedEvent { NewPhase = CurrentPhase });
        }

        public string GetFormattedTime()
        {
            int hours = Mathf.FloorToInt(_currentHour);
            int minutes = Mathf.FloorToInt((_currentHour - hours) * 60f);
            return $"{hours:00}:{minutes:00}";
        }

        public float GetPhaseProgress() => Mathf.Clamp01(_phaseTimer / phaseDurationSeconds);
        public float GetCurrentHour() => _currentHour;
    }
}
