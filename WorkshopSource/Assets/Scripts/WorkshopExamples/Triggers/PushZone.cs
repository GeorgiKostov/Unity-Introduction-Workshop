using UnityEngine;

namespace WorkshopExamples.Triggers
{
    /// <summary>
    /// Applies a continuous physics force to the player while they stay inside this trigger.
    /// Useful for jump pads, wind zones, or launch areas.
    /// Set "Is Trigger" on the Collider of this GameObject.
    /// </summary>
    public class PushZone : MonoBehaviour
    {
        #region Fields
        [Header("Push Settings")]
        [Tooltip("Direction to push the player. Will be normalized at startup.")]
        [SerializeField] private Vector3 m_pushDirection = Vector3.up;

        [Tooltip("Strength of the push force in Newtons (ForceMode.Force).")]
        [SerializeField] private float m_pushForce = 8f;

        private Rigidbody m_playerRigidbody;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Normalize so only the direction drives the push; magnitude is handled by pushForce.
            this.m_pushDirection.Normalize();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.m_playerRigidbody = other.GetComponent<Rigidbody>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.m_playerRigidbody = null;
            }
        }

        private void FixedUpdate()
        {
            if (this.m_playerRigidbody != null)
            {
                this.m_playerRigidbody.AddForce(this.m_pushDirection * this.m_pushForce, ForceMode.Force);
            }
        }
        #endregion
    }
}
