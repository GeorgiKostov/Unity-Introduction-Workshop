namespace WorkshopExamples.Manager
{
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Any script can call SceneLoader.Instance.LoadScene() without an Inspector reference
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private string loadingSceneName = "LoadingScreen";

    void Awake()
    {
        // Singleton guard — destroy any duplicate that spawns when a scene revisits
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Survive all scene loads — this object is never destroyed automatically
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneLoader.LoadScene: sceneName is empty. Aborting.");
            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        bool hasLoadingScreen = !string.IsNullOrEmpty(loadingSceneName);

        // Step 1 — Overlay the loading screen on top of the current scene
        if (hasLoadingScreen)
        {
            SceneManager.LoadScene(loadingSceneName, LoadSceneMode.Additive);
            yield return null; // Wait one frame so the loading screen renders
        }

        // Step 2 — Begin loading the target scene without activating it yet
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        // Step 3 — Wait until Unity reports the scene is fully loaded (0.9 = done)
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        // Step 4 — Activate the loaded scene and wait for it to finish
        op.allowSceneActivation = true;
        yield return new WaitUntil(() => op.isDone);

        // Step 5 — Remove the loading screen now that the new scene is live
        if (hasLoadingScreen)
        {
            SceneManager.UnloadSceneAsync(loadingSceneName);
        }
    }
}
}