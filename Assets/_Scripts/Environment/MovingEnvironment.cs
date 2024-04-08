using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace SpeedPlatformer.Environment {
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingEnvironment : MonoBehaviour {
        [SerializeField] private float moveAngle = 180f;
        [SerializeField] private float maxMoveSpeed;
        [SerializeField] private float maxRotationSpeed;

        private float moveSpeed;
        private float rotationSpeed;

        [SerializeField] private float moveAcceleration = 20f;

        public bool continuousMovement = true;
        [HideInInspector] public float moveDistance;


        private Rigidbody2D rb;

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        enum MovementType { Waiting, Accelerating, Constant, Deacelerating, Moved }
        private MovementType currentMovement = MovementType.Waiting;

        private void Update() {
            if (currentMovement == MovementType.Waiting) {
                Vector3 vpPos = Camera.main.WorldToViewportPoint(transform.position);
                bool inFrame = vpPos.x >= 0f && vpPos.x <= 1f && vpPos.y >= 0f && vpPos.y <= 1f && vpPos.z > 0f;

                if (inFrame) {
                    currentMovement = MovementType.Accelerating;
                }
            }
        }

        float distanceMoved = 0;

        private void FixedUpdate() {

            switch (currentMovement) {
                case MovementType.Accelerating:
                    // accelerate speed and rotation
                    moveSpeed = Mathf.MoveTowards(moveSpeed, maxMoveSpeed, moveAcceleration * Time.fixedDeltaTime);
                    rotationSpeed = Mathf.MoveTowards(rotationSpeed, maxRotationSpeed, moveAcceleration * Time.fixedDeltaTime);

                    MoveEnvironment();

                    bool reachedMaxMoveSpeed = Mathf.Abs(moveSpeed - maxMoveSpeed) < 0.05f;
                    bool reachedMaxRotationSpeed = Mathf.Abs(rotationSpeed - maxRotationSpeed) < 0.05f;
                    if (reachedMaxMoveSpeed && reachedMaxRotationSpeed) {
                        currentMovement = MovementType.Constant;
                    }
                    break;
                case MovementType.Constant:
                    MoveEnvironment();
                    break;
                case MovementType.Deacelerating:
                    moveSpeed = Mathf.MoveTowards(moveSpeed, 0, moveAcceleration * Time.fixedDeltaTime);
                    rotationSpeed = Mathf.MoveTowards(rotationSpeed, 0, moveAcceleration * Time.fixedDeltaTime);

                    MoveEnvironment();

                    bool stoppedMoving = Mathf.Abs(moveSpeed) < 0.05f;
                    bool stoppedRotating = Mathf.Abs(rotationSpeed) < 0.05f;
                    if (stoppedMoving && stoppedRotating) {
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

        private void OnDrawGizmos() {
            if (continuousMovement) {
                Gizmos.color = Color.green;
                float lineLength = 10f;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)(moveAngle.AngleToDirection() * lineLength));
            }
            else {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)(moveAngle.AngleToDirection() * moveDistance));
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MovingEnvironment))]
    public class MovingEnvironmentEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            // to hide the moveDistance float if continuous movement is checked
            MovingEnvironment movingEnvironment = target as MovingEnvironment;

            if (!movingEnvironment.continuousMovement) {
                movingEnvironment.moveDistance = EditorGUILayout.FloatField("Move Distance", movingEnvironment.moveDistance);
            }
        }
    }
#endif
}
