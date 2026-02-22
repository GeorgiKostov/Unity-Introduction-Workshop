using UnityEngine;
using UnityEngine.Events;

namespace Workshop.Session3.GameFlow
{
    /// <summary>
    /// Counts down from a set time in seconds.
    /// Fires TimerExpired when it hits zero — wire this to a Game Over panel
    /// via the Inspector without writing any extra code.
    /// </summary>
    public class CountdownTimer : MonoBehaviour
    {
        #region Fields
        [Header("Timer Settings")]
        [Tooltip("Starting time in seconds. 60 = one minute.")]
        [SerializeField] private float m_startTime = 60f;

        [Header("Events")]
        [Tooltip("Fires when the timer reaches zero. Wire up your Game Over panel here.")]
        [SerializeField] private UnityEvent m_timerExpired;

        [Tooltip("Fires every second with the remaining time — connect to TimerDisplay.")]
        [SerializeField] private UnityEvent<int> m_timerTicked;

        private float m_timeRemaining;
        private bool m_isTimerActive;
        private int m_lastTickSecond;
        #endregion

        #region Properties
        public bool IsTimerActive => m_isTimerActive;
        
        public float TimeRemaining => m_timeRemaining;

        public UnityEvent TimerExpired => m_timerExpired;

        public UnityEvent<int> TimerTicked => m_timerTicked;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            InitializeTimer();
        }

        private void Update()
        {
            if (!m_isTimerActive)
            {
                return;
            }

            UpdateTimer();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Pauses the countdown.
        /// </summary>
        public void StopTimer()
        {
            m_isTimerActive = false;
        }

        /// <summary>
        /// Resumes or starts the countdown.
        /// </summary>
        public void StartTimer()
        {
            m_isTimerActive = true;
        }

        /// <summary>
        /// Resets the timer to the start time.
        /// </summary>
        public void ResetTimer()
        {
            InitializeTimer();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets up the initial timer state.
        /// </summary>
        private void InitializeTimer()
        {
            m_timeRemaining = m_startTime;
            m_isTimerActive = true;
            m_lastTickSecond = Mathf.CeilToInt(m_timeRemaining);
        }

        /// <summary>
        /// Increments the timer and checks for expiry.
        /// </summary>
        private void UpdateTimer()
        {
            m_timeRemaining -= Time.deltaTime;

            // Fire a tick event each time a whole second passes.
            int currentSecond = Mathf.CeilToInt(m_timeRemaining);
            
            if (currentSecond != m_lastTickSecond)
            {
                m_lastTickSecond = currentSecond;
                OnTimerTicked(Mathf.Max(0, currentSecond));
            }

            // Check if the timer has run out.
            if (m_timeRemaining <= 0f)
            {
                m_timeRemaining = 0f;
                m_isTimerActive = false;
                OnTimerExpired();
            }
        }

        /// <summary>
        /// Raises the timer tick event.
        /// </summary>
        /// <param name="secondsRemaining">Whole seconds left on the clock.</param>
        private void OnTimerTicked(int secondsRemaining)
        {
            m_timerTicked?.Invoke(secondsRemaining);
        }

        /// <summary>
        /// Raises the timer expired event.
        /// </summary>
        private void OnTimerExpired()
        {
            m_timerExpired?.Invoke();
        }
        #endregion
    }
}
