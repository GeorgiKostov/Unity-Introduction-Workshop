using UnityEngine;

namespace WorkshopBehaviours.Session3
{
    /// <summary>
    /// Drives the Animator by reading Rigidbody velocity.
    /// Attach this to the same GameObject as PlayerMover and PlayerJumper.
    /// 
    /// Animator Controller needs two Bool parameters:
    ///   "IsRunning"  — true when the player is moving horizontally
    ///   "IsJumping"  — true when the player is in the air
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private float moveThreshold = 0.1f;
        [SerializeField] private float groundCheckDistance = 1.1f;
        [SerializeField] private float groundCheckRadius = 0.45f;
        [SerializeField] private LayerMask groundLayer;

        [SerializeField]private Animator animator;
        private Rigidbody rb;

        // Hashing the parameter names once is faster than
        // passing strings every frame — and catches typos at startup
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");

        private void Awake()
        {
            this.rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // Horizontal speed only — Y axis is jumping, not running
            float horizontalSpeed = new Vector2(
                this.rb.linearVelocity.x,
                this.rb.linearVelocity.z
            ).magnitude;

            bool grounded = IsGrounded();

            // Running: moving horizontally AND on the ground
            // Without the grounded check, running plays in the air
            this.animator.SetBool(IsRunning, horizontalSpeed > this.moveThreshold && grounded);

            // Jumping: not on the ground
            // This covers both rising and falling — the air state
            this.animator.SetBool(IsJumping, !grounded);
        }

        private bool IsGrounded()
        {
            return Physics.SphereCast(
                transform.position,
                this.groundCheckRadius,
                Vector3.down,
                out _,
                this.groundCheckDistance,
                this.groundLayer
            );
        }
    }
}