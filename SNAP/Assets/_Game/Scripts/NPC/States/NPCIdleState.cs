using UnityEngine;
using GPOyun.NPC;
using GPOyun.Environment;

namespace GPOyun.NPC.States
{
    public class NPCIdleState : NPCState
    {
        private float _wanderTimer;
        private float _nextWanderTime;
        private Quaternion _lookTarget;
        private float _lookTimer;

        public NPCIdleState(NPCController npc) : base(npc) { }

        public override string StateName => "IDLE";

        public override void Enter()
        {
            ResetWanderTimer();
            SetNewLookTarget();
        }

        private void ResetWanderTimer()
        {
            _wanderTimer = 0f;
            _nextWanderTime = Random.Range(5f, 10f); // Natural pause between walks
        }

        private void SetNewLookTarget()
        {
            float randomAngle = Random.Range(0, 360f);
            _lookTarget = Quaternion.Euler(0, randomAngle, 0);
            _lookTimer = Random.Range(2f, 4f);
        }

        private bool _returningToHome = false;

        public override void Tick()
        {
            _wanderTimer += Time.deltaTime;
            _lookTimer -= Time.deltaTime;

            // Micro-Look Basics (Makes them feel alive)
            Npc.transform.rotation = Quaternion.Slerp(Npc.transform.rotation, _lookTarget, Time.deltaTime * 0.5f);
            if (_lookTimer <= 0) SetNewLookTarget();

            if (_wanderTimer >= _nextWanderTime)
            {
                ResetWanderTimer();

                if (_returningToHome)
                {
                    Npc.BehaviourMachine.ChangeState(new NPCWalkingState(Npc, Npc.GetStartingPosition()));
                    _returningToHome = false;
                }
                else
                {
                    Vector3 randomPoint = new Vector3(Random.Range(-12f, 12f), 0, Random.Range(-12f, 12f));
                    Npc.BehaviourMachine.ChangeState(new NPCWalkingState(Npc, randomPoint));
                    _returningToHome = true;
                }
            }
        }
    }
}
