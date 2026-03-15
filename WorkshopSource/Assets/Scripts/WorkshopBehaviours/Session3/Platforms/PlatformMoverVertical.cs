using UnityEngine;

namespace WorkshopBehaviours.Session3.Platforms
{
    /// <summary>
    /// Moves a platform up and down along the Y axis using a sine wave.
    ///
    /// Teaching points — identical pattern to PlatformMoverHorizontal:
    ///   - Same Rigidbody.MovePosition approach, same FixedUpdate, same
    ///     startPosition anchor. Only the axis changes (Y instead of X).
    ///   - Students see that the mechanic is the same; configuration
    ///     determines behaviour, not different code.
    ///
    /// Debrief questions:
    ///   - What happens if moveHeight is 0?  (Platform does not move — no amplitude)
    ///   - What happens if moveSpeed is negative?  (Direction reverses — starts
    ///     moving down first instead of up)
    ///   - Can you put both Horizontal and Vertical on the same object?
    ///     (Yes — each modifies a different axis — but only one MovePosition
    ///      wins per frame; a single combined script is the correct approach
    ///      for multi-axis movement.)
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlatformMoverVertical : MonoBehaviour
    {
        [SerializeField] private float moveHeight = 4f;  // amplitude in world units
        [SerializeField] private float moveSpeed  = 1f;  // oscillation cycles per second

        private Rigidbody   _rb;
        private Vector3     _startPosition;

        private void Awake()
        {
            // Cache once in Awake — never call GetComponent in Update.
            this._rb = GetComponent<Rigidbody>();
            this._startPosition = transform.position;
        }

        private void FixedUpdate()
        {
            // Sine wave offset on the Y axis
            float   yOffset = Mathf.Sin(Time.time * this.moveSpeed) * this.moveHeight;
            Vector3 target  = this._startPosition + new Vector3(0f, yOffset, 0f);

            // MovePosition informs the physics engine of the intended position
            // before collision resolution, so objects standing on the platform
            // move with it rather than sliding off.
            this._rb.MovePosition(target);
        }
    }
}
