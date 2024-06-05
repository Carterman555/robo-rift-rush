using RoboRiftRush.Audio;
using UnityEngine;

namespace RoboRiftRush {
	public class UpDownMovement : MonoBehaviour {

		private bool reversed;
		private float distance;
		private float speed;

		private bool hasReachedExtreme = false;

		// only return true on one frame for every time it's at an extreme
		public bool AtExtreme() {
            bool atExtreme = Mathf.Abs(Mathf.Sin(Time.time * speed)) >= 0.99f;

            if (atExtreme && !hasReachedExtreme) {
                hasReachedExtreme = true;
                return true;
            }

            return false;
        }

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

            bool atExtreme = Mathf.Abs(Mathf.Sin(Time.time * speed)) >= 0.99f;
            if (!atExtreme) {
                hasReachedExtreme = false;
            }
        }
    }
}