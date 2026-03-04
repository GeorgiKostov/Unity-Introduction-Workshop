using UnityEngine;

namespace Workshop.Experiments.Cameras
{
    /// <summary>
    /// Maintains a fixed offset from the target and follows smoothly.
    /// </summary>
    public class TopDownCamera : MonoBehaviour
    {
        #region Fields
        [Header("Settings")]
        [Tooltip("The object the camera should follow.")]
        [SerializeField] private Transform m_target;
        
        [Tooltip("Position offset from the target.")]
        [SerializeField] private Vector3 m_offset = new Vector3(0f, 10f, -5f);
        
        [Tooltip("How smoothly the camera catches up.")]
        [SerializeField] private float m_smoothTime = 0.3f;

        private Vector3 m_currentVelocity = Vector3.zero;
        #endregion

        #region MonoBehaviour Methods
        private void LateUpdate()
        {
            if (m_target == null)
            {
                return;
            }

            // Calculate target position based on offset
            Vector3 targetPosition = m_target.position + m_offset;

            // Smoothly move the camera to that position
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref m_currentVelocity, 
                m_smoothTime
            );
        }
        #endregion
    }
}
