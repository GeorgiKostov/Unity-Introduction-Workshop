using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours
{
    public class CharacterAnimator : MonoBehaviour
    {
        private Animator animator;
        private InputActions inputActions;

        void Start()
        {
            animator = GetComponent<Animator>();
            inputActions = new InputActions();
            inputActions.Basic.Enable();

            inputActions.Basic.Jump.performed += Jump;
            inputActions.Basic.MoveX.performed += Move;
            inputActions.Basic.MoveX.canceled += MoveCancel;
        }

        public void ChangeAnimationState(string newState)
        {
            animator.SetTrigger(newState);
        }

        private void Move(InputAction.CallbackContext context)
        {
            ChangeAnimationState("Run");
            float direction = context.ReadValue<float>();
            if (direction != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1); // Flip character
            }
        }

        private void MoveCancel(InputAction.CallbackContext context)
        {
            ChangeAnimationState("Idle");
        }

        private void Jump(InputAction.CallbackContext context)
        {
            ChangeAnimationState("Jump");
        }
    }
}