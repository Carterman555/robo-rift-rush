using SpeedPlatformer.Management;
using UnityEngine;

namespace SpeedPlatformer
{
    public class PlayerFallDeath : MonoBehaviour
    {
        [SerializeField] private float killPlayerYPos;

        private void Update() {
            if (transform.position.y < killPlayerYPos) {
                GameProgress.ResetLevel();
            }
        }
    }
}
