using UnityEngine;

namespace Workshop.Session2.Collectibles
{
    /// <summary>
    /// Makes a GameObject collectable.
    /// Requires a Collider with "Is Trigger" checked.
    /// Tags the Player as "Player" in the Inspector for detection.
    /// </summary>
    public class Collectible : MonoBehaviour
    {
        #region Fields
        [Header("Score Settings")]
        [Tooltip("How many points collecting this item awards.")]
        [SerializeField] private int m_pointValue = 10;

        [Header("Feedback")]
        [Tooltip("Optional: A particle effect to spawn on collection.")]
        [SerializeField] private GameObject m_collectEffectPrefab;
        #endregion

        #region MonoBehaviour Methods
        private void OnTriggerEnter(Collider other)
        {
            // Only react to the Player.
            if (!other.CompareTag(Workshop.Tags.Player))
            {
                return;
            }

            Collect();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the collection logic, feedback, and destruction.
        /// </summary>
        private void Collect()
        {
            // Spawn optional visual effect at this position.
            if (m_collectEffectPrefab != null)
            {
                Instantiate(m_collectEffectPrefab, transform.position, Quaternion.identity);
            }

            // Tell the ScoreManager to add points.
            // FindFirstObjectByType is safe here — it runs rarely (once per pickup).
            ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
            
            if (scoreManager != null)
            {
                scoreManager.AddScore(m_pointValue);
            }

            // Remove this object from the scene.
            Destroy(gameObject);
        }
        #endregion
    }
}
