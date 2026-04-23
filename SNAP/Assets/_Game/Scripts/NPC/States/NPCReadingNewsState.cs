using UnityEngine;
using GPOyun.NPC;

namespace GPOyun.NPC.States
{
    public class NPCReadingNewsState : NPCState
    {
        private float _readTimer;
        private float _readDuration;

        public NPCReadingNewsState(NPCController npc) : base(npc)
        {
            // Random duration between 3 and 8 seconds as per specs
            _readDuration = Random.Range(3f, 8f);
        }

        public override string StateName => "READING_NEWS";

        public override void Enter()
        {
            _readTimer = 0f;
            Debug.Log($"[NPC_{Npc.NpcId}] Started reading news for {_readDuration:F1}s");
            
            // In the future, trigger a 'Reading' animation here
            if (Npc.Animator != null)
            {
                // Npc.Animator.SetBool("IsReading", true);
            }
        }

        public override void Tick()
        {
            _readTimer += Time.deltaTime;
            if (_readTimer >= _readDuration)
            {
                // Process the news we just read
                Npc.ProcessReadNews();

                // Finished reading -> Transition back to Idle
                Npc.BehaviourMachine.ChangeState(new NPCIdleState(Npc));
            }
        }

        public override void Exit()
        {
            if (Npc.Animator != null)
            {
                // Npc.Animator.SetBool("IsReading", false);
            }
        }
    }
}
