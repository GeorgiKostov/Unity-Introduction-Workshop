using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorkshopBehaviours.Session4.Advanced
{
    /// <summary>
    /// Provides scene-loading methods that can be wired to UI Button OnClick events
    /// without any additional code. Add all scenes to Build Settings first.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        #region Fields
        [Tooltip("Optional delay before loading (seconds). Useful for fade-out timing.")]
        [SerializeField] private float m_loadDelayInSeconds = 0f;
        #endregion

        #region Public Methods
        /// <summary>
        /// Restarts the current scene. Wire to a "Restart" button.
        /// </summary>
        public void ReloadCurrentScene()
        {
            int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
            LoadSceneAsync(currentBuildIndex);
        }

        /// <summary>
        /// Loads a scene by its build index. Wire to "Next Level" or "Main Menu".
        /// </summary>
        /// <param name="index">Build index of the target scene.</param>
        public void LoadSceneByIndex(int index)
        {
            LoadSceneAsync(index);
        }

        /// <summary>
        /// Loads a scene by name. Wire to any button and type the scene name.
        /// </summary>
        /// <param name="sceneName">Exact name of the scene asset.</param>
        public void LoadSceneByName(string sceneName)
        {
            LoadSceneByNameAsync(sceneName);
        }

        /// <summary>
        /// Quits the application. Works in exported builds, not in the Editor.
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quit called — only works in a built executable.");
            Application.Quit();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the delayed scene load for build indices.
        /// </summary>
        /// <param name="index">Target build index.</param>
        private async void LoadSceneAsync(int index)
        {
            if (this.m_loadDelayInSeconds > 0)
            {
                await Awaitable.WaitForSecondsAsync(this.m_loadDelayInSeconds, destroyCancellationToken);
            }

            ResetTimeAndLoad(index);
        }

        /// <summary>
        /// Handles the delayed scene load for scene names.
        /// </summary>
        /// <param name="sceneName">Target scene name.</param>
        private async void LoadSceneByNameAsync(string sceneName)
        {
            if (this.m_loadDelayInSeconds > 0)
            {
                await Awaitable.WaitForSecondsAsync(this.m_loadDelayInSeconds, destroyCancellationToken);
            }

            ResetTimeAndLoad(sceneName);
        }

        /// <summary>
        /// Resets the time scale and loads the scene by index.
        /// </summary>
        private void ResetTimeAndLoad(int index)
        {
            Time.timeScale = 1f; // Ensure time is moving normally for the new scene.
            SceneManager.LoadScene(index);
        }

        /// <summary>
        /// Resets the time scale and loads the scene by name.
        /// </summary>
        private void ResetTimeAndLoad(string sceneName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
        #endregion
    }
}
