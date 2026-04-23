using UnityEngine;
using GPOyun.NPC;
using GPOyun.Events;

namespace GPOyun.Emotions.States
{
    /// <summary>
    /// NPC State triggered by tragic or highly personal negative news.
    /// </summary>
    public class SadState : EmotionState
    {
        private float _duration;
        private float _timer;

        public SadState(NPCController npc, float duration = 5f) : base(npc) => _duration = duration;

        public override EmotionType EmotionType => EmotionType.Sad;

        public override void Enter()
        {
            _timer = 0f;
            if (Npc.Animator != null) Npc.Animator.SetTrigger("Emotion_Sad");
            Debug.Log($"[NPC_{Npc.NpcId}] State: SAD.");
        }

        public override void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration) Npc.EmotionMachine.ChangeState(new NeutralState(Npc));
        }
    }
}
