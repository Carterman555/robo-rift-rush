using SpeedPlatformer.Audio;
using System.Collections;
using UnityEngine;

namespace SpeedPlatformer {
	public class TrapSpikeManager : MonoBehaviour {

		[SerializeField] private UpDownMovement spikePrefab;
		[SerializeField] private SpriteRenderer spriteRenderer;

        private void Start() {

            int trapWidth = Mathf.RoundToInt(spriteRenderer.size.x);
            spriteRenderer.size = new Vector2(trapWidth, 1);

            SpawnSpikes(trapWidth);
        }

        [SerializeField] private float moveSpeed;
        private UpDownMovement firstSpike;

        private void SpawnSpikes(int amount) {

            float spikeXPos = -(amount * 0.5f) + 0.5f;

            for (int i = 0; i < amount; i++) {

                UpDownMovement spike = Instantiate(spikePrefab, spriteRenderer.transform, true);

                if (i == 0) {
                    firstSpike = spike;
                }

                spike.transform.localPosition = new Vector3(spikeXPos, 0);
                spike.transform.localRotation = Quaternion.Euler(0, 0, 0);

                // spikes alternate up and down
                bool evenSpike = i % 2 == 0;
                float distance = 0.5f;
                spike.Setup(evenSpike, distance, moveSpeed);

                spikeXPos++;
            }
        }

        #region Sound Effect

        private bool inFrame;
        private static bool isTrapSoundPlaying;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            isTrapSoundPlaying = false;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                inFrame = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                inFrame = false;
            }
        }

        private void Update() {
            if (inFrame && firstSpike.AtExtreme() && !isTrapSoundPlaying) {
                isTrapSoundPlaying = true;
                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.TrapMovement, true, 0.5f);
                StartCoroutine(ResetTrapSound());
            }
        }

        private IEnumerator ResetTrapSound() {
            // Assuming the sound length is known or you have a way to determine when the sound ends.
            yield return new WaitForSeconds(AudioSystem.SoundClips.TrapMovement.length / 2f);
            isTrapSoundPlaying = false;
        }

        #endregion
    }
}