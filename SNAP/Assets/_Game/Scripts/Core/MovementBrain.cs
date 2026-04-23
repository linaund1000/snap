using UnityEngine;

namespace GPOyun.Core
{
    /// <summary>
    /// Higher-level coordinator that combines the Motor and ObstacleAvoidance.
    /// Acts as the 'navigator' for characters to ensure collision-free travel.
    /// </summary>
    [RequireComponent(typeof(CharacterMotor))]
    [RequireComponent(typeof(ObstacleAvoidance))]
    public class MovementBrain : MonoBehaviour
    {
        private CharacterMotor _motor;
        private ObstacleAvoidance _avoidance;

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();
            _avoidance = GetComponent<ObstacleAvoidance>();
        }

        /// <summary>
        /// Moves towards a target while surgically avoiding Layer 7 obstacles.
        /// </summary>
        public void MoveSafely(Vector3 inputDir)
        {
            if (inputDir.sqrMagnitude < 0.01f)
            {
                _motor.MoveInDirection(Vector3.zero);
                return;
            }

            // Calculate avoidance correction
            Vector3 correction = _avoidance.GetAvoidanceVector(inputDir);
            
            // Combine Intent + Avoidance
            Vector3 finalDir = (inputDir + correction).normalized;
            
            _motor.MoveInDirection(finalDir);
        }

        /// <summary>
        /// Dedicated logic for NPC random wandering with avoidance.
        /// </summary>
        public void NavigateNPC(Vector3 worldTarget)
        {
            Vector3 toTarget = (worldTarget - transform.position).normalized;
            toTarget.y = 0;
            
            MoveSafely(toTarget);
        }
    }
}
