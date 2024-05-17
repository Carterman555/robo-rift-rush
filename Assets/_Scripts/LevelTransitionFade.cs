using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SpeedPlatformer {
	public class LevelTransitionFade : StaticInstance<LevelTransitionFade> {

		private SpriteRenderer speedFade;

        float fadeDuration = 2.5f;

        protected override void Awake() {
            base.Awake();
            speedFade = GetComponent<SpriteRenderer>();
        }

        private void Start() {
            FadeIn();
        }

        public void FadeIn() {
            speedFade.Fade(1f);
            speedFade.DOFade(0f, fadeDuration);
        }

        public void FadeOut() {
            speedFade.Fade(0f);
            speedFade.DOFade(1f, fadeDuration);
        }

        private void OnDestroy() {
            speedFade.DOKill();
        }
    }
}