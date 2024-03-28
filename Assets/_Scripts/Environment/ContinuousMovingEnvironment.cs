using DG.Tweening;
using UnityEngine;

namespace SpeedPlatformer.Environment {
    [RequireComponent(typeof(Rigidbody2D))]
    public class ContinuousMovingEnvironment : MonoBehaviour {
        [SerializeField] private TriggerEvent startTrigger;

        [SerializeField] private float moveAngle;
        [SerializeField] private float maxMoveSpeed;
        [SerializeField] private float maxRotationSpeed;

        private float moveSpeed;
        private float rotationSpeed;

        [SerializeField] private float moveAcceleration;

        private bool moving;

        private Rigidbody2D rb;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable() {
            startTrigger.OnTriggerEntered += TryStartMovement;
        }

        private void OnDisable() {
            startTrigger.OnTriggerEntered -= TryStartMovement;
        }

        private void TryStartMovement(Collider2D collision) {
            int cameraFrameLayer = 8;
            if (!moving && collision.gameObject.layer == cameraFrameLayer) {
                moving = true;
            }
        }

        private void FixedUpdate() {
            if (!moving) {
                return;
            }

            // accelerate speed and rotation
            moveSpeed = Mathf.MoveTowards(moveSpeed, maxMoveSpeed, moveAcceleration * Time.fixedDeltaTime);
            rotationSpeed = Mathf.MoveTowards(rotationSpeed, maxRotationSpeed, moveAcceleration * Time.fixedDeltaTime);

            // translate
            Vector3 moveDirection = moveAngle.AngleToDirection();
            rb.velocity = moveDirection * moveSpeed;

            // rotate
            rb.angularVelocity = rotationSpeed;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;

            float lineLength = 10f;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(moveAngle.AngleToDirection() * lineLength));
        }
    }
}
