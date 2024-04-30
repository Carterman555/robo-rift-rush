using SpeedPlatformer.Environment;
using UnityEditor;
using UnityEngine;

namespace SpeedPlatformer {
    [RequireComponent(typeof(Rigidbody2D))]
    public class TranslateEnvironment : MonoBehaviour {

        [Header ("References")]
        [SerializeField] private TriggerEvent moveTrigger;
        [SerializeField] private Transform targetTransform;

        [Header("Movement Parameters")]
        [SerializeField] private float maxMoveSpeed;

        public bool startAtMaxMoveSpeed = true;
        [HideInInspector] public float moveAcceleration;

        enum MovementType { Waiting, Moving, Deaccelerating, Moved }
        private MovementType currentMovement = MovementType.Waiting;

        private Rigidbody2D rb;

        #region Get Methods

        public bool Deaccelerating() {
            return currentMovement == MovementType.Deaccelerating;
        }

        #endregion

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable() {
            moveTrigger.OnTriggerEntered += TryStartMovement;
        }
        private void OnDisable() {
            moveTrigger.OnTriggerEntered -= TryStartMovement;
        }

        private void TryStartMovement(Collider2D collision) {
            if (currentMovement == MovementType.Waiting && collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                currentMovement = MovementType.Moving; // accelerating or constant speed

                if (startAtMaxMoveSpeed) {
                    moveSpeed = maxMoveSpeed;
                }
            }
        }

        private float moveSpeed;
        private float deacceleration;

        private void FixedUpdate() {

            switch (currentMovement) {
                case MovementType.Moving:
                    // accelerate speed and rotation
                    moveSpeed = Mathf.MoveTowards(moveSpeed, maxMoveSpeed, moveAcceleration * Time.fixedDeltaTime);
                    MoveEnvironment();
                    CheckNearTarget();
                    break;
                case MovementType.Deaccelerating:
                    moveSpeed = Mathf.MoveTowards(moveSpeed, 0, deacceleration * Time.fixedDeltaTime);
                    MoveEnvironment();
                    CheckStopMoving();
                    break;
            }
        }

        private void MoveEnvironment() {
            Vector3 moveDirection = (targetTransform.position - transform.position).normalized;
            rb.velocity = moveDirection * moveSpeed;
        }

        private void CheckNearTarget() {
            float distanceToDeaccelerate = 2f;
            float distanceFromTarget = (targetTransform.position - transform.position).magnitude;
            if (distanceFromTarget < distanceToDeaccelerate) {
                currentMovement = MovementType.Deaccelerating;

                //... calculate seconds using area under velocity-time graph,
                float secondsToStop = (2 * distanceToDeaccelerate) / moveSpeed;

                //...  then calculate deacceleration using slope of velocity-time graph (rise/run or vel/time)
                deacceleration = moveSpeed / secondsToStop;
            }
        }

        private void CheckStopMoving() {
            bool stoppedMoving = Mathf.Abs(moveSpeed) < 0.05f;
            if (stoppedMoving) {
                rb.velocity = Vector3.zero;
                currentMovement = MovementType.Moved;
            }
        }

        public void TryUpdateMoveTriggerPosition() {
            if (moveTrigger != null) {
                moveTrigger.transform.position = transform.position;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TranslateEnvironment))]
    public class TranslateEnvironmentEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            TranslateEnvironment movingEnvironment = target as TranslateEnvironment;

            //... move the trigger with the section when it's moved in editor mode
            movingEnvironment.TryUpdateMoveTriggerPosition();

            if (!movingEnvironment.startAtMaxMoveSpeed) {
                movingEnvironment.moveAcceleration = EditorGUILayout.FloatField("Move Acceleration", movingEnvironment.moveAcceleration);
            }
        }
    }
#endif
}
