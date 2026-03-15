using UnityEngine;

namespace WorkshopBehaviours.Session2_New
{
    /// <summary>
    /// Moves the player relative to the camera direction.
    /// Uses Rigidbody linearVelocity for physics-based movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Transform cameraTransform;

        private Rigidbody rb;

        private void Awake()
        {
            this.rb = GetComponent<Rigidbody>();
            if (this.cameraTransform == null)
            {
                Debug.LogWarning("PlayerMover: cameraTransform is not assigned. Please assign the Main Camera in the Inspector.");
            }
        }

        private void FixedUpdate()
        {
            if (this.cameraTransform == null) return;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Project camera forward onto horizontal plane
            Vector3 camForward = this.cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = this.cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            // Calculate movement direction
            Vector3 moveDirection = (camForward * vertical + camRight * horizontal).normalized;
            Vector3 targetVelocity = moveDirection * this.moveSpeed;

            // Apply velocity while preserving gravity
            this.rb.linearVelocity = new Vector3(targetVelocity.x, this.rb.linearVelocity.y, targetVelocity.z);
        }
    }
}
