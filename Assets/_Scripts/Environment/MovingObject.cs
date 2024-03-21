using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

namespace SpeedPlatformer.Environment
{
    public class MovingObject : MonoBehaviour
    {
        [SerializeField] private Vector3 moveAmount;

        [SerializeField] private float speed = 4.0f; // Speed of the movement in units per second

        private void Start()
        {
            float totalDistance = moveAmount.magnitude;
            float totalDuration = totalDistance / speed;

            Vector3 pos1 = transform.position;
            Vector3 pos2 = transform.position + moveAmount;

            // loop movement between pos1 and pos2
            transform.DOMove(pos2, totalDuration / 2).SetEase(Ease.InOutSine)
                    .OnComplete(() => transform.DOMove(pos1, totalDuration / 2).SetEase(Ease.InOutSine))
                    .SetLoops(-1, LoopType.Yoyo); // Infinite loops, Yoyo style (back and forth)
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + moveAmount);
        }
    }
}
