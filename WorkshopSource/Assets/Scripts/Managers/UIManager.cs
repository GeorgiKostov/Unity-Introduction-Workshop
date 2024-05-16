using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                    if (_instance == null)
                    {
                        // Create a new GameManager GameObject
                        GameObject gameManagerObject = new GameObject("UIManager");
                        _instance = gameManagerObject.AddComponent<UIManager>();
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        private static UIManager _instance;

        [Header("UI Elements")]
        public Button button1;
        public Button button2;
        public TextMeshProUGUI DebugText;
        public TextMeshProUGUI CollisionsText;


        [Header("Tests")] 
        public string button1Text;
        public string button2Text;

        private void Awake()
        {
            button1.onClick.AddListener(delegate
            {
                DebugText.text = button1Text;
            });

            button2.onClick.AddListener(delegate
            {
                DebugText.text = button2Text;
            });
        }
    }
}