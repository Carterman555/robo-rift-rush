using DG.Tweening;
using SpeedPlatformer.Audio;
using System.Collections;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace SpeedPlatformer.Environment {
    public class BackForthMovement : MonoBehaviour {

        [SerializeField] private Vector2 moveVector;
        [SerializeField] private float speed = 6.0f; // Speed of the movement in units per second
        [SerializeField] private AudioClip moveClip; // Audio clip to play
        [SerializeField] private AudioClip switchDirectionClip; // Audio clip to play

        private void Awake() {
            Vector2 endPos = (Vector2)transform.localPosition + moveVector;

            float distance = moveVector.magnitude;
            float duration = distance / speed;

            transform.DOLocalMove(endPos, duration)
            .SetEase(Ease.InOutSine) // Set the type of easing
            .SetLoops(-1, LoopType.Yoyo) // Set infinite yoyo loops
            .OnStepComplete(PlaySwitchSound); // Add callback to play sound
        }

        private void OnDestroy() {
            transform.DOKill();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + moveVector);
        }

        #region Sound Effects

        private void PlaySwitchSound() {
            if (switchDirectionClip != null) {
                //AudioSystem.Instance.PlaySound(switchDirectionClip, true, 0.5f);
            }
        }
        
        private bool inFrame;
        private static bool isTrapSoundPlaying;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            isTrapSoundPlaying = false;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                inFrame = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                inFrame = false;
            }
        }

        private void Update() {
            if (inFrame && !isTrapSoundPlaying) {
                isTrapSoundPlaying = true;
                AudioSystem.Instance.PlaySound(moveClip, 0.25f, 0.5f);
                StartCoroutine(ResetTrapSound());
            }
        }

        private IEnumerator ResetTrapSound() {
            // Assuming the sound length is known or you have a way to determine when the sound ends.
            yield return new WaitForSeconds(moveClip.length);
            isTrapSoundPlaying = false;
        }

        #endregion
    }
}
