using UnityEngine;
using UnityEngine.Events;
using GPOyun;
using GPOyun.Managers;
using GPOyun.Events;
using GPOyun.Newspaper;
using GPOyun.NPC;

namespace GPOyun.Core
{
    /// <summary>
    /// Singleton GameManager — owns global game state and lifecycle.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState { Loading, Playing, Paused, GameOver }
        public GameState CurrentState { get; private set; }

        // Published events so other systems can subscribe without coupling.
        public UnityEvent<GameState> OnGameStateChanged = new();

        [Header("Systems")]
        [SerializeField] private NPCManager npcManager;
        [SerializeField] private NewspaperManager newspaperManager;
        [SerializeField] private EventBus eventBus;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() => ChangeState(GameState.Playing);

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged.Invoke(newState);
        }

        public void Initialize(NPCManager npcs, NewspaperManager news, EventBus bus)
        {
            npcManager = npcs;
            newspaperManager = news;
            eventBus = bus;
            Debug.Log("[GameManager] Systems initialized via Bootstrap.");
        }

        public void PauseGame()  => ChangeState(GameState.Paused);
        public void ResumeGame() => ChangeState(GameState.Playing);
    }
}
