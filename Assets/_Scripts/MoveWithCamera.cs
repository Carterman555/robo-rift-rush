using UnityEngine;

namespace RoboRiftRush {
	public class MoveWithCamera : MonoBehaviour {

		[SerializeField] private Vector2 offset;

		private Transform cameraTransform;

        private void Awake() {
			cameraTransform = Camera.main.transform;
        }

        private void Update() {
			transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y) + (Vector3) offset;
		}

	}
}