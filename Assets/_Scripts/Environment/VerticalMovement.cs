using UnityEngine;

namespace RoboRiftRush {
    [RequireComponent(typeof(Rigidbody2D))]
    public class VerticalMovement : MonoBehaviour {

        [SerializeField] private float moveDistance;

        [SerializeField] private float maxSpeed = 6.0f; // Speed of the movement in units per second
        [SerializeField] private float acceleration = 6.0f;

        private float highPos;
        private float lowPos;

        private bool movingUp;

        private Rigidbody2D rb;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();

            float startPos = transform.localPosition.y;
            float endPos = transform.localPosition.y + moveDistance;

            if (endPos > startPos) {
                highPos = endPos;
                lowPos = startPos;
                movingUp = true;
            }
            else {
                highPos = startPos;
                lowPos = endPos;
                movingUp = false;
            }
        }

        private void FixedUpdate() {
            
            if (movingUp) {
                rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(0, maxSpeed), acceleration * Time.fixedDeltaTime);

                if (transform.localPosition.y > highPos) {
                    movingUp = false;
                }
            }
            else {
                rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(0, -maxSpeed), acceleration * Time.fixedDeltaTime);

                if (transform.localPosition.y < lowPos) {
                    movingUp = true;
                }
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, moveDistance));
        }
    }
}
