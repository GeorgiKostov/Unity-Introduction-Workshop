using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CharacterMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private InputActions inputActions;
    private Rigidbody2D rb;
    private float movementX;
    [FormerlySerializedAs("healthBar")] public PlayerStats playerStats;
    private int maxHealth = 10;
    public int HealthPoints;
    public int Coins;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new InputActions();
        inputActions.Basic.Enable();

        inputActions.Basic.Jump.performed += Jump;
        inputActions.Basic.MoveX.performed += MoveXOnperformed;
        inputActions.Basic.MoveX.canceled += MoveXOnperformed;
        this.HealthPoints = this.maxHealth;
        this.playerStats.UpdateHealth(this.HealthPoints, maxHealth);
    }

    private void MoveXOnperformed(InputAction.CallbackContext context)
    {
        movementX = context.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movementX * moveSpeed, rb.velocity.y);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        // Check if player is grounded before jumping
        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            ContactPoint2D contact = other.GetContact(0); // Actual collision point
            Collider2D enemyCollider = other.collider;
            float enemyTop = enemyCollider.bounds.max.y;

            // Slight tolerance to avoid edge cases (like grazing the side)
            bool isPlayerAbove = contact.point.y >= enemyTop - 0.1f && rb.velocity.y < 0;
            if (isPlayerAbove)
            {
                Destroy(other.gameObject);
                rb.AddForce(new Vector2(0, 5f), ForceMode2D.Impulse);
                Debug.Log("Kill   Enemy!");
            }
            else
            {
                ChangeHealth(-1);
                Debug.Log("Damage Player!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Health"))
        {
            Destroy(other.gameObject); // Collect the pickup
            ChangeHealth(1);
            Debug.Log("Health Collected!");
        }

        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject); // Collect the pickup
            Debug.Log("Coin Collected!");
            ChangeCoins(1);
        }
    }

    void ChangeCoins(int amount)
    {
        this.Coins += amount;
        this.playerStats.UpdateCoins(this.Coins);
    }

    void ChangeHealth(int amount)
    {
        this.HealthPoints += amount;
        this.playerStats.UpdateHealth(this.HealthPoints, maxHealth);
    }
}