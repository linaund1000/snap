using UnityEngine;
using GPOyun.UI;

namespace GPOyun.Core.FSM
{
    public class JournalMachine : MonoBehaviour, IMiniStateMachine
    {
        public static JournalMachine Instance { get; private set; }

        public string MachineName => "JournalMachine";

        public enum State
        {
            Closed,
            Open_Matrix,
            Open_Feed,
            Open_Focus
        }

        private State _currentState = State.Closed;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StateBus.Register(this);
        }

        private void OnDestroy()
        {
            if (Instance == this) StateBus.Unregister(this);
        }

        public string GetCurrentState() => _currentState.ToString();

        public void OnEvent(string eventName)
        {
            if (eventName == "Global_ForceClose")
            {
                TransitionTo(State.Closed);
                return;
            }

            switch (_currentState)
            {
                case State.Closed:
                    if (eventName == "Input_J_Press")
                    {
                        var target = TargetInspectorUI.Instance?.CurrentTarget;
                        if (target != null)
                        {
                            TransitionTo(State.Open_Focus);
                            JournalUI.Instance?.ShowFocus(target);
                        }
                        else
                        {
                            TransitionTo(State.Open_Matrix);
                            JournalUI.Instance?.ShowMatrix();
                        }
                    }
                    break;
                case State.Open_Matrix:
                    if (eventName == "Input_J_Press")
                    {
                        TransitionTo(State.Open_Feed);
                        JournalUI.Instance?.ShowFeed();
                    }
                    break;
                case State.Open_Feed:
                    if (eventName == "Input_J_Press")
                    {
                        TransitionTo(State.Closed);
                        // Handled in TransitionTo
                    }
                    break;
                case State.Open_Focus:
                    if (eventName == "Input_J_Press")
                    {
                        TransitionTo(State.Closed);
                        // Handled in TransitionTo
                    }
                    break;
            }
        }

        private void TransitionTo(State newState)
        {
            if (_currentState == newState) return;
            
            bool wasClosed = _currentState == State.Closed;
            _currentState = newState;
            Debug.Log($"[{MachineName}] Transitioned to {_currentState}");

            if (wasClosed && newState != State.Closed)
            {
                StateBus.Emit("UI_Opened");
            }
            else if (!wasClosed && newState == State.Closed)
            {
                // We closed.
                StateBus.Emit("All_UI_Closed");
                if (JournalUI.Instance != null)
                {
                    // Force the UI script to close if it didn't
                    JournalUI.Instance.Hide();
                }
            }
        }
    }
}
