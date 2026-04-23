using UnityEngine;
using UnityEngine.InputSystem;
using GPOyun.Events;

namespace GPOyun.Player
{
    /// <summary>
    /// Basic MVP Player Controller.
    /// Handles WASD movement and simple camera capture logic.
    /// </summary>
    [RequireComponent(typeof(GPOyun.Core.MovementBrain))]
    public class PlayerController : MonoBehaviour
    {
        private GPOyun.Core.MovementBrain _brain;
        private GPOyun.Core.CharacterMotor _motor; // Keep for telemetry

        private float _telemetryTimer;

        private void Awake()
        {
            _brain = GetComponent<GPOyun.Core.MovementBrain>();
            _motor = GetComponent<GPOyun.Core.CharacterMotor>();
            
            // Physical Assurance
            var cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = true;

            CreateLabel();
        }

        private void CreateLabel()
        {
            GameObject labelGo = new GameObject("ID_Label");
            labelGo.transform.SetParent(this.transform);
            labelGo.transform.localPosition = new Vector3(0, 2.2f, 0);
            
            var text = labelGo.AddComponent<TextMesh>();
            text.text = "PLAYER (0)";
            text.characterSize = 0.2f;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = Color.white;
            text.fontStyle = FontStyle.Bold;
        }

        [Header("Controls")]
        [SerializeField] private float lookSensitivity = 1.0f;

        private void Update()
        {
            HandleMovement();
            MeasureTelemetry();
        }

        private void HandleMovement()
        {
            if (_brain == null) return;

            // 1. Get Input Values
            float x = 0;
            float z = 0;
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            
            if (keyboard == null) return;

            if (keyboard.wKey.isPressed) z += 1;
            if (keyboard.sKey.isPressed) z -= 1;
            if (keyboard.aKey.isPressed) x -= 1;
            if (keyboard.dKey.isPressed) x += 1;

            Vector3 inputVector = new Vector3(x, 0, z);
            Vector3 moveDir = Vector3.zero;

            // 2. Transform relative to Camera
            if (inputVector.sqrMagnitude > 0.01f)
            {
                var camera = UnityEngine.Camera.main;
                if (camera != null)
                {
                    Vector3 forward = camera.transform.forward;
                    Vector3 right = camera.transform.right;
                    forward.y = 0;
                    right.y = 0;
                    forward.Normalize();
                    right.Normalize();

                    moveDir = (forward * inputVector.z + right * inputVector.x).normalized;
                }
                else
                {
                    moveDir = inputVector.normalized;
                }
            }

            // 3. Apply Speed/Sensitivity (Sensitivity here acts as a general speed multiplier if desired, 
            // but primarily we use the base MoveSpeed in Motor. We allow scaling here if needed.)
            _brain.MoveSafely(moveDir);
        }

        public void SetSensitivity(float value) => lookSensitivity = value;

        private void MeasureTelemetry()
        {
            _telemetryTimer += Time.deltaTime;
            if (_telemetryTimer >= 0.1f && EventBus.Instance != null && _motor != null)
            {
                _telemetryTimer = 0f;
                EventBus.Instance.Publish(new MovementMetricsEvent {
                    EntityId = "Player",
                    Velocity = _motor.GetCurrentVelocity(),
                    Speed = _motor.GetCurrentSpeed(),
                    Position = transform.position
                });
            }
        }
    }
}
