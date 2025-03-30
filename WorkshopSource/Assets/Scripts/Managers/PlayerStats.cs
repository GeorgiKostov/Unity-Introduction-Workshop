using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class PlayerStats : MonoBehaviour
    {
        public Image healthImage;
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI coinsText;
        
        public void UpdateHealth(int current, int max)
        {
            healthImage.fillAmount = (float)current / (float)max;
            healthText.text = $"{current} / {max}";
        }

        public void UpdateCoins(int current)
        {
            this.coinsText.text = $"{current}";
        }
    }
}