using UnityEngine;
using System.Collections;
using GPOyun.Emotions;
using GPOyun.Emotions.States;
using GPOyun.NPC.States;
using GPOyun.Events;
using GPOyun.Core;
using GPOyun;

namespace GPOyun.NPC
{
    /// <summary>
    /// Core NPC brain. Holds both Emotional and Behavioural state machines.
    /// Handles physical movement via NPCStateMachine layer.
    /// </summary>
    [RequireComponent(typeof(GPOyun.Core.MovementBrain))]
    public class NPCController : MonoBehaviour
    {
        [Header("Identity")]
        public int NpcId;
        [SerializeField] private NPCPersonalityData personality;
        private TextMesh _nameLabel;

        [Header("Movement (Home Base)")]
        [SerializeField] private Transform boardPosition; 
        private Vector3 _startingPosition;

        // ---- Systems ----
        public Animator Animator { get; private set; }
        public GPOyun.Core.CharacterMotor Motor { get; private set; }
        public GPOyun.Core.MovementBrain Brain { get; private set; }
        public EmotionStateMachine EmotionMachine { get; private set; }
        public NPCStateMachine BehaviourMachine { get; private set; }

        // ---- State ----
        private EmotionType _currentEmotion;
        private NewsPublishedEvent? _pendingNews;
        private bool _hasReadTodayNews;

        [Header("Propagation Settings")]
        [SerializeField] private float maxPhaseDelay = 15f;
        [SerializeField] private bool isFatma; // Fatma has 0 delay

        private System.Collections.Generic.List<NewsStory> _memory = new System.Collections.Generic.List<NewsStory>();

        [Header("Ghostly Movement")]
        [SerializeField] private float bobFrequency = 2f;
        [SerializeField] private float bobAmplitude = 0.1f;
        private float _bobTimer;
        private Vector3 _visualBasePos;

        private void Awake()
        {
            Animator         = GetComponent<Animator>();
            Motor            = GetComponent<GPOyun.Core.CharacterMotor>();
            Brain            = GetComponent<GPOyun.Core.MovementBrain>();
            EmotionMachine   = new EmotionStateMachine();
            BehaviourMachine = new NPCStateMachine();
            
            _startingPosition = transform.position;
            CreateLabel();
        }

        private void CreateLabel()
        {
            GameObject labelGo = new GameObject("ID_Label");
            labelGo.transform.SetParent(this.transform);
            labelGo.transform.localPosition = new Vector3(0, 2.2f, 0);
            
            _nameLabel = labelGo.AddComponent<TextMesh>();
            _nameLabel.text = NpcId.ToString();
            _nameLabel.characterSize = 0.2f;
            _nameLabel.anchor = TextAnchor.MiddleCenter;
            _nameLabel.alignment = TextAlignment.Center;
            _nameLabel.color = Color.white;
            _nameLabel.fontStyle = FontStyle.Bold;
        }

        public Vector3 GetStartingPosition() => _startingPosition;

        private void Start()
        {
            EmotionMachine.Initialize(new NeutralState(this));
            BehaviourMachine.Initialize(new NPCIdleState(this));

            if (GPOyun.Events.EventBus.Instance != null)
            {
                GPOyun.Events.EventBus.Instance.Subscribe<NewsPublishedEvent>(OnNewsPublished);
                GPOyun.Events.EventBus.Instance.Subscribe<DayPhaseChangedEvent>(OnPhaseChanged);
            }
            else
            {
                Debug.LogError($"[NPC_{NpcId}] EventBus not found! Ensure [CORE] is setup correctly.");
            }
        }

        public void InitializeForTest(NPCPersonalityData data, Transform board)
        {
            personality   = data;
            boardPosition = board;
            
            if (EmotionMachine == null) EmotionMachine = new EmotionStateMachine();
            if (BehaviourMachine == null) BehaviourMachine = new NPCStateMachine();
            
            EmotionMachine.Initialize(new NeutralState(this));
            BehaviourMachine.Initialize(new NPCIdleState(this));
        }

        private float _telemetryTimer;

        private void Update()
        {
            if (EmotionMachine != null) EmotionMachine.Tick();
            if (BehaviourMachine != null) BehaviourMachine.Tick();
            UpdateAnimation();
            UpdateGhostlyBob();
            MeasureMovement();
        }

        private void MeasureMovement()
        {
            _telemetryTimer += Time.deltaTime;
            if (_telemetryTimer >= 0.1f && EventBus.Instance != null && Motor != null)
            {
                _telemetryTimer = 0f;
                EventBus.Instance.Publish(new MovementMetricsEvent
                {
                    EntityId = $"NPC_{NpcId}",
                    Velocity = Motor.GetCurrentVelocity(),
                    Speed = Motor.GetCurrentSpeed(),
                    Position = transform.position
                });
            }
        }


        private void UpdateGhostlyBob()
        {
            // Re-enabled as per user request: "move in y axis when the game start"
            // This provides the 'Spirit-like' weightless feel described in the GDD.
            _bobTimer += Time.deltaTime;
            float bob = Mathf.Sin(_bobTimer * bobFrequency) * bobAmplitude;

            // We apply the offset only to the visuals (Body and Head) 
            // so we don't interfere with the physics-based grounding logic.
            Transform bodyTransform = transform.Find("Body");
            Transform headTransform = transform.Find("Head");

            if (bodyTransform != null) bodyTransform.localPosition = new Vector3(0, bob, 0);
            if (headTransform != null) headTransform.localPosition = new Vector3(0, 1.2f + bob, 0);
        }

