using UnityEngine;

namespace WorkshopBehaviours.Session2.Triggers
{
    /// <summary>
    /// Destroys any GameObject tagged "Destructible" that enters this trigger.
    /// Set "Is Trigger" on the Collider of this GameObject.
    /// </summary>
    public class DestructionZone : MonoBehaviour
    {
        #region Constants
        private const string k_destructibleTag = "Destructible";
        #endregion

        #region MonoBehaviour Methods
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(k_destructibleTag))
            {
                return;
            }

            // Destroy marks the object for removal at the end of the current frame.
            // It is not immediate — other code running this frame can still access it.
            Destroy(other.gameObject);
        }
        #endregion
    }
}
