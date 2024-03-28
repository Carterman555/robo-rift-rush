using TarodevController;
using UnityEngine;

namespace SpeedPlatformer {
    [RequireComponent (typeof(Rigidbody2D))]
    public class CopyMovementToPlayer : MonoBehaviour {

        private PlayerController playerController = null;
        private Rigidbody2D rb;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.TryGetComponent(out PlayerController controller)) {
                playerController = controller;
            }
        }

        private void OnCollisionExit2D(Collision2D collision) {
            if (collision.gameObject.TryGetComponent(out PlayerController controller)) {
                playerController.SetEnvironmentVelocity(Vector2.zero);
                playerController = null;
            }
        }

        private void Update() {
            if (playerController != null) {
                playerController.SetEnvironmentVelocity(rb.velocity);
            }
        }
    }
}
