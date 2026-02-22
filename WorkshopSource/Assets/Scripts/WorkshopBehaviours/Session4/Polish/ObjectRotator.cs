using UnityEngine;

namespace Workshop.Session4.Polish
{
    /// <summary>
    /// Rotates a GameObject continuously on any combination of axes.
    /// Useful for decorative objects, collectibles, or environmental details.
    /// </summary>
    public class ObjectRotator : MonoBehaviour
    {
        #region Fields
        [Header("Rotation Speed (degrees per second)")]
        [SerializeField] private float m_rotationSpeedX = 0f;
        [SerializeField] private float m_rotationSpeedY = 90f;
        [SerializeField] private float m_rotationSpeedZ = 0f;

        [Tooltip("Use world space or local space rotation?")]
        [SerializeField] private Space m_rotationSpace = Space.Self;
        #endregion

        #region MonoBehaviour Methods
        private void Update()
        {
            ApplyRotation();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Applies continuous rotation based on the configured speeds and space.
        /// </summary>
        private void ApplyRotation()
        {
            transform.Rotate(
                m_rotationSpeedX * Time.deltaTime,
                m_rotationSpeedY * Time.deltaTime,
                m_rotationSpeedZ * Time.deltaTime,
                m_rotationSpace
            );
        }
        #endregion
    }
}
