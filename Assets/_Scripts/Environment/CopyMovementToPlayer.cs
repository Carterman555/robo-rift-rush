using SpeedPlatformer.Player;
using TarodevController;
using UnityEngine;

namespace SpeedPlatformer {
    [RequireComponent (typeof(Rigidbody2D))]
    public class CopyMovementToPlayer : MonoBehaviour {

        private Rigidbody2D rb;
        private PlayerController playerController;
        private PlayerGrapple playerGrapple;

        private bool touchingPlayer = false;
        private bool touchingGrapple = false;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
            playerController = FindObjectOfType<PlayerController>();
            playerGrapple = FindObjectOfType<PlayerGrapple>();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer) {
                touchingPlayer = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer) {
                playerController.SetEnvironmentVelocity(Vector2.zero);
                touchingPlayer = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.GrappleLayer) {
                touchingGrapple = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.GrappleLayer) {
                playerGrapple.SetEnvironmentVelocity(Vector2.zero);
                touchingGrapple = false;
            }
        }

        private void Update() {
            if (touchingPlayer) {
                playerController.SetEnvironmentVelocity(rb.velocity);
            }

            if (touchingGrapple) {
                playerController.SetEnvironmentVelocity(rb.velocity);
                playerGrapple.SetEnvironmentVelocity(rb.velocity);
            }
        }
    }
}
