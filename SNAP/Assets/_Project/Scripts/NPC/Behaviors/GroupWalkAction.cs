using System.Collections;
using UnityEngine;
using GPOyun.Core;

namespace GPOyun.NPC.UtilityAI
{
    public class GroupWalkAction : MoveAction
    {
        private SocialGroup _walkGroup;
        private Vector3 _destination;
        private bool _isLeader;

        public override void Initialize(NPCController controller, NPCNeeds needs)
        {
            base.Initialize(controller, needs);
            ActionName = "GroupWalk";
        }

        public override float CalculateUtility()
        {
            // Only naturally triggered if we have a deficit in boredom and want to walk
            if (Controller.Brain != null)
            {
                float currentBoredomDeficit = Mathf.Abs(Needs.Boredom - Controller.Brain.ComfortZone.IdealBoredom);
                if (currentBoredomDeficit > 20f && Needs.Energy > 40f)
                {
                    return BaseUtility + currentBoredomDeficit;
                }
            }
            return 0f;
        }

        public override IEnumerator Execute()
        {
            _isExecuting = true;

            // 1. Gather friends or join existing group
            if (SocialGroupManager.Instance != null)
            {
                _walkGroup = SocialGroupManager.Instance.TryJoinNearbyGroup(Controller, 40f);
                if (_walkGroup == null)
                {
                    _walkGroup = SocialGroupManager.Instance.CreateGroup(Controller, transform.position);
                    _isLeader = true;
                    
                    // Send out invites
                    Collider[] hits = Physics.OverlapSphere(transform.position, 25f);
                    foreach (var hit in hits)
                    {
                        if (_walkGroup.IsFull) break;

                        var otherNpc = hit.GetComponentInParent<NPCController>();
                        if (otherNpc != null && otherNpc != Controller && otherNpc.Brain != null)
                        {
                            bool accepted = otherNpc.Brain.ReceiveHandshakeRequest("🚶‍♂️", Controller.NpcId);
                            if (accepted)
                            {
                                _walkGroup.AddMember(otherNpc);
                            }
                        }
                    }
                }
            }

            if (_walkGroup == null)
            {
                _isExecuting = false;
                yield break;
            }

            // Continuously loop walking to random waypoints until someone is bored or tired
            while (_isExecuting && Needs.Boredom > 10f && Needs.Energy > 10f)
            {
                if (_isLeader)
                {
                    Controller.TriggerReaction("🗺️", Color.yellow);
                    Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(30f, 50f);
                    _destination = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
                    _destination.y = transform.position.y;
                    _walkGroup.UpdateCenter(_destination);

                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    _destination = _walkGroup.CenterPosition;
                    Controller.TriggerReaction("👍", Color.green);
                }

                // Walk to the destination
                while (_isExecuting && Vector3.Distance(transform.position, _destination) > 3f)
                {
                    // Sync speeds
                    if (!_isLeader && _walkGroup.Members.Count > 0 && _walkGroup.Members[0] != null)
                    {
                        float distToLeader = Vector3.Distance(transform.position, _walkGroup.Members[0].transform.position);
                        if (distToLeader > 15f) yield return MoveToTarget(_destination, 1.0f, 7f); // Sprint
                        else yield return MoveToTarget(_destination, 1.0f, 3.5f); // Walk
                    }
                    else
                    {
                        yield return MoveToTarget(_destination, 1.0f, 3.5f);
                    }

                    if (!_isExecuting) break;
                }

                if (!_isExecuting) break;

                // Stop and chat for a bit at the destination
                Controller.TriggerReaction("💬", Color.white);
                Needs.SatisfyBoredom(-10f); // Walking relieves boredom
                Needs.RestoreEnergy(-10f);  // Walking drains energy

                yield return new WaitForSeconds(Random.Range(5f, 10f));
            }

            // Cleanup
            if (SocialGroupManager.Instance != null && _walkGroup != null)
            {
                SocialGroupManager.Instance.LeaveGroup(Controller, _walkGroup);
            }

            _isExecuting = false;
        }
    }
}
