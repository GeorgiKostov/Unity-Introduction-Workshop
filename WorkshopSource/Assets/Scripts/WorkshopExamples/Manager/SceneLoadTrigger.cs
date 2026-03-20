using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }
}