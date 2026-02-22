using UnityEngine;
using UnityEngine.Events;

namespace Workshop.Session2.Collectibles
{
    /// <summary>
    /// Tracks the player's score and notifies listeners when it changes.
    /// Other scripts call AddScore() to increase it.
    /// Use the ScoreChanged UnityEvent to connect UI via the Inspector.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        #region Fields
        [Header("Score State")]
        [Tooltip("Visible in Inspector during Play mode for debugging.")]
        [SerializeField] private int m_currentScore = 0;

        [Header("Events")]
        [Tooltip("Fires when the score value changes. Drag ScoreDisplay.UpdateText here.")]
        [SerializeField] private UnityEvent<int> m_scoreChanged = new UnityEvent<int>();
        #endregion

        #region Properties
        public int CurrentScore => m_currentScore;
        
        public UnityEvent<int> ScoreChanged => m_scoreChanged;
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds points to the total score and triggers the change event.
        /// </summary>
        /// <param name="amount">Number of points to add.</param>
        public void AddScore(int amount)
        {
            m_currentScore += amount;
            
            Debug.Log($"Score updated: {m_currentScore}");

            // Notify all listeners through the UnityEvent.
            OnScoreChanged(m_currentScore);
        }

        /// <summary>
        /// Resets the score back to zero.
        /// </summary>
        public void ResetScore()
        {
            m_currentScore = 0;
            OnScoreChanged(m_currentScore);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Raises the score change event.
        /// </summary>
        /// <param name="newScore">The current score after modification.</param>
        private void OnScoreChanged(int newScore)
        {
            m_scoreChanged?.Invoke(newScore);
        }
        #endregion
    }
}
