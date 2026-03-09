using UnityEngine;

namespace Workshop.Session2_New
{
    /// <summary>
    /// Orbits this object around a target Transform at a fixed radius.
    /// Uses no physics components.
    /// </summary>
    public class ObjectOrbiter : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float orbitRadius = 5f;
        [SerializeField] private float orbitSpeed = 45f;
        [SerializeField] private Vector3 orbitAxis = Vector3.up;

        private void Start()
        {
            if (target == null)
            {
                Debug.LogWarning("ObjectOrbiter has no target assigned. Orbiting will not work properly.");
                return;
            }

            // Position at correct initial distance
            transform.position = target.position + (transform.position - target.position).normalized * orbitRadius;
        }

        private void Update()
        {
            if (target == null) return;
            
            transform.RotateAround(target.position, orbitAxis, orbitSpeed * Time.deltaTime);
        }
    }
}
