using GPOyun.Events;
using GPOyun.Emotions;
using GPOyun.NPC;
using UnityEngine;

namespace GPOyun.Emotions.States
{
    public class FearState : EmotionState
    {
        private float _duration;
        private float _timer;

        public FearState(NPCController npc, float duration = 4f) : base(npc) => _duration = duration;
        public override EmotionType EmotionType => EmotionType.Fearful;

        public override void Enter() { _timer = 0f; if (Npc.Animator != null) Npc.Animator.SetTrigger("Emotion_Fear"); }
        public override void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration) Npc.EmotionMachine.ChangeState(new NeutralState(Npc));
        }
    }
}
