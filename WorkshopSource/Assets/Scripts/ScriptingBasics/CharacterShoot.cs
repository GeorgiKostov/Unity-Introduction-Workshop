using UnityEngine;
using UnityEngine.InputSystem;

namespace ScriptingBasics
{
    public class CharacterShoot : MonoBehaviour
    {
        public GameObject bulletPrefab; // Assign Bullet Prefab in Inspector
        public Transform firePoint; // Empty GameObject as spawn point
        public float bulletSpeed = 10f;
        public Camera mainCamera; // Main camera to convert screen position to world position

        private InputActions inputActions;

        void Awake()
        {
            inputActions = new InputActions();
            inputActions.Basic.MouseL.performed += MouseLOnperformed;
        }

        private void MouseLOnperformed(InputAction.CallbackContext obj)
        {
            FireBullet();
        }

        private void OnEnable()
        {
            inputActions.Basic.Enable();
        }

        private void OnDisable()
        {
            inputActions.Basic.Disable();
        }
        
        void FireBullet()
        {
            if (bulletPrefab == null || firePoint == null || mainCamera == null) return;

            // Instantiate bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            
            // Shoot directly forward
            Vector2 direction = firePoint.right.normalized;

            // Apply velocity
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = direction * bulletSpeed;
        }
    }
}