using RoboRiftRush.Audio;
using RoboRiftRush.Management;
using UnityEngine;

namespace RoboRiftRush
{
    public class PlayerFallDeath : MonoBehaviour
    {
        [SerializeField] private float killPlayerYPos;

        private bool startFade;

        private void Awake() {
            startFade = false;
        }

        private void Update() {
            if (!startFade && transform.position.y < killPlayerYPos) {
                startFade = true;

                float fadeDuration = 1f;
                LevelTransitionFade.Instance.FadeOut(fadeDuration);

                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.FallVoid, 0);
            }

            if (startFade && LevelTransitionFade.Instance.Faded()) {
                GameProgress.ResetLevel();
            }
        }
    }
}
