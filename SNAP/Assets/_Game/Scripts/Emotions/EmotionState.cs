using UnityEngine;
using GPOyun.Events;
using GPOyun.NPC;

namespace GPOyun.Emotions
{
    /// <summary>
    /// Abstract base for all NPC emotion states.
    /// Pattern: State Machine (Enter / Tick / Exit)
    /// </summary>
    public abstract class EmotionState
    {
        protected NPCController Npc;

        protected EmotionState(NPCController npc) => Npc = npc;

        public virtual void Enter()  { }
        public virtual void Tick()   { }
        public virtual void Exit()   { }

        public abstract EmotionType EmotionType { get; }
    }
}
