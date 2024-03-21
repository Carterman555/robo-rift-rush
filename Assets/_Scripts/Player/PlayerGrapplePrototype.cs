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

    public class PlayerGrapplePrototype : MonoBehaviour
    {
        [SerializeField] private LineRenderer grappleLine;
        [SerializeField] private Transform grapplePoint;


        private Vector2 grappledPos;

        [SerializeField] private TriggerEvent grappleTrigger;

        [SerializeField] private float launchSpeed = 50f;

        private GrappleState state;

        private Vector2 launchDirection;

        private PlayerController playerController;
        private DistanceJoint2D joint;
        
        private void Awake() {
            playerController = GetComponent<PlayerController>();
            joint = GetComponent<DistanceJoint2D>();

            grappleLine.positionCount = 2;

            grappleLine.enabled = false;
            grapplePoint.gameObject.SetActive(false);
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

        private void Update() {

            grappleLine.SetPosition(0, transform.position);

            if (state == GrappleState.Deactive) {
                if (Input.GetMouseButtonDown(0)) {
                    grappleLine.enabled = true;
                    grapplePoint.gameObject.SetActive(true);

                    ChangeState(GrappleState.Launching);
                    launchDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    launchDirection.Normalize();
                    grapplePoint.position = transform.position;

                    grappleLine.SetPosition(1, transform.position);
                }

                playerController.StopSwing();
            }
            else if (state == GrappleState.Launching) {
                grapplePoint.position += (Vector3)launchDirection * launchSpeed * Time.deltaTime;
                grappleLine.SetPosition(1, grapplePoint.position);

                CheckForStopGrapple();

            }
            else if (state == GrappleState.Grappled) {
                grappleLine.SetPosition(1, grapplePoint.position);

                CheckForStopGrapple();
            }
        }

        private void ChangeState(GrappleState newState) {

            state = newState;

            if (newState == GrappleState.Deactive) {
                joint.enabled = false;
                playerController.StopSwing();
            }
            else if (newState == GrappleState.Launching) {
                joint.enabled = false;
                playerController.StopSwing();
            }
            else if (newState == GrappleState.Grappled) {
                joint.enabled = true;

                grappledPos = grapplePoint.position;

                playerController.StartSwing(grappledPos);
            }
        }

        private void CheckForStopGrapple() {
            if (!Input.GetMouseButton(0)) {
                grappleLine.enabled = false;
                grapplePoint.gameObject.SetActive(false);

                ChangeState(GrappleState.Deactive);
                grappleLine.SetPosition(1, transform.position);

            }
        }
    }
}
