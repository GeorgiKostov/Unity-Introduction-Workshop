using UnityEngine;
using System.Collections;

namespace Workshop.Session2_New
{
    /// <summary>
    /// Changes the player's material color on trigger entry,
    /// and resets it after a delay.
    /// </summary>
    public class ColorChanger : MonoBehaviour
    {
        [SerializeField] private Color targetColor = Color.red;
        [SerializeField] private float resetDelay = 1.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Acceptable to call GetComponent inside OnTriggerEnter
                // since it only happens once per collision event.
                Renderer playerRenderer = other.GetComponent<Renderer>();
                if (playerRenderer != null)
                {
                    StartCoroutine(ResetColorRoutine(playerRenderer));
                }
            }
        }

        private IEnumerator ResetColorRoutine(Renderer targetRenderer)
        {
            Color originalColor = targetRenderer.material.color;
            targetRenderer.material.color = targetColor;

            yield return new WaitForSeconds(resetDelay);

            targetRenderer.material.color = originalColor;
        }
    }
}
