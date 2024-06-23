using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class HealthBar : MonoBehaviour
    {
        public Image healthImage;
        public TextMeshProUGUI healthText;


        public void UpdateHealth(int current, int max)
        {
            healthImage.fillAmount = (float)current / (float)max;
            healthText.text = $"{current} / {max}";
        }
    }
}