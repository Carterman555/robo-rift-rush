using UnityEngine;

namespace SpeedPlatformer.Environment
{
    public class KillPlayerOnContact : MonoBehaviour
    {        
        private void OnTriggerEnter2D(Collider2D collision) {
            int playerLayer = 6;
            if (collision.gameObject.layer == playerLayer) {
                collision.gameObject.GetComponent<PlayerDeath>().Kill();
            }
        }
    }
}
