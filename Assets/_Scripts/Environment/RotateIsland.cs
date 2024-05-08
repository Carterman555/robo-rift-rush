using UnityEngine;
using UnityEditor;

namespace SpeedPlatformer.Environment {
    [RequireComponent(typeof(Rigidbody2D))]
    public class RotateIsland : MonoBehaviour {

        //[Header("References")]
        //[SerializeField] private TriggerEvent moveTrigger; // TODO - delete or keep all move trigger related code

        [Header("Rotate Parameters")]
        [SerializeField] private bool rotateClockwise = true;
        [SerializeField] private float maxRotationSpeed;
        public bool startAtMaxRotationSpeed = true;
        [HideInInspector] public float RotationAcceleration;

        private enum RotationEndCondition { MatchTranslate, Continuous, Rotation }
        [SerializeField] private RotationEndCondition rotationEndCondition;
        [HideInInspector] public float EndRotation;


        enum MovementType { Waiting, Moving, Deaccelerating }
        private MovementType currentMovement = MovementType.Waiting;

        private Rigidbody2D rb;

        //... using this instead of transform.eulerAngles.z because transform.eulerAngles.z is only between 0 and 360
        //... only accurate if rotationEndCondition is rotation
        private float currentRotation;

        #region Get Methods

        public bool EndFromRotation(){
            return rotationEndCondition == RotationEndCondition.Rotation;
        }

        #endregion

        #region Set Methods

        //public void SetMoveTrigger(TriggerEvent trigger) {
        //    moveTrigger = trigger;
        //}

        #endregion

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
            currentRotation = transform.eulerAngles.z;
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

        private float rotationSpeed;

        private void TryStartMovement(Collider2D collision) {
            if (currentMovement == MovementType.Waiting && collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                currentMovement = MovementType.Moving; // accelerating or constant speed

                if (startAtMaxRotationSpeed) {
                    rotationSpeed = maxRotationSpeed;
                }
            }
        }

        private float deacceleration;

        private void FixedUpdate() {
            switch (currentMovement) {
                case MovementType.Moving:
                    rotationSpeed = Mathf.MoveTowards(rotationSpeed, maxRotationSpeed, RotationAcceleration * Time.fixedDeltaTime);
                    SetRotation();
                    CheckStartDeaccelerating();
                    break;
                case MovementType.Deaccelerating:
                    rotationSpeed = Mathf.MoveTowards(rotationSpeed, 0, deacceleration * Time.fixedDeltaTime);
                    SetRotation();
                    CheckStopRotating();
                    break;
            }
        }

        private void SetRotation() {
            // velocity is negative when rotating clockwise and it's positive when rotating counter clockwise
            int direction = rotateClockwise ? -1 : 1;
            rb.angularVelocity = rotationSpeed * direction;
        }

        /// <summary>
        /// there are different ways the rotation can stop depending on rotationEndCondition
        /// </summary>
        private void CheckStartDeaccelerating() {
            
            // check if environment is done translating 
            if (rotationEndCondition == RotationEndCondition.MatchTranslate) {
                if (TryGetComponent(out TranslateIsland translateEnvironment)){
                    if (translateEnvironment.Deaccelerating()) {
                        currentMovement = MovementType.Deaccelerating;

                        float defaultDeacceration = 5f;
                        deacceleration = defaultDeacceration;
                    }
                }
                else {
                    Debug.LogWarning("Rotation End Type is MatchTranslate, but does not have TranslateEnvironment Component!");
                }
            }

            // check if past end rotation
            else if (rotationEndCondition == RotationEndCondition.Rotation) {

                currentRotation += rb.angularVelocity * Time.fixedDeltaTime;

                float rotateAmountToDeaccelerate = 3f;
                bool pastGoingClockwise = rotateClockwise && currentRotation < EndRotation + rotateAmountToDeaccelerate;
                bool pastGoingCounterClockwise = !rotateClockwise && currentRotation > EndRotation - rotateAmountToDeaccelerate;

                // if within 10 degrees of stopping, start deaccelerating and do math to figure at how fast
                if (pastGoingClockwise || pastGoingCounterClockwise) {
                    currentMovement = MovementType.Deaccelerating;
                    
                    //... calculate seconds using area under velocity-time graph,
                    float secondsToStop = (2 * rotateAmountToDeaccelerate) / rotationSpeed;

                    //...  then calculate deacceleration using slope of velocity-time graph (rise/run or vel/time)
                    deacceleration = rotationSpeed / secondsToStop;
                }
            }
        }

        private void CheckStopRotating() {
            bool stoppedRotating = Mathf.Abs(rotationSpeed) < 0.05f;
            if (stoppedRotating) {
                rb.angularVelocity = 0f;
                TryEnableFloatingMovement();
                enabled = false;
            }
        }

        // enable floating movement if not translating
        private void TryEnableFloatingMovement() {
            if (TryGetComponent(out TranslateIsland translateEnvironment)) {
                if (!translateEnvironment.enabled) {
                    GetComponent<FloatingIslandMovementPerlin>().enabled = true;
                }
            }
            else {
                GetComponent<FloatingIslandMovementPerlin>().enabled = true;
            }
        }

        //[ContextMenu("Create Move Trigger")]
        //public void CreateMoveTrigger() {

        //    //... add and assign moveTrigger component
        //    moveTrigger = Helpers.CreateMoveTrigger(transform);

        //    if (TryGetComponent(out TranslateEnvironment translateEnvironment)) {
        //        translateEnvironment.SetMoveTrigger(moveTrigger);
        //    }
        //}
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RotateIsland))]
    public class RotateEnvironmentEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            RotateIsland rotateEnvironment = target as RotateIsland;

            if (!rotateEnvironment.startAtMaxRotationSpeed) {
                rotateEnvironment.RotationAcceleration = EditorGUILayout.FloatField("Rotation Acceleration", rotateEnvironment.RotationAcceleration);
            }

            if (rotateEnvironment.EndFromRotation()) {
                rotateEnvironment.EndRotation = EditorGUILayout.FloatField("End Rotation", rotateEnvironment.EndRotation);
            }
        }
    }
#endif
}
