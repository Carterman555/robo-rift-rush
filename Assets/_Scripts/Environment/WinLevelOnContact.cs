using SpeedPlatformer.Management;
using UnityEngine;

namespace SpeedPlatformer {
    public class WinLevelOnContact : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer) {
                GameProgress.ContinueNextLevel();
            }
        }
    }
}
