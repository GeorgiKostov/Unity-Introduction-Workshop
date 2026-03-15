using UnityEngine;
using WorkshopBehaviours.Session2.Movement;

namespace WorkshopBehaviours.Session4.Advanced
{
    public class CheckpointZone : MonoBehaviour
    {
        private bool m_activated = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (this.m_activated || !other.CompareTag("Player"))
                return;

            PlayerRespawner respawner = other.GetComponent<PlayerRespawner>();
            if (respawner != null)
            {
                respawner.SetSpawnPoint(transform);
                this.m_activated = true;

                // Change material emission color to indicate activated state
                Renderer render = GetComponent<Renderer>();
                if (render != null && render.material != null)
                {
                    render.material.SetColor("_EmissionColor", new Color(0.3f, 1.0f, 0.3f) * 1.5f);
                }
            }
        }
    }
}
