using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class UIShowcase : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button button1;
        public Toggle toggle1;
        
        public TextMeshProUGUI DebugText;


        [Header("Tests")]
        public string button1Text;

        private void Awake()
        {
            button1.onClick.AddListener(delegate
            {
                DebugText.text = button1Text;
            });
        }
    }
}