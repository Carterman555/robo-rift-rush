using UnityEngine;
using UnityEditor;

namespace SpeedPlatformer {
    [RequireComponent(typeof(Rigidbody2D))]
    public class RotateEnvironment : MonoBehaviour {

        [Header("References")]
        [SerializeField] private TriggerEvent moveTrigger;


        [Header("Rotate Parameters")]
        [SerializeField] private bool rotateClockwise = true;
        [SerializeField] private float maxRotationSpeed;
        public bool startAtMaxRotationSpeed = true;
        [HideInInspector] public float RotationAcceleration;

        private enum RotateEndCondition { MatchTranslate, Continuous, Rotation }
        [SerializeField] private RotateEndCondition rotationEndCondition;
        [HideInInspector] public float EndRotation;
        [HideInInspector] public int EndRotateCount;

        private int rotateCount = 0;


        enum MovementType { Waiting, Moving, Deaccelerating, Moved }
        private MovementType currentMovement = MovementType.Waiting;

        private Rigidbody2D rb;

        #region Get Methods

        public bool EndFromRotation(){
            return rotationEndCondition == RotateEndCondition.Rotation;
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

        private bool resetRotation;

        /// <summary>
        /// there are different ways the rotation can stop depending on rotationEndCondition
        /// </summary>
        private void CheckStartDeaccelerating() {
            
            // check if environment is done translating 
            if (rotationEndCondition == RotateEndCondition.MatchTranslate) {
                if (TryGetComponent(out TranslateEnvironment translateEnvironment)){
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
            else if (rotationEndCondition == RotateEndCondition.Rotation) {

                if (360f - transform.eulerAngles.z <= 0.1f) {
                    resetRotation = true;
                }

                float rotateAmountToDeaccelerate = 3f;
                bool pastGoingClockwise = rotateClockwise && transform.eulerAngles.z < EndRotation + rotateAmountToDeaccelerate;
                bool pastGoingCounterClockwise = !rotateClockwise && transform.eulerAngles.z > EndRotation - rotateAmountToDeaccelerate;

                if (!resetRotation && rotateCount <= EndRotateCount) {

                    if (pastGoingClockwise || pastGoingCounterClockwise) {
                        rotateCount++;
                        resetRotation = false;
                    }

                    return;
                }

                // if within 10 degrees of stopping, start deaccelerating and do math to figure at how fast
                if (pastGoingClockwise || pastGoingCounterClockwise) {
                    currentMovement = MovementType.Deaccelerating;
                    
                    //... calculate seconds using area under velocity-time graph,
                    float secondsToStop = (2 * rotateAmountToDeaccelerate) / rotationSpeed;

                    //...  then calculate deacceleration using slope of velocity-time graph (rise/run or vel/time)
                    deacceleration = rotationSpeed / secondsToStop;

                    print("Start deaccelerating: " + deacceleration);
                }
            }
        }

        private void CheckStopRotating() {
            bool stoppedRotating = Mathf.Abs(rotationSpeed) < 0.05f;
            if (stoppedRotating) {
                currentMovement = MovementType.Moved;
                rb.angularVelocity = 0f;
                print("stop");
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RotateEnvironment))]
    public class RotateEnvironmentEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            RotateEnvironment rotateEnvironment = target as RotateEnvironment;

            if (!rotateEnvironment.startAtMaxRotationSpeed) {
                rotateEnvironment.RotationAcceleration = EditorGUILayout.FloatField("Rotation Acceleration", rotateEnvironment.RotationAcceleration);
            }

            if (rotateEnvironment.EndFromRotation()) {
                rotateEnvironment.EndRotation = EditorGUILayout.FloatField("End Rotation", rotateEnvironment.EndRotation);
                rotateEnvironment.EndRotateCount = EditorGUILayout.IntField("End Rotate Count", rotateEnvironment.EndRotateCount);
            }
        }
    }
#endif
}
