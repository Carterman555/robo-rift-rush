using DG.Tweening;
using UnityEngine;

namespace SpeedPlatformer {
	public class LevelTransitionFade : StaticInstance<LevelTransitionFade> {

		private SpriteRenderer speedFade;

        public bool Faded() {
            return speedFade.color.a == 1f;
        }

        protected override void Awake() {
            base.Awake();
            speedFade = GetComponent<SpriteRenderer>();
        }

        private void Start() {
            FadeIn();
        }

        public void FadeIn(float fadeDuration = 1.25f) {
            speedFade.DOKill();

            speedFade.Fade(1f);
            speedFade.DOFade(0f, fadeDuration);
        }

        public void FadeOut(float fadeDuration = 1.25f) {
            speedFade.DOKill();

            speedFade.Fade(0f);
            speedFade.DOFade(1f, fadeDuration);
        }

        private void OnDestroy() {
            speedFade.DOKill();
        }
    }
}