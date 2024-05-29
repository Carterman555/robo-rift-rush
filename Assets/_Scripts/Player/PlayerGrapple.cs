using ClipperLib;
using SpeedPlatformer.Audio;
using SpeedPlatformer.Environment;
using TarodevController;
using UnityEngine;
using UnityEngine.U2D;

namespace SpeedPlatformer.Player {
    enum GrappleState {
        Deactive = 0,
        Launching = 1,
        Grappled = 2
    }

    public class PlayerGrapple : MonoBehaviour {
        [SerializeField] private bool startUnlocked;

        [SerializeField] private LayerMask grappleSurfaceLayerMask;
        [SerializeField] private LayerMask obstacleLayerMask;

        [Tooltip("The point on the player where the grapple starts")]
        [SerializeField] private Transform playerGrapplePoint;

        [SerializeField] private LineRenderer grappleLine;
        [SerializeField] private Transform grapplePoint;

        [SerializeField] private float launchSpeed;
        [SerializeField] private float maxDistance;

        [SerializeField] private float correctRotationSpeed;

        private GrappleState state;

        private static bool unlocked;

        private Vector2 launchDirection;
        private float distanceTraveled;

        [SerializeField] private float grappleCooldown;
        private float grappleTimer = float.MaxValue;

        private PlayerController playerController;
        private DistanceJoint2D joint;

        private Vector2 environmentVelocity;

        #region Get Methods

        public bool IsUnlocked() {
            return unlocked;
        }

        public Vector2 GetGrapplePointPosition() {
            return grapplePoint.position;
        }

        #endregion

        #region Set Methods

        public void SetEnvironmentVelocity(Vector2 velocity) {
            environmentVelocity = velocity;
        }

        public void Unlock() {
            unlocked = true;
        }

        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            unlocked = false;
        }

        private void Awake() {
            playerController = GetComponent<PlayerController>();
            joint = GetComponent<DistanceJoint2D>();

            grappleLine.positionCount = 2;

            ChangeState(GrappleState.Deactive);

            if (!unlocked && startUnlocked) {
                unlocked = true;
            }
        }

        /// <summary>
        /// If in the deactive state, wait for mouse button down. If mouse button is clicked, start launching the grapple.
        /// If in the Launching state, move the grapple point towards the target pos
        /// </summary>
        private void Update() {

            if (!unlocked) return;

            grappleTimer += Time.deltaTime;

            grappleLine.SetPosition(0, playerGrapplePoint.position);

            if (state == GrappleState.Deactive) {
                if (Input.GetMouseButtonDown(0) && grappleTimer > grappleCooldown) {
                    ChangeState(GrappleState.Launching);
                    grappleTimer = 0;
                }

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, correctRotationSpeed * Time.deltaTime);
            }
            else if (state == GrappleState.Launching) {

                Vector2 newPosition = (Vector2)grapplePoint.position + (launchDirection * launchSpeed * Time.deltaTime);
                SetGrappleObjectPos(newPosition);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, correctRotationSpeed * Time.deltaTime);

                distanceTraveled += launchSpeed * Time.deltaTime;
                if (distanceTraveled > maxDistance) {
                    ChangeState(GrappleState.Deactive);
                }

                if (DetectingGrappleSurface(out Vector2 hitPoint)) {

                    // so can only grapple to surfaces above player
                    if (IsWithinUpVector(launchDirection)) {
                        SetGrappleObjectPos(hitPoint);
                        ChangeState(GrappleState.Grappled);
                    }
                    else {
                        ChangeState(GrappleState.Deactive);
                        AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.GrappleBreak, 0, 0.5f);
                    }
                }

                if (DetectingObstacle()) {
                    ChangeState(GrappleState.Deactive);
                    AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.GrappleBreak, 0, 0.5f);
                }

                ReleaseGrappleCheck();
            }
            else if (state == GrappleState.Grappled) {
                ReleaseGrappleCheck();

                if (!playerController.GroundBlockingPlayer) {
                    Vector3 directionToGrapple = grapplePoint.position - transform.position;
                    transform.up = directionToGrapple;
                }
            }
        }

        private void FixedUpdate() {
            if (state == GrappleState.Grappled) {

                // so moves with translating environment
                Vector3 newPosition = grapplePoint.position + ((Vector3)environmentVelocity * Time.fixedDeltaTime);

                SetGrappleObjectPos(newPosition);
            }
        }

        private void ReleaseGrappleCheck() {
            if (!Input.GetMouseButton(0)) {
                ChangeState(GrappleState.Deactive);
            }
        }

        /// <summary>
        /// When grapple gets deactivated: disable the grapple and joint and stop applying swing physics
        /// 
        /// When grapple gets launched: enable the grapple object and line, set their position, and get the target
        /// grapple position for the object to move towards
        /// 
        /// When the grapple objects touches the grapple surface layer, and grapples: enable the joint, set the
        /// distance, and start applying swing physics
        /// </summary>
        private void ChangeState(GrappleState newState) {

            state = newState;

            if (newState == GrappleState.Deactive) {
                grappleLine.enabled = false;
                grapplePoint.gameObject.SetActive(false);

                joint.enabled = false;

                distanceTraveled = 0;

                playerController.StopSwing();
            }
            else if (newState == GrappleState.Launching) {

                grapplePoint.gameObject.SetActive(true);
                grappleLine.enabled = true;

                SetGrappleObjectPos(playerGrapplePoint.position);

                launchDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerGrapplePoint.position).normalized;

                grapplePoint.up = launchDirection;

                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.GrappleLaunch, 0, 0.3f);
            }
            else if (newState == GrappleState.Grappled) {
                joint.enabled = true;
                joint.distance = Vector3.Distance(playerGrapplePoint.position, grapplePoint.position);

                playerController.StartSwing(grapplePoint.position);

                grapplePoint.rotation = Quaternion.identity;

                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.GrappleAttach, 0, 0.5f);
            }
        }

        private bool DetectingGrappleSurface(out Vector2 hitPoint) {

            float rayLength = 1f;
            RaycastHit2D hit = Physics2D.Raycast(grapplePoint.position,
                launchDirection,
                rayLength,
                grappleSurfaceLayerMask);

            bool hitGrappleSurface = hit.collider != null;
            hitPoint = hitGrappleSurface ? hit.point : Vector2.zero;
            return hitGrappleSurface;
        }

        private bool DetectingObstacle() {

            float rayLength = 1f;
            RaycastHit2D hit = Physics2D.Raycast(grapplePoint.position,
                launchDirection,
                rayLength,
                obstacleLayerMask);

            return hit.collider != null;
        }

        public bool IsWithinUpVector(Vector3 direction) {
            direction.Normalize();
            float angle = Vector3.Angle(direction, Vector3.up);

            float thresholdAngle = 70f;
            return angle <= thresholdAngle;
        }

        private void SetGrappleObjectPos(Vector3 pos) {
            grapplePoint.position = pos;
            grappleLine.SetPosition(1, pos);
        }
    }
}
