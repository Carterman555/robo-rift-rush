using SpeedPlatformer.Management;
using UnityEngine;

namespace SpeedPlatformer {
    public class WinLevelOnContact : MonoBehaviour {

        private static float winTimer;
        private float winDelay = 1f;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer && winTimer > winDelay) {
                GameProgress.ContinueNextLevel();
                winTimer = 0f;
            }
        }

        private void Update() {
            winTimer += Time.deltaTime;
        }
    }
}
