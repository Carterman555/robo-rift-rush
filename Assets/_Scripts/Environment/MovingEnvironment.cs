using SpeedPlatformer.Triggers;
using UnityEditor;
using UnityEngine;

namespace SpeedPlatformer.Environment {
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingEnvironment : MonoBehaviour {

        [SerializeField] private TriggerEvent moveTrigger;

        [SerializeField] private float moveAngle = 180f;
        [SerializeField] private float maxMoveSpeed;
        public bool startAtMaxMoveSpeed = true;
        [HideInInspector] public float moveAcceleration;

        [SerializeField] private float maxRotationSpeed;
        public bool startAtMaxRotationSpeed = true;
        [HideInInspector] public float rotationAcceleration;

        private float moveSpeed;
        private float rotationSpeed;

        public bool continuousMovement = true;
        [HideInInspector] public float moveDistance;

        enum MovementType { Waiting, Accelerating, Constant, Deacelerating, Moved }
        private MovementType currentMovement = MovementType.Waiting;

        private Rigidbody2D rb;

        #region Get Methods

        public float GetMoveAngle() {
            return moveAngle;
        }

        public float GetMaxMoveSpeed() {
            return maxMoveSpeed;
        }

        public bool GetStartMaxSpeed() {
            return startAtMaxMoveSpeed;
        }

        public float GetMaxRotationSpeed() {
            return maxRotationSpeed;
        }

        public bool GetStartMaxRotationSpeed() {
            return startAtMaxRotationSpeed;
        }

        public bool GetContinuousMovement() {
            return continuousMovement;
        }

        public float GetMoveDistance() {
            return moveDistance;
        }

        #endregion

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            moveTrigger.transform.position = transform.position;
        }

        private void OnEnable() {
            moveTrigger.OnTriggerEntered += TryStartMovement;
        }
        private void OnDisable() {
            moveTrigger.OnTriggerEntered -= TryStartMovement;
        }

        private void TryStartMovement(Collider2D collision) {
            if (currentMovement == MovementType.Waiting && collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                currentMovement = MovementType.Accelerating;
            }
        }

        float distanceMoved = 0;

        private void FixedUpdate() {

            switch (currentMovement) {
                case MovementType.Accelerating:

                    // accelerate speed and rotation
                    moveSpeed = Mathf.MoveTowards(moveSpeed, maxMoveSpeed, moveAcceleration * Time.fixedDeltaTime);
                    rotationSpeed = Mathf.MoveTowards(rotationSpeed, maxRotationSpeed, moveAcceleration * Time.fixedDeltaTime);

                    if (startAtMaxMoveSpeed) {
                        moveSpeed = maxMoveSpeed;
                    }

                    if (startAtMaxRotationSpeed) {
                        rotationSpeed = maxRotationSpeed;
                    }

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

        // TODO - for cleaner code move to another script (SRP)
        public void CreateMoveTrigger(Transform moveTriggerContainer) {
            GameObject moveTriggerObj = Instantiate(new GameObject(), moveTriggerContainer);

            moveTriggerObj.transform.position = transform.position;
            moveTriggerObj.name = "MoveTrigger_" + name.TryGetEndingNumber('_');

            BoxCollider2D collider = moveTriggerObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            // use the environments collider bounds to setup the bounds of the move trigger
            Bounds environmentBounds = GetComponent<CompositeCollider2D>().bounds;

            Vector2 offset = environmentBounds.center - collider.bounds.center;
            collider.offset = offset;

            float xSize = environmentBounds.size.x / collider.bounds.size.x;
            float ySize = environmentBounds.size.y / collider.bounds.size.y;
            collider.size = new Vector2(xSize, ySize);

            //... add and assign moveTrigger component
            moveTrigger = moveTriggerObj.AddComponent<TriggerEvent>();
        }

        public void TryUpdateMoveTriggerPosition() {
            if (moveTrigger != null) {
                moveTrigger.transform.position = transform.position;
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

            MovingEnvironment movingEnvironment = target as MovingEnvironment;

            //... move the trigger with the section when it's moved in editor mode
            movingEnvironment.TryUpdateMoveTriggerPosition();

            if (!movingEnvironment.startAtMaxMoveSpeed) {
                movingEnvironment.moveAcceleration = EditorGUILayout.FloatField("Move Acceleration", movingEnvironment.moveAcceleration);
            }

            if (!movingEnvironment.startAtMaxRotationSpeed) {
                movingEnvironment.rotationAcceleration = EditorGUILayout.FloatField("Rotation Acceleration", movingEnvironment.rotationAcceleration);
            }

            // to hide the moveDistance float if continuous movement is checked
            if (!movingEnvironment.continuousMovement) {
                
                movingEnvironment.moveDistance = EditorGUILayout.FloatField("Move Distance", movingEnvironment.moveDistance);
            }

            // spawn in move trigger
            if (GUILayout.Button("Create Move Trigger")) {

                if (Helpers.TryFindByName(out GameObject triggerContainer, "MoveTriggers")) {
                    movingEnvironment.CreateMoveTrigger(triggerContainer.transform);
                }
            }
        }
    }
#endif
}
