using UnityEngine;

namespace WorkshopBehaviours.Session2.Camera
{
    /// <summary>
    /// Orbits around a target using right-click + mouse drag.
    /// Scroll wheel controls zoom distance.
    /// Attach to Main Camera. Assign Player as target.
    /// Do NOT use alongside CameraFollower.
    /// </summary>
    public class CameraOrbiter : MonoBehaviour
    {
        #region Constants
        private const int k_rightMouseButton = 1;
        private const string k_mouseXAxis = "Mouse X";
        private const string k_mouseYAxis = "Mouse Y";
        private const string k_scrollWheelAxis = "Mouse ScrollWheel";
        #endregion

        #region Fields
        [Header("Target")]
        [Tooltip("Drag the Player GameObject here.")]
        [SerializeField] private Transform m_target;

        [Header("Orbit Settings")]
        [Tooltip("How far the camera sits from the target.")]
        [Range(2f, 20f)]
        [SerializeField] private float m_distance = 8f;

        [Tooltip("Mouse sensitivity for horizontal and vertical orbit.")]
        [SerializeField] private float m_orbitSpeed = 3f;

        [Tooltip("Minimum and maximum vertical angle in degrees.")]
        [SerializeField] private Vector2 m_verticalClamp = new Vector2(-20f, 80f);

        [Header("Zoom")]
        [SerializeField] private float m_zoomSpeed = 2f;
        [SerializeField] private Vector2 m_zoomClamp = new Vector2(2f, 15f);

        private float m_yaw;
        private float m_pitch;
        private float m_mouseXInput;
        private float m_mouseYInput;
        private float m_scrollInput;
        private bool m_isOrbiting;
        #endregion

        #region MonoBehaviour Methods
        private void Update()
        {
            HandleInput();
        }

        private void LateUpdate()
        {
            if (this.m_target == null)
            {
                return;
            }

            ApplyOrbit();
            ApplyZoom();
            UpdateTransform();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Reads mouse input for orbiting and zooming.
        /// </summary>
        private void HandleInput()
        {
            this.m_isOrbiting = Input.GetMouseButton(k_rightMouseButton);
            
            if (this.m_isOrbiting)
            {
                this.m_mouseXInput = Input.GetAxis(k_mouseXAxis);
                this.m_mouseYInput = Input.GetAxis(k_mouseYAxis);
            }
            
            this.m_scrollInput = Input.GetAxis(k_scrollWheelAxis);
        }

        /// <summary>
        /// Calculates new angles based on mouse movement.
        /// </summary>
        private void ApplyOrbit()
        {
            if (!this.m_isOrbiting)
            {
                return;
            }

            this.m_yaw += this.m_mouseXInput * this.m_orbitSpeed;
            this.m_pitch -= this.m_mouseYInput * this.m_orbitSpeed;
            this.m_pitch = Mathf.Clamp(this.m_pitch, this.m_verticalClamp.x, this.m_verticalClamp.y);
        }

        /// <summary>
        /// Updates distance based on scroll input.
        /// </summary>
        private void ApplyZoom()
        {
            this.m_distance -= this.m_scrollInput * this.m_zoomSpeed;
            this.m_distance = Mathf.Clamp(this.m_distance, this.m_zoomClamp.x, this.m_zoomClamp.y);
        }

        /// <summary>
        /// Positions the camera relative to the target.
        /// </summary>
        private void UpdateTransform()
        {
            // Calculate camera position from angles and distance.
            Quaternion rotation = Quaternion.Euler(this.m_pitch, this.m_yaw, 0f);
            transform.position = this.m_target.position - rotation * Vector3.forward * this.m_distance;
            transform.LookAt(this.m_target.position);
        }
        #endregion
    }
}
