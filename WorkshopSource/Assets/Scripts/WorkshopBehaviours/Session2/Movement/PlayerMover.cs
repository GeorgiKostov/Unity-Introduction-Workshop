using UnityEngine;

namespace WorkshopBehaviours.Session2.Movement
{
    /// <summary>
    /// Moves the player along the X and Z axes using keyboard input.
    /// Requires a Rigidbody on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMover : MonoBehaviour
    {
        #region Constants
        private const string k_horizontalAxis = "Horizontal";
        private const string k_verticalAxis = "Vertical";
        #endregion

        #region Fields
        [Header("Movement Settings")]
        [Tooltip("How fast the player moves across the ground.")]
        [SerializeField] private float m_moveSpeed = 6f;

        private Rigidbody m_rigidbody;
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
            // Cache reference to the Rigidbody component.
            this.m_rigidbody = GetComponent<Rigidbody>();

            // Prevent the player from tipping over when hit by physics.
            this.m_rigidbody.freezeRotation = true;
        }

        private void Update()
        {
            // Read input in Update for responsiveness.
            HandleInput();
        }

        private void FixedUpdate()
        {
            // Apply physics movement in FixedUpdate.
            ApplyMovement();
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
        /// Applies velocity to the Rigidbody based on input.
        /// </summary>
        private void ApplyMovement()
        {
            // Build a direction vector on the flat XZ plane.
            Vector3 direction = new Vector3(this.m_horizontalInput, 0f, this.m_verticalInput).normalized;

            // Apply velocity directly so movement feels instant and responsive.
            Vector3 targetVelocity = direction * this.m_moveSpeed;
            
            this.m_rigidbody.linearVelocity = new Vector3(
                targetVelocity.x,
                this.m_rigidbody.linearVelocity.y, // Preserve vertical velocity (gravity/jump).
                targetVelocity.z
            );
        }
        #endregion
    }
}
