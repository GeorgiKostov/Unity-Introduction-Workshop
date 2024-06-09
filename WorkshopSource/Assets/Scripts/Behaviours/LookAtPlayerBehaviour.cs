using Assets.Scripts.Controllers;
using UnityEngine;

namespace Assets.Scripts.Behaviours
{
    public class LookAtPlayerBehaviour : MonoBehaviour
    {
        private Transform target;

        private void Awake()
        {
            target = FindObjectOfType<Player3DMoveController>().transform;
        }

        void Update()
        {
            if (transform != null)
            {
                transform.LookAt(target);
            }
        }
    }
}