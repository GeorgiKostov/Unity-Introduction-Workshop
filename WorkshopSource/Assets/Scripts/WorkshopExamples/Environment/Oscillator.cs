using UnityEngine;

namespace WorkshopBehaviours.Session2.Environment
{
    /// <summary>
    /// Oscillates this GameObject back and forth along a configurable local axis using Mathf.Sin.
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

        [Tooltip("Local-space direction of oscillation. Does not need to be normalised.")]
        [SerializeField] private Vector3 m_axis = Vector3.right;

        private Vector3 m_startPosition;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            m_startPosition = transform.localPosition;
        }

        private void Update()
        {
            float offset = Mathf.Sin(Time.time * m_frequency) * m_amplitude;
            transform.localPosition = m_startPosition + m_axis.normalized * offset;
        }
        #endregion
    }
}