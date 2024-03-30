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

        [SerializeField] private TriggerEvent grappleTrigger;
        [SerializeField] private LineRenderer grappleLine;
        [SerializeField] private Transform grapplePoint;

        [SerializeField] private float launchSpeed = 50f;

        private GrappleState state;

        private Vector2 targetPos;

        private PlayerController playerController;
        private DistanceJoint2D joint;
        
        private void Awake() {
            playerController = GetComponent<PlayerController>();
            joint = GetComponent<DistanceJoint2D>();

            grappleLine.positionCount = 2;

            ChangeState(GrappleState.Deactive);
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
                ChangeState(GrappleState.Grappled);
            }
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
                Vector2 newPosition = Vector2.MoveTowards(grapplePoint.position, targetPos, launchSpeed * Time.deltaTime);
                SetGrappleObjectPos(newPosition);

                CheckForStopGrapple();
            }
            else if (state == GrappleState.Grappled) {
                CheckForStopGrapple();
            }
        }

        private void CheckForStopGrapple() {
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
        /// When the grapple objects touches the ground layer, and grapples: enable the joint and start applying
        /// swing physics
        /// </summary>
        private void ChangeState(GrappleState newState) {

            state = newState;

            if (newState == GrappleState.Deactive) {
                grappleLine.enabled = false;
                grapplePoint.gameObject.SetActive(false);

                joint.enabled = false;

                playerController.StopSwing();
            }
            else if (newState == GrappleState.Launching) {

                grapplePoint.gameObject.SetActive(true);
                grappleLine.enabled = true;

                SetGrappleObjectPos(transform.position);

                targetPos = GetTargetPosition();
            }
            else if (newState == GrappleState.Grappled) {
                joint.enabled = true;

                playerController.StartSwing(grapplePoint.position);
            }
        }

        private void SetGrappleObjectPos(Vector3 pos) {
            grapplePoint.position = pos;
            grappleLine.SetPosition(1, pos);
        }

        private Vector2 GetTargetPosition() {
            Vector2 launchDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float grappleDistance = 100f;

            RaycastHit2D hit = Physics2D.Raycast(transform.position,
                launchDirection.normalized,
                grappleDistance,
                groundLayerMask);

            // if found ground, set the target position
            if (hit.collider != null) {
                return hit.point;
            }
            else {
                // this allows the player to grapple from far out, which isn't what's wanted - fix
                return launchDirection * grappleDistance;
            }
        }
    }
}
