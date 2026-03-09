using UnityEngine;

namespace Workshop.Session2_New
{
    /// <summary>
    /// Handles jumping using a physics impulse.
    /// Uses SphereCast for robust ground detection.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJumper : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float groundCheckDistance = 1.1f;
        [SerializeField] private float groundCheckRadius = 0.45f;
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (groundLayer.value == 0)
            {
                Debug.LogWarning("PlayerJumper: groundLayer has no layers selected. The player will never be able to jump.");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        private bool IsGrounded()
        {
            // SphereCast down to detect ground reliably
            return Physics.SphereCast(transform.position, groundCheckRadius, Vector3.down, out _, groundCheckDistance, groundLayer);
        }
    }
}
