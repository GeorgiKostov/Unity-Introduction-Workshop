using UnityEngine;

namespace WorkshopBehaviours.Session2_New
{
    /// <summary>
    /// Plays an audio clip when the player enters the trigger.
    /// Will not replay until the player exits and reenters.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private AudioClip audioClip;

        private AudioSource audioSource;
        private bool isPlayerInside;

        private void Awake()
        {
            this.audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (this.audioClip == null)
            {
                Debug.LogWarning("SoundTrigger: audioClip is missing. Sound will not play.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!this.isPlayerInside && other.CompareTag("Player"))
            {
                this.isPlayerInside = true;
                if (this.audioClip != null)
                {
                    this.audioSource.PlayOneShot(this.audioClip);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.isPlayerInside = false;
            }
        }
    }
}
