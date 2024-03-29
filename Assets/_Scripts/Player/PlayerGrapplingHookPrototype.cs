using SpeedPlatformer.Player;
using TarodevController;
using UnityEngine;

namespace SpeedPlatformer {

    

    /// <summary>
    /// This class allows the player to launch a grappling hook by holding right mouse button. When the player grapples something,
    /// they get pulled towards it. They can release the grappling hook by releasing the right mouse button, which can launch them.
    /// </summary>
    public class PlayerGrapplingHookPrototype : MonoBehaviour {

        enum GrapplingHookState {
            Deactive = 0,
            Launching = 1,
            Grappled = 2
        }

        [SerializeField] private LineRenderer grappleLine;
        [SerializeField] private Transform grapplePoint;

        private Vector2 grappledPos;

        [SerializeField] private TriggerEvent grappleTrigger;

        [SerializeField] private float launchSpeed = 50f;

        private GrapplingHookState state;

        private Vector2 launchDirection;

        private PlayerController playerController;

        private void Awake() {
            playerController = GetComponent<PlayerController>();

            grappleLine.positionCount = 2;

            grappleLine.enabled = false;
            grapplePoint.gameObject.SetActive(false);

            state = GrapplingHookState.Deactive;
        }

        private void OnEnable() {
            grappleTrigger.OnTriggerEntered += GrappleTriggered;
        }
        private void OnDisable() {
            grappleTrigger.OnTriggerEntered -= GrappleTriggered;
        }

        private void GrappleTriggered(Collider2D collision) {

            int groundLayer = 3;
            if (collision.gameObject.layer == groundLayer) {
                ChangeState(GrapplingHookState.Grappled);
            }
        }

        private void Update() {

            grappleLine.SetPosition(0, transform.position);

            if (state == GrapplingHookState.Deactive) {
                if (Input.GetMouseButtonDown(1)) {
                    grappleLine.enabled = true;
                    grapplePoint.gameObject.SetActive(true);

                    ChangeState(GrapplingHookState.Launching);
                    launchDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    launchDirection.Normalize();
                    grapplePoint.position = transform.position;

                    grappleLine.SetPosition(1, transform.position);
                }
            }
            else if (state == GrapplingHookState.Launching) {
                grapplePoint.position += (Vector3)launchDirection * launchSpeed * Time.deltaTime;
                grappleLine.SetPosition(1, grapplePoint.position);

                CheckForStopGrapple();

            }
            else if (state == GrapplingHookState.Grappled) {
                grappleLine.SetPosition(1, grapplePoint.position);

                CheckForStopGrapple();
            }
        }

        private void ChangeState(GrapplingHookState newState) {

            state = newState;

            if (newState == GrapplingHookState.Deactive) {
                playerController.StopGrappling();
            }
            else if (newState == GrapplingHookState.Launching) {
                playerController.StopGrappling();
            }
            else if (newState == GrapplingHookState.Grappled) {
                grappledPos = grapplePoint.position;
                playerController.StartGrappling(grappledPos);
            }
        }

        private void CheckForStopGrapple() {
            if (!Input.GetMouseButton(1)) {
                grappleLine.enabled = false;
                grapplePoint.gameObject.SetActive(false);

                ChangeState(GrapplingHookState.Deactive);
                grappleLine.SetPosition(1, transform.position);
            }
        }
    }
}