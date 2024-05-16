using UnityEngine;

namespace SpeedPlatformer {
	public class MovingTrapVisual : MonoBehaviour {

		[SerializeField] private TrailRenderer leftTrailRenderer;
		[SerializeField] private TrailRenderer rightTrailRenderer;

        private SpriteRenderer spriteRenderer;

		private float oldXPosition;
        private bool movingLeft;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();

            oldXPosition = transform.position.x;
        }

        private void Start() {
            // set starting positions at ends of trap
            float toEndMult = 0.45f;
            leftTrailRenderer.transform.localPosition = new Vector3(-spriteRenderer.size.x * toEndMult, leftTrailRenderer.transform.localPosition.y);
            rightTrailRenderer.transform.localPosition = new Vector3(spriteRenderer.size.x * toEndMult, leftTrailRenderer.transform.localPosition.y);
        }

        private void Update() {
            float xVel = transform.position.x - oldXPosition;

            if (xVel > 0 && movingLeft) {
                movingLeft = false;

                leftTrailRenderer.emitting = true;
                rightTrailRenderer.emitting = false;
            }
            else if (xVel < 0 && !movingLeft) {
                movingLeft = true;

                leftTrailRenderer.emitting = false;
                rightTrailRenderer.emitting = true;
            }

            oldXPosition = transform.position.x;
        }
    }
}