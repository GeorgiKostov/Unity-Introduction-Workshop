using System.Collections.Generic;
using UnityEngine;

namespace Workshop.Session4.Spawning
{
    /// <summary>
    /// Spawns a prefab at this object's position on a repeating interval.
    /// Tracks spawned objects and respects a maximum active count.
    /// Useful for respawning collectibles or generating obstacles.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour
    {
        #region Fields
        [Header("Spawn Settings")]
        [Tooltip("The prefab to spawn. Must be in the Project window.")]
        [SerializeField] private GameObject m_prefabToSpawn;

        [Tooltip("Seconds between each spawn attempt.")]
        [SerializeField] private float m_spawnInterval = 3f;

        [Tooltip("Maximum number of this prefab allowed alive at once.")]
        [SerializeField] private int m_maxActiveCount = 5;

        [Header("Randomness")]
        [Tooltip("Add a random position offset so spawns aren't all stacked.")]
        [SerializeField] private Vector3 m_randomPositionOffset = new Vector3(1f, 0f, 1f);

        [SerializeField] private List<GameObject> m_activeObjects = new();
        private float m_spawnTimer;
        #endregion

        #region MonoBehaviour Methods
        private void Update()
        {
            UpdateSpawnTimer();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Increments the timer and attempts to spawn if the interval has passed.
        /// </summary>
        private void UpdateSpawnTimer()
        {
            m_spawnTimer += Time.deltaTime;

            if (m_spawnTimer >= m_spawnInterval)
            {
                m_spawnTimer = 0f;
                TrySpawn();
            }
        }

        /// <summary>
        /// Validates limits and instantiates a new object.
        /// </summary>
        private void TrySpawn()
        {
            if (m_prefabToSpawn == null)
            {
                return;
            }

            // Remove null entries — objects may have been destroyed (e.g. collected).
            m_activeObjects.RemoveAll(obj => obj == null);

            if (m_activeObjects.Count >= m_maxActiveCount)
            {
                return;
            }

            ExecuteSpawn();
        }

        /// <summary>
        /// Handles the actual instantiation with random offset.
        /// </summary>
        private void ExecuteSpawn()
        {
            // Random offset within the defined range.
            Vector3 offset = new Vector3(
                Random.Range(-m_randomPositionOffset.x, m_randomPositionOffset.x),
                Random.Range(-m_randomPositionOffset.y, m_randomPositionOffset.y),
                Random.Range(-m_randomPositionOffset.z, m_randomPositionOffset.z)
            );

            GameObject spawned = Instantiate(
                m_prefabToSpawn,
                transform.position + offset,
                Quaternion.identity
            );

            m_activeObjects.Add(spawned);
        }
        #endregion
    }
}
