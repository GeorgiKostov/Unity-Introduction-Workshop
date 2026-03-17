using UnityEngine;

namespace WorkshopBehaviours.Session2.Environment
{
    /// <summary>
    /// Rotates this GameObject continuously around one or more of its local axes.
    /// Uses no physics components — purely transform-based.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        #region Fields
        [Header("Rotation Settings")]
        [Tooltip("Rotation speed in degrees per second on each axis.")]
        [SerializeField] private Vector3 m_rotationSpeed = new Vector3(0f, 90f, 0f);
        #endregion

        #region MonoBehaviour Methods
        private void Update()
        {
            transform.Rotate(this.m_rotationSpeed * Time.deltaTime);
        }
        #endregion
    }
}
