using UnityEngine;
using GPOyun.Core;

namespace GPOyun.Testing
{
    /// <summary>
    /// Utility to verify that the movement pipeline is deterministic.
    /// It moves a character to a target and measures precision.
    /// </summary>
    public class DeterministicMovementTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        public Vector3 TestTarget = new Vector3(10, 0, 10);
        public float TimeLimit = 5.0f;
        public float PrecisionThreshold = 0.5f;

        private CharacterMotor _motor;
        private float _startTime;
        private bool _isTesting = false;

        [ContextMenu("Run Motor Test")]
        public void StartTest()
        {
            // Setup a clean test subject if none exists
            GameObject testSubject = GameObject.Find("MovementTestSubject");
            if (testSubject == null)
            {
                testSubject = new GameObject("MovementTestSubject");
                testSubject.transform.position = Vector3.zero;
                testSubject.AddComponent<CharacterController>();
                _motor = testSubject.AddComponent<CharacterMotor>();
                
                // Add a visual so we can see it
                GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                visual.transform.SetParent(testSubject.transform);
                visual.transform.localPosition = Vector3.zero;
            }
            else
            {
                _motor = testSubject.GetComponent<CharacterMotor>();
                testSubject.transform.position = Vector3.zero;
            }

            _startTime = Time.time;
            _isTesting = true;
            Debug.Log($"[Test] Starting deterministic walk to {TestTarget}...");
        }

        private void Update()
        {
            if (!_isTesting || _motor == null) return;

            // Execute automated walk
            // Using explicit cast or check to ensure we use GPOyun.Core.CharacterMotor
            ((GPOyun.Core.CharacterMotor)_motor).MoveToDestination(TestTarget);

            float distance = Vector3.Distance(_motor.transform.position, TestTarget);
            float elapsed = Time.time - _startTime;

            if (distance < PrecisionThreshold)
            {
                _isTesting = false;
                Debug.Log($"<color=green>[Test Passed]</color> Reached target in {elapsed:F2}s with {distance:F4} precision.");
                return;
            }

            if (elapsed > TimeLimit)
            {
                _isTesting = false;
                Debug.LogError($"<color=red>[Test Failed]</color> Timeout! Still {distance:F2}m away from target after {elapsed}s.");
            }
        }
    }
}
