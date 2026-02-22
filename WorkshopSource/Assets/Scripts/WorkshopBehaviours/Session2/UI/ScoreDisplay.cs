using UnityEngine;
using TMPro;

namespace Workshop.Session2.UI
{
    /// <summary>
    /// Updates a TextMeshPro label whenever the score changes.
    /// Connect ScoreManager's ScoreChanged event to the UpdateScoreText method via the Inspector.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ScoreDisplay : MonoBehaviour
    {
        #region Fields
        [Header("Display Settings")]
        [Tooltip("Text shown before the number. E.g. 'Score: '")]
        [SerializeField] private string m_prefix = "Score: ";

        private TextMeshProUGUI m_scoreText;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Cache reference to the Text component.
            m_scoreText = GetComponent<TextMeshProUGUI>();
            
            // Set initial state.
            UpdateScoreText(0);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the UI text with a formatted score string.
        /// Wire this to ScoreManager.ScoreChanged in the Inspector.
        /// </summary>
        /// <param name="newScore">The new score value to display.</param>
        public void UpdateScoreText(int newScore)
        {
            if (m_scoreText == null)
            {
                return;
            }

            m_scoreText.text = $"{m_prefix}{newScore}";
        }
        #endregion
    }
}
