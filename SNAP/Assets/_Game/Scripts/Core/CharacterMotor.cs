using UnityEngine;

namespace GPOyun.Core
{
    /// <summary>
    /// The deterministic physical driver for all characters.
    /// Acts as the single source of truth for movement physics, gravity, and rotation.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMotor : MonoBehaviour
    {
        [Header("Movement Tuning")]
        public float MoveSpeed = 7f;
        public float Acceleration = 10f;
        public float Deceleration = 15f;
        public float TurnSpeed = 15f;
        public float Gravity = 20f;

        private CharacterController _cc;
        private Vector3 _currentVelocity;
        private Vector3 _inputDirection;
        private float _verticalVelocity;
        private bool _isGrounded;
        private int _groundMask;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            
            // Default Layers (6: Ground, 8: Character)
            _groundMask = (1 << 6); 
            gameObject.layer = 8;    
        }

        /// <summary>
        /// Sets the intended movement direction.
        /// Decoupled from the physical update cycle.
        /// </summary>
        public void SetInputDirection(Vector3 direction)
        {
            _inputDirection = direction;
        }

        /// <summary>
        /// Legacy support for older calls.
        /// </summary>
        public void MoveInDirection(Vector3 direction) => SetInputDirection(direction);

        /// <summary>
        /// Moves towards a specific world-space destination.
        /// Primarily used by tests and high-level controllers.
        /// </summary>
        public void MoveToDestination(Vector3 destination)
        {
            Vector3 toTarget = (destination - transform.position);
            toTarget.y = 0;
            
            if (toTarget.sqrMagnitude > 0.01f)
            {
                SetInputDirection(toTarget.normalized);
            }
            else
            {
                SetInputDirection(Vector3.zero);
            }
        }

        private void Update()
        {
            CheckGrounding();
            ProcessMovement();
        }

        private void CheckGrounding()
        {
            float kneeOffset = 0.5f;
            float castDist = 0.7f; 
            _isGrounded = Physics.Raycast(transform.position + Vector3.up * kneeOffset, Vector3.down, castDist, _groundMask);
            
            // Reset vertical velocity when grounded
            if (_isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = -2f; // Slight force to keep grounded
            }
            else
            {
                _verticalVelocity -= Gravity * Time.deltaTime;
            }
        }

        private void ProcessMovement()
        {
            if (_cc == null || !_cc.enabled) return;

            // 1. Calculate Target Velocity
            Vector3 targetDir = _inputDirection;
            targetDir.y = 0;
            
            Vector3 targetVel = targetDir.normalized * MoveSpeed;
            float currentAccel = targetDir.sqrMagnitude > 0.01f ? Acceleration : Deceleration;
            
            // 2. Smoothly approach target velocity
            // Multiplier (5f) remains to preserve existing "feel" of snappy acceleration
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVel, currentAccel * Time.deltaTime * 5f);

            // 3. Apply Movement
            Vector3 finalMove = _currentVelocity;
            finalMove.y = _verticalVelocity;
            
            _cc.Move(finalMove * Time.deltaTime);

            // 4. Handle Rotation
            if (targetDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, TurnSpeed * Time.deltaTime);
            }
        }

        public float GetCurrentSpeed() => _currentVelocity.magnitude;
        public Vector3 GetCurrentVelocity() => _currentVelocity;
    }
}
