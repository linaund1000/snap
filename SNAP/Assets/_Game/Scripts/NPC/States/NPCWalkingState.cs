using UnityEngine;
using GPOyun.NPC;

namespace GPOyun.NPC.States
{
    public class NPCWalkingState : NPCState
    {
        private Vector3 _destination;
        private float _arrivalDistance = 0.5f;

        public NPCWalkingState(NPCController npc, Vector3 destination) : base(npc)
        {
            _destination = destination;
        }

        public override string StateName => "WALKING";

        public override void Enter()
        {
            Debug.Log($"[NPC_{Npc.NpcId}] Started walking towards {_destination}");
        }

        public override void Tick()
        {
            // Direct steering towards target with avoidance
            if (Npc.Brain != null)
            {
                Npc.Brain.NavigateNPC(_destination);
            }

            // Check for arrival (2D Distance based to prevent height-offset issues)
            Vector3 diff = Npc.transform.position - _destination;
            diff.y = 0;
            if (diff.magnitude <= _arrivalDistance)
            {
                Npc.OnReachDestination();
            }
        }
    }
}
