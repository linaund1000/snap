using UnityEngine;
using GPOyun.NPC;
using GPOyun.NPC.Data;
using GPOyun.NPC.Appraisal;
using System.Collections.Generic;

namespace GPOyun.NPC.UtilityAI
{
    /// <summary>
    /// The Cognitive Engine. Bridges physiological Needs with logical Actions.
    /// Manages the Whim (Chaos) multiplier and handles systemic Emoji AoE impacts.
    /// </summary>
    public class NPCBrain : MonoBehaviour
    {
        private NPCController _controller;
        private NPCNeeds _needs;
        private NPCAppraisalEngine _appraisal;
        private NPCActionPlanner _planner;

        [System.Serializable]
        public struct HomeostasisProfile
        {
            public float IdealSocial;
            public float IdealBoredom;
            public float IdealIntroversion;
        }

        [System.Serializable]
        public class ActionWeightRecord
        {
            public string ActionName;
            public float Weight;
        }

        [Header("Neural Weights")]
        public List<ActionWeightRecord> ActionWeights = new List<ActionWeightRecord>();

        public float GetActionWeight(string actionName, float defaultWeight)
        {
            var record = ActionWeights.Find(r => r.ActionName == actionName);
            if (record == null)
            {
                record = new ActionWeightRecord { ActionName = actionName, Weight = defaultWeight };
                ActionWeights.Add(record);
            }
            return record.Weight;
        }

        public void UpdateActionWeight(string actionName, float delta)
        {
            var record = ActionWeights.Find(r => r.ActionName == actionName);
            if (record != null)
            {
                record.Weight = Mathf.Clamp(record.Weight + delta, 0.01f, 1000f);
                if (GPOyun.UI.HUDManager.Instance != null && Mathf.Abs(delta) > 5f)
                {
                    GPOyun.UI.HUDManager.Instance.SpawnEmojiReaction(_controller.transform, delta > 0 ? "🧠+" : "🧠-", Color.cyan);
                }
            }
        }

        [Header("Cognitive State")]
        public string CurrentAim = "Idle";
        public string CurrentContext = "Waiting for simulation to begin.";
        public bool IsWhimActive = false;

        [Header("Psychological Setpoints")]
        public HomeostasisProfile ComfortZone;

        // Emoji tracking to prevent infinite feedback loops (diminishing returns)
        private Dictionary<int, float> _recentEmojis = new Dictionary<int, float>();

        public void Initialize(NPCController controller, NPCNeeds needs, NPCAppraisalEngine appraisal, NPCActionPlanner planner)
        {
            _controller = controller;
            _needs = needs;
            _appraisal = appraisal;
            _planner = planner;
        }

        private void Start()
        {
            if (_controller == null)
            {
                _controller = GetComponent<NPCController>();
                _needs = GetComponent<NPCNeeds>();
                _appraisal = GetComponent<NPCAppraisalEngine>();
                _planner = GetComponent<NPCActionPlanner>();
            }

            // Generate Homeostasis Comfort Zones
            // Base is around 44% (mild melancholy / realistic setpoint) but modified by personality
            float baseSetpoint = 44f;
            float extraversionOffset = (_controller != null && _controller.personality != null) ? (_controller.personality.Extraversion - 0.5f) * 40f : 0f;
            float neuroticismOffset = (_controller != null && _controller.personality != null) ? (_controller.personality.Neuroticism - 0.5f) * 40f : 0f;
            float opennessOffset = (_controller != null && _controller.personality != null) ? (_controller.personality.Openness - 0.5f) * 40f : 0f;

            ComfortZone.IdealSocial = Mathf.Clamp(baseSetpoint + extraversionOffset, 10f, 90f);
            ComfortZone.IdealIntroversion = Mathf.Clamp(baseSetpoint + neuroticismOffset, 10f, 90f); // Higher neuroticism = higher ideal stress/tension (drama addicted)
            ComfortZone.IdealBoredom = Mathf.Clamp(baseSetpoint + opennessOffset, 10f, 90f); // Higher openness = needs more stimulation (lower ideal boredom)
        }

        /// <summary>
        /// Called by Actions when they begin execution to write their rationale to the Brain.
        /// </summary>
        public void SetCognitiveState(string aim, string context, bool isWhim)
        {
            CurrentAim = aim;
            CurrentContext = context;
            IsWhimActive = isWhim;
            
            // Map dominant emotion based on needs if not overridden by Appraisal
            UpdateDominantEmotion();
        }

        private void UpdateDominantEmotion()
        {
            if (_controller.currentEmotion == EmotionType.Angry || _controller.currentEmotion == EmotionType.Fearful || _controller.currentEmotion == EmotionType.Surprised)
            {
                // Let volatile emotions decay naturally before overriding them with baseline needs.
                return;
            }

            if (_needs.SocialDesire < 20)
                _controller.currentEmotion = EmotionType.Sad;
            else if (_needs.Introversion < 10)
                _controller.currentEmotion = EmotionType.Angry;
            else if (_needs.Boredom < 40 && _needs.Introversion > 60)
                _controller.currentEmotion = EmotionType.Bored;
            else if (_needs.SocialDesire > 60 && _needs.Introversion > 40)
                _controller.currentEmotion = EmotionType.Happy;
            else
                _controller.currentEmotion = EmotionType.Neutral;
        }

