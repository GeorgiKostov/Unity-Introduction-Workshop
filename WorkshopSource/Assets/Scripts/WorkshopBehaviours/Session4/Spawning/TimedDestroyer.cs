using UnityEngine;

namespace Workshop.Session4.Spawning
{
    /// <summary>
    /// Destroys the GameObject it is attached to after a set number of seconds.
    /// Attach to any spawned object to automatically clean it up.
    /// </summary>
    public class TimedDestroyer : MonoBehaviour
    {
        #region Fields
        [Tooltip("Seconds until this object destroys itself.")]
        [SerializeField] private float m_lifetimeInSeconds = 5f;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            ApplyDelayedDestruction();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Schedules the object for destruction.
        /// </summary>
        private void ApplyDelayedDestruction()
        {
            // Destroy(gameObject, delay) is a built-in Unity overload.
            Destroy(gameObject, m_lifetimeInSeconds);
        }
        #endregion
    }
}
