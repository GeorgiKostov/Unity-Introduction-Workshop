using UnityEngine;

namespace Workshop.Session3.Platforms
{
    /// <summary>
    /// Moves a platform left and right along the X axis using a sine wave.
    ///
    /// Teaching points:
    ///   1. Rigidbody.MovePosition informs the physics engine of the intended
    ///      position before collision resolution — objects standing on the
    ///      platform move with it rather than sliding off.
    ///   2. transform.position is a teleport; MovePosition is physics-aware.
    ///   3. StartPosition is stored in Awake so the oscillation always has
    ///      a fixed origin to return to.
    ///   4. Runs in FixedUpdate because MovePosition is a physics operation
    ///      and must match the physics timestep.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlatformMoverHorizontal : MonoBehaviour
    {
        [SerializeField] private float moveDistance = 5f;  // amplitude in world units
        [SerializeField] private float moveSpeed    = 1f;  // oscillation cycles per second

        private Rigidbody   _rb;
        private Vector3     _startPosition;

        private void Awake()
        {
            // Cache once in Awake — never call GetComponent in Update.
            _rb = GetComponent<Rigidbody>();
            _startPosition = transform.position;
        }

        private void FixedUpdate()
        {
            // Sine wave offset on the X axis
            float   xOffset  = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            Vector3 target   = _startPosition + new Vector3(xOffset, 0f, 0f);

            // MovePosition informs the physics engine of the intended position
            // before collision resolution, so objects standing on the platform
            // move with it rather than sliding off.
            _rb.MovePosition(target);
        }
    }
}
