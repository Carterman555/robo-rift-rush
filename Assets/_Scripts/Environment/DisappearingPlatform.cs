using SpeedPlatformer.Audio;
using UnityEngine;

namespace SpeedPlatformer {
	public class DisappearingPlatform : MonoBehaviour {

		[SerializeField] private SpriteRenderer visual;

		private float disappearTimer;

		private bool disappearing = false;

		private float startingAlpha;

        private void Awake() {
            startingAlpha = visual.color.a;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
			if (collision.gameObject.layer == GameLayers.PlayerLayer && !disappearing) {
				disappearing = true;

                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.PlatformDisappear, .1f, 1f);
            }
        }

		private void Update() {
			if (disappearing) {
				float disappearTime = 1f; // alpha starts at 170, so really takes .666 seconds to disappear
				disappearTimer += Time.deltaTime;

				Color color = visual.color;
				color.a = startingAlpha - (disappearTimer / disappearTime);
				visual.color = color;

				if (color.a <= 0) {
					Destroy(gameObject);
				}
			}
		}
	}
}