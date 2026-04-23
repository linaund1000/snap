using NUnit.Framework;
using UnityEngine;
using GPOyun;
using GPOyun.Events;
using GPOyun.NPC;
using GPOyun.NPC.States;
using GPOyun.Emotions;

namespace GPOyun.Tests
{
    /// <summary>
    /// Deterministic Test Suite for NPC FSM and News Propagation.
    /// Run these in Unity via Window > General > Test Runner.
    /// </summary>
    public class DeterministicFSMTests
    {
        private GameObject _npcObject;
        private NPCController _npc;
        private NPCPersonalityData _mockPersonality;
        private GameObject _boardObject;

        [SetUp]
        public void Setup()
        {
            // Create a clean NPC
            _npcObject = new GameObject("TestNPC");
            _npc = _npcObject.AddComponent<NPCController>();
            
            // Mock Personality: 1.0 reaction to Scandal
            _mockPersonality = ScriptableObject.CreateInstance<NPCPersonalityData>();

            _boardObject = new GameObject("BoardPosition");
            
            // Force initialize logic
            _npc.InitializeForTest(_mockPersonality, _boardObject.transform);
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_npcObject);
            Object.DestroyImmediate(_boardObject);
            Object.DestroyImmediate(_mockPersonality);
        }

        [Test]
        public void Test_NewsBuffering_DoesNotTriggerReactionImmediately()
        {
            // 1. Send news
            var news = new NewsPublishedEvent { 
                FrontPage = new NewsStory { Category = NewsCategory.Scandal, Headline = "Title" } 
            };
            
            _npc.SendMessage("OnNewsPublished", news); 

            // 2. Verify state is still Neutral (INITIAL state)
            Assert.AreEqual(EmotionType.Neutral, _npc.EmotionMachine.CurrentState.EmotionType);
        }

        [Test]
        public void Test_FullReadingSequence_TriggersReactionAtTheEnd()
        {
            // 1. Buffer news
            var news = new NewsPublishedEvent { 
                FrontPage = new NewsStory { Category = NewsCategory.Scandal, Headline = "Title" } 
            };
            _npc.SendMessage("OnNewsPublished", news);

            // 2. Start Walking to Board
            _npc.BehaviourMachine.ChangeState(new NPCWalkingState(_npc, _boardObject.transform.position));
            Assert.IsInstanceOf<NPCWalkingState>(_npc.BehaviourMachine.CurrentState);

            // 3. Reach Destination -> Switch to Reading
            _npc.OnReachDestination();
            Assert.IsInstanceOf<NPCReadingNewsState>(_npc.BehaviourMachine.CurrentState);
            
            // Still no reaction while reading
            Assert.AreEqual(EmotionType.Neutral, _npc.EmotionMachine.CurrentState.EmotionType);

            // 4. Process news (simulates reading completion)
            _npc.ProcessReadNews();

            // 5. Verify Reaction Triggered
            Assert.IsNotNull(_npc.EmotionMachine.CurrentState);
        }

        [Test]
        public void Test_NightlyCleanup_ClearsPendingNews()
        {
            // 1. Receive news
            var news = new NewsPublishedEvent { 
                FrontPage = new NewsStory { Category = NewsCategory.Local, Headline = "Title" } 
            };
            _npc.SendMessage("OnNewsPublished", news);

            // 2. Transition to NIGHT
            var nightEvent = new DayPhaseChangedEvent { NewPhase = DayPhase.Night };
            _npc.SendMessage("OnPhaseChanged", nightEvent);

            // 3. Verify news is gone
            _npc.ProcessReadNews();
            // Reaction shouldn't change from Neutral
            Assert.AreEqual(EmotionType.Neutral, _npc.EmotionMachine.CurrentState.EmotionType);
        }
    }
}
