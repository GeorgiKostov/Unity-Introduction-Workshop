using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PlayerMoveController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public Camera mainCamera; // Assign your camera in the Inspector

        void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Get camera's forward and right vectors
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            // Project forward and right vectors onto the horizontal plane (remove Y-axis)
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Calculate desired movement direction
            Vector3 movementDirection = forward * verticalInput + right * horizontalInput;

            // Apply movement
            transform.Translate(movementDirection * moveSpeed * Time.deltaTime);
        }
    }
}
