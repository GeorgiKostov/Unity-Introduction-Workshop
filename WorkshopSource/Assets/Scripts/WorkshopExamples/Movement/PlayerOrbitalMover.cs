using UnityEngine;

namespace WorkshopExamples.Movement
{
    /// <summary>
    /// Moves the player relative to the camera's viewing angle.
    /// Requires a Rigidbody and Main Camera in the scene.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerOrbitalMover : MonoBehaviour
    {
        #region Constants
        private const string k_horizontalAxis = "Horizontal";
        private const string k_verticalAxis = "Vertical";
        #endregion

        #region Fields
        [Header("Movement Settings")]
        [Tooltip("How fast the player moves across the ground.")]
        [SerializeField] private float m_moveSpeed = 7f;

        [Tooltip("How fast the player rotates to face the move direction.")]
        [SerializeField] private float m_rotationSpeed = 25f;

        private Rigidbody m_rigidbody;
        private UnityEngine.Camera m_mainCamera;
        private float m_horizontalInput;
        private float m_verticalInput;
        #endregion

        #region Properties
        public float MoveSpeed
        {
            get => m_moveSpeed;
            set => m_moveSpeed = value;
        }
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.freezeRotation = true;
            m_mainCamera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Reads raw axis input for movement.
        /// </summary>
        private void HandleInput()
        {
            m_horizontalInput = Input.GetAxisRaw(k_horizontalAxis);
            m_verticalInput = Input.GetAxisRaw(k_verticalAxis);
        }

        /// <summary>
        /// Applies velocity to the Rigidbody based on input relative to the camera.
        /// </summary>
        private void ApplyMovement()
        {
            if (m_mainCamera == null)
            {
                m_mainCamera = UnityEngine.Camera.main;
                if (m_mainCamera == null) return;
            }

            // Get camera forward and right vectors, flattened on the XZ plane
            Vector3 camForward = m_mainCamera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = m_mainCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            // Calculate movement direction relative to camera
            Vector3 direction = (camForward * m_verticalInput + camRight * m_horizontalInput).normalized;

            // Apply velocity
            Vector3 targetVelocity = direction * m_moveSpeed;
            
            m_rigidbody.linearVelocity = new Vector3(
                targetVelocity.x,
                m_rigidbody.linearVelocity.y, // Preserve vertical velocity (gravity/jump)
                targetVelocity.z
            );

            // Rotate the player model to face the direction of movement
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * m_rotationSpeed);
            }
        }
        #endregion
    }
}
