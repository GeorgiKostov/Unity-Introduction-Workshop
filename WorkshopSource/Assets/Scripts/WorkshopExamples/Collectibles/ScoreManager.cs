using UnityEngine;
using UnityEngine.Events;

namespace WorkshopExamples.Collectibles
{
    /// <summary>
    /// Tracks the player's score and notifies listeners when it changes.
    /// Exposes a singleton Instance for direct access by Collectible scripts.
    /// Use the OnScoreChanged UnityEvent to connect UI via the Inspector.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        #region Singleton
        /// <summary>
        /// Global singleton reference. Only one ScoreManager should exist per scene.
        /// </summary>
        public static ScoreManager Instance { get; private set; }
        #endregion

        #region Fields
        [Header("Score State")]
        [Tooltip("Visible in Inspector during Play mode for debugging.")]
        [SerializeField] private int m_currentScore = 0;

        [Header("Events")]
        [Tooltip("Fires when the score value changes. Drag ScoreDisplay.UpdateScoreText here.")]
        [SerializeField] private UnityEvent<int> m_scoreChanged = new UnityEvent<int>();
        #endregion

        #region Properties
        public int CurrentScore => this.m_currentScore;

        public UnityEvent<int> OnScoreChanged => this.m_scoreChanged;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("ScoreManager: Multiple ScoreManagers found in scene! Only one should exist.", this);
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds points to the total score and fires the change event.
        /// </summary>
        /// <param name="amount">Number of points to add.</param>
        public void AddScore(int amount)
        {
            this.m_currentScore += amount;

            Debug.Log($"Score updated: {this.m_currentScore}");

            this.m_scoreChanged?.Invoke(this.m_currentScore);
        }

        /// <summary>
        /// Resets the score back to zero.
        /// </summary>
        public void ResetScore()
        {
            this.m_currentScore = 0;
            this.m_scoreChanged?.Invoke(this.m_currentScore);
        }
        #endregion
    }
}
