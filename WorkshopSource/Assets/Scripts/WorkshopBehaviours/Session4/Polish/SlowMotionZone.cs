using UnityEngine;

namespace Workshop.Session4.Polish
{
    /// <summary>
    /// Slows time when the player enters this trigger zone.
    /// Restores normal speed when the player exits.
    /// Time.timeScale affects all physics and animations globally.
    /// </summary>
    public class SlowMotionZone : MonoBehaviour
    {
        #region Constants
        private const float k_defaultFixedDeltaTime = 0.02f;
        #endregion

        #region Fields
        [Header("Slow Motion Settings")]
        [Tooltip("Time scale inside zone. 0.5 = half speed, 0.2 = very slow.")]
        [Range(0.05f, 1f)]
        [SerializeField] private float m_slowTimeScale = 0.3f;

        [Tooltip("How quickly time transitions in and out.")]
        [SerializeField] private float m_transitionSpeed = 3f;

        private float m_targetTimeScale = 1f;
        private int m_playersInsideCount = 0;
        #endregion

        #region MonoBehaviour Methods
        private void Update()
        {
            ApplyTimeTransition();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Workshop.Tags.Player))
            {
                return;
            }

            m_playersInsideCount++;
            m_targetTimeScale = m_slowTimeScale;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(Workshop.Tags.Player))
            {
                return;
            }

            m_playersInsideCount = Mathf.Max(0, m_playersInsideCount - 1);
            
            if (m_playersInsideCount == 0)
            {
                m_targetTimeScale = 1f;
            }
        }

        private void OnDisable()
        {
            // Safety: always restore time if this object is disabled.
            ResetTimeScale();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Smoothly moves timeScale toward the target value.
        /// </summary>
        private void ApplyTimeTransition()
        {
            // Smoothly move timeScale toward the target value.
            // Use unscaledDeltaTime because timeScale affects regular deltaTime.
            Time.timeScale = Mathf.MoveTowards(
                Time.timeScale,
                m_targetTimeScale,
                m_transitionSpeed * Time.unscaledDeltaTime
            );

            // Keep fixedDeltaTime in sync with timeScale for physics accuracy.
            Time.fixedDeltaTime = k_defaultFixedDeltaTime * Time.timeScale;
        }

        /// <summary>
        /// Instantly restores time scale to defaults.
        /// </summary>
        private void ResetTimeScale()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = k_defaultFixedDeltaTime;
        }
        #endregion
    }
}
