using NUnit.Framework;
using UnityEngine;
using GPOyun.NPC.UtilityAI;
using GPOyun.Core;

namespace GPOyun.Tests
{
    public class NPCBrainTests
    {
        private GameObject _npcObject;
        private NPCBrain _brain;
        private NPCNeeds _needs;
        private RelationshipMatrix _matrix;
        
        [SetUp]
        public void Setup()
        {
            // Set up a mock GameObject and brain
            _npcObject = new GameObject("TestNPC");
            _brain = _npcObject.AddComponent<NPCBrain>();
            _needs = _npcObject.AddComponent<NPCNeeds>();
            
            // We need a dummy controller for the ID
            var controller = _npcObject.AddComponent<NPC.NPCController>();
            
            // Initialize ServiceLocator and mock RelationshipMatrix
            ServiceLocator.ClearAll();
            var matrixObj = new GameObject("MockMatrix");
            _matrix = matrixObj.AddComponent<RelationshipMatrix>();
            // The Awake of RelationshipMatrix will register it automatically!
            _matrix.InitializeMatrix();

            // Initialize the brain
            _brain.Initialize(controller, _needs, null, null);
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_npcObject);
            if (_matrix != null) Object.DestroyImmediate(_matrix.gameObject);
            ServiceLocator.ClearAll();
        }

        [Test]
        public void Brain_Generates_Valid_ComfortZone()
        {
            // Act
            // Since Initialize was called, Start hasn't been called. We simulate Start logic manually for testing
            _brain.SendMessage("Start");

            // Assert
            var comfortZone = _brain.ComfortZone;
            Assert.IsTrue(comfortZone.IdealSocial >= 0f && comfortZone.IdealSocial <= 100f, "IdealSocial out of bounds.");
            Assert.IsTrue(comfortZone.IdealBoredom >= 0f && comfortZone.IdealBoredom <= 100f, "IdealBoredom out of bounds.");
            Assert.IsTrue(comfortZone.IdealIntroversion >= 0f && comfortZone.IdealIntroversion <= 100f, "IdealIntroversion out of bounds.");
        }

        [Test]
        public void ReceiveHandshakeRequest_From_Enemy_Is_Rejected()
        {
            // Arrange
            _brain.SendMessage("Start");
            int myId = _npcObject.GetComponent<NPC.NPCController>().NpcId;
            int enemyId = 99;

            // Make them an enemy (-50 relation)
            _matrix.ModifyRelationship(myId, enemyId, -50);

            // Act
            bool accepted = _brain.ReceiveHandshakeRequest("👋", enemyId);

            // Assert
            Assert.IsFalse(accepted, "NPC should reject handshake from an enemy.");
        }

        [Test]
        public void ReceiveHandshakeRequest_When_Socially_Saturated_Is_Rejected()
        {
            // Arrange
            _brain.SendMessage("Start");
            int myId = _npcObject.GetComponent<NPC.NPCController>().NpcId;
            int friendId = 55;

            // Make them a friend
            _matrix.ModifyRelationship(myId, friendId, 50);

            // Force social need to EXACTLY match the comfort zone ideal (meaning perfectly comfortable)
            // If they are perfectly comfortable, they won't want to socialize and will reject.
            _needs.SatisfySocial(-100f); // Reset
            _needs.SatisfySocial(_brain.ComfortZone.IdealSocial); 

            // Act
            bool accepted = _brain.ReceiveHandshakeRequest("👋", friendId);

            // Assert
            Assert.IsFalse(accepted, "NPC should reject handshake when perfectly comfortable socially.");
        }
    }
}
