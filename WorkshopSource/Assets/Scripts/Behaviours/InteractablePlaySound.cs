using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Behaviours
{
    public class InteractablePlaySound : MonoBehaviour, IInteractable
    {
        public AudioSource source;

        public void Enter()
        {
            UIManager.Instance.DebugText.text = "Play Sound";
            source.Play();
        }

        public void Exit()
        {
            source.Stop();
        }
    }
}