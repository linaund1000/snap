using System;
using System.Collections.Generic;
using UnityEngine;
using GPOyun.Newspaper;
using GPOyun.NPC;

namespace GPOyun
{
    /// <summary>
    /// ScriptableObject that defines an NPC's personality and how they react
    /// to different news categories. Configure in the Unity Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "NPC_Personality", menuName = "GPOyun/NPC Personality")]
    public class NPCPersonalityData : ScriptableObject
    {
        [Header("Personality Traits")]
        [Range(0f, 1f)] public float Agreeableness   = 0.5f;
        [Range(0f, 1f)] public float Neuroticism      = 0.5f;
        [Range(0f, 1f)] public float Conscientiousness = 0.5f;
        [Range(0f, 1f)] public float Extraversion     = 0.5f;
        [Range(0f, 1f)] public float Openness         = 0.5f;

        [Header("News Reactions")]
        [SerializeField] private List<NewsReactionRule> reactionRules;

        public NewsReaction GetReactionTo(NewsCategory category)
        {
            if (reactionRules != null)
            {
                foreach (var rule in reactionRules)
                    if (rule.Category == category) return rule.Reaction;
            }

            // Default fallback: neutral
            return new NewsReaction { Emotion = EmotionType.Neutral, Intensity = 0f };
        }

        /// <summary>
        /// Creates a unique instance of this personality by adding random variance to each trait.
        /// This ensures NPCs sharing a base profile feel authentic.
        /// </summary>
        public NPCPersonalityData CreateVariant(float variance)
        {
            var variant = Instantiate(this);
            variant.name = this.name + "_Variant";

            variant.Agreeableness = Mathf.Clamp01(Agreeableness + UnityEngine.Random.Range(-variance, variance));
            variant.Neuroticism = Mathf.Clamp01(Neuroticism + UnityEngine.Random.Range(-variance, variance));
            variant.Conscientiousness = Mathf.Clamp01(Conscientiousness + UnityEngine.Random.Range(-variance, variance));
            variant.Extraversion = Mathf.Clamp01(Extraversion + UnityEngine.Random.Range(-variance, variance));
            variant.Openness = Mathf.Clamp01(Openness + UnityEngine.Random.Range(-variance, variance));

            return variant;
        }
    }

    [Serializable]
    public class NewsReactionRule
    {
        public NewsCategory Category;
        public NewsReaction Reaction;
    }

    [Serializable]
    public class NewsReaction
    {
        public EmotionType Emotion;
        [Range(0f, 1f)] public float Intensity;
    }
}
