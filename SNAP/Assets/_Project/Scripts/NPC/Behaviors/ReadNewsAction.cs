using System.Collections;
using UnityEngine;

namespace GPOyun.NPC.UtilityAI
{
    public class ReadNewsAction : MoveAction
    {
        public override void Initialize(NPCController controller, NPCNeeds needs)
        {
            base.Initialize(controller, needs);
            ActionName = "ReadNews";
        }


        public override IEnumerator Execute()
        {
            _isExecuting = true;

            if (Controller.boardPosition != null)
            {
                yield return MoveToTarget(Controller.boardPosition.position, 1.2f, 8f);
                if (!_isExecuting) yield break;

                // Face the board
                Controller.transform.rotation = Quaternion.LookRotation(Controller.boardPosition.position - Controller.transform.position);
                Controller.ProcessReadNews();
                Needs.HasPendingNews = false;

                yield return new WaitForSeconds(Random.Range(3f, 7f));
            }

            _isExecuting = false;
        }
    }
}
