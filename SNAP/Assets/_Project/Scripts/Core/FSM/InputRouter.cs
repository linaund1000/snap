using UnityEngine;
using UnityEngine.InputSystem;
using GPOyun.UI;

namespace GPOyun.Core.FSM
{
    public enum GlobalRouterState
    {
        Immersion,
        UI_Active,
        Framing,
        Paused
    }

    /// <summary>
    /// The primary input reader. Emits events to the StateBus based on raw input and its own Global State.
    /// Replaces the old monolithic GlobalInputListener.
    /// </summary>
    public class InputRouter : MonoBehaviour, IMiniStateMachine
    {
        public static InputRouter Instance { get; private set; }

        public string MachineName => "InputRouter";
        private GlobalRouterState _currentState = GlobalRouterState.Immersion;

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
            TransitionTo(GlobalRouterState.Immersion);
        }

        private void OnDestroy()
        {
            if (Instance == this) StateBus.Unregister(this);
        }

        public string GetCurrentState() => _currentState.ToString();

        public void OnEvent(string eventName)
        {
            if (eventName == "UI_Opened" && _currentState != GlobalRouterState.UI_Active)
            {
                TransitionTo(GlobalRouterState.UI_Active);
            }
            else if (eventName == "All_UI_Closed" && _currentState == GlobalRouterState.UI_Active)
            {
                TransitionTo(GlobalRouterState.Immersion);
            }
        }

        private void TransitionTo(GlobalRouterState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;
            Debug.Log($"[{MachineName}] Transitioned to {_currentState}");
            
            // Moore Outputs via EventBus to UIManager
            if (_currentState == GlobalRouterState.Immersion)
            {
                StateBus.Emit("Req_LockCursor");
                StateBus.Emit("Req_FreeMove");
                StateBus.Emit("Req_ResumeTime");
            }
            else if (_currentState == GlobalRouterState.UI_Active)
            {
                StateBus.Emit("Req_UnlockCursor");
                StateBus.Emit("Req_BlockMove");
                StateBus.Emit("Req_ResumeTime"); // Live Ticker needs time to run
            }
            else if (_currentState == GlobalRouterState.Framing)
            {
                StateBus.Emit("Req_LockCursor");
                StateBus.Emit("Req_SlowMove");
                StateBus.Emit("Req_HideHUD");
                StateBus.Emit("Req_ResumeTime");
            }
            else if (_currentState == GlobalRouterState.Paused)
            {
                StateBus.Emit("Req_UnlockCursor");
                StateBus.Emit("Req_BlockMove");
                StateBus.Emit("Req_PauseTime");
            }
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // Global interception for PhotoReview Modal (Modal blocks all)
            if (PhotoReviewUI.Instance != null && PhotoReviewUI.Instance.IsOpen)
            {
                if (keyboard.escapeKey.wasPressedThisFrame) PhotoReviewUI.Instance.Hide();
                return;
            }

            switch (_currentState)
            {
                case GlobalRouterState.Immersion:
                    if (keyboard.escapeKey.wasPressedThisFrame)
                    {
                        TransitionTo(GlobalRouterState.Paused);
                        StateBus.Emit("Input_Esc_Press");
                    }
                    else if (keyboard.cKey.wasPressedThisFrame)
                    {
                        TransitionTo(GlobalRouterState.Framing);
                        StateBus.Emit("Input_C_Hold");
                    }
                    else if (keyboard.jKey.wasPressedThisFrame) StateBus.Emit("Input_J_Press");
                    else if (keyboard.gKey.wasPressedThisFrame) StateBus.Emit("Input_G_Press");
                    else if (keyboard.bKey.wasPressedThisFrame) StateBus.Emit("Input_B_Press");
                    break;

                case GlobalRouterState.UI_Active:
                    if (keyboard.escapeKey.wasPressedThisFrame)
                    {
                        StateBus.Emit("Global_ForceClose");
                        TransitionTo(GlobalRouterState.Immersion);
                    }
                    else if (keyboard.cKey.wasPressedThisFrame)
                    {
                        StateBus.Emit("Global_ForceClose");
                        TransitionTo(GlobalRouterState.Framing);
                        StateBus.Emit("Input_C_Hold");
                    }
                    else if (keyboard.jKey.wasPressedThisFrame) StateBus.Emit("Input_J_Press");
                    else if (keyboard.gKey.wasPressedThisFrame) StateBus.Emit("Input_G_Press");
                    else if (keyboard.bKey.wasPressedThisFrame) StateBus.Emit("Input_B_Press");
                    break;

                case GlobalRouterState.Framing:
                    if (keyboard.escapeKey.wasPressedThisFrame)
                    {
                        StateBus.Emit("Input_C_Release"); // Ensure UI closes
                        StateBus.Emit("Req_ShowHUD");
                        TransitionTo(GlobalRouterState.Immersion);
                    }
                    else if (keyboard.cKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
                    {
                        // Pressing C or Space while in Framing mode captures a photo
                        StateBus.Emit("Input_Capture");
                        // Wait for use case to complete, or exit framing?
                        // "c> c is capture foto, share and come back"
                        StateBus.Emit("Input_C_Release");
                        StateBus.Emit("Req_ShowHUD");
                        TransitionTo(GlobalRouterState.Immersion);
                    }
                    // In Framing, J, G, B do nothing (Blocked)
                    break;

                case GlobalRouterState.Paused:
                    if (keyboard.escapeKey.wasPressedThisFrame)
                    {
                        StateBus.Emit("Global_ForceClose");
                        TransitionTo(GlobalRouterState.Immersion);
                    }
                    else if (keyboard.cKey.wasPressedThisFrame)
                    {
                        StateBus.Emit("Req_PlayError"); // Blocked with feedback
                    }
                    break;
            }
        }
    }
}
