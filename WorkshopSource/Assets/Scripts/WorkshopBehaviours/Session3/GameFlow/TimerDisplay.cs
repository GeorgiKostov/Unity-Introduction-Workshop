using UnityEngine;
using TMPro;

namespace Workshop.Session3.GameFlow
{
    /// <summary>
    /// Displays the remaining time from CountdownTimer.
    /// Wire CountdownTimer.TimerTicked to this script's UpdateTimerDisplay method.
    /// Changes color when time is low.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimerDisplay : MonoBehaviour
    {
        #region Fields
        [Header("Display Settings")]
        [Tooltip("Text shown before the number. E.g. 'Time: '")]
        [SerializeField] private string m_prefix = "Time: ";

        [Header("Warning")]
        [Tooltip("Seconds remaining when the text turns warning color.")]
        [SerializeField] private int m_warningThreshold = 10;
        
        [SerializeField] private Color m_normalColor = Color.white;
        [SerializeField] private Color m_warningColor = Color.red;

        private TextMeshProUGUI m_timerText;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Cache reference to the Text component.
            m_timerText = GetComponent<TextMeshProUGUI>();
            
            // Set initial color.
            if (m_timerText != null)
            {
                m_timerText.color = m_normalColor;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the UI text and color based on seconds remaining.
        /// Wire to CountdownTimer.TimerTicked in the Inspector.
        /// </summary>
        /// <param name="secondsRemaining">Current seconds left on the timer.</param>
        public void UpdateTimerDisplay(int secondsRemaining)
        {
            if (m_timerText == null)
            {
                return;
            }

            // Update text using interpolation.
            m_timerText.text = $"{m_prefix}{secondsRemaining}";

            // Handle color warning.
            UpdateTextColor(secondsRemaining);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Changes the text color if time is below the threshold.
        /// </summary>
        /// <param name="seconds">Seconds remaining.</param>
        private void UpdateTextColor(int seconds)
        {
            if (seconds <= m_warningThreshold)
            {
                m_timerText.color = m_warningColor;
            }
            else
            {
                m_timerText.color = m_normalColor;
            }
        }
        #endregion
    }
}
