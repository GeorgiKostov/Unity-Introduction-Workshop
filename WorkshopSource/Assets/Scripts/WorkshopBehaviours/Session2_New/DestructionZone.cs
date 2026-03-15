using UnityEngine;

namespace WorkshopBehaviours.Session2_New
{
    /// <summary>
    /// Destroys any object tagged "Destructible" that enters the trigger.
    /// </summary>
    public class DestructionZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Destructible"))
            {
                // Destroy marks the object for removal at the end of the current frame.
                // It is not immediate - other code running this frame can still access it.
                Destroy(other.gameObject);
            }
        }
    }
}
