using DG.Tweening;
using UnityEngine;

namespace SpeedPlatformer.Environment
{
    public class MovingEnvironment : MonoBehaviour
    {
        [SerializeField] private TriggerEvent startTrigger;

        [SerializeField] private Vector3 moveAmount;
        [SerializeField] private float finalRotation;
        [SerializeField] private float moveSpeed;

        private bool _moved; // moving or moved

        private void OnEnable() {
            startTrigger.OnTriggerEntered += StartMovement;
        }

        private void OnDisable() {
            startTrigger.OnTriggerEntered -= StartMovement;
        }

        [SerializeField] private Ease ease;

        private void StartMovement(Collider2D collision) {
            int playerLayer = 6;
            if (!_moved && collision.gameObject.layer == playerLayer) {
                _moved = true;

                float moveDuration = moveAmount.magnitude / moveSpeed;

                transform.DOMove(transform.position + moveAmount, moveDuration).SetEase(ease);
                transform.DORotate(new Vector3(0, 0, finalRotation), moveDuration).SetEase(ease);
            }
        }

        private Vector3 originalPos;
        private Vector3 originalRot; 
        private void Awake() {
            originalPos = transform.position;
            originalRot = transform.eulerAngles;
        }

        [ContextMenu("Reset Pos")]
        private void ResetPosAndRot() {
            transform.position = originalPos;
            transform.eulerAngles = originalRot;
            transform.DOKill();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + moveAmount);
        }
    }
}
