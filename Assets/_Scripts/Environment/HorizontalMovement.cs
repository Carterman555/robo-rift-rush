using UnityEngine;

namespace SpeedPlatformer {
    [RequireComponent(typeof(Rigidbody2D))]
    public class HorizontalMovement : MonoBehaviour {

        [SerializeField] private float moveDistance;

        [SerializeField] private float maxSpeed = 6.0f; // Speed of the movement in units per second
        [SerializeField] private float acceleration = 6.0f;

        private float rightPos;
        private float leftPos;

        private bool movingRight;

        private Rigidbody2D rb;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();

            float startPos = transform.localPosition.x;
            float endPos = transform.localPosition.x + moveDistance;

            if (endPos > startPos) {
                rightPos = endPos;
                leftPos = startPos;
                movingRight = true;
            }
            else {
                rightPos = startPos;
                leftPos = endPos;
                movingRight = false;
            }
        }

        private void FixedUpdate() {

            if (movingRight) {
                rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(maxSpeed, 0), acceleration * Time.fixedDeltaTime);

                print("Moving right: " + transform.localPosition.x);

                if (transform.localPosition.x > rightPos) {
                    movingRight = false;
                }
            }
            else {
                rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(-maxSpeed, 0), acceleration * Time.fixedDeltaTime);

                print("Moving left: " + transform.localPosition.x);

                if (transform.localPosition.x < leftPos) {
                    movingRight = true;
                }
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(moveDistance, 0));
        }
    }
}
