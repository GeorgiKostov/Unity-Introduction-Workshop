using UnityEngine;

namespace WorkshopExamples.Collectibles
{
    public class TriggerZoneSound:MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip enterTriggerClip;
        public AudioClip exitTriggerClip;
        
        
        private void OnTriggerEnter(Collider other)
        {
            audioSource.PlayOneShot(enterTriggerClip);
            Debug.Log("OnTriggerEnter");
        }

        private void OnTriggerExit(Collider other)
        {
            audioSource.PlayOneShot(exitTriggerClip);

            Debug.Log("OnTriggerExit");
        }
    }
}