using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class ItemMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button openButton;
        public Button closeButton;

        public CanvasGroup group;

        private void Awake()
        {
            openButton.onClick.AddListener(delegate
            {
                Show();
            });

            closeButton.onClick.AddListener(delegate
            {
                Hide();
            });
        }

        public void Show()
        {
            group.alpha = 1;
        }

        public void Hide()
        {
            group.alpha = 0;
        }
    }
}