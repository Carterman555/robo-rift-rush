using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace SpeedPlatformer.Environment
{
    public class MovingEnvironment : MonoBehaviour
    {
        [SerializeField] private TriggerEvent startTrigger;
        private bool moving = false;

        [SerializeField] private Vector3 moveAmount;
        [SerializeField] private float finalRotation;
        [SerializeField] private float moveDuration;

        private void OnEnable() {
            startTrigger.OnTriggerEntered += StartMovement;
        }

        private void OnDisable() {
            startTrigger.OnTriggerEntered -= StartMovement;
        }

        [SerializeField] private Ease ease;

        private void StartMovement(Collider2D collision) {
            int playerLayer = 6;
            if (collision.gameObject.layer == playerLayer) {
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
