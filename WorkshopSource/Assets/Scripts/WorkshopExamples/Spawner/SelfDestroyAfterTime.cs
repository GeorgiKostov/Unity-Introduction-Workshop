

using UnityEngine;

public class SelfDestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnDestroy()
    {
        // Fires when this GameObject is removed from the scene.
        // Use this for cleanup: unsubscribing from events, releasing resources, etc.
        // Note: also fires when the scene unloads — guard against that if needed.
        Debug.Log($"{gameObject.name} was destroyed.");
    }
}