using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Behaviours
{
    public class InteractableChangeSprite : MonoBehaviour, IInteractable
    {
        public Sprite OnEnterSprite;
        public SpriteRenderer spriteRenderer;

        private Sprite defaultSprite;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            defaultSprite = spriteRenderer.sprite;
        }

        public void Enter()
        {
            UIManager.Instance.DebugText.text = "Enter Interactable";
            
            spriteRenderer.sprite = OnEnterSprite;
        }

        public void Exit()
        {
            UIManager.Instance.DebugText.text = "Exit Interactable";

            spriteRenderer.sprite = defaultSprite;
        }
    }
}