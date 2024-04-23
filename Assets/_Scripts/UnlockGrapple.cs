using SpeedPlatformer.Player;
using UnityEngine;

namespace SpeedPlatformer {
    public class UnlockGrapple : MonoBehaviour {

        private void Awake() {
            if (FindObjectOfType<PlayerGrapple>().IsUnlocked()) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.TryGetComponent(out PlayerGrapple playerGrapple)) {
                playerGrapple.Unlock();
                Destroy(gameObject);
            }
        }
    }
}
