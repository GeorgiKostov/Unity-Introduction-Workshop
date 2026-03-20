using UnityEngine;


public class SpawnObstaclesOverTime : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private Vector3 launchDirection = new Vector3(0, 0, 1);
    [SerializeField] private float launchForce = 80f;

    private Vector3 _normalizedDirection;

    void Start()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogWarning("SpawnObstaclesOverTime: obstaclePrefab is not assigned.", this);
            return;
        }

        // Normalise in Start so only direction matters, not magnitude.
        // Typing (0, 0, 10) and (0, 0, 1) will produce identical launch speeds.
        _normalizedDirection = launchDirection.normalized;

        // InvokeRepeating arguments: method name, delay before first call, repeat interval.
        InvokeRepeating(nameof(SpawnObstacle), spawnInterval, spawnInterval);
    }

    private void SpawnObstacle()
    {
        GameObject instance = Instantiate(obstaclePrefab, transform.position, Quaternion.identity);

        Rigidbody rb = instance.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // ForceMode.Impulse applies the full force in a single frame.
            // Correct for a launch. ForceMode.Force applies over time and would barely move the object.
            rb.AddForce(_normalizedDirection * launchForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("SpawnObstaclesOverTime: instantiated prefab has no Rigidbody — cannot apply launch force.", instance);
        }
    }
}
