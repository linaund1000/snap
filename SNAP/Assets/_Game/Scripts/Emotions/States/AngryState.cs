using GPOyun.Events;
using GPOyun.Emotions;
using GPOyun.NPC;
using UnityEngine;

namespace GPOyun.Emotions.States
{
    public class AngryState : EmotionState
    {
        private float _duration;
        private float _timer;

        public AngryState(NPCController npc, float duration = 5f) : base(npc) => _duration = duration;
        public override EmotionType EmotionType => EmotionType.Angry;

        public override void Enter() { _timer = 0f; if (Npc.Animator != null) Npc.Animator.SetTrigger("Emotion_Angry"); }
        public override void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration) Npc.EmotionMachine.ChangeState(new NeutralState(Npc));
        }
    }
}
