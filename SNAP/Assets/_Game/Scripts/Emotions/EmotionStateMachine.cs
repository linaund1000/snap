using UnityEngine;

namespace GPOyun.Emotions
{
    public class EmotionStateMachine
    {
        public EmotionState CurrentState { get; private set; }

        public void Initialize(EmotionState initialState)
        {
            CurrentState = initialState;
            CurrentState.Enter();
        }

        public void ChangeState(EmotionState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void Tick() => CurrentState?.Tick();
    }
}
