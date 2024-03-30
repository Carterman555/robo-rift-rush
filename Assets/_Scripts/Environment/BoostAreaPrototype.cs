using TarodevController;
using UnityEngine;

namespace SpeedPlatformer {
    public class BoostAreaPrototype : MonoBehaviour {

        [SerializeField] private float boostAmount;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.TryGetComponent(out PlayerController playerController)) {
                playerController.Boost(transform.right * boostAmount);
            }
        }
    }
}
