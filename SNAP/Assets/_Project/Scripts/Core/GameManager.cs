using UnityEngine;
using UnityEngine.Events;
using GPOyun.Managers;
using GPOyun.Newspaper;
using GPOyun.NPC;

namespace GPOyun.Core
{
    /// <summary>
    /// A1 Level GameManager
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public enum GameState { Loading, Playing, Paused, GameOver }
        public GameState CurrentState { get; private set; }

        // Published events so other systems can subscribe without coupling.
        public UnityEvent<GameState> OnGameStateChanged = new();

        [Header("Systems")]
        [SerializeField] private NPCManager npcManager;
        [SerializeField] private NewspaperManager newspaperManager;

        private void Awake()
        {
            ServiceLocator.Register<GameManager>(this);
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<GameManager>();
        }

        private void Start() => ChangeState(GameState.Playing);

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged.Invoke(newState);
        }

        public void Initialize(NPCManager npcs, NewspaperManager news)
        {
            npcManager = npcs;
            newspaperManager = news;
            Debug.Log("[GameManager] Systems initialized.");
        }

        public void PauseGame()  => ChangeState(GameState.Paused);
        public void ResumeGame() => ChangeState(GameState.Playing);
    }
}
