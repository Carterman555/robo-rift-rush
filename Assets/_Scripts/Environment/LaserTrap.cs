using SpeedPlatformer.Management;
using UnityEngine;

namespace SpeedPlatformer.Environment
{
    public class LaserTrap : MonoBehaviour
    {
        [SerializeField] private LineRenderer laserLineRenderer;
        [SerializeField] private LayerMask obstacleLayerMask;

        [SerializeField] private float laserSpeed;
        private bool laserActive = true;

        private float laserLength;

        private void Start() {
            laserLineRenderer.positionCount = 2;
            laserLineRenderer.SetPosition(0, (Vector2)transform.position);
            laserLineRenderer.SetPosition(1, (Vector2)transform.position);

            DeactivateLaser();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                ActivateLaser();
            }
        }

        private void Update(){

            if (!laserActive) {
                return;
            }

            float laserRange = 100f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, laserLength, obstacleLayerMask);

            // if hit something, make laser stop at that point
            if (hit.collider != null) {
                laserLength = hit.distance;

                if (hit.collider.gameObject.layer == GameLayers.PlayerLayer) {
                    GameProgress.ResetLevel();
                }
            }

            // if not hitting obstacle, grow laser
            else {
                laserLength = Mathf.MoveTowards(laserLength, laserRange, laserSpeed * Time.deltaTime);
            }

            laserLineRenderer.SetPosition(0, (Vector2)transform.position);

            Vector2 endPoint = transform.position + (laserLength * -transform.right);
            laserLineRenderer.SetPosition(1, endPoint);
        }

        private void ActivateLaser() {
            laserActive = true;
            laserLineRenderer.enabled = true;
        }

        private void DeactivateLaser() {
            laserActive = false;
            laserLineRenderer.enabled = false;
        }
    }
}
