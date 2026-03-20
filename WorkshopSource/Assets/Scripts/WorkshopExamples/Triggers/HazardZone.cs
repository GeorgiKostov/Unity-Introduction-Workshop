using UnityEngine;
using WorkshopExamples.Movement;

namespace WorkshopExamples.Triggers
{
    /// <summary>
    /// Teleports the player to their spawn point when they enter this trigger zone.
    /// Requires the player to have a PlayerRespawner component.
    /// Set "Is Trigger" on the Collider of this GameObject.
    /// </summary>
    public class HazardZone : MonoBehaviour
    {
        #region MonoBehaviour Methods
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Tags.Player))
            {
                return;
            }

            // Acceptable to call GetComponent here — only fires once per trigger event.
            PlayerRespawner respawner = other.GetComponent<PlayerRespawner>();

            if (respawner != null)
            {
                respawner.Respawn();
            }
            else
            {
                Debug.LogWarning("HazardZone: Object tagged Player entered, but has no PlayerRespawner component.");
            }
        }
        #endregion
    }
}
