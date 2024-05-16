using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class CameraOrbit : MonoBehaviour
    {
        public Transform target; // The object to rotate around
        public float rotationSpeed = 10f;
        public bool invertY = false; // Option to invert vertical rotation

        private Vector3 _offset;

        void Start()
        {
            // Calculate initial offset based on camera's starting position
            _offset = transform.position - target.position;
        }

        void LateUpdate() // Use LateUpdate for smoother camera movement
        {
            if (Input.GetMouseButton(1))
            {
                float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
                float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;

                if (invertY) vertical *= -1;

                // Rotate offset around the target object
                _offset = Quaternion.AngleAxis(horizontal, Vector3.up) * _offset;
                _offset = Quaternion.AngleAxis(vertical, Vector3.right) * _offset;

                // Update the camera position with the rotated offset
                transform.position = target.position + _offset;

                // Always make the camera look at the target
                transform.LookAt(target);
            }
        }
    }
}
