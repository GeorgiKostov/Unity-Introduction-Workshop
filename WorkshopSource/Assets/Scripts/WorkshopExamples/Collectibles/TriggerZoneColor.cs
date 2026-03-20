using UnityEngine;

namespace WorkshopExamples.Collectibles
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Collider))]
    public class TriggerZoneColor:MonoBehaviour
    {
        private Renderer renderer;
        public Color colorEnter;
        public Color exitColor;

        private void Awake()
        {
            renderer = GetComponent<Renderer>();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            renderer.material.color = colorEnter;
            Debug.Log("OnCollisionEnter");
        }
        
        private void OnCollisionExit(Collision other)
        {
            renderer.material.color = exitColor;
            Debug.Log("OnCollisionExit");
        }
    }
}