        /// <summary>
        /// Updates the Animator parameters based on current states.
        /// Handles 'headless' runs where Animator might be missing.
        /// </summary>
        private void UpdateAnimation()
        {
            if (Animator == null) return;

            // Movement intensity (from Motor velocity)
            float speed = Motor != null ? Motor.GetCurrentSpeed() : 0f;
            Animator.SetFloat("Speed", speed);

            // Emotion status
            Animator.SetInteger("Emotion", (int)_currentEmotion);
        }

        private void OnDestroy()
        {
            if (GPOyun.Events.EventBus.Instance != null)
            {
                GPOyun.Events.EventBus.Instance.Unsubscribe<NewsPublishedEvent>(OnNewsPublished);
                GPOyun.Events.EventBus.Instance.Unsubscribe<DayPhaseChangedEvent>(OnPhaseChanged);
            }
        }

        private void OnPhaseChanged(DayPhaseChangedEvent phaseEvent)
        {
            // Reset daily flag at Morning
            if (phaseEvent.NewPhase == DayPhase.Morning) 
            {
                _hasReadTodayNews = false;
            }

            // Cleanup stale news at Night so we don't react to old news tomorrow
            if (phaseEvent.NewPhase == DayPhase.Night)
            {
                _pendingNews = null;
            }

            // Staggered routine: Wait for a random duration before walking to the board
            if (phaseEvent.NewPhase == DayPhase.Morning && boardPosition != null)
            {
                StartCoroutine(StaggeredRoutine(boardPosition.position));
            }
        }

        private System.Collections.IEnumerator StaggeredRoutine(Vector3 destination)
        {
            float delay = isFatma ? 0f : Random.Range(0f, maxPhaseDelay);
            yield return new WaitForSeconds(delay);

            BehaviourMachine.ChangeState(new NPCWalkingState(this, destination));
        }


        public void OnReachDestination()
        {
            // 1. Check for interactive objects at destination
            Collider[] hits = Physics.OverlapSphere(transform.position, 2.0f);
            foreach (var hit in hits)
            {
                var bench = hit.GetComponentInParent<GPOyun.Environment.BenchObject>();
                if (bench != null && bench.IsOccupied) 
                {
                    // Verify if WE ARE the one who occupied it
                    BehaviourMachine.ChangeState(new NPCSittingState(this, bench));
                    return;
                }
            }

            // 2. Default logic
            if (BehaviourMachine.CurrentState is NPCWalkingState)
            {
                BehaviourMachine.ChangeState(new NPCReadingNewsState(this));
            }
            else
            {
                BehaviourMachine.ChangeState(new NPCIdleState(this));
            }
        }

        public void ProcessReadNews()
        {
            if (_pendingNews == null) return;
            if (_hasReadTodayNews) return;

            var newsEvent = _pendingNews.Value;
            _hasReadTodayNews = true;

            // Level 1: Everyone notices the Front Page
            NewsStory targetStory = newsEvent.FrontPage;

            // Level 2: 70% notice the Second Story
            if (newsEvent.SecondStory != null && Random.value < 0.7f)
            {
                targetStory = newsEvent.SecondStory;
            }
 
            // Level 3: 30% notice the Small Story
            if (newsEvent.SmallStory != null && Random.value < 0.3f)
            {
                targetStory = newsEvent.SmallStory;
            }
 
            if (targetStory != null)
            {
                var story = targetStory;
                
                // Add to memory
                if (!_memory.Contains(story)) _memory.Add(story);
                
                if (personality != null)
                {
                    var reaction = personality.GetReactionTo(story.Category);
                    ApplyEmotion(reaction.Emotion, reaction.Intensity);
                    Debug.Log($"[NPC_{NpcId}] Read {story.Category} story on the board. Reacted with {reaction.Emotion}.");
                }
                else
                {
                    ApplyEmotion(EmotionType.Surprised, 0.5f);
                }
            }

            _pendingNews = null;
        }

        // ---- News reaction ----
        private void OnNewsPublished(NewsPublishedEvent news)
        {
            // Buffer news instead of reacting immediately
            _pendingNews = news;
            _hasReadTodayNews = false;
        }

        public void ApplyEmotion(EmotionType emotion, float intensity)
        {
            _currentEmotion = emotion;
            EmotionState newState = emotion switch
            {
                EmotionType.Angry      => new AngryState(this, 4f + intensity * 4f),
                EmotionType.Happy      => new HappyState(this, 3f + intensity * 5f),
                EmotionType.Fearful    => new FearState(this,  3f + intensity * 3f),
                EmotionType.Sad        => new SadState(this, 5f + intensity * 5f),
                EmotionType.Surprised  => new SurprisedState(this, 2f + intensity * 2f),
                _                      => new NeutralState(this)
            };
            EmotionMachine.ChangeState(newState);

            if (GPOyun.Events.EventBus.Instance != null)
            {
                GPOyun.Events.EventBus.Instance.Publish(new NPCEmotionChangedEvent
                {
                    NpcId      = NpcId,
                    NewEmotion = emotion,
                    Intensity  = intensity
                });
            }
        }
    }
}

