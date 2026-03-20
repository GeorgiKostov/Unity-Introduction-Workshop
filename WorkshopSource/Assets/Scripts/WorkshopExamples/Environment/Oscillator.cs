using UnityEngine;

namespace WorkshopExamples.Environment
{
    /// <summary>
    /// Oscillates this GameObject back and forth along its local X axis using Mathf.Sin.
    /// Uses no physics components — purely transform-based.
    /// </summary>
    public class Oscillator : MonoBehaviour
    {
        #region Fields
        [Header("Oscillation Settings")]
        [Tooltip("Maximum distance from the start position.")]
        [SerializeField] private float m_amplitude = 1f;

        [Tooltip("How many full cycles per second.")]
        [SerializeField] private float m_frequency = 1f;

        private Vector3 m_startPosition;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            this.m_startPosition = transform.localPosition;
        }

        private void Update()
        {
            float offset = Mathf.Sin(Time.time * this.m_frequency) * this.m_amplitude;
            transform.localPosition = this.m_startPosition + new Vector3(offset, 0f, 0f);
        }
        #endregion
    }
}
