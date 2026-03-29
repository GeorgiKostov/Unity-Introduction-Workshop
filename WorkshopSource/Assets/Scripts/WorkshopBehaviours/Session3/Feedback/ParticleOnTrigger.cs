using UnityEngine;

namespace WorkshopBehaviours.Session3.Feedback
{
    /// <summary>
    /// Instantiates a particle effect prefab when the player enters this trigger.
    /// Assign a particle system prefab — it should have "Stop Action: Destroy"
    /// set in its main module so it cleans itself up automatically.
    /// </summary>
    public class ParticleOnTrigger : MonoBehaviour
    {
        #region Fields
        [Header("Effect Settings")]
        [Tooltip("A particle system prefab. Enable 'Stop Action: Destroy' on it.")]
        [SerializeField] private GameObject m_particlePrefab;

        [Tooltip("If true, spawns at player position. If false, spawns at this object's position.")]
        [SerializeField] private bool m_shouldSpawnAtPlayer = true;

        [Tooltip("If true, the effect only triggers once.")]
        [SerializeField] private bool m_isSingleTrigger = true;

        private bool m_hasTriggered;
        #endregion

        #region MonoBehaviour Methods
        private void OnTriggerEnter(Collider other)
        {
            // Only react to the player.
            if (!other.CompareTag(Tags.Player))
            {
                return;
            }

            // Check if we already triggered and it's a one-time trigger.
            if (this.m_isSingleTrigger && this.m_hasTriggered)
            {
                return;
            }

            SpawnEffect(other.gameObject);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Instantiates the particle prefab at the chosen location.
        /// </summary>
        /// <param name="playerObject">The player GameObject that hit the trigger.</param>
        private void SpawnEffect(GameObject playerObject)
        {
            if (this.m_particlePrefab == null)
            {
                return;
            }

            Vector3 spawnPos = this.m_shouldSpawnAtPlayer
                ? playerObject.transform.position
                : transform.position;

            Instantiate(this.m_particlePrefab, spawnPos, Quaternion.identity);
            this.m_hasTriggered = true;
        }
        #endregion
    }
}
