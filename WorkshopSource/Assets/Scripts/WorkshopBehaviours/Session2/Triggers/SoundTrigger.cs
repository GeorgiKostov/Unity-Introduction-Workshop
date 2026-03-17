using UnityEngine;

namespace WorkshopBehaviours.Session2.Triggers
{
    /// <summary>
    /// Plays an AudioClip once when the player enters the trigger zone.
    /// Will not replay until the player exits and re-enters.
    /// Requires an AudioSource on the same GameObject.
    /// Set "Is Trigger" on the Collider.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundTrigger : MonoBehaviour
    {
        #region Fields
        [Header("Audio Settings")]
        [Tooltip("The clip to play when the player enters.")]
        [SerializeField] private AudioClip m_audioClip;

        private AudioSource m_audioSource;
        private bool m_isPlayerInside;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            this.m_audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (this.m_audioClip == null)
            {
                Debug.LogWarning("SoundTrigger: audioClip is not assigned. Sound will not play.", this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (this.m_isPlayerInside || !other.CompareTag(Tags.Player))
            {
                return;
            }

            this.m_isPlayerInside = true;

            if (this.m_audioClip != null)
            {
                this.m_audioSource.PlayOneShot(this.m_audioClip);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tags.Player))
            {
                this.m_isPlayerInside = false;
            }
        }
        #endregion
    }
}
