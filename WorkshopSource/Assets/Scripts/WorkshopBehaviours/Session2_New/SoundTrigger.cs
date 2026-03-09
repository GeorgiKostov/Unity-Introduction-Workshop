using UnityEngine;

namespace Workshop.Session2_New
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
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (audioClip == null)
            {
                Debug.LogWarning("SoundTrigger: audioClip is missing. Sound will not play.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isPlayerInside && other.CompareTag("Player"))
            {
                isPlayerInside = true;
                if (audioClip != null)
                {
                    audioSource.PlayOneShot(audioClip);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInside = false;
            }
        }
    }
}
