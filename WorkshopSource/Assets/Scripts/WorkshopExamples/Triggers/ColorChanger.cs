using System.Collections;
using UnityEngine;

namespace WorkshopBehaviours.Session2.Triggers
{
    /// <summary>
    /// Changes the triggering player's material color temporarily,
    /// then resets it to the original after a configurable delay.
    /// Set "Is Trigger" on the Collider of this GameObject.
    /// </summary>
    public class ColorChanger : MonoBehaviour
    {
        #region Fields
        [Header("Color Settings")]
        [Tooltip("The color to apply to the player's material on entry.")]
        [SerializeField] private Color m_targetColor = Color.red;

        [Tooltip("Seconds before the original color is restored.")]
        [SerializeField] private float m_resetDelay = 1.5f;
        #endregion

        #region MonoBehaviour Methods
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Tags.Player))
            {
                return;
            }

            // Acceptable to call GetComponent inside OnTriggerEnter — fires once per collision.
            Renderer playerRenderer = other.GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                StartCoroutine(ResetColorRoutine(playerRenderer));
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Changes the material color then restores it after the configured delay.
        /// </summary>
        private IEnumerator ResetColorRoutine(Renderer targetRenderer)
        {
            Color originalColor = targetRenderer.material.color;
            targetRenderer.material.color = this.m_targetColor;

            yield return new WaitForSeconds(this.m_resetDelay);

            targetRenderer.material.color = originalColor;
        }
        #endregion
    }
}
