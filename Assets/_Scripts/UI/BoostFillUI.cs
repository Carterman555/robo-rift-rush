using SpeedPlatformer.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedPlatformer {
	public class BoostFillUI : MonoBehaviour {

		private Image fillImage;
        private PlayerBoostCooldown playerBoostCooldown;

        private void Awake() {
            fillImage = GetComponent<Image>();
            playerBoostCooldown = FindObjectOfType<PlayerBoostCooldown>();
        }

        private void Update() {
            fillImage.fillAmount = playerBoostCooldown.GetBoostPower();
        }
    }
}