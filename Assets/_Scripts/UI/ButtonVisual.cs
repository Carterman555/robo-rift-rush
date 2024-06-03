using DG.Tweening;
using SpeedPlatformer.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpeedPlatformer.UI {
    public class ButtonVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public void OnPointerEnter(PointerEventData eventData) {

            float duration = 0.1f;
            transform.DOScale(1.05f, duration).SetEase(Ease.InOutSine);
            transform.DORotate(new Vector3(0, 0, 3), duration).SetEase(Ease.InOutSine);

            AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.MouseOn, 0f, 0.5f);
        }
        public void OnPointerExit(PointerEventData eventData) {

            float duration = 0.1f;
            transform.DOScale(1f, duration).SetEase(Ease.InOutSine);
            transform.DORotate(Vector3.zero, duration).SetEase(Ease.InOutSine);

            AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.MouseOff, 0f, 0.5f);
        }
    }
}
