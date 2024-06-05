using System;
using UnityEngine;

namespace RoboRiftRush.Environment
{
    public class DeathFogPrototype : MonoBehaviour
    {
        public event Action OnReachedPoint;
        private bool reachedPoint;

        private float maxLength;
        private float speed;

        public void Setup(Vector3 endPos, float width, float speed) {

            Vector3 startPos = transform.position;

            //... rotate to face endpoint
            transform.up = endPos - startPos;

            transform.localScale = new Vector3(width, 0);

            reachedPoint = false;
            maxLength = Vector3.Distance(startPos, endPos);
            this.speed = speed;
        }

        private void FixedUpdate() {

            if (reachedPoint) {
                return;
            }

            //... move at half the speed so starting side stays in same position
            transform.position += transform.up * speed * Time.fixedDeltaTime * 0.5f;

            float newLength = transform.localScale.y + speed * Time.fixedDeltaTime;
            transform.localScale = new Vector3(transform.localScale.x, newLength);

            // if reached endpoint, invoke action to spawn more fog
            if (newLength > maxLength) {
                reachedPoint = true;
                OnReachedPoint?.Invoke();
            }
        }
    }
}
