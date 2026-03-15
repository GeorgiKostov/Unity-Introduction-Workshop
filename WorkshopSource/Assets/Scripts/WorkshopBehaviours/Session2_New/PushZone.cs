using UnityEngine;

namespace WorkshopBehaviours.Session2_New
{
    /// <summary>
    /// Applies a continuous physics force to the player
    /// while they remain inside the trigger volume.
    /// </summary>
    public class PushZone : MonoBehaviour
    {
        [SerializeField] private Vector3 pushDirection = new Vector3(0, 1, 0);
        [SerializeField] private float pushForce = 8f;

        private Rigidbody playerRb;

        private void Awake()
        {
            // Normalize so only the direction drives the push, magnitude is handled by pushForce
            this.pushDirection.Normalize();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.playerRb = other.GetComponent<Rigidbody>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.playerRb = null;
            }
        }

        private void FixedUpdate()
        {
            if (this.playerRb != null)
            {
                this.playerRb.AddForce(this.pushDirection * this.pushForce, ForceMode.Force);
            }
        }
    }
}
