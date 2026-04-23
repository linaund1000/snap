using UnityEngine;
using GPOyun.Events;
using GPOyun.NPC;
using GPOyun.Newspaper;

namespace GPOyun.Testing
{
    /// <summary>
    /// A First Principles 'Smoke Test' to verify the core game loop.
    /// This script demonstrates how the 'Theatre' of the game works.
    /// Attach to any GameObject and check the console.
    /// </summary>
    public class GP_System_SmokeTest : MonoBehaviour
    {
        [Header("References")]
        public NPCController testNpc;
        
        [ContextMenu("Run Full Game Cycle Test")]
        public void RunTest()
        {
            Debug.Log("--- [START SMOKE TEST] ---");

            // 1. PHASE CHANGE: EVENING
            Debug.Log("1. Phase -> Evening. Player is at the office.");
            EventBus.Instance.Publish(new DayPhaseChangedEvent { NewPhase = DayPhase.Evening });

            // 2. PLAYER DECISION: PUBLISH
            Debug.Log("2. Player publishes a SCANDAL photo.");
            NewspaperManager.Instance.PublishEdition(new NewsStory { 
                Headline = "SCANDAL IN THE PLAZA", 
                Category = NewsCategory.Scandal 
            });

            // 3. PHASE CHANGE: NIGHT -> MORNING
            Debug.Log("3. Phase -> Night (Cleanup) -> Morning (Routine Start).");
            EventBus.Instance.Publish(new DayPhaseChangedEvent { NewPhase = DayPhase.Night });
            EventBus.Instance.Publish(new DayPhaseChangedEvent { NewPhase = DayPhase.Morning });

            // 4. VERIFY NPC BEHAVIOUR
            // We can't wait for real-time here in a simple method, but we can see the logs:
            // - The NPC will stagger.
            // - The NPC will move to the board.
            // - The NPC will read and THEN react.
            
            Debug.Log("4. Verification: Look for 'Reacted to news after reading' in the console and watch the NPC move.");
            Debug.Log("--- [END SMOKE TEST] ---");
        }

        private void Start()
        {
            Debug.Log("GP-OYUN Smoke Test ready. Right-click the component in Inspector and select 'Run Full Game Cycle Test' to simulate a day.");
        }
    }
}
