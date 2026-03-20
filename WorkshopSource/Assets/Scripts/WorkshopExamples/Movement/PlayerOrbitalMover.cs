using UnityEngine;

    /// <summary>
    /// Moves the player relative to the camera's viewing angle.
    /// Detects moving platforms via downward raycast and adds their
    /// velocity on top of input velocity so neither overwrites the other.
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

        [Header("Platform Detection")]
        [Tooltip("Layer mask for moving platforms.")]
        [SerializeField] private LayerMask m_platformLayer;

        [Tooltip("How far below the player's origin to raycast. Match to half player height.")]
        [SerializeField] private float m_groundCheckDistance = 1.1f;

        private Rigidbody m_rigidbody;
        private UnityEngine.Camera m_mainCamera;
        private Rigidbody m_currentPlatform;
        private float m_horizontalInput;
        private float m_verticalInput;
        #endregion

        #region Properties
        public float MoveSpeed
        {
            get => this.m_moveSpeed;
            set => this.m_moveSpeed = value;
        }
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            this.m_rigidbody = GetComponent<Rigidbody>();
            this.m_rigidbody.freezeRotation = true;
            this.m_mainCamera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            DetectPlatform();
            ApplyMovement();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                transform.position,
                transform.position + Vector3.down * this.m_groundCheckDistance
            );
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Reads raw axis input for movement.
        /// </summary>
        private void HandleInput()
        {
            this.m_horizontalInput = Input.GetAxisRaw(k_horizontalAxis);
            this.m_verticalInput = Input.GetAxisRaw(k_verticalAxis);
        }

        /// <summary>
        /// Raycasts downward to find a moving platform beneath the player.
        /// </summary>
        private void DetectPlatform()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, this.m_groundCheckDistance, this.m_platformLayer))
                this.m_currentPlatform = hit.collider.attachedRigidbody;
            else
                this.m_currentPlatform = null;
        }

        /// <summary>
        /// Applies velocity to the Rigidbody based on input relative to the camera,
        /// with platform velocity added on top so neither overwrites the other.
        /// </summary>
        private void ApplyMovement()
        {
            if (this.m_mainCamera == null)
            {
                this.m_mainCamera = UnityEngine.Camera.main;
                if (this.m_mainCamera == null) return;
            }

            // Get camera forward and right vectors, flattened on the XZ plane
            Vector3 camForward = this.m_mainCamera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = this.m_mainCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            // Calculate movement direction relative to camera
            Vector3 direction = (camForward * this.m_verticalInput + camRight * this.m_horizontalInput).normalized;

            // Start with input velocity
            Vector3 targetVelocity = direction * this.m_moveSpeed;

            // Add platform velocity on top so input doesn't overwrite it
            if (this.m_currentPlatform != null)
            {
                targetVelocity.x += this.m_currentPlatform.linearVelocity.x;
                targetVelocity.z += this.m_currentPlatform.linearVelocity.z;
            }

            // Apply XZ velocity, preserve Y for gravity and jumping
            this.m_rigidbody.linearVelocity = new Vector3(
                targetVelocity.x,
                this.m_rigidbody.linearVelocity.y,
                targetVelocity.z
            );

            // Rotate the player model to face the direction of movement
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * this.m_rotationSpeed);
            }
        }
        #endregion
    }
