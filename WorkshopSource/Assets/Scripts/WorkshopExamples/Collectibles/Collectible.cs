using UnityEngine;

namespace WorkshopExamples.Collectibles
{
    /// <summary>
    /// Makes a GameObject collectable.
    /// Requires a Collider with "Is Trigger" checked.
    /// Tag the Player as "Player" in the Inspector for detection.
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
            if (!other.CompareTag(Tags.Player))
            {
                return;
            }

            Collect();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles collection logic: visual feedback, scoring, and self-destruction.
        /// </summary>
        private void Collect()
        {
            // Spawn optional visual effect at this position.
            if (this.m_collectEffectPrefab != null)
            {
                Instantiate(this.m_collectEffectPrefab, transform.position, Quaternion.identity);
            }

            // Use singleton Instance for performance — avoids FindFirstObjectByType on every pickup.
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(this.m_pointValue);
            }
            else
            {
                Debug.LogWarning("Collectible: ScoreManager.Instance is null. Is a ScoreManager present in the scene?", this);
            }

            // Remove this object from the scene.
            Destroy(gameObject);
        }
        #endregion
    }
}
