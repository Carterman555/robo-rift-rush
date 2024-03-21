using TarodevController;
using UnityEngine;

namespace SpeedPlatformer
{
    public class CopyMovementToPlayer : MonoBehaviour
    {
        private Transform player = null;

        private void OnCollisionEnter2D(Collision2D collision) {
            int playerLayer = 6;
            if (collision.gameObject.layer == playerLayer) {
                player = collision.transform;

                player.SetParent(transform);
            }
        }

        private void OnCollisionExit2D(Collision2D collision) {
            int playerLayer = 6;
            if (collision.gameObject.layer == playerLayer) {
                player.SetParent(null);

                //player.GetComponent<PlayerController>().SetEnvironmentVelocity(Vector2.zero);
                player = null;

            }
        }

        private Vector3 previousPos;

        private void FixedUpdate() {

            if (player != null) {
                Vector3 frameVelocity = transform.position - previousPos;
                Vector3 velocity = frameVelocity / Time.fixedDeltaTime; // per sec

                //player.GetComponent<PlayerController>().SetEnvironmentVelocity(velocity);
            }

            previousPos = transform.position;
        }
    }
}
