using Blobber;
using RoboRiftRush.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace RoboRiftRush.Environment
{
    public class BallTrapLauncher : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D ballTrapPrefab;
        [SerializeField] private Transform spawnPoint;

        [SerializeField] private float shootForce;
        [SerializeField] private float shootDelay;
        private float shootTimer = 0;

        private void Update() {
            shootTimer += Time.deltaTime;
            if (shootTimer > shootDelay) {
                shootTimer = 0;
                ShootBall();
            }
        }
        
        private void ShootBall() {
            Rigidbody2D ballTrap = ObjectPoolManager.SpawnObject(ballTrapPrefab, spawnPoint.position, Quaternion.identity, Parents.GetParent("Traps"));

            ballTrap.AddForce(shootForce * transform.up); 
        }
    }
}
