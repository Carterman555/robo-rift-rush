using SpeedPlatformer.Audio;
using SpeedPlatformer.Management;
using SpeedPlatformer.Triggers;
using TarodevController;
using UnityEngine;

namespace SpeedPlatformer {
	public class LevelCompleteManager : MonoBehaviour {

		[SerializeField] private TriggerEvent startWinTrigger;
		[SerializeField] private ParticleSystem winParticles;
        [SerializeField] private float direction;

        private void OnEnable() {
            startWinTrigger.OnTriggerEntered += TryStartWinAnimation;
        }
        private void OnDisable() {
            startWinTrigger.OnTriggerEntered -= TryStartWinAnimation;
        }

        private bool startWinAnimation;

        private void TryStartWinAnimation(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer && !startWinAnimation) {
                startWinAnimation = true;

                winParticles.Play();
                FindObjectOfType<PlayerController>().ForceRun((int)Mathf.Sign(direction));

                LevelTransitionFade.Instance.FadeOut();

                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.CompleteLevel, false, 0.75f);
            }
        }

        private void Update() {
             if (startWinAnimation && LevelTransitionFade.Instance.Faded()) {
                GameProgress.ContinueNextLevel();
                startWinAnimation = false;
             }
        }
    }
}