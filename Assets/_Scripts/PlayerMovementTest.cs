using UnityEngine;

namespace SpeedPlatformer {
	public class PlayerMovementTest : MonoBehaviour {

        [SerializeField] private float speed;

        private Rigidbody2D rb;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update() {
            rb.velocity = Vector3.right * speed;
        }
    }
}