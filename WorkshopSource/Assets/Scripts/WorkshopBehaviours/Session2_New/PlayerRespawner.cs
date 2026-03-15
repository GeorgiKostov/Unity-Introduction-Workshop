using UnityEngine;

namespace WorkshopBehaviours.Session2_New
{
    /// <summary>
    /// Respawns the player if they fall below a certain height threshold.
    /// Also provides a public way to respawn the player from other scripts (e.g. hazards).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerRespawner : MonoBehaviour
    {
        [SerializeField] private float fallThreshold = -5f;
        [SerializeField] private Transform spawnPoint;

        private Rigidbody rb;

        private void Awake()
        {
            this.rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (this.spawnPoint == null)
            {
                Debug.LogWarning("PlayerRespawner: spawnPoint is null. Respawn will not work.");
            }
        }

        private void Update()
        {
            if (transform.position.y < this.fallThreshold)
            {
                Respawn();
            }
        }

        public void Respawn()
        {
            if (this.spawnPoint == null) return;

            transform.position = this.spawnPoint.position;
            this.rb.linearVelocity = Vector3.zero;
        }
    }
}
