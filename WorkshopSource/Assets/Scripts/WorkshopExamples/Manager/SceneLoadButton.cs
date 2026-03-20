
namespace WorkshopExamples.Manager
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class SceneLoadButton : MonoBehaviour
    {
        // Set this in the Inspector — must match a scene name in Build Settings exactly
        [SerializeField] private string targetSceneName;

        void Start()
        {
            // AddListener wires the button in code — no manual Inspector wiring needed
            Button button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            if (string.IsNullOrEmpty(targetSceneName))
                Debug.LogWarning("SceneLoadButton: targetSceneName is empty. Button will do nothing.", this);
        }

        private void OnClick()
        {
            if (SceneLoader.Instance == null)
            {
                Debug.LogWarning("SceneLoadButton: SceneLoader.Instance is null. Is a SceneLoader in the scene?");
                return;
            }

            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogWarning("SceneLoadButton: targetSceneName is empty. Assign a scene name in the Inspector.");
                return;
            }

            SceneLoader.Instance.LoadScene(targetSceneName);
        }
    }
}
