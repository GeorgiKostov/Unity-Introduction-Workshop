using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Behaviours
{
    public class InteractableSpawnParticles : MonoBehaviour, IInteractable
    {
        public GameObject prefab;


        public void Enter()
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }

        public void Exit()
        {
        }
    }
}