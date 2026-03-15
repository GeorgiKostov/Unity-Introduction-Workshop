using UnityEngine;

namespace WorkshopBehaviours.Session3.Hazards
{
    /// <summary>
    /// Rotates a GameObject back and forth around a chosen axis.
    /// Place the pivot point at the rotation center — use an empty parent
    /// GameObject at the hinge point and attach this script to that parent.
    /// </summary>
    public class SwingingBar : MonoBehaviour
    {
        #region Fields
        [Header("Swing Settings")]
        [Tooltip("Maximum angle the bar swings to each side in degrees.")]
        [SerializeField] private float m_swingAngle = 60f;

        [Tooltip("How many full swings per second.")]
        [SerializeField] private float m_swingSpeed = 1f;

        [Tooltip("Starting angle offset — stagger multiple bars so they don't sync.")]
        [Range(0f, 360f)]
        [SerializeField] private float m_phaseOffset = 0f;

        [Header("Axis")]
        [Tooltip("Which local axis to rotate around. Y = horizontal sweep, Z = vertical sweep.")]
        [SerializeField] private Vector3 m_swingAxis = Vector3.up;

        private Quaternion m_initialRotation;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            // Cache the initial rotation state.
            this.m_initialRotation = transform.localRotation;
        }

        private void FixedUpdate()
        {
            ApplySwing();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates and applies the oscillating rotation.
        /// </summary>
        private void ApplySwing()
        {
            // Sin oscillates between -1 and +1.
            float angle = Mathf.Sin(
                (Time.time * this.m_swingSpeed * Mathf.PI * 2f) + this.m_phaseOffset
            ) * this.m_swingAngle;

            transform.localRotation = this.m_initialRotation * Quaternion.AngleAxis(angle, this.m_swingAxis);
        }
        #endregion
    }
}
