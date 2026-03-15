using UnityEngine;
using WorkshopBehaviours.Session3.Movement;

namespace WorkshopBehaviours.Session2.Movement
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
            this.m_playerMover = GetComponent<PlayerMover>();
            this.m_playerOrbitalMover = GetComponent<PlayerOrbitalMover>();
            
            if (this.m_playerMover != null)
            {
                this.m_originalMoveSpeed = this.m_playerMover.MoveSpeed;
            }
            else if (this.m_playerOrbitalMover != null)
            {
                this.m_originalMoveSpeed = this.m_playerOrbitalMover.MoveSpeed;
            }
        }

        private void Update()
        {
            UpdateCooldown();

            if (this.m_isSliding)
            {
                HandleSliding();
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && this.m_cooldownTimer <= 0f)
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
            if (this.m_cooldownTimer > 0f)
            {
                this.m_cooldownTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Manages the active slide state.
        /// </summary>
        private void HandleSliding()
        {
            this.m_slideTimer -= Time.deltaTime;
            
            if (this.m_slideTimer <= 0f)
            {
                EndSlide();
            }
        }

        /// <summary>
        /// Activates the slide speed boost.
        /// </summary>
        private void StartSlide()
        {
            this.m_isSliding = true;
            this.m_slideTimer = this.m_slideDuration;
            
            if (this.m_playerMover != null)
            {
                this.m_playerMover.MoveSpeed = this.m_originalMoveSpeed * this.m_slideSpeedMultiplier;
            }
            else if (this.m_playerOrbitalMover != null)
            {
                this.m_playerOrbitalMover.MoveSpeed = this.m_originalMoveSpeed * this.m_slideSpeedMultiplier;
            }
        }

        /// <summary>
        /// Deactivates the slide speed boost.
        /// </summary>
        private void EndSlide()
        {
            this.m_isSliding = false;
            this.m_cooldownTimer = this.m_slideCooldown;
            
            if (this.m_playerMover != null)
            {
                this.m_playerMover.MoveSpeed = this.m_originalMoveSpeed;
            }
            else if (this.m_playerOrbitalMover != null)
            {
                this.m_playerOrbitalMover.MoveSpeed = this.m_originalMoveSpeed;
            }
        }
        #endregion
    }
}
