using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Behaviours
{
    public class InteractableChangeMaterial : MonoBehaviour, IInteractable
    {
        public Material OnEnterMaterial;
        public MeshRenderer renderer;

        private Material defaultMaterial;

        private void Awake()
        {
            if (renderer == null)
            {
                renderer = GetComponentInChildren<MeshRenderer>();
            }
            defaultMaterial = renderer.material;
        }

        public void Enter()
        {
            UIManager.Instance.DebugText.text = "Enter Interactable";
            renderer.material = OnEnterMaterial;
        }

        public void Exit()
        {
            UIManager.Instance.DebugText.text = "Exit Interactable";
            renderer.material = defaultMaterial;
        }
    }
}
