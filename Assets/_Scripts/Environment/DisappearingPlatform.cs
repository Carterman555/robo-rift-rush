using Unity.VisualScripting;
using UnityEngine;

namespace SpeedPlatformer {
	public class DisappearingPlatform : MonoBehaviour {

		[SerializeField] private SpriteRenderer visual;

		private float disappearTimer;

		private bool disappearing = false;
		

		private void OnCollisionEnter2D(Collision2D collision) {
			if (collision.gameObject.layer == GameLayers.PlayerLayer) {
				disappearing = true;
			}
		}

		private void Update() {
			if (disappearing) {
				float disappearTime = 1f;
				disappearTimer += Time.deltaTime;

				Color color = visual.color;
				color.a = 1f - (disappearTimer / disappearTime);
				visual.color = color;

				if (disappearTimer > disappearTime) {
					Destroy(gameObject);
				}
			}
 			
		}

	}
}