using DG.Tweening;
using UnityEngine;

namespace SpeedPlatformer {
	public class TrapSpikeManager : MonoBehaviour {

		[SerializeField] private UpDownMovement spikePrefab;

		private SpriteRenderer spriteRenderer;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start() {

            int trapWidth = Mathf.RoundToInt(spriteRenderer.size.x);
            spriteRenderer.size = new Vector2(trapWidth, 1);

            SpawnSpikes(trapWidth);
        }

        [SerializeField] private float moveSpeed;

        private void SpawnSpikes(int amount) {

            float spikeXPos = -(amount * 0.5f) + 0.5f;

            for (int i = 0; i < amount; i++) {

                UpDownMovement spike = Instantiate(spikePrefab, transform, true);

                spike.transform.localPosition = new Vector3(spikeXPos, 0);
                spike.transform.localRotation = Quaternion.Euler(0, 0, 0);

                // spikes alternate up and down
                bool evenSpike = i % 2 == 0;
                float distance = 0.5f;
                spike.Setup(evenSpike, distance, moveSpeed);

                spikeXPos++;
            }
        }
    }
}