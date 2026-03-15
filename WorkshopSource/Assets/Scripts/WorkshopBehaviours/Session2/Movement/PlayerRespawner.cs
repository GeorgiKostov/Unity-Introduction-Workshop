using UnityEngine;

namespace WorkshopBehaviours.Session2.Movement
{
    /// <summary>
    /// Stores the player's spawn point and handles teleportation back to it.
    /// Assign a SpawnPoint empty GameObject in the scene via the Inspector.
    /// Called by HazardZone when the player touches a hazard.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerRespawner : MonoBehaviour
    {
        #region Fields
        [Header("Spawn Settings")]
        [Tooltip("Drag an empty GameObject here to mark the spawn position.")]
        [SerializeField] private Transform m_spawnPoint;

        [Tooltip("Brief freeze duration after respawning (seconds).")]
        [SerializeField] private float m_respawnDelay = 0.5f;

        private Rigidbody m_rigidbody;
        private bool m_isRespawning;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Cache reference to the Rigidbody component.
            this.m_rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (transform.position.y < -10f && !this.m_isRespawning)
            {
                Respawn();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Teleports the player to the spawn point and resets velocity.
        /// Called by HazardZone.
        /// </summary>
        public async void Respawn()
        {
            if (this.m_isRespawning)
            {
                return; // Prevent double-trigger.
            }

            await RespawnAsync();
        }
        public void SetSpawnPoint(Transform newSpawnPoint)
        {
            this.m_spawnPoint = newSpawnPoint;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Hanldes the respawn sequence asynchronously using Unity's Awaitable API.
        /// </summary>
        private async Awaitable RespawnAsync()
        {
            this.m_isRespawning = true;

            // Freeze physics so the player doesn't slide during teleport.
            this.m_rigidbody.isKinematic = true;

            // Move to spawn point.
            Vector3 destination = this.m_spawnPoint != null
                ? this.m_spawnPoint.position
                : Vector3.up * 2f; // Fallback: lift above origin.

            transform.position = destination;

            // Wait a moment before re-enabling physics.
            // Using Awaitable.WaitForSecondsAsync as per Unity 6 style guide.
            await Awaitable.WaitForSecondsAsync(this.m_respawnDelay, destroyCancellationToken);

            // Guard continuation in case object was destroyed during wait.
            if (this == null || !isActiveAndEnabled)
            {
                return;
            }

            this.m_rigidbody.isKinematic = false;
            this.m_rigidbody.linearVelocity = Vector3.zero;
            this.m_isRespawning = false;
        }
        #endregion
    }
}
