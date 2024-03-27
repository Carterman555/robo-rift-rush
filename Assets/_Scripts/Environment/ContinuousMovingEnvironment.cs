using DG.Tweening;
using UnityEngine;

namespace SpeedPlatformer.Environment
{
    public class ContinuousMovingEnvironment : MonoBehaviour
    {
        [SerializeField] private TriggerEvent startTrigger;

        [SerializeField] private float moveAngle;
        [SerializeField] private float maxMoveSpeed;
        [SerializeField] private float maxRotationSpeed;

        private float moveSpeed;
        private float rotationSpeed;

        [SerializeField] private float moveAcceleration;

        private bool moving;

        private void OnEnable() {
            startTrigger.OnTriggerEntered += StartMovement;
        }

        private void OnDisable() {
            startTrigger.OnTriggerEntered -= StartMovement;
        }

        private void StartMovement(Collider2D collision) {
            int playerLayer = 6;
            if (!moving && collision.gameObject.layer == playerLayer) {
                moving = true;
            }
        }

        private Vector3 originalPos;
        private Vector3 originalRot; 
        private void Awake() {
            originalPos = transform.position;
            originalRot = transform.eulerAngles;
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
            transform.position += moveDirection * moveSpeed * Time.fixedDeltaTime;

            // rotate
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime));
        }

        [ContextMenu("Reset Pos")]
        private void ResetPosAndRot() {
            transform.position = originalPos;
            transform.eulerAngles = originalRot;
            transform.DOKill();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;

            float lineLength = 10f;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(moveAngle.AngleToDirection() * lineLength));
        }
    }
}
