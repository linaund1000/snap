using GPOyun.Events;
using GPOyun.Emotions;
using GPOyun.NPC;

namespace GPOyun.Emotions.States
{
    public class NeutralState : EmotionState
    {
        public NeutralState(NPCController npc) : base(npc) { }
        public override EmotionType EmotionType => EmotionType.Neutral;
        public override void Enter() { if (Npc.Animator != null) Npc.Animator.SetTrigger("Emotion_Neutral"); }
    }
}
