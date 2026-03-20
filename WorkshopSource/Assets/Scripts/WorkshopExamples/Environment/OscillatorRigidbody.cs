using UnityEngine;

/// <summary>
/// Oscillates this GameObject back and forth along a configurable local axis.
/// Requires a Rigidbody — uses MovePosition so the physics engine correctly
/// pushes/carries other Rigidbody objects (e.g. a character controller).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class OscillatorRigidbody : MonoBehaviour
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
    private Rigidbody m_rigidbody;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.isKinematic = true; // Kinematic: we drive it, physics reacts to it
        m_startPosition = transform.position; // World space — Rigidbody lives in world space
    }

    private void FixedUpdate() // FixedUpdate, not Update — always use this with physics
    {
        float offset = Mathf.Sin(Time.fixedTime * m_frequency) * m_amplitude;
        Vector3 targetPosition = m_startPosition + m_axis.normalized * offset;
        m_rigidbody.MovePosition(targetPosition);
    }
    #endregion
}