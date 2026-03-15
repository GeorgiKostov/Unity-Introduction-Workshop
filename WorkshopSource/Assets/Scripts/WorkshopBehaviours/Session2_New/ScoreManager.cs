using UnityEngine;
using UnityEngine.Events;

namespace WorkshopBehaviours.Session2_New
{
    /// <summary>
    /// Singleton manager that tracks the player's score globally.
    /// Fires OnScoreChanged to update UI when the score is added to.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public UnityEvent<int> OnScoreChanged;

        private int score;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("ScoreManager: Multiple ScoreManagers found in scene! Only one should exist.");
                Destroy(gameObject);
            }
        }

        public void AddScore(int amount)
        {
            this.score += amount;
            this.OnScoreChanged?.Invoke(this.score);
        }
    }
}
