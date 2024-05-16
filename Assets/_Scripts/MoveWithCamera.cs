using UnityEngine;

namespace SpeedPlatformer {
	public class MoveWithCamera : MonoBehaviour {

		[SerializeField] private Vector2 offset;

		private Transform cameraTransform;

		private void Update() {
			transform.position = cameraTransform.position + (Vector3) offset;
		}

	}
}