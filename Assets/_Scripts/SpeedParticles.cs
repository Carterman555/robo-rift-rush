using TarodevController;
using UnityEngine;

namespace SpeedPlatformer {
    public class SpeedParticles : MonoBehaviour {

        [SerializeField] private int rateMult = 50;
        private ParticleSystem particles;
        private PlayerController playerController;

        private void Awake() {
            particles = GetComponent<ParticleSystem>();
            playerController = FindObjectOfType<PlayerController>();
        }

        private void Update() {
            var emission = particles.emission;
            float playerSpeed = Mathf.Abs(playerController.FrameVelocity.x);
            emission.rateOverTime = Mathf.RoundToInt(playerSpeed * rateMult);
        }

    }
}
