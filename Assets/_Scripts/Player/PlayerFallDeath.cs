using SpeedPlatformer.Management;
using UnityEngine;

namespace SpeedPlatformer
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
            }

            if (startFade && LevelTransitionFade.Instance.Faded()) {
                GameProgress.ResetLevel();
            }
        }
    }
}
