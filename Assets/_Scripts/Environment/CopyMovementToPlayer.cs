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
                playerController.SetEnvironmentVelocity(Vector2.zero);
                touchingGrapple = false;
            }
        }

        private void Update() {
            if (touchingPlayer && !touchingGrapple) {
                Vector2 velocity = rb.velocity;
                velocity += GetVelocityFromRotation(playerController.transform.position, rb.angularVelocity);
                playerController.SetEnvironmentVelocity(velocity);
            }

            if (touchingGrapple) {

                Vector2 velocity = rb.velocity + GetVelocityFromRotation(playerGrapple.GetGrapplePointPosition(), rb.angularVelocity);
                playerController.SetEnvironmentVelocity(velocity);
                playerGrapple.SetEnvironmentVelocity(velocity);

                playerController.UpdateGrapplePos(playerGrapple.GetGrapplePointPosition());
            }
        }

        private Vector2 GetVelocityFromRotation(Vector3 objectPos, float angularVelocityDegrees) {

            // Calculate the angular velocity of the square in radians per second
            float angularVelocity = angularVelocityDegrees * Mathf.Deg2Rad;

            // Calculate the position of the circle relative to the square
            Vector3 relativePosition = objectPos - transform.position;

            // Calculate the velocity of the circle
            Vector2 velocity = new Vector2(
                -relativePosition.y * angularVelocity,
                relativePosition.x * angularVelocity
            );

            Debug.DrawRay(objectPos, velocity);

            return velocity;
        }
    }
}