        /// <summary>
        /// Processes physical AoE impacts from nearby emojis.
        /// </summary>
        public void ProcessEmojiStimulus(string emojiCode, int casterId)
        {
            if (casterId == _controller.NpcId) return; // Don't react to own emojis

            // Desensitization Check
            if (_recentEmojis.TryGetValue(casterId, out float lastTime))
            {
                if (Time.time - lastTime < 5f) return; // Ignore spam
            }
            _recentEmojis[casterId] = Time.time;

            int relationship = GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>() != null 
                ? GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().GetRelationship(_controller.NpcId, casterId) 
                : 0;

            switch (emojiCode)
            {
                case "🤬": // Rage -> Tension / Stress
                    _needs.SatisfyIntroversion(-20f);
                    
                    // IDIOSYNCRATIC: Does this NPC actually LIKE stress?
                    if (ComfortZone.IdealIntroversion < 30f)
                    {
                        // Drama Addict! They enjoy the rage.
                        _controller.currentEmotion = EmotionType.Happy;
                        _needs.SatisfyBoredom(30f);
                    }
                    else if (_needs.Introversion < 30)
                    {
                        // Normal reaction: anger/stress
                        _controller.currentEmotion = EmotionType.Angry;
                        _planner.ForceReevaluate();
                    }
                    break;
                case "🥰": // Love -> Social fulfillment
                    _needs.SatisfySocial(15f);
                    
                    // IDIOSYNCRATIC: Does this NPC hate socializing?
                    if (ComfortZone.IdealSocial < 30f && _needs.SocialDesire > ComfortZone.IdealSocial)
                    {
                        // Smothered! They don't want this love.
                        _controller.currentEmotion = EmotionType.Disgusted;
                        _needs.SatisfyIntroversion(-10f); // Stresses them out
                    }
                    else if (_controller.currentEmotion == EmotionType.Sad) 
                    {
                        _controller.currentEmotion = EmotionType.Neutral;
                    }
                    break;
                case "💔": // Heartbreak -> Empathy or Schadenfreude
                    if (relationship > 50)
                    {
                        _controller.currentEmotion = EmotionType.Sad;
                    }
                    else if (relationship < -50)
                    {
                        _controller.currentEmotion = EmotionType.Happy; // Schadenfreude
                        _needs.SatisfyBoredom(30f);
                    }
                    break;
                case "😨": // Panic -> Cascading fear
                    // IDIOSYNCRATIC: Fear is universally bad, but high-boredom NPCs might find it entertaining after the fact.
                    _controller.currentEmotion = EmotionType.Fearful;
                    _planner.ForceAction(typeof(FleeAction));
                    break;
            }
        }

        /// <summary>
        /// Processes direct peer-to-peer communication requests (e.g., asking to join a group).
        /// Returns true if accepted, false if rejected.
        /// </summary>
        public bool ReceiveHandshakeRequest(string emojiCode, int casterId)
        {
            if (casterId == _controller.NpcId) return false;

            int relationship = GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>() != null 
                ? GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().GetRelationship(_controller.NpcId, casterId) 
                : 0;

            int favoriteId = GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>() != null 
                ? GPOyun.Core.ServiceLocator.Get<GPOyun.Core.RelationshipMatrix>().GetFavorite(_controller.NpcId) 
                : -1;

            bool isLoyaltyOverride = (casterId == favoriteId);

            if (emojiCode == "👋") // Invite to socialize/walk
            {
                // Setpoint Evaluation: Do I want to socialize right now?
                float currentSocialDeficit = Mathf.Abs(_needs.SocialDesire - ComfortZone.IdealSocial);
                float predictedSocialDeficit = ComfortZone.IdealSocial; // Assuming socializing drops it to 0

                // Also check if we hate the person
                if (relationship < -20)
                {
                    if (UI.HUDManager.Instance != null) UI.HUDManager.Instance.SpawnEmojiReaction(_controller.transform, "🤬", Color.red);
                    return false; // Reject!
                }

                // If socializing pulls us away from comfort zone (Behavioral Degradation), we reject it!
                if (!isLoyaltyOverride && currentSocialDeficit < predictedSocialDeficit - 10f)
                {
                    // I am perfectly comfortable right now. Go away.
                    if (UI.HUDManager.Instance != null) UI.HUDManager.Instance.SpawnEmojiReaction(_controller.transform, "🙅‍♂️", new Color(0.8f, 0.8f, 0.8f));
                    return false; // Reject!
                }

                // Accept!
                if (UI.HUDManager.Instance != null) UI.HUDManager.Instance.SpawnEmojiReaction(_controller.transform, "🥰", new Color(1f, 0.4f, 0.6f));
                
                // Force our planner to switch to SocializeAction to join them
                _planner.ForceAction(typeof(SocializeAction));
                return true;
            }

            if (emojiCode == "🚶‍♂️") // Invite for a group walk
            {
                // Similar logic but forces GroupWalkAction
                float currentBoredomDeficit = Mathf.Abs(_needs.Boredom - ComfortZone.IdealBoredom);
                float predictedBoredomDeficit = ComfortZone.IdealBoredom; // Walking drops boredom to 0

                if (relationship < -20)
                {
                    if (UI.HUDManager.Instance != null) UI.HUDManager.Instance.SpawnEmojiReaction(_controller.transform, "🤬", Color.red);
                    return false; // Reject!
                }

                if (!isLoyaltyOverride && currentBoredomDeficit < predictedBoredomDeficit - 10f)
                {
                    if (UI.HUDManager.Instance != null) UI.HUDManager.Instance.SpawnEmojiReaction(_controller.transform, "🥱", new Color(0.8f, 0.8f, 0.8f));
                    return false; // Reject!
                }

                if (UI.HUDManager.Instance != null) UI.HUDManager.Instance.SpawnEmojiReaction(_controller.transform, "👍", Color.green);
                _planner.ForceAction(typeof(GroupWalkAction));
                return true;
            }

            return false;
        }
    }
}
