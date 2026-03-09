using UnityEngine;

namespace Workshop.Session2_New
{
    /// <summary>
    /// Adds points to the global ScoreManager when picked up by the player.
    /// Destroys itself after pickup.
    /// </summary>
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private int pointValue = 10;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(pointValue);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("Collectible: Tried to add score but ScoreManager.Instance is null. Is ScoreManager present in the scene?");
                }
            }
        }
    }
}
