using UnityEngine;

namespace WorkshopBehaviours.Session3
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private float moveThreshold = 0.1f;

        private Animator animator;
        private Rigidbody rb;

        // These strings must match the parameter names
        // you create in the Animator Controller exactly
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");

        void Awake()
        {
            this.animator = GetComponent<Animator>();
            this.rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            // Horizontal speed only — ignore Y so landing doesn't
            // read as movement
            float horizontalSpeed = new Vector2(this.rb.linearVelocity.x, this.rb.linearVelocity.z).magnitude;

            this.animator.SetBool(IsRunning, horizontalSpeed > this.moveThreshold);

            // Jumping: positive Y velocity means we are rising
            // Negative or zero means grounded or falling
            this.animator.SetBool(IsJumping, this.rb.linearVelocity.y > 0.1f);
        }
    }
}