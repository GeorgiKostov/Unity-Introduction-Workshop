using UnityEngine;
using UnityEngine.Events;
using WorkshopBehaviours.Session2.Collectibles;

namespace WorkshopBehaviours.Session3.GameFlow
{
    /// <summary>
    /// Tracks how many Collectible objects are in the scene and triggers a win event.
    /// Responds to score changes as a trigger to check for remaining collectibles.
    /// </summary>
    public class WinCondition : MonoBehaviour
    {
        #region Fields
        [Header("Events")]
        [Tooltip("Fire this when all collectibles are collected. Wire to Win panel.")]
        [SerializeField] private UnityEvent m_allCollected;

        [Header("Dependencies")]
        [Tooltip("Optional: also stop the timer on win.")]
        [SerializeField] private CountdownTimer m_timerToStop;

        private ScoreManager m_scoreManager;
        private int m_totalCollectibles;
        private bool m_hasWon;
        #endregion

        #region Properties
        public UnityEvent AllCollected => this.m_allCollected;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Cache reference to the ScoreManager in the scene.
            this.m_scoreManager = FindFirstObjectByType<ScoreManager>();
        }

        private void OnEnable()
        {
            // Subscribe to score changes to check win condition efficiently.
            if (this.m_scoreManager != null)
            {
                this.m_scoreManager.OnScoreChanged.AddListener(HandleScoreChanged);
            }
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks.
            if (this.m_scoreManager != null)
            {
                this.m_scoreManager.OnScoreChanged.RemoveListener(HandleScoreChanged);
            }
        }

        private void Start()
        {
            InitializeWinCondition();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Counts initial collectibles and sets up state.
        /// </summary>
        private void InitializeWinCondition()
        {
            // Count every Collectible active in the scene at game start.
            this.m_totalCollectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;

            Debug.Log($"Win Condition: tracking {this.m_totalCollectibles} collectibles.");
            
            // If there are no collectibles at start, check for immediate win? 
            // Usually we expect at least one.
        }

        /// <summary>
        /// Callback triggered whenever the score is updated.
        /// </summary>
        /// <param name="currentScore">The new score value.</param>
        private void HandleScoreChanged(int currentScore)
        {
            if (this.m_hasWon)
            {
                return;
            }

            CheckWinCondition();
        }

        /// <summary>
        /// Checks if any collectibles remain in the scene and triggers win logic.
        /// </summary>
        private void CheckWinCondition()
        {
            // Check if any Collectibles remain in the scene.
            int remaining = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;

            if (remaining == 0 && this.m_totalCollectibles > 0)
            {
                TriggerWin();
            }
        }

        /// <summary>
        /// Executes the win logic.
        /// </summary>
        private void TriggerWin()
        {
            this.m_hasWon = true;

            if (this.m_timerToStop != null)
            {
                this.m_timerToStop.StopTimer();
            }

            this.m_allCollected?.Invoke();
        }
        #endregion
    }
}
