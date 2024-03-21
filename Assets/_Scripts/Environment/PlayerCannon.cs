using System.Collections;
using System.Collections.Generic;
using TarodevController;
using Unity.VisualScripting;
using UnityEngine;

namespace SpeedPlatformer.Environment
{
    public class PlayerCannon : MonoBehaviour
    {
        [SerializeField] private TriggerEvent _playerDetector;

        private void OnEnable() {
            _playerDetector.OnTriggerEntered += PlayerDetected;
        }

        private void OnDisable() {
            _playerDetector.OnTriggerEntered -= PlayerDetected;
        }

        private Transform player;
        private float enterCannonTimer = float.MaxValue;

        private void PlayerDetected(Collider2D collision) {
            int playerLayer = 6;
            float enterCannonDelay = 0.5f;

            if (collision.gameObject.layer == playerLayer && enterCannonTimer > enterCannonDelay) {
                player = collision.transform;
                PutPlayerInCannon();
            }
        }

        private float shootTimer = float.MinValue;
        private bool inCannon;

        private void Update() {
            enterCannonTimer += Time.deltaTime;

            shootTimer += Time.deltaTime;
            float timeInCannon = 1.5f;
            if (shootTimer > timeInCannon) {
                ShootPlayer();

            }

            if (inCannon) {
                float rotationSpeed = 75f;
                if (Input.GetKey(KeyCode.A)) {
                    transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
                }
                else if (Input.GetKey(KeyCode.D)) {
                    transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
                }
            }
        }

        private void PutPlayerInCannon() {
            player.gameObject.SetActive(false);
            player.transform.position = transform.position;
            shootTimer = 0;
            inCannon = true;
        }

        private void ShootPlayer() {
            player.gameObject.SetActive(true);
            shootTimer = float.MinValue;
            enterCannonTimer = 0;
            inCannon = false;

            player.GetComponent<PlayerController>().AddCannonForce(transform.right);
        }
    }
}
