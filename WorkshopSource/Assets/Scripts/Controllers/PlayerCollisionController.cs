using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PlayerCollisionController : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            GameManager.Instance.RegisterCollision();

            if (other.GetComponents<IInteractable>() != null)
            {
                foreach (var interactive in other.GetComponents<IInteractable>())
                {
                    interactive.Enter();
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponents<IInteractable>() != null)
            {
                foreach (var interactive in other.GetComponents<IInteractable>())
                {
                    interactive.Exit();
                }
            }
        }
    }
}