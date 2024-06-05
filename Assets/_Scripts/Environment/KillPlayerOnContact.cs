using TarodevController;
using UnityEngine;

namespace RoboRiftRush.Environment
{
    public class KillPlayerOnContact : MonoBehaviour
    {        
        private void OnTriggerEnter2D(Collider2D collision) {
            int playerLayer = 6;
            if (collision.gameObject.layer == playerLayer && !PlayerAnimator.Instance.IsFading()) {
                PlayerAnimator.Instance.StartDeathFade();
            }
        }
    }
}
