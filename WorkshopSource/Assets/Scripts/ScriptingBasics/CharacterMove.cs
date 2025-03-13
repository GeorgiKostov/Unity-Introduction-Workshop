using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private InputActions inputActions;
    private Rigidbody2D rb;
    private float movementX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new InputActions();
        
        inputActions.Basic.Jump.performed += Jump;
        inputActions.Basic.MoveX.performed += MoveXOnperformed;
        inputActions.Basic.MoveX.canceled += MoveXOnperformed;
    }

    private void MoveXOnperformed(InputAction.CallbackContext context)
    {
        movementX = context.ReadValue<float>();
    }

    private void OnEnable()
    {
        inputActions.Basic.Enable();
    }

    private void OnDisable()
    {
        inputActions.Basic.Disable();
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with Enemy!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Health"))
        {
            Destroy(other.gameObject); // Collect the pickup
            Debug.Log("Health Collected!");
        }

        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject); // Collect the pickup
            Debug.Log("Coin Collected!");
        }
    }
}