using UnityEngine;


/// <summary>
/// Attach to the player. Raycasts downward each FixedUpdate to detect a moving
/// platform and inherits its horizontal velocity automatically.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MovingPlatformRider : MonoBehaviour
{
    #region Fields
    [Header("Platform Detection")]
    [Tooltip("Layer mask for the moving platform(s).")]
    [SerializeField] private LayerMask m_platformLayer;

    [Tooltip("How far below the player's origin to raycast.")]
    [SerializeField] private float m_groundCheckDistance = 1.1f;

    private Rigidbody m_rigidbody;
    private Rigidbody m_currentPlatform;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        DetectPlatform();
        ApplyPlatformVelocity();
    }
    #endregion

    #region Private Methods
    private void DetectPlatform()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, m_groundCheckDistance, m_platformLayer))
            m_currentPlatform = hit.collider.attachedRigidbody;
        else
            m_currentPlatform = null;
    }

    private void ApplyPlatformVelocity()
    {
        if (m_currentPlatform == null) return;

        Vector3 v = m_currentPlatform.linearVelocity;
        m_rigidbody.linearVelocity = new Vector3(
            v.x,
            m_rigidbody.linearVelocity.y, // preserve Y so jumping still works
            v.z
        );
    }
    #endregion
}
