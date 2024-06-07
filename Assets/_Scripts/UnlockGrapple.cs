using DG.Tweening;
using RoboRiftRush.Audio;
using RoboRiftRush.Player;
using UnityEngine;

namespace RoboRiftRush {
    public class UnlockGrapple : MonoBehaviour {

        private void Awake() {
            if (FindObjectOfType<PlayerGrapple>().IsUnlocked()) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.TryGetComponent(out PlayerGrapple playerGrapple)) {
                playerGrapple.Unlock();

                float duration = 0.5f;
                transform.DOScale(0, duration).SetEase(Ease.InSine).OnComplete(() => {
                    Destroy(gameObject);
                });

                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.GrapplePickup);
            }
        }
    }
}
