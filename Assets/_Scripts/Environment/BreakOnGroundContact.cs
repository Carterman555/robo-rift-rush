using UnityEngine;

namespace SpeedPlatformer.Environment {
    public class BreakOnGroundContact : MonoBehaviour {

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.layer == GameLayers.GroundLayer) {
                Destroy(gameObject);
            }
        }
    }
}
