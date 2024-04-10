using DG.Tweening;
using UnityEngine;

namespace SpeedPlatformer {
    public class BackForthMovement : MonoBehaviour {

        [SerializeField] private Vector2 moveVector;
        [SerializeField] private float speed = 6.0f; // Speed of the movement in units per second

        private void Awake() {
            Vector2 endPos = (Vector2)transform.localPosition + moveVector;

            float distance = moveVector.magnitude;
            float duration = distance / speed;

            transform.DOLocalMove(endPos, duration)
            .SetEase(Ease.InOutSine) // Set the type of easing
            .SetLoops(-1, LoopType.Yoyo); // Set infinite yoyo loops
        }

        private void OnDestroy() {
            transform.DOKill();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + moveVector);
        }
    }
}
