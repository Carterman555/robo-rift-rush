using System;
using TarodevController;
using UnityEngine;

namespace SpeedPlatformer.Player
{
    enum GrappleState {
        Deactive = 0,
        Launching = 1,
        Grappled = 2
    }

    public class PlayerGrapple : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayerMask;

        [SerializeField] private LineRenderer grappleLine;
        [SerializeField] private Transform grapplePoint;

        [SerializeField] private float launchSpeed;
        [SerializeField] private float maxDistance;

        private GrappleState state;

        private Vector2 launchDirection;
        private float distanceTraveled;

        private PlayerController playerController;
        private DistanceJoint2D joint;

        private Vector2 environmentVelocity;

        #region Set Methods

        public void SetEnvironmentVelocity(Vector2 velocity) {
            environmentVelocity = velocity;
        }

        #endregion

        private void Awake() {
            playerController = GetComponent<PlayerController>();
            joint = GetComponent<DistanceJoint2D>();

            grappleLine.positionCount = 2;

            ChangeState(GrappleState.Deactive);
        }

        /// <summary>
        /// If in the deactive state, wait for mouse button down. If mouse button is clicked, start launching the grapple.
        /// If in the Launching state, move the grapple point towards the target pos
        /// </summary>
        private void Update() {

            grappleLine.SetPosition(0, transform.position);

            if (state == GrappleState.Deactive) {
                if (Input.GetMouseButtonDown(0)) {
                    ChangeState(GrappleState.Launching);
                }
            }
            else if (state == GrappleState.Launching) {

                Vector2 newPosition = (Vector2)grapplePoint.position + (launchDirection * launchSpeed * Time.deltaTime);
                SetGrappleObjectPos(newPosition);

                distanceTraveled += launchSpeed * Time.deltaTime;
                if (distanceTraveled > maxDistance) {
                    ChangeState(GrappleState.Deactive);
                }

                RayCastForGround();

                ReleaseGrappleCheck();
            }
            else if (state == GrappleState.Grappled) {
                ReleaseGrappleCheck();
            }
        }

        private void FixedUpdate() {
            if (state == GrappleState.Grappled) {
                SetGrappleObjectPos(grapplePoint.position + ((Vector3)environmentVelocity * Time.fixedDeltaTime));
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
        /// When the grapple objects touches the ground layer, and grapples: enable the joint, set the distance, and start applying
        /// swing physics
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

                SetGrappleObjectPos(transform.position);

                launchDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            }
            else if (newState == GrappleState.Grappled) {
                joint.enabled = true;
                joint.distance = Vector3.Distance(transform.position, grapplePoint.position);

                playerController.StartSwing(grapplePoint.position);
            }
        }

        private void RayCastForGround() {

            float rayLength = 1f;
            RaycastHit2D hit = Physics2D.Raycast(grapplePoint.position,
                launchDirection,
                rayLength,
                groundLayerMask);

            if (hit.collider != null) {
                SetGrappleObjectPos(hit.point);
                ChangeState(GrappleState.Grappled);
            }
        }

        private void SetGrappleObjectPos(Vector3 pos) {
            grapplePoint.position = pos;
            grappleLine.SetPosition(1, pos);
        }
    }
}
