using UnityEngine;

namespace ScriptingBasics
{
    public class Bullet : MonoBehaviour
    {
        void Start()
        {
            Destroy(gameObject, 2f); // Destroy bullet after 2 seconds
        }
    }
}