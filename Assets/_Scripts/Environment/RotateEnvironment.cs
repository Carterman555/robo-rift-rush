using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedPlatformer {
    [RequireComponent(typeof(Rigidbody2D))]
    public class RotateEnvironment : MonoBehaviour {

        [Header("References")]
        [SerializeField] private TriggerEvent moveTrigger;


        [Header("Rotate Parameters")]
        [SerializeField] private float maxRotationSpeed;
        public bool startAtMaxRotationSpeed = true;
        [HideInInspector] public float rotationAcceleration;

        private enum RotateEndCondition { MatchTranslate, Continuous, Rotation }
        [SerializeField] private RotateEndCondition rotationEndCondition;
        [HideInInspector] public float endRotation;


        enum MovementType { Waiting, Moving, Deaccelerating, Moved }
        private MovementType currentMovement = MovementType.Waiting;

        private Rigidbody2D rb;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable() {
            moveTrigger.OnTriggerEntered += TryStartMovement;
        }
        private void OnDisable() {
            moveTrigger.OnTriggerEntered -= TryStartMovement;
        }

        private float rotationSpeed;

        private void TryStartMovement(Collider2D collision) {
            if (currentMovement == MovementType.Waiting && collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                currentMovement = MovementType.Moving; // accelerating or constant speed

                if (startAtMaxRotationSpeed) {
                    rotationSpeed = maxRotationSpeed;
                }
            }
        }

        private void FixedUpdate() {

            switch (currentMovement) {
                case MovementType.Moving:
                    rotationSpeed = Mathf.MoveTowards(rotationSpeed, maxRotationSpeed, rotationAcceleration * Time.fixedDeltaTime);
                    MoveEnvironment();
                    break;
                case MovementType.Deaccelerating:
                    rotationSpeed = Mathf.MoveTowards(rotationSpeed, 0, rotationAcceleration * Time.fixedDeltaTime);

                    MoveEnvironment();

                    bool stoppedRotating = Mathf.Abs(rotationSpeed) < 0.05f;
                    if (stoppedRotating) {
                        currentMovement = MovementType.Moved;
                    }

                    break;
            }
        }

        private void MoveEnvironment() {
            // translate
            Vector3 moveDirection = moveAngle.AngleToDirection();
            rb.velocity = moveDirection * moveSpeed;

            // rotate
            rb.angularVelocity = rotationSpeed;

            if (!continuousMovement) {
                distanceMoved += rb.velocity.magnitude * Time.deltaTime;
                if (distanceMoved > moveDistance) {
                    currentMovement = MovementType.Deacelerating;
                }
            }
        }
    }
}
