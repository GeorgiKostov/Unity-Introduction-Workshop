using UnityEngine;

namespace WorkshopExamples.Movement
{
    /// <summary>
    /// Allows the player to jump when pressing Space.
    /// Uses a downward raycast to detect if the player is grounded.
    /// Requires a Rigidbody on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJumper : MonoBehaviour
    {
        #region Constants
        private const string k_jumpButton = "Jump";
        #endregion

        #region Fields
        [Header("Jump Settings")]
        [Tooltip("How high the player jumps. Try values between 4 and 12.")]
        [SerializeField] private float m_jumpForce = 9f;

        [Tooltip("Multiplier applied to gravity when the player is falling.")]
        [SerializeField] private float m_fallMultiplier = 2.5f;

        [Tooltip("Multiplier applied to gravity when the player releases the jump button early.")]
        [SerializeField] private float m_lowJumpMultiplier = 2f;

        [Header("Ground Detection")]
        [Tooltip("How far down to check for ground. Match to half player height.")]
        [SerializeField] private float m_groundCheckDistance = 1.1f;

        [Tooltip("Which layers count as ground. Set to 'Default' to start.")]
        [SerializeField] private LayerMask m_groundLayer;

        private Rigidbody m_rigidbody;
        private bool m_isJumpRequested;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Cache reference to the Rigidbody component.
            m_rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // Read input in Update.
            // GetButtonDown only fires once per key press — perfect for jumping.
            if (Input.GetButtonDown(k_jumpButton) && IsGrounded())
            {
                m_isJumpRequested = true;
            }
        }

        private void FixedUpdate()
        {
            // Apply physics movement in FixedUpdate.
            if (m_isJumpRequested)
            {
                ApplyJump();
                m_isJumpRequested = false;
            }

            ApplyGravityModifiers();
        }

        // Draws the ground check ray in the Scene view for debugging.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                transform.position,
                transform.position + Vector3.down * m_groundCheckDistance
            );
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Applies an upward force to the Rigidbody.
        /// </summary>
        private void ApplyJump()
        {
            // Reset Y velocity first so double-jumps don't stack.
            m_rigidbody.linearVelocity = new Vector3(
                m_rigidbody.linearVelocity.x, 
                0f, 
                m_rigidbody.linearVelocity.z
            );

            // Apply an instant upward force (Impulse mode ignores mass scaling).
            m_rigidbody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        /// <summary>
        /// Modifies gravity to create snappier, Mario-style jumps.
        /// </summary>
        private void ApplyGravityModifiers()
        {
            if (m_rigidbody.linearVelocity.y < 0)
            {
                // Falling: apply heavier gravity to snap down quickly
                m_rigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (m_fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (m_rigidbody.linearVelocity.y > 0 && !Input.GetButton(k_jumpButton))
            {
                // Rising but player released jump button early: apply heavier gravity for a short hop
                m_rigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (m_lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// Shoots a short ray downward from the player's center.
        /// Returns true if it hits something on the ground layer.
        /// </summary>
        private bool IsGrounded()
        {
            return Physics.Raycast(
                transform.position,
                Vector3.down,
                m_groundCheckDistance,
                m_groundLayer
            );
        }
        #endregion
    }
}
