using UnityEngine;
using UnityEngine.UI;

namespace SpeedPlatformer.UI {
	public class Scroller : MonoBehaviour {

		private RawImage image;

		[SerializeField] private float xSpeed;
		[SerializeField] private float ySpeed;

        private void Awake() {
            image = GetComponent<RawImage>();
        }

        private void Update() {
            image.uvRect = new Rect(image.uvRect.position + new Vector2(xSpeed, ySpeed) * Time.deltaTime, image.uvRect.size);
        }
    }
}