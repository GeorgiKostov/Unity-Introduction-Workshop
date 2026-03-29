using UnityEngine;

namespace WorkshopBehaviours.Session4.Polish
{
    /// <summary>
    /// Moves a GameObject up and down in a smooth sine wave.
    /// Great for collectibles, power-ups, or floating decorations.
    /// Does not use physics — moves the Transform directly.
    /// </summary>
    public class ObjectBobber : MonoBehaviour
    {
        #region Fields
        [Header("Bob Settings")]
        [Tooltip("How far up and down the object moves (metres).")]
        [SerializeField] private float m_bobHeight = 0.3f;

        [Tooltip("How many full bobs per second.")]
        [SerializeField] private float m_bobSpeed = 1.5f;

        [Tooltip("Offset so grouped objects don't all bob in sync.")]
        [Range(0f, 6.28f)] // 0 to 2*PI
        [SerializeField] private float m_phaseOffset = 0f;

        [Header("Rotation")]
        [Tooltip("Spin the object while it bobs? Set to 0 to disable.")]
        [SerializeField] private float m_rotationSpeed = 90f;

        private Vector3 m_startPosition;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            // Cache initial position to oscillate around.
            this.m_startPosition = transform.position;
        }

        private void Update()
        {
            ApplyBobbing();
            ApplyRotation();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates and applies the vertical sine wave movement.
        /// </summary>
        private void ApplyBobbing()
        {
            // Sine wave between -1 and 1, scaled by bobHeight.
            float newY = this.m_startPosition.y + Mathf.Sin(
                Time.time * this.m_bobSpeed * Mathf.PI * 2f + this.m_phaseOffset
            ) * this.m_bobHeight;

            transform.position = new Vector3(
                this.m_startPosition.x,
                newY,
                this.m_startPosition.z
            );
        }

        /// <summary>
        /// Applies continuous rotation around the Y axis if enabled.
        /// </summary>
        private void ApplyRotation()
        {
            if (this.m_rotationSpeed == 0f)
            {
                return;
            }

            transform.Rotate(Vector3.up, this.m_rotationSpeed * Time.deltaTime);
        }
        #endregion
    }
}
