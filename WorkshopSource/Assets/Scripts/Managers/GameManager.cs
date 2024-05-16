using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        // Create a new GameManager GameObject
                        GameObject gameManagerObject = new GameObject("GameManager");
                        _instance = gameManagerObject.AddComponent<GameManager>();
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        private static GameManager _instance;

        private int collisions;

        public void RegisterCollision()
        {
            collisions += 1;
            UIManager.Instance.CollisionsText.text = $"Collisions: {collisions}";
        }
    }
}