using UnityEngine;

namespace SpeedPlatformer.Environment
{
    public class LaserTrap : MonoBehaviour
    {
        [SerializeField] private LineRenderer laserLineRenderer;
        [SerializeField] private LayerMask obstacleLayerMask;

        [SerializeField] private float laserActiveTime;
        [SerializeField] private float laserDeactiveTime;
        private float laserActiveTimer = 0;
        private bool laserActive = true;

        private void Start() {
            laserLineRenderer.positionCount = 2;
        }

        private void Update(){
            laserActiveTimer += Time.deltaTime;

            if (laserActive) {
                if (laserActiveTimer > laserDeactiveTime) {
                    laserActiveTimer = 0;
                    DeactivateLaser();
                }
            }
            else {
                if (laserActiveTimer > laserActiveTime) {
                    laserActiveTimer = 0;
                    ActivateLaser();
                }
            }

            if (!laserActive) {
                return;
            }

            laserLineRenderer.SetPosition(0, (Vector2)transform.position);

            float laserRange = 100f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, laserRange, obstacleLayerMask);

            // if hit something, make laser stop at that point
            if (hit.collider != null) {
                laserLineRenderer.SetPosition(1, hit.point);

                int playerLayer = 6;
                if (hit.collider.gameObject.layer == playerLayer) {
                    PlayerDeath.Instance.Kill();
                }
            }

            // if didn't hit something, the laser will go as long as the laser range is
            else {
                Vector2 endPoint = transform.position + (laserRange * -transform.right);
                laserLineRenderer.SetPosition(1, endPoint);
            }
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
