using UnityEngine;

namespace WorkshopExamples.Environment
{
    /// <summary>
    /// Orbits this GameObject around a target Transform at a fixed radius.
    /// Uses no physics components — purely transform-based.
    /// </summary>
    public class ObjectOrbiter : MonoBehaviour
    {
        #region Fields
        [Header("Orbit Settings")]
        [Tooltip("The Transform to orbit around.")]
        [SerializeField] private Transform m_target;

        [Tooltip("Distance from the target.")]
        [SerializeField] private float m_orbitRadius = 5f;

        [Tooltip("Degrees per second.")]
        [SerializeField] private float m_orbitSpeed = 45f;

        [Tooltip("Axis to rotate around. Vector3.up for horizontal orbiting.")]
        [SerializeField] private Vector3 m_orbitAxis = Vector3.up;
        #endregion

        #region Properties
        public Transform Target
        {
            get => this.m_target;
            set => this.m_target = value;
        }

        public float OrbitRadius
        {
            get => this.m_orbitRadius;
            set => this.m_orbitRadius = value;
        }

        public float OrbitSpeed
        {
            get => this.m_orbitSpeed;
            set => this.m_orbitSpeed = value;
        }
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            if (this.m_target == null)
            {
                Debug.LogWarning("ObjectOrbiter: no target assigned. Orbiting will not work.", this);
                return;
            }

            // Position at the correct initial distance from the target.
            transform.position = this.m_target.position + (transform.position - this.m_target.position).normalized * this.m_orbitRadius;
        }

        private void Update()
        {
            if (this.m_target == null)
            {
                return;
            }

            transform.RotateAround(this.m_target.position, this.m_orbitAxis, this.m_orbitSpeed * Time.deltaTime);
        }
        #endregion
    }
}
