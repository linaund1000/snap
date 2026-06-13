using System.Collections;
using UnityEngine;
using GPOyun.Core;
using GPOyun.NPC.Data;

namespace GPOyun.NPC.UtilityAI
{
    public class SocializeAction : MoveAction
    {
        private NPCController _targetNPC;

        public override void Initialize(NPCController controller, NPCNeeds needs)
        {
            base.Initialize(controller, needs);
            ActionName = "Socialize";
        }

        public override float CalculateUtility()
        {
            float utility = BaseUtility;
            if (Controller.Brain != null)
            {
                // Setpoint Minimization (Homeostasis)
                float currentDeficit = Mathf.Abs(Needs.SocialDesire - Controller.Brain.ComfortZone.IdealSocial);
                float predictedDeficit = Controller.Brain.ComfortZone.IdealSocial; // Because this action drops SocialDesire to 0 

                if (currentDeficit > predictedDeficit)
                {
                    utility += (currentDeficit - predictedDeficit) * 1.5f;
                }
                else
                {
                    // Behavioral Degradation: Socializing right now would pull us away from our comfort zone!
                    utility -= (predictedDeficit - currentDeficit) * 2f;
                }
            }
            else
            {
                utility += (Needs.SocialDesire * 0.9f);
            }

            if (Needs.Energy < 30f) utility -= 40f;
            return Mathf.Max(0, utility);
        }

