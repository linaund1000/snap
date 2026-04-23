using UnityEngine;
using GPOyun.NPC.States;

namespace GPOyun.NPC
{
    /// <summary>
    /// Manages the behavioural states of an NPC (Physical layer).
    /// </summary>
    public class NPCStateMachine
    {
        public NPCState CurrentState { get; private set; }

        public void Initialize(NPCState initialState)
        {
            CurrentState = initialState;
            CurrentState.Enter();
            Debug.Log($"[NPC_{initialState.StateName}] Initialized");
        }

        public void ChangeState(NPCState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
            Debug.Log($"[NPC_Behaviour] Switched to {newState.StateName}");
        }

        public void Tick() => CurrentState?.Tick();
    }
}
