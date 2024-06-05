using UnityEngine;

namespace RoboRiftRush.UI {
	public class DeactivateButton : GameButton {

        [SerializeField] private string UIObject;
        [SerializeField] private float delay = 0f;

        [SerializeField] private bool playUISlideAudio = true;

        protected override void OnClicked() {
            base.OnClicked();

            Invoke(nameof(DeactivateObject), delay);
            //DeactivateObject();
        }

        private void DeactivateObject() {
            PopupCanvas.Instance.DeactivateUIObject(UIObject, playUISlideAudio);
        }
    }
}