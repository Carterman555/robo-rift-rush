using TarodevController;
using UnityEngine;

namespace SpeedPlatformer
{
    public class Tramp : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision) {
            int playerLayer = 6;
            if (collision.gameObject.layer == playerLayer) {
                collision.gameObject.GetComponent<PlayerController>().HitTramp(transform.up);
            }
        }
    }
}
