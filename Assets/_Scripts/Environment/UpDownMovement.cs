using UnityEngine;

namespace SpeedPlatformer {
	public class UpDownMovement : MonoBehaviour {

		private bool reversed;
		private float distance;
		private float speed;

		public void Setup(bool reversed, float distance, float speed) {
            this.reversed = reversed;
			this.distance = distance;
			this.speed = speed;
		}

        private void Update() {
			
			// yPos ranges from 0 to distance
			float radius = distance * 0.5f;
            float yPos = (Mathf.Sin(Time.time * speed) * radius) + radius;

            if (reversed) {
				yPos = distance - yPos;
            }

			transform.localPosition = new Vector3(transform.localPosition.x, yPos);
        }
    }
}