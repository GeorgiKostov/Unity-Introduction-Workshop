using UnityEngine;
using Workshop.Session2.Movement;
using Workshop.Session3.GameFlow;

namespace Workshop.Session3.Hazards
{
    /// <summary>
    /// Detects when the Player enters a trigger zone and tells the
    /// PlayerRespawner to teleport the player back to the spawn point.
    /// Requires a Collider with "Is Trigger" checked.
    /// </summary>
    public class HazardZone : MonoBehaviour
    {
        #region Fields
        [Header("Feedback")]
        [Tooltip("Optional particle to spawn at the player's position on hit.")]
        [SerializeField] private GameObject m_hitEffectPrefab;

        [Tooltip("Optional audio clip to play on contact.")]
        [SerializeField] private AudioClip m_hitSoundClip;
        #endregion

        #region MonoBehaviour Methods
        private void OnTriggerEnter(Collider other)
        {
            // Only react to objects tagged as Player.
            if (!other.CompareTag(Workshop.Tags.Player))
            {
                return;
            }

            HandleHazardContact(other.gameObject);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Executes feedback and triggers the player respawn logic.
        /// </summary>
        /// <param name="playerObject">The player GameObject that hit the hazard.</param>
        private void HandleHazardContact(GameObject playerObject)
        {
            // Spawn optional effect at player position.
            if (m_hitEffectPrefab != null)
            {
                Instantiate(m_hitEffectPrefab, playerObject.transform.position, Quaternion.identity);
            }

            // Play optional sound at player position.
            if (m_hitSoundClip != null)
            {
                AudioSource.PlayClipAtPoint(m_hitSoundClip, playerObject.transform.position);
            }

            // Ask the PlayerRespawner component on the player to respawn.
            PlayerRespawner respawner = playerObject.GetComponent<PlayerRespawner>();
            
            if (respawner != null)
            {
                respawner.Respawn();
            }
        }
        #endregion
    }
}
