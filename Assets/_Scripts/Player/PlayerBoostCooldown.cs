using TarodevController;
using UnityEngine;

namespace SpeedPlatformer.Player {
	[RequireComponent(typeof(PlayerController))]
	public class PlayerBoostCooldown : MonoBehaviour {

		private PlayerController playerController;

        private float boostPower = 1f;

        [SerializeField] private float boostDrain = 0f;
        [SerializeField] private float boostRegen = 0.3f;

        public float GetBoostPower() {
            return boostPower;
        }

        private void Awake() {
            playerController = GetComponent<PlayerController>();
        }

        private void Update() {
            if (playerController.BoostInput) {
                boostPower = Mathf.MoveTowards(boostPower, 0, boostDrain * Time.deltaTime);
            }
            else {
                boostPower = Mathf.MoveTowards(boostPower, 1f, boostRegen * Time.deltaTime);
            }

            playerController.SetCanBoost(boostPower != 0f);
        }
    }
}