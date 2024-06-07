using System.Collections;
using UnityEngine;

namespace RoboRiftRush.UI {
	public class DeactivateButton : GameButton {

        [SerializeField] private string UIObject;
        [SerializeField] private float delay = 0f;

        [SerializeField] private bool playUISlideAudio = true;

        protected override void OnClicked() {
            base.OnClicked();

            StartCoroutine(DeactivateObject());
        }

        private IEnumerator DeactivateObject() {
            yield return new WaitForSecondsRealtime(delay);
            PopupCanvas.Instance.DeactivateUIObject(UIObject, playUISlideAudio);
        }
    }
}