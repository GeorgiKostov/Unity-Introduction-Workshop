using UnityEngine;

namespace WorkshopExamples.Movement
{
    /// <summary>
    /// Temporarily multiplies the player's move speed when Left Shift is held.
    /// Communicates with PlayerMover on the same GameObject.
    /// Requires PlayerMover to be present.
    /// </summary>
    public class PlayerSlider : MonoBehaviour
    {
        #region Fields
        [Header("Slide Settings")]
        [Tooltip("How much faster the player moves while sliding. Try 2 to 3.")]
        [SerializeField] private float m_slideSpeedMultiplier = 2.5f;

        [Tooltip("Maximum time (seconds) the slide boost lasts.")]
        [SerializeField] private float m_slideDuration = 0.8f;

        [Tooltip("Seconds before the player can slide again.")]
        [SerializeField] private float m_slideCooldown = 1.5f;

        private PlayerMover m_playerMover;
        private PlayerOrbitalMover m_playerOrbitalMover;
        private float m_originalMoveSpeed;
        private float m_slideTimer;
        private float m_cooldownTimer;
        private bool m_isSliding;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            m_playerMover = GetComponent<PlayerMover>();
            m_playerOrbitalMover = GetComponent<PlayerOrbitalMover>();
            
            if (m_playerMover != null)
            {
                m_originalMoveSpeed = m_playerMover.MoveSpeed;
            }
            else if (m_playerOrbitalMover != null)
            {
                m_originalMoveSpeed = m_playerOrbitalMover.MoveSpeed;
            }
        }

        private void Update()
        {
            UpdateCooldown();

            if (m_isSliding)
            {
                HandleSliding();
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && m_cooldownTimer <= 0f)
            {
                StartSlide();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Decrements the cooldown timer.
        /// </summary>
        private void UpdateCooldown()
        {
            if (m_cooldownTimer > 0f)
            {
                m_cooldownTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Manages the active slide state.
        /// </summary>
        private void HandleSliding()
        {
            m_slideTimer -= Time.deltaTime;
            
            if (m_slideTimer <= 0f)
            {
                EndSlide();
            }
        }

        /// <summary>
        /// Activates the slide speed boost.
        /// </summary>
        private void StartSlide()
        {
            m_isSliding = true;
            m_slideTimer = m_slideDuration;
            
            if (m_playerMover != null)
            {
                m_playerMover.MoveSpeed = m_originalMoveSpeed * m_slideSpeedMultiplier;
            }
            else if (m_playerOrbitalMover != null)
            {
                m_playerOrbitalMover.MoveSpeed = m_originalMoveSpeed * m_slideSpeedMultiplier;
            }
        }

        /// <summary>
        /// Deactivates the slide speed boost.
        /// </summary>
        private void EndSlide()
        {
            m_isSliding = false;
            m_cooldownTimer = m_slideCooldown;
            
            if (m_playerMover != null)
            {
                m_playerMover.MoveSpeed = m_originalMoveSpeed;
            }
            else if (m_playerOrbitalMover != null)
            {
                m_playerOrbitalMover.MoveSpeed = m_originalMoveSpeed;
            }
        }
        #endregion
    }
}
