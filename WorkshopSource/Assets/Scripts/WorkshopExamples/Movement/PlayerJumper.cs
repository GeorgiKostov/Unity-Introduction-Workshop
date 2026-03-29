using UnityEngine;

namespace WorkshopBehaviours.Session2.Movement
{
    /// <summary>
    /// Allows the player to jump when pressing Space.
    /// Uses a SphereCast for reliable ground detection even near edges.
    /// Also applies gravity modifiers for snappier Mario-style jump feel.
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
        [Tooltip("How far down to cast for ground detection. Match to roughly half player height.")]
        [SerializeField] private float m_groundCheckDistance = 1.1f;

        [Tooltip("Radius of the sphere used for ground detection. Match to capsule radius.")]
        [SerializeField] private float m_groundCheckRadius = 0.45f;

        [Tooltip("Which layers count as ground.")]
        [SerializeField] private LayerMask m_groundLayer;

        private Rigidbody m_rigidbody;
        private bool m_isJumpRequested;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Cache reference to the Rigidbody component.
            this.m_rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (this.m_groundLayer.value == 0)
            {
                Debug.LogWarning("PlayerJumper: groundLayer has no layers selected. The player will never be able to jump.", this);
            }
        }

        private void Update()
        {
            // Read input in Update.
            // GetButtonDown only fires once per key press — perfect for jumping.
            if (Input.GetButtonDown(k_jumpButton) && IsGrounded())
            {
                this.m_isJumpRequested = true;
            }
        }

        private void FixedUpdate()
        {
            // Apply physics movement in FixedUpdate.
            if (this.m_isJumpRequested)
            {
                ApplyJump();
                this.m_isJumpRequested = false;
            }

            ApplyGravityModifiers();
        }

        // Draws the ground check sphere in the Scene view for debugging.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                transform.position + Vector3.down * this.m_groundCheckDistance,
                this.m_groundCheckRadius
            );
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Applies an upward impulse force to the Rigidbody.
        /// </summary>
        private void ApplyJump()
        {
            // Reset Y velocity first so double-jumps don't stack.
            this.m_rigidbody.linearVelocity = new Vector3(
                this.m_rigidbody.linearVelocity.x,
                0f,
                this.m_rigidbody.linearVelocity.z
            );

            // Apply an instant upward force (Impulse mode ignores mass scaling).
            this.m_rigidbody.AddForce(Vector3.up * this.m_jumpForce, ForceMode.Impulse);
        }

        /// <summary>
        /// Modifies gravity to create snappier, Mario-style jumps.
        /// </summary>
        private void ApplyGravityModifiers()
        {
            if (this.m_rigidbody.linearVelocity.y < 0)
            {
                // Falling: apply heavier gravity to snap down quickly.
                this.m_rigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (this.m_fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (this.m_rigidbody.linearVelocity.y > 0 && !Input.GetButton(k_jumpButton))
            {
                // Rising but player released jump button early: apply heavier gravity for a short hop.
                this.m_rigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (this.m_lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// SphereCast downward from the player's center to detect ground reliably near edges.
        /// </summary>
        private bool IsGrounded()
        {
            return Physics.SphereCast(
                transform.position,
                this.m_groundCheckRadius,
                Vector3.down,
                out _,
                this.m_groundCheckDistance,
                this.m_groundLayer
            );
        }
        #endregion
    }
}
