using UnityEngine;

namespace WorkshopBehaviours.Session2_New
{
    /// <summary>
    /// Teleports the player to the spawn point when they enter this zone.
    /// Uses the PlayerRespawner component attached to the Player.
    /// </summary>
    public class HazardZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Acceptable to call GetComponent here because it only fires once
                // when the player enters the trigger, not every frame like in Update().
                PlayerRespawner respawner = other.GetComponent<PlayerRespawner>();
                
                if (respawner != null)
                {
                    respawner.Respawn();
                }
                else
                {
                    Debug.LogWarning("HazardZone: Object tagged Player entered, but it has no PlayerRespawner component.");
                }
            }
        }
    }
}
