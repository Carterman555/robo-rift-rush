using System.Collections;
using UnityEngine;

namespace RoboRiftRush.UI {
    public class ActivateButton : GameButton {

        [SerializeField] private string UIObject;
        [SerializeField] private float delay = 0f;

        [SerializeField] private bool playUISlideAudio = true;

        protected override void OnClicked() {
            base.OnClicked();

            StartCoroutine(ActivateObject());
        }

        private IEnumerator ActivateObject() {
            yield return new WaitForSecondsRealtime(delay);
            PopupCanvas.Instance.ActivateUIObject(UIObject, playUISlideAudio);
        }
    }
}