using UnityEngine;

namespace Workshop.Session2_New
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
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (spawnPoint == null)
            {
                Debug.LogWarning("PlayerRespawner: spawnPoint is null. Respawn will not work.");
            }
        }

        private void Update()
        {
            if (transform.position.y < fallThreshold)
            {
                Respawn();
            }
        }

        public void Respawn()
        {
            if (spawnPoint == null) return;

            transform.position = spawnPoint.position;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
