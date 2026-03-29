using UnityEngine;

namespace WorkshopBehaviours.Session4.Advanced
{
    /// <summary>
    /// Keeps this GameObject alive across scene loads so music plays continuously.
    /// Prevents duplicate music objects if the scene is reloaded.
    /// Requires an AudioSource component with a clip assigned and Loop checked.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PersistentMusic : MonoBehaviour
    {
        #region Singleton
        private static PersistentMusic s_instance;
        #endregion

        #region Fields
        private AudioSource m_audioSource;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            InitializePersistence();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Ensures only one instance of the music exists and it survives scene loads.
        /// </summary>
        private void InitializePersistence()
        {
            // If a music object already exists from a previous scene load, destroy
            // this new duplicate — the original keeps playing uninterrupted.
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;

            // Cache reference and ensure the music is playing.
            this.m_audioSource = GetComponent<AudioSource>();
            
            if (this.m_audioSource != null && !this.m_audioSource.isPlaying)
            {
                this.m_audioSource.Play();
            }

            // Survive scene transitions.
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}
