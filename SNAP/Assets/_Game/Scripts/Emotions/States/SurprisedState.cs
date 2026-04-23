using UnityEngine;
using GPOyun.NPC;
using GPOyun.Events;

namespace GPOyun.Emotions.States
{
    /// <summary>
    /// NPC State triggered by shocking or positive news they didn't expect.
    /// </summary>
    public class SurprisedState : EmotionState
    {
        private float _duration;
        private float _timer;

        public SurprisedState(NPCController npc, float duration = 3f) : base(npc) => _duration = duration;

        public override EmotionType EmotionType => EmotionType.Surprised;

        public override void Enter()
        {
            _timer = 0f;
            if (Npc.Animator != null) Npc.Animator.SetTrigger("Emotion_Surprised");
            Debug.Log($"[NPC_{Npc.NpcId}] State: SURPRISED!");
        }

        public override void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration) Npc.EmotionMachine.ChangeState(new NeutralState(Npc));
        }
    }
}
