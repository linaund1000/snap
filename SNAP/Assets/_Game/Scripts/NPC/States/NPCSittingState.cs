using UnityEngine;
using GPOyun.NPC;
using GPOyun.Environment;

namespace GPOyun.NPC.States
{
    /// <summary>
    /// NPC is currently seated on a bench.
    /// Can stay here until a routine change or emotional spike.
    /// </summary>
    public class NPCSittingState : NPCState
    {
        private BenchObject _bench;
        private float _duration;
        private float _timer;

        public NPCSittingState(NPCController npc, BenchObject bench, float duration = 15f) : base(npc)
        {
            _bench = bench;
            _duration = duration;
        }

        public override string StateName => "SITTING";

        public override void Enter()
        {
            _timer = 0f;
            
            // Snap to bench position
            if (_bench != null)
            {
                Npc.transform.position = _bench.GetSitPosition();
                Npc.transform.rotation = _bench.transform.rotation;
            }

            Debug.Log($"[NPC_{Npc.NpcId}] Seated on bench.");
            
            if (Npc.Animator != null)
            {
                Npc.Animator.SetBool("IsSitting", true);
            }
        }

        public override void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _duration)
            {
                Npc.BehaviourMachine.ChangeState(new NPCIdleState(Npc));
            }
        }

        public override void Exit()
        {
            if (_bench != null) _bench.Vacate();
            
            if (Npc.Animator != null)
            {
                Npc.Animator.SetBool("IsSitting", false);
            }
        }
    }
}
