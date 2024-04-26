using UnityEngine;

namespace SpeedPlatformer {
	public class FloatingIslandMovementPerlin : MonoBehaviour {

        private Vector2 originalPos;
        private Vector2 noiseOffset;

        void Start() {
            noiseOffset = new Vector2(Random.value * 1000, Random.value * 1000);
            originalPos = transform.position;
        }

        void FixedUpdate() {
            // Calculate new position using Perlin Noise
            float xNoise = Mathf.PerlinNoise(noiseOffset.x, Time.time);
            float yNoise = Mathf.PerlinNoise(noiseOffset.y, Time.time);

            float radius = 1f;
            Vector2 newPosition = new Vector2(xNoise, yNoise) * radius * (2 - radius); // Remap noise values to [-radius, radius] range
            newPosition += originalPos; // Add target point position

            // Clamp position within the radius
            if (Vector2.Distance(newPosition, originalPos) > radius) {
                newPosition = (newPosition - originalPos).normalized * radius + originalPos;
            }

            // Move the kinematic rigid body
            float moveSpeed = 2f;
            GetComponent<Rigidbody2D>().MovePosition(Vector2.Lerp(transform.position, newPosition, moveSpeed * Time.fixedDeltaTime));

            // Increment Perlin Noise offsets
            noiseOffset.x += Time.fixedDeltaTime * 0.1f;
            noiseOffset.y += Time.fixedDeltaTime * 0.1f;
        }

        
    }
}