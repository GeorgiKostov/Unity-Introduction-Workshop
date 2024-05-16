using UnityEngine;

namespace Assets.Scripts.Behaviours
{
    public class AutoDestroySelf : MonoBehaviour
    {
        public float DestroyTime;

        void Awake()
        {
            Invoke(nameof(SelfDestroy), DestroyTime);
        }

        private void SelfDestroy()
        {
            Destroy(gameObject);
        }
    }
}