using UnityEngine;

namespace WorkshopBehaviours.Session2_New
{
    /// <summary>
    /// Moves the object back and forth along its local X axis using Mathf.Sin.
    /// Uses no physics components.
    /// </summary>
    public class Oscillator : MonoBehaviour
    {
        [SerializeField] private float amplitude = 1f;
        [SerializeField] private float frequency = 1f;

        private Vector3 startPosition;

        private void Awake()
        {
            this.startPosition = transform.localPosition;
        }

        private void Update()
        {
            float offset = Mathf.Sin(Time.time * this.frequency) * this.amplitude;
            transform.localPosition = this.startPosition + new Vector3(offset, 0, 0);
        }
    }
}
