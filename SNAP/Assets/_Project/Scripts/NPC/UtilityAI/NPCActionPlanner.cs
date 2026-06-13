using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPOyun.NPC.UtilityAI
{
    public class NPCActionPlanner : MonoBehaviour
    {
        private NPCController _controller;
        private NPCNeeds _needs;
        
        private List<NPCAction> _availableActions = new List<NPCAction>();
        private NPCAction _activeAction;

        private float _evaluationTimer = 0f;
        private float _evaluationInterval = 1.0f; // Re-evaluate every 1 second

        public void Initialize(NPCController controller, NPCNeeds needs)
        {
            _controller = controller;
            _needs = needs;

            // Load all attached actions
            var actions = GetComponents<NPCAction>();
            foreach (var action in actions)
            {
                action.Initialize(controller, needs);
                _availableActions.Add(action);
            }
        }

        private void Start()
        {
            // Give each NPC a deterministic start offset based on their ID to prevent evaluation frame spikes
            _evaluationTimer = (_controller.NpcId * 0.1f) % _evaluationInterval;
        }

        private void Update()
        {
            if (_controller == null) return;

            _evaluationTimer += Time.deltaTime;
            if (_evaluationTimer >= _evaluationInterval)
            {
                _evaluationTimer = 0f;
                EvaluateUtilities();
            }
        }

        public void ForceReevaluate()
        {
            _evaluationTimer = 0f;
            EvaluateUtilities();
        }

        public void ForceAction(System.Type actionType)
        {
            foreach (var action in _availableActions)
            {
                if (action.GetType() == actionType)
                {
                    if (_activeAction != action)
                    {
                        if (_activeAction != null)
                        {
                            _activeAction.Interrupt();
                            StopAllCoroutines();
                        }
                        
                        _activeAction = action;
                        StartCoroutine(_activeAction.Execute());
                        Debug.Log($"[NPC {_controller.NpcName}] FORCED Action: {_activeAction.ActionName}");
                    }
                    return;
                }
            }
        }

        private void EvaluateUtilities()
        {
            if (_availableActions.Count == 0) return;

            NPCAction bestAction = null;
            float highestUtility = -1f;

            foreach (var action in _availableActions)
            {
                float utility = action.CalculateUtility();
                if (utility > highestUtility)
                {
                    highestUtility = utility;
                    bestAction = action;
                }
            }

            // INSTINCT FALLBACK: If all actions failed or have <= 0 utility, trigger CryInstinctAction
            if (highestUtility <= 0f)
            {
                var cryAction = _availableActions.Find(a => a is CryInstinctAction);
                if (cryAction != null)
                {
                    bestAction = cryAction;
                    highestUtility = 0.1f;
                }
            }

            bool isWhim = false;
            // The Chaos Multiplier (Whim System)
            // 5% base chance to completely ignore logical utility and pick a random action
            if (_controller != null && _controller.personality != null)
            {
                float whimChance = 0.05f;
                if (_controller.personality.Neuroticism > 0.7f) whimChance = 0.10f; // Neurotic = more chaotic
                if (_controller.personality.Conscientiousness > 0.7f) whimChance = 0.01f; // Conscientious = very logical

                // Observer effect dampens chaos
                if (_needs.IsPlayerNearby) whimChance = 0.0f;

                if (Random.value < whimChance)
                {
                    bestAction = _availableActions[Random.Range(0, _availableActions.Count)];
                    highestUtility = 9999f;
                    isWhim = true;
                }
            }

            // If a new action is vastly superior (or our current action is null), switch!
            if (_activeAction == null || (bestAction != null && bestAction != _activeAction && highestUtility > (_activeAction.CalculateUtility() + 5f)))
            {
                if (_activeAction != null)
                {
                    _activeAction.Interrupt();
                    StopAllCoroutines();
                }

                _activeAction = bestAction;
                
                // Write cognitive state to the Brain
                if (_controller.Brain != null)
                {
                    string context = isWhim ? "Acting on a sudden, unpredictable whim!" : "Logical utility choice based on physiological needs.";
                    _controller.Brain.SetCognitiveState(_activeAction.ActionName, context, isWhim);
                }

                StartCoroutine(_activeAction.Execute());
            }
        }
    }
}
