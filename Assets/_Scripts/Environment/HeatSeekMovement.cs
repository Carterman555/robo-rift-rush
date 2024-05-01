using SpeedPlatformer.Triggers;
using UnityEngine;

namespace SpeedPlatformer.Environment
{
    public class HeatSeekMovement : MonoBehaviour
    {
        [SerializeField] private TriggerEvent _playerDetector;
        private Transform player;

        private void OnEnable() {
            _playerDetector.OnTriggerEntered += PlayerDetected;
            _playerDetector.OnTriggerExited += PlayerUndetected;
        }

        private void OnDisable() {
            _playerDetector.OnTriggerEntered -= PlayerDetected;
            _playerDetector.OnTriggerExited -= PlayerUndetected;
        }


        private void PlayerDetected(Collider2D collision) {
            int playerLayer = 6;
            if (collision.gameObject.layer == playerLayer) {
                player = collision.transform;
            }
        }

        private void PlayerUndetected(Collider2D collision) {
            int playerLayer = 6;
            if (collision.gameObject.layer == playerLayer) {
                player = null;
            }
        }

        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed;

        private void Update() {
            if (player == null) {
                return;
            }

            // slowly change to face player
            Vector2 playerDirection = player.transform.position - transform.position;
            transform.right = Vector2.MoveTowards(transform.right, playerDirection, rotateSpeed * Time.deltaTime);

            // move in direction facing
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }
    }
}
