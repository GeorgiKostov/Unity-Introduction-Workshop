using UnityEngine;

namespace WorkshopBehaviours.Session4.Polish
{
    /// <summary>
    /// Shakes the camera by randomising its local position offset.
    /// Uses the Singleton pattern so any script can call it easily.
    /// 
    /// HOW TO USE FROM ANOTHER SCRIPT:
    ///   ScreenShake.Instance.Shake(0.3f, 0.2f);
    /// 
    /// Attach to the Main Camera.
    /// </summary>
    public class ScreenShake : MonoBehaviour
    {
        #region Singleton
        private static ScreenShake s_instance;

        /// <summary>
        /// Public access to the ScreenShake singleton.
        /// </summary>
        public static ScreenShake Instance => s_instance;
        #endregion

        #region Fields
        [Header("Default Shake Values")]
        [Tooltip("How long the shake lasts in seconds.")]
        [SerializeField] private float m_defaultDuration = 0.3f;

        [Tooltip("How far the camera moves from its true position. Try 0.1 to 0.5.")]
        [SerializeField] private float m_defaultMagnitude = 0.2f;

        private Vector3 m_originalLocalPosition;
        private bool m_isShaking;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            InitializeSingleton();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Trigger a shake from any script.
        /// </summary>
        /// <param name="duration">Duration of the shake. Use -1 for default.</param>
        /// <param name="magnitude">Strength of the shake. Use -1 for default.</param>
        public async void Shake(float duration = -1f, float magnitude = -1f)
        {
            // Use defaults if caller doesn't specify.
            float targetDuration = duration < 0f ? this.m_defaultDuration : duration;
            float targetMagnitude = magnitude < 0f ? this.m_defaultMagnitude : magnitude;

            // We use the Awaitable API to handle the shake sequence.
            await ShakeAsync(targetDuration, targetMagnitude);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets up the Singleton and caches initial state.
        /// </summary>
        private void InitializeSingleton()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            s_instance = this;
            this.m_originalLocalPosition = transform.localPosition;
        }

        /// <summary>
        /// Operates the shake effect over time using the Awaitable API.
        /// </summary>
        private async Awaitable ShakeAsync(float duration, float magnitude)
        {
            // If already shaking, we can either return or override. 
            // Current implementation allows multiple calls to overlap but using Awaitable 
            // keeps it simpler than StopAllCoroutines.
            this.m_isShaking = true;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // Guard against object destruction.
                if (this == null)
                {
                    return;
                }

                elapsed += Time.deltaTime;

                // Lerp magnitude from full to zero over the shake duration.
                float strength = Mathf.Lerp(magnitude, 0f, elapsed / duration);

                // Random.insideSphere gives a point inside a unit sphere.
                Vector3 offset = Random.insideUnitSphere * strength;

                // Only shake on X and Y to avoid depth issues.
                transform.localPosition = this.m_originalLocalPosition + new Vector3(offset.x, offset.y, 0f);

                // Wait for the next frame.
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }

            // Restore exact original position once shake ends.
            if (this != null)
            {
                transform.localPosition = this.m_originalLocalPosition;
                this.m_isShaking = false;
            }
        }
        #endregion
    }
}
