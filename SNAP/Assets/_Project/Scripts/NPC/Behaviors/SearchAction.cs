using System.Collections;
using UnityEngine;

namespace GPOyun.NPC.UtilityAI
{
    public class SearchAction : MoveAction
    {
        public override void Initialize(NPCController controller, NPCNeeds needs)
        {
            base.Initialize(controller, needs);
            ActionName = "Search";
        }


        public override IEnumerator Execute()
        {
            _isExecuting = true;
            
            var board = GameObject.Find("NewspaperBoardInteractable");
            if (board != null)
            {
                yield return MoveToTarget(board.transform.position, 2.5f, 8f);
                if (!_isExecuting) yield break;
                Vector3 lookDir = (board.transform.position - transform.position).normalized;
                lookDir.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDir);

                Controller.TriggerReaction("📰", Color.white);
                yield return new WaitForSeconds(Random.Range(5f, 10f));
                Needs.SatisfyBoredom(50f);
            }
            else
            {

                Controller.TriggerReaction("❓", Color.yellow);
                yield return new WaitForSeconds(3f);
                Needs.SatisfyBoredom(20f);
            }

            _isExecuting = false;
        }
    }
}
