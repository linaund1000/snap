using System.Collections;
using UnityEngine;
using GPOyun.Core;

namespace GPOyun.NPC.UtilityAI
{
    public class CryInstinctAction : NPCAction
    {
        private bool _isExecuting = false;

        public override void Initialize(NPCController controller, NPCNeeds needs)
        {
            base.Initialize(controller, needs);
            ActionName = "CryInstinct";
            BaseUtility = 0f; // Only triggers as a fallback when everything else fails
        }


        public override IEnumerator Execute()
        {
            _isExecuting = true;
            Controller.currentEmotion = EmotionType.Sad;

            var gestures = Controller.GetComponent<PantomimeGestures>();
            if (gestures != null) gestures.SetSadness(true);

            float cryTimer = 0f;
            while (_isExecuting && cryTimer < 10f)
            {
                cryTimer += 2f;

                // Spawn Crying Emoji
                if (UI.HUDManager.Instance != null)
                {
                    UI.HUDManager.Instance.SpawnEmojiReaction(transform, "😭", new Color(0.3f, 0.6f, 1f));
                }

                // AoE Effect: Stress out nearby NPCs because someone is crying nearby!
                Collider[] hits = Physics.OverlapSphere(transform.position, 15f);
                foreach (var hit in hits)
                {
                    var otherNpc = hit.GetComponentInParent<NPCController>();
                    if (otherNpc != null && otherNpc != Controller && otherNpc.Brain != null)
                    {
                        otherNpc.Brain.ProcessEmojiStimulus("😭", Controller.NpcId);
                    }
                }

                yield return new WaitForSeconds(2f);
            }

            if (gestures != null) gestures.SetSadness(false);
            
            // Just satisfy a bit of energy so they stop crying forever
            Needs.RestoreEnergy(10f);
            _isExecuting = false;
        }

        public override void Interrupt()
        {
            _isExecuting = false;
        }
    }
}
