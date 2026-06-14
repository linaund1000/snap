using System.Collections;
using UnityEngine;
using GPOyun.Core;
using GPOyun.NPC.Data;

namespace GPOyun.NPC.UtilityAI
{
    public class ApproachPlayerAction : MoveAction
    {
        private Transform _playerTransform;

        public override void Initialize(NPCController controller, NPCNeeds needs)
        {
            base.Initialize(controller, needs);
            ActionName = "ApproachPlayer";
        }


        public override IEnumerator Execute()
        {
            _isExecuting = true;

            while (_isExecuting)
            {
                if (_playerTransform == null) break;

                // Move to player
                yield return MoveToTarget(_playerTransform.position, 3.0f, 6f);
                if (!_isExecuting || _playerTransform == null) break;

                // Face player
                Vector3 lookDir = (_playerTransform.position - transform.position).normalized;
                lookDir.y = 0;
                if (lookDir != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookDir);

                // Grab player's attention physically!
                var playerController = _playerTransform.GetComponent<GPOyun.Player.PlayerController>();
                if (playerController != null)
                {
                    // Force player to look at NPC's face
                    Vector3 myFacePos = transform.position + Vector3.up * 1.5f;
                    playerController.ForceLookAt(myFacePos, 1.5f);
                }

                // Introduce!
                Controller.currentEmotion = EmotionType.Happy;
                
                if (Controller.relationshipWithPlayer > 20)
                {
                    Controller.TriggerReaction("🥰", new Color(1f, 0.4f, 0.6f)); // Happy to see you!
                }
                else
                {
                    Controller.TriggerReaction("👋", new Color(0.9f, 0.9f, 0.9f)); // Polite wave
                    Controller.relationshipWithPlayer += 10; // Boost relation for saying hi
                }

                var gestures = Controller.GetComponent<PantomimeGestures>();
                if (gestures != null) gestures.PlayAffection();

                Needs.SatisfySocial(40f);
                
                // Stand there and smile for a few seconds
                yield return new WaitForSeconds(Random.Range(3f, 5f));
                break;
            }

            _isExecuting = false;
        }
    }
}
