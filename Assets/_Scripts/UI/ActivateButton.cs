using UnityEngine;

namespace SpeedPlatformer.UI {
    public class ActivateButton : GameButton {

        [SerializeField] private string UIObject;
        [SerializeField] private float delay = 0f;

        [SerializeField] private bool playUISlideAudio = true;

        protected override void OnClicked() {
            base.OnClicked();

            //Invoke(nameof(ActivateObject), delay);
            ActivateObject();
        }

        private void ActivateObject() {
            PopupCanvas.Instance.ActivateUIObject(UIObject, playUISlideAudio);
        }
    }
}