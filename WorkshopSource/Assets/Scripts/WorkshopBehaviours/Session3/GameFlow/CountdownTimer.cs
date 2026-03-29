using UnityEngine;
using UnityEngine.Events;

namespace WorkshopBehaviours.Session3.GameFlow
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
        public bool IsTimerActive => this.m_isTimerActive;
        
        public float TimeRemaining => this.m_timeRemaining;

        public UnityEvent TimerExpired => this.m_timerExpired;

        public UnityEvent<int> TimerTicked => this.m_timerTicked;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            InitializeTimer();
        }

        private void Update()
        {
            if (!this.m_isTimerActive)
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
            this.m_isTimerActive = false;
        }

        /// <summary>
        /// Resumes or starts the countdown.
        /// </summary>
        public void StartTimer()
        {
            this.m_isTimerActive = true;
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
            this.m_timeRemaining = this.m_startTime;
            this.m_isTimerActive = true;
            this.m_lastTickSecond = Mathf.CeilToInt(this.m_timeRemaining);
        }

        /// <summary>
        /// Increments the timer and checks for expiry.
        /// </summary>
        private void UpdateTimer()
        {
            this.m_timeRemaining -= Time.deltaTime;

            // Fire a tick event each time a whole second passes.
            int currentSecond = Mathf.CeilToInt(this.m_timeRemaining);
            
            if (currentSecond != this.m_lastTickSecond)
            {
                this.m_lastTickSecond = currentSecond;
                OnTimerTicked(Mathf.Max(0, currentSecond));
            }

            // Check if the timer has run out.
            if (this.m_timeRemaining <= 0f)
            {
                this.m_timeRemaining = 0f;
                this.m_isTimerActive = false;
                OnTimerExpired();
            }
        }

        /// <summary>
        /// Raises the timer tick event.
        /// </summary>
        /// <param name="secondsRemaining">Whole seconds left on the clock.</param>
        private void OnTimerTicked(int secondsRemaining)
        {
            this.m_timerTicked?.Invoke(secondsRemaining);
        }

        /// <summary>
        /// Raises the timer expired event.
        /// </summary>
        private void OnTimerExpired()
        {
            this.m_timerExpired?.Invoke();
        }
        #endregion
    }
}
