using UnityEngine;
using GPOyun.NPC;

namespace GPOyun.NPC.States
{
    /// <summary>
    /// Abstract base for all NPC behavioural states (Physical layer).
    /// </summary>
    public abstract class NPCState
    {
        protected NPCController Npc;

        protected NPCState(NPCController npc) => Npc = npc;

        public virtual void Enter()  { }
        public virtual void Tick()   { }
        public virtual void Exit()   { }

        public abstract string StateName { get; }
    }
}
