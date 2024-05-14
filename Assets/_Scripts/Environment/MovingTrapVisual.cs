using UnityEngine;

namespace SpeedPlatformer {
	public class MovingTrapVisual : MonoBehaviour {

		[SerializeField] private TrailRenderer leftTrailRenderer;
		[SerializeField] private TrailRenderer rightTrailRenderer;

		private float oldXPosition;
        private bool movingLeft;

        private void Update() {
            float xVel = transform.position.x - oldXPosition;

            if (xVel > 0 && movingLeft) {
                movingLeft = false;

                leftTrailRenderer.enabled = true;
                rightTrailRenderer.enabled = false;
            }
            else if (xVel < 0 && !movingLeft) {
                movingLeft = true;

                leftTrailRenderer.enabled = false;
                rightTrailRenderer.enabled = true;
            }
        }
    }
}