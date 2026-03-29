using UnityEngine;

namespace WorkshopBehaviours.Session3.Feedback
{
    /// <summary>
    /// Plays an audio clip when the Player enters this trigger zone.
    /// Uses PlayClipAtPoint so no AudioSource component is needed on this object.
    /// </summary>
    public class AudioOnTrigger : MonoBehaviour
    {
        #region Fields
        [Header("Audio Settings")]
        [Tooltip("The audio clip to play on trigger enter.")]
        [SerializeField] private AudioClip m_audioClip;

        [Range(0f, 1f)]
        [SerializeField] private float m_volume = 1f;

        [Tooltip("If true, the sound will only play once ever.")]
        [SerializeField] private bool m_isSinglePlay = false;

        private bool m_hasPlayed;
        #endregion

        #region MonoBehaviour Methods
        private void OnTriggerEnter(Collider other)
        {
            // Only react to the player.
            if (!other.CompareTag(Tags.Player))
            {
                return;
            }

            // Check if we already played and it's a one-time trigger.
            if (this.m_isSinglePlay && this.m_hasPlayed)
            {
                return;
            }

            PlaySound();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Plays the assigned clip at this object's position.
        /// </summary>
        private void PlaySound()
        {
            if (this.m_audioClip == null)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(this.m_audioClip, transform.position, this.m_volume);
            this.m_hasPlayed = true;
        }
        #endregion
    }
}