        public override IEnumerator Execute()
        {
            _isExecuting = true;

            SocialGroup group = null;

            while (_isExecuting)
            {
                // 1. Try to join an existing group
                if (SocialGroupManager.Instance != null)
                {
                    group = SocialGroupManager.Instance.TryJoinNearbyGroup(Controller);
                }

                // 2. If no group found, try to find a friend and create a group
                if (group == null)
                {
                    _targetNPC = FindFriend();
                    if (_targetNPC == null)
                    {
                        // Literally nobody is in our vision cone.
                        yield return new WaitForSeconds(2f);
                        continue;
                    }

                    // --- HANDSHAKE PROTOCOL ---
                    // 1. Emit the invite emoji
                    if (UI.HUDManager.Instance != null) 
                        UI.HUDManager.Instance.SpawnEmojiReaction(transform, "👋", Color.white);
                    
                    // Face the target while waiting for their answer
                    Vector3 lookDirToTarget = (_targetNPC.transform.position - transform.position).normalized;
                    lookDirToTarget.y = 0;
                    if (lookDirToTarget != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookDirToTarget);

                    yield return new WaitForSeconds(1.5f); // Wait for them to notice and evaluate

                    // 2. Did they accept?
                    bool accepted = false;
                    if (_targetNPC.Brain != null)
                    {
                        accepted = _targetNPC.Brain.ReceiveHandshakeRequest("👋", Controller.NpcId);
                    }

                    if (!accepted)
                    {
                        // Rejection! We take a social hit and abort.
                        Needs.SatisfySocial(-15f); // Rejection increases social starvation!
                        _isExecuting = false;
                        yield break;
                    }

                    // 3. They accepted! Form the group.
                    if (SocialGroupManager.Instance != null)
                    {
                        group = SocialGroupManager.Instance.CreateGroup(Controller, transform.position);
                        group.AddMember(_targetNPC);
                    }
                }

                // 3. Move to the group center or target
                Vector3 targetPos = group != null ? group.CenterPosition : _targetNPC.transform.position;
                
                // Add a small random offset so they stand in a circle
                float angle = Random.Range(0, Mathf.PI * 2);
                targetPos += new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 1.5f;

                yield return MoveToTarget(targetPos, NPCState.Socializing, 1.0f, 5f);
                if (!_isExecuting || (group != null && group.Members.Count <= 1)) break;

                // 4. Stand in the circle and socialize!
                if (group != null)
                {
                    Vector3 lookDir = (group.CenterPosition - transform.position).normalized;
                    lookDir.y = 0;
                    if (lookDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(lookDir);
                    }
                }

                // Interact with random member of the group
                NPCController chatPartner = null;
                if (group != null && group.Members.Count > 1)
                {
                    foreach (var member in group.Members)
                    {
                        if (member != Controller) { chatPartner = member; break; }
                    }
                }
                else
                {
                    chatPartner = _targetNPC;
                }

                if (chatPartner != null)
                {
                    int relationScore = GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>() != null ? 
                        GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().GetRelationship(Controller.NpcId, chatPartner.NpcId) : 0;

                    EmotionType reactionEmotion = EmotionType.Neutral;
                    string emoji = "💬";
                    Color emojiColor = Color.white;

                    var gestures = Controller.GetComponent<PantomimeGestures>();

                    if (relationScore < -20)
                    {
                        reactionEmotion = EmotionType.Disgusted;
                        emoji = "🙄"; // Rolling eyes at someone you don't like
                        emojiColor = new Color(0.7f, 0.7f, 0.7f);
                        if (gestures != null) gestures.SetSadness(true);
                    }
                    else if (relationScore > 50)
                    {
                        reactionEmotion = EmotionType.Happy;
                        emoji = "❤️";
                        emojiColor = new Color(1f, 0.4f, 0.6f);
                        if (gestures != null) gestures.PlayAffection();
                        
                        // Form Opinion
                        if (GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>() != null && Random.value > 0.5f)
                        {
                            GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().RecordOpinion(Controller.NpcId, chatPartner.NpcId, "We had a really warm, lovely chat today.");
                            GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().ModifyRelationship(Controller.NpcId, chatPartner.NpcId, +5);
                            GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().RecordOpinion(chatPartner.NpcId, Controller.NpcId, "Such a sweet chat!");
                            GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().ModifyRelationship(chatPartner.NpcId, Controller.NpcId, +5);
                        }
                    }
                    else
                    {
                        reactionEmotion = EmotionType.Neutral;
                        emoji = "👋";
                        emojiColor = new Color(0.9f, 0.9f, 0.9f);
                        if (gestures != null) gestures.SetSadness(false);
                        
                        // Form Opinion
                        if (GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>() != null && Random.value > 0.8f)
                        {
                            GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().RecordOpinion(Controller.NpcId, chatPartner.NpcId, "They were okay to talk to. Nice weather.");
                            GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().ModifyRelationship(Controller.NpcId, chatPartner.NpcId, +2);
                            GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().RecordOpinion(chatPartner.NpcId, Controller.NpcId, "A normal conversation.");
                            GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().ModifyRelationship(chatPartner.NpcId, Controller.NpcId, +2);
                        }
                    }

                    Controller.currentEmotion = reactionEmotion;
                    Controller.TriggerReaction(emoji, emojiColor);

                    var memStream = Controller.GetComponent<Memory.NPCMemoryStream>();
                    if (memStream != null)
                    {
                        memStream.AddMemory(new MemoryEvent(Time.time, StimulusType.FriendlyGreeting, chatPartner.NpcId, reactionEmotion));
                    }
                }

                Needs.SatisfySocial(100f);
                yield return new WaitForSeconds(Random.Range(4f, 8f));
                break;
            }

            if (group != null && SocialGroupManager.Instance != null)
            {
                SocialGroupManager.Instance.LeaveGroup(Controller, group);
            }

            _isExecuting = false;
        }

        private NPCController FindFriend()
        {
            if (Controller.Senses == null) return null;

            NPCController bestFriend = null;
            int highestRelation = -999;

            foreach (var otherNpc in Controller.Senses.VisibleNPCs)
            {
                if (otherNpc != null)
                {
                    int relation = GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>() != null ? 
                        GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().GetRelationship(Controller.NpcId, otherNpc.NpcId) : 0;
                    
                    // Don't socialize with enemies, they use ArgueAction for that!
                    if (relation >= -10 && relation > highestRelation)
                    {
                        highestRelation = relation;
                        bestFriend = otherNpc;
                    }
                }
            }
            return bestFriend;
        }
    }
}
