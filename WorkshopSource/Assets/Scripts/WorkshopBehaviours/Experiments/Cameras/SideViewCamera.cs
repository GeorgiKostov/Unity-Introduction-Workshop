using UnityEngine;

namespace Workshop.Experiments.Cameras
{
    /// <summary>
    /// A side-view camera that remains completely stationary until the target 
    /// exits the defined view frustum/screen bounds, at which point it smoothly 
    /// lerps to recenter on the target's new position.
    /// You can freely assign the camera's rotation in the Editor, it will not be overridden.
    /// </summary>
    public class SideViewCamera : MonoBehaviour
    {
        #region Fields
        [Header("Targeting")]
        [Tooltip("The object the camera should track (e.g., the player).")]
        [SerializeField] private Transform m_target;
        
        [Tooltip("The camera component. Will find self if empty.")]
        [SerializeField] private Camera m_camera;

        [Header("Movement Settings")]
        [Tooltip("Positional offset from the target when the camera recenters itself.")]
        [SerializeField] private Vector3 m_followOffset = new Vector3(0f, 2f, -10f);

        [Tooltip("How fast the camera lerps to catch up to the target.")]
        [SerializeField] private float m_lerpSpeed = 5f;

        [Tooltip("How close to the edge of the screen (0 to 0.5) the target must get before the camera moves.")]
        [Range(0f, 0.49f)]
        [SerializeField] private float m_edgeThreshold = 0.05f;

        private Vector3 m_targetPosition;
        private bool m_isTransitioning;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            if (m_camera == null)
            {
                m_camera = GetComponent<Camera>();
            }

            // Initialize position target exactly at the follow offset relative to the player
            if (m_target != null)
            {
                m_targetPosition = m_target.position + m_followOffset;
                transform.position = m_targetPosition;
            }
            else
            {
                m_targetPosition = transform.position;
            }
        }

        private void LateUpdate()
        {
            if (m_target == null || m_camera == null)
            {
                return;
            }

            // If the camera is currently moving to recenter, continue the lerp until it arrives.
            if (m_isTransitioning)
            {
                transform.position = Vector3.Lerp(transform.position, m_targetPosition, Time.deltaTime * m_lerpSpeed);
                
                // Snap to target if very close to prevent infinite micro-lerping
                if (Vector3.Distance(transform.position, m_targetPosition) < 0.05f)
                {
                    transform.position = m_targetPosition;
                    m_isTransitioning = false;
                }
                return;
            }

            // Check if the target has exited the established frustum bounds.
            Vector3 viewportPos = m_camera.WorldToViewportPoint(m_target.position);
            
            bool isOutsideBounds = viewportPos.x < m_edgeThreshold || 
                                   viewportPos.x > (1f - m_edgeThreshold) ||
                                   viewportPos.y < m_edgeThreshold || 
                                   viewportPos.y > (1f - m_edgeThreshold);

            if (isOutsideBounds)
            {
                // Calculate the new static destination for the camera.
                m_targetPosition = m_target.position + m_followOffset;
                m_isTransitioning = true;
            }
        }
        #endregion
    }
}
