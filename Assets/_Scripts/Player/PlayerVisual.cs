using TarodevController;
using UnityEngine;

namespace SpeedPlatformer {
	public class PlayerVisual : MonoBehaviour {

		[SerializeField] private PlayerController playerController;

        private void Update() {

            // shrink to circle when rolling
            if (!playerController.Rolling) {
                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;
            }
            else {
                transform.localScale = new Vector3(1f, 0.5f, 1f);
                transform.localPosition = new Vector3(0f, -0.5f);
            }
        }

    }
}