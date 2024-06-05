using DG.Tweening;
using RoboRiftRush.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoboRiftRush.UI {
    public class ButtonVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        public void OnPointerEnter(PointerEventData eventData) {

            float duration = 0.1f;
            transform.DOScale(1.05f, duration).SetEase(Ease.InOutSine).SetUpdate(true);
            transform.DORotate(new Vector3(0, 0, 3), duration).SetEase(Ease.InOutSine).SetUpdate(true);

            AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.MouseOn, 0f, 0.5f);
        }

        public void OnPointerExit(PointerEventData eventData) {

            float duration = 0.1f;
            transform.DOScale(1f, duration).SetEase(Ease.InOutSine).SetUpdate(true);
            transform.DORotate(Vector3.zero, duration).SetEase(Ease.InOutSine).SetUpdate(true);

            AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.MouseOff, 0f, 0.5f);
        }

        private void OnDisable() {
            transform.DOKill();
        }
    }
}
