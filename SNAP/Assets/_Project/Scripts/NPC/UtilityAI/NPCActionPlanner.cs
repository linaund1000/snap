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

            // 1. Calculate all utilities (which are now reading directly from Neural Weights!)
            float totalWeight = 0f;
            Dictionary<NPCAction, float> actionWeights = new Dictionary<NPCAction, float>();

            foreach (var action in _availableActions)
            {
                float weight = Mathf.Max(0.1f, action.CalculateUtility()); // Ensure no zero weights
                actionWeights[action] = weight;
                totalWeight += weight;
            }

            // 2. Weighted Random Selection (Probability Distribution)
            // This replaces rigid if-else logic with human-like unpredictability
            float randomPoint = Random.Range(0, totalWeight);
            float currentWeight = 0f;
            NPCAction selectedAction = null;

            foreach (var kvp in actionWeights)
            {
                currentWeight += kvp.Value;
                if (randomPoint <= currentWeight)
                {
                    selectedAction = kvp.Key;
                    break;
                }
            }

            if (selectedAction == null) selectedAction = _availableActions[0];

            bool isWhim = false;
            // The Chaos Multiplier (Whim System) - Keeps them a bit erratic
            if (_controller != null && _controller.personality != null)
            {
                float whimChance = 0.05f;
                if (_controller.personality.Neuroticism > 0.7f) whimChance = 0.10f; 
                if (_controller.personality.Conscientiousness > 0.7f) whimChance = 0.01f; 

                if (Random.value < whimChance)
                {
                    selectedAction = _availableActions[Random.Range(0, _availableActions.Count)];
                    isWhim = true;
                }
            }

            // If a different action was rolled, switch!
            // Note: Since it's weighted random, we might roll the same action. If so, just continue it.
            if (_activeAction == null || selectedAction != _activeAction)
            {
                if (_activeAction != null)
                {
                    _activeAction.EvaluateReward(); // Learn from the previous action!
                    _activeAction.Interrupt();
                    StopAllCoroutines();
                }

                _activeAction = selectedAction;
                _activeAction.SnapshotNeeds(); // Record state before execution
                
                if (_controller.Brain != null)
                {
                    string context = isWhim ? "Acting on a sudden, unpredictable whim!" : "Neural weight probability distribution selection.";
                    _controller.Brain.SetCognitiveState(_activeAction.ActionName, context, isWhim);
                }

                StartCoroutine(_activeAction.Execute());
            }
        }
    }
}
