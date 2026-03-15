using UnityEngine;

namespace WorkshopBehaviours.Session3.Hazards
{
    /// <summary>
    /// Moves a platform back and forth between two world positions.
    /// Set Point A and Point B in the Inspector using world coordinates,
    /// or assign child Transform markers for easier visual placement.
    /// </summary>
    public class MovingPlatform : MonoBehaviour
    {
        #region Fields
        [Header("Waypoints")]
        [Tooltip("Start position. Click the field and type coordinates.")]
        [SerializeField] private Vector3 m_pointA = new Vector3(-4f, 0f, 0f);

        [Tooltip("End position.")]
        [SerializeField] private Vector3 m_pointB = new Vector3(4f, 0f, 0f);

        [Tooltip("Optional: assign empty child GameObjects as visual markers.")]
        [SerializeField] private Transform m_markerA;
        [SerializeField] private Transform m_markerB;

        [Header("Movement")]
        [Tooltip("How fast the platform travels. Try 1 to 4.")]
        [SerializeField] private float m_speed = 2f;

        [Tooltip("Smooth in/out at each end, or constant speed?")]
        [SerializeField] private bool m_isSmoothPingPong = true;

        private Vector3 m_startPosition;
        private Vector3 m_endPosition;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            InitializeWaypoints();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        // Draw the path in Scene view so students can see the range.
        private void OnDrawGizmos()
        {
            Vector3 a = this.m_markerA != null ? this.m_markerA.position : this.m_pointA;
            Vector3 b = this.m_markerB != null ? this.m_markerB.position : this.m_pointB;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(a, b);
            Gizmos.DrawSphere(a, 0.2f);
            Gizmos.DrawSphere(b, 0.2f);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the start and end positions based on markers or manual coordinates.
        /// </summary>
        private void InitializeWaypoints()
        {
            // Use marker Transforms if assigned, otherwise use Vector3 fields.
            this.m_startPosition = this.m_markerA != null ? this.m_markerA.position : this.m_pointA;
            this.m_endPosition = this.m_markerB != null ? this.m_markerB.position : this.m_pointB;
        }

        /// <summary>
        /// Interpolates the platform position over time.
        /// </summary>
        private void ApplyMovement()
        {
            // PingPong oscillates t between 0 and 1 continuously.
            float t = Mathf.PingPong(Time.time * this.m_speed, 1f);

            if (this.m_isSmoothPingPong)
            {
                t = Mathf.SmoothStep(0f, 1f, t);
            }

            transform.position = Vector3.Lerp(this.m_startPosition, this.m_endPosition, t);
        }
        #endregion
    }
}
