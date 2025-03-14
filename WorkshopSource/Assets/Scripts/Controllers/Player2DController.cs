using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;

public class Player2DController : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    int coinsCollected;
    bool isGrounded;
    Rigidbody2D rb;
    string currentState;

    // Animation states
    const string PLAYER_IDLE = "Idle";
    const string PLAYER_RUN = "Run";
    const string PLAYER_JUMP = "Jump";

    private Animator animator;
    public HealthBar healthBar;
    private int currentHealth;
    public int MaxHealth = 100;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = MaxHealth / 2;
        healthBar.UpdateHealth(currentHealth, MaxHealth);
        isGrounded = true;
    }

    void Update()
    {
        Move();
        Jump();
        Animate();
    }

    void Move()
    {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1); // Flip character
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            ChangeAnimationState(PLAYER_JUMP);
            isGrounded = false;
        }
    }

    void Animate()
    {
        if (isGrounded)
        {
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                ChangeAnimationState(PLAYER_RUN);
            }
            else
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
        }
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.SetTrigger(newState);
        currentState = newState;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            coinsCollected++;
            coinText.text = coinsCollected.ToString();
        }

        if (other.gameObject.CompareTag("Health"))
        {
            if (currentHealth < MaxHealth)
            {
                currentHealth++;
            }
            Destroy(other.gameObject);
            healthBar.UpdateHealth(currentHealth, MaxHealth);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (currentHealth > 0)
            {
                currentHealth--;
            }
            Destroy(other.gameObject);
            healthBar.UpdateHealth(currentHealth, MaxHealth);
        }
    }
}
