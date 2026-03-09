using UnityEngine;

namespace Workshop.Session2_New
{
    /// <summary>
    /// Rotates the object continuously around one or more of its local axes.
    /// Uses no physics components.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 90f, 0);

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}
