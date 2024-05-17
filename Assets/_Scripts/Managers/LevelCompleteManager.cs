using SpeedPlatformer.Triggers;
using System;
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

        private void TryStartWinAnimation(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer) {
                winParticles.Play();
                FindObjectOfType<PlayerController>().ForceRun((int)Mathf.Sign(direction));
            }
        }
    }
}