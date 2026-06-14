using System.Collections;
using UnityEngine;

namespace GPOyun.NPC.UtilityAI
{
    public class GoHomeAction : MoveAction
    {
        public override void Initialize(NPCController controller, NPCNeeds needs)
        {
            base.Initialize(controller, needs);
            ActionName = "GoHome";
        }


        public override IEnumerator Execute()
        {
            _isExecuting = true;

            yield return MoveToTarget(Controller.homePosition, 1.2f, 8f);
            if (!_isExecuting) yield break;

            while (_isExecuting && Needs.IsNightTime)
            {
                Needs.RestoreEnergy(3.0f * Time.deltaTime);
                yield return null;
            }

            _isExecuting = false;
        }
    }
}
