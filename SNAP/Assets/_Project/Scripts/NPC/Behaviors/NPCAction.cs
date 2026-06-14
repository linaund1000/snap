using System.Collections;
using UnityEngine;

namespace GPOyun.NPC.UtilityAI
{
    public abstract class NPCAction : MonoBehaviour
    {
        public string ActionName;
        public float BaseUtility = 10f;
        
        protected NPCController Controller;
        protected NPCNeeds Needs;

        public virtual void Initialize(NPCController controller, NPCNeeds needs)
        {
            Controller = controller;
            Needs = needs;
        }

        protected float PreActionSocial;
        protected float PreActionBoredom;
        protected float PreActionIntroversion;

        /// <summary>
        /// Returns the dynamically learned utility weight from the NPC's Neural Brain.
        /// </summary>
        public virtual float CalculateUtility()
        {
            if (Controller != null && Controller.Brain != null)
            {
                // Social priority: Loneliness crushes non-social action utility!
                float weight = Controller.Brain.GetActionWeight(ActionName, BaseUtility);
                
                if (Needs.SocialDesire > 80f && ActionName != "Socialize" && ActionName != "Argue" && ActionName != "GroupWalk")
                {
                    weight *= 0.1f; // Massive penalty for loneliness!
                }
                
                return weight;
            }
            return BaseUtility;
        }

        public void SnapshotNeeds()
        {
            if (Needs != null)
            {
                PreActionSocial = Needs.SocialDesire;
                PreActionBoredom = Needs.Boredom;
                PreActionIntroversion = Needs.Introversion;
            }
        }

        public virtual float EvaluateReward()
        {
            if (Needs == null || Controller == null || Controller.Brain == null) return 0f;

            float idealSocial = Controller.Brain.ComfortZone.IdealSocial;
            float idealBoredom = Controller.Brain.ComfortZone.IdealBoredom;
            float idealIntroversion = Controller.Brain.ComfortZone.IdealIntroversion;

            float preDistSocial = Mathf.Abs(PreActionSocial - idealSocial);
            float postDistSocial = Mathf.Abs(Needs.SocialDesire - idealSocial);

            float preDistBoredom = Mathf.Abs(PreActionBoredom - idealBoredom);
            float postDistBoredom = Mathf.Abs(Needs.Boredom - idealBoredom);

            float preDistIntro = Mathf.Abs(PreActionIntroversion - idealIntroversion);
            float postDistIntro = Mathf.Abs(Needs.Introversion - idealIntroversion);

            float reward = 0f;
            reward += (preDistSocial - postDistSocial); 
            reward += (preDistBoredom - postDistBoredom);
            reward += (preDistIntro - postDistIntro);

            // Online Reinforcement Learning (Train the action weight)
            Controller.Brain.UpdateActionWeight(ActionName, reward * 0.5f); 
            
            return reward;
        }

        /// <summary>
        /// The main coroutine logic for executing the action.
        /// </summary>
        public abstract IEnumerator Execute();

        /// <summary>
        /// Called if another action suddenly spikes in utility and overrides this one.
        /// </summary>
        public abstract void Interrupt();
    }
}
