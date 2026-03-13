using UnityEngine;

namespace WorkshopExamples.Camera
{
    /// <summary>
    /// Smoothly follows a target Transform at a fixed offset.
    /// Attach to the Main Camera and assign the Player as the target.
    /// </summary>
    public class CameraFollower : MonoBehaviour
    {
        #region Fields
        [Header("Target")]
        [Tooltip("Drag the Player GameObject here.")]
        [SerializeField] private Transform m_target;

        [Header("Follow Settings")]
        [Tooltip("Position offset from the target. Adjust Y and Z for height/distance.")]
        [SerializeField] private Vector3 m_offset = new Vector3(0f, 5f, -8f);

        [Tooltip("How smoothly the camera catches up. Lower = snappier, Higher = floatier.")]
        [Range(0.01f, 1f)]
        [SerializeField] private float m_smoothSpeed = 0.12f;
        #endregion

        #region MonoBehaviour Methods
        private void LateUpdate()
        {
            // LateUpdate runs after all Updates — ensures player has moved first.
            if (m_target == null)
            {
                return;
            }

            ApplyFollow();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Moves and rotates the camera to follow the target.
        /// </summary>
        private void ApplyFollow()
        {
            Vector3 desiredPosition = m_target.position + m_offset;

            // Lerp gradually moves from current position toward the desired position.
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                m_smoothSpeed
            );

            // Always look at the player.
            transform.LookAt(m_target);
        }
        #endregion
    }
}
