using UnityEngine;
using TMPro;

namespace Workshop.Session2_New
{
    /// <summary>
    /// Listens for OnScoreChanged and updates a TextMeshPro UI text element.
    /// </summary>
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Start()
        {
            if (scoreText == null)
            {
                Debug.LogWarning("ScoreDisplay: scoreText reference is missing. Ensure the Inspector field is assigned.");
            }
        }

        public void UpdateText(int newScore)
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + newScore;
            }
        }
    }
}
