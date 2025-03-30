using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float patrolDistance = 3f;      // Total distance to move left/right from start point
    public float cycleDuration = 4f;       // Time in seconds to complete one full left-right cycle

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (cycleDuration <= 0f) return;

        float time = Time.time % cycleDuration;
        float normalizedTime = time / cycleDuration;

        // PingPong between -1 and 1
        float offset = Mathf.Sin(normalizedTime * 2f * Mathf.PI);

        transform.position = startPos + new Vector3(offset * patrolDistance * 0.5f, 0f, 0f);
    }
}
