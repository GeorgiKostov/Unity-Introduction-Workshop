using UnityEngine;

namespace WorkshopBehaviours.Session4.Polish
{
    /// <summary>
    /// Pulses a GameObject's material color between two colors.
    /// Works on any object with a MeshRenderer and a material.
    /// Creates a material instance automatically to avoid modifying shared assets.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class ColorPulse : MonoBehaviour
    {
        #region Fields
        [Header("Colors")]
        [SerializeField] private Color m_colorA = Color.white;
        [SerializeField] private Color m_colorB = Color.red;

        [Header("Pulse Speed")]
        [SerializeField] private float m_pulseSpeed = 2f;

        private MeshRenderer m_meshRenderer;
        private Material m_materialInstance;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Cache reference to the MeshRenderer.
            this.m_meshRenderer = GetComponent<MeshRenderer>();

            // Accessing .material creates a private instance of the material.
            // This prevents changing the shared asset in the Project project.
            if (this.m_meshRenderer != null)
            {
                this.m_materialInstance = this.m_meshRenderer.material;
            }
        }

        private void Update()
        {
            ApplyPulse();
        }

        private void OnDestroy()
        {
            // Clean up the material instance when the object is destroyed to prevent memory leaks.
            if (this.m_materialInstance != null)
            {
                Destroy(this.m_materialInstance);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates the interpolated color based on a sine wave and applies it.
        /// </summary>
        private void ApplyPulse()
        {
            if (this.m_materialInstance == null)
            {
                return;
            }

            // Convert sin (-1 to 1) to a 0-to-1 range using * 0.5 + 0.5.
            float t = Mathf.Sin(Time.time * this.m_pulseSpeed * Mathf.PI * 2f) * 0.5f + 0.5f;
            this.m_materialInstance.color = Color.Lerp(this.m_colorA, this.m_colorB, t);
        }
        #endregion
    }
}
