using System;
using UnityEditor;
using UnityEngine;

namespace SpeedPlatformer.Environment {
    [RequireComponent(typeof(Rigidbody2D))]
    public class TranslateIsland : MonoBehaviour {

        [Header ("References")]
        //[SerializeField] private TriggerEvent moveTrigger; // TODO - delete or keep all move trigger related code
        [SerializeField] private Transform targetTransform;
        private Vector3 targetPos;

        [Header("Movement Parameters")]
        [SerializeField] private float maxMoveSpeed;

        public bool startAtMaxMoveSpeed = true;
        [HideInInspector] public float moveAcceleration;

        enum MovementType { Waiting, Moving, Deaccelerating }
        private MovementType currentMovement = MovementType.Waiting;

        private Rigidbody2D rb;

        #region Get Methods

        public bool Deaccelerating() {
            return currentMovement == MovementType.Deaccelerating;
        }

        public Vector3 GetTargetPosition() {
            return targetTransform.position;
        }

        #endregion

        #region Set Methods

        public void SetTargetLocalPosition(Vector3 pos) {
            targetTransform.localPosition = pos;
            targetPos = targetTransform.position;
        }

        public void SetMaxMoveSpeed(float speed) {
            maxMoveSpeed = speed;
        }

        public void SetStartMaxSpeed(bool max) {
            startAtMaxMoveSpeed = max;
        }

        public void SetAcceleration(float acceleration) {
            moveAcceleration = acceleration;
        }

        #endregion

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();

            //... store starting pos because it is a child of the island and will change as island moves
            targetPos = targetTransform.position;
        }

        //private void OnEnable() {
        //    moveTrigger.OnTriggerEntered += TryStartMovement;
        //}
        //private void OnDisable() {
        //    moveTrigger.OnTriggerEntered -= TryStartMovement;
        //}

        // start movement when inframe
        private void OnTriggerEnter2D(Collider2D collision) {
            TryStartMovement(collision);
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
            Vector3 moveDirection = (targetPos - transform.position).normalized;
            rb.velocity = moveDirection * moveSpeed;
        }

        private void CheckNearTarget() {
            float distanceToDeaccelerate = 2f;
            float distanceFromTarget = (targetPos - transform.position).magnitude;
            if (distanceFromTarget < distanceToDeaccelerate) {
                currentMovement = MovementType.Deaccelerating;

                //... calculate seconds using area under velocity-time graph - distanceToDeaccelerate is area of triangle so * 2 to get
                //... area of square, then divide by speed to get time
                float secondsToStop = (2 * distanceToDeaccelerate) / moveSpeed;

                //...  then calculate deacceleration using slope of velocity-time graph (rise/run or vel/time)
                deacceleration = moveSpeed / secondsToStop;
            }
        }

        private void CheckStopMoving() {
            bool stoppedMoving = Mathf.Abs(moveSpeed) < 0.05f;
            if (stoppedMoving) {
                rb.velocity = Vector3.zero;
                TryEnableFloatingMovement();
                enabled = false;
            }
        }

        // enable floating movement if not rotating
        private void TryEnableFloatingMovement() {
            if (TryGetComponent(out RotateIsland rotateEnvironment)) {
                if (!rotateEnvironment.enabled) {
                    GetComponent<FloatingIslandMovementPerlin>().enabled = true;
                }
            }
            else {
                GetComponent<FloatingIslandMovementPerlin>().enabled = true;
            }
        }

        //public void TryUpdateMoveTriggerPosition() {
        //    if (moveTrigger != null) {
        //        moveTrigger.transform.position = transform.position;
        //    }
        //}

        //[ContextMenu("Create Move Trigger")]
        //public void CreateMoveTrigger() {

        //    //... add and assign moveTrigger component
        //    moveTrigger = Helpers.CreateMoveTrigger(transform);

        //    if (TryGetComponent(out RotateEnvironment rotateEnvironment)) {
        //        rotateEnvironment.SetMoveTrigger(moveTrigger);
        //    }
        //}
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TranslateIsland))]
    public class TranslateEnvironmentEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            TranslateIsland movingEnvironment = target as TranslateIsland;

            //... move the trigger with the section when it's moved in editor mode
            //movingEnvironment.TryUpdateMoveTriggerPosition();

            if (!movingEnvironment.startAtMaxMoveSpeed) {
                movingEnvironment.moveAcceleration = EditorGUILayout.FloatField("Move Acceleration", movingEnvironment.moveAcceleration);
            }


        }
    }
#endif
}
