using UnityEngine;

namespace SpeedPlatformer.UI {
    public class ActivateButton : GameButton {

        [SerializeField] private string UIObject;
        [SerializeField] private float delay = 0f;

        protected override void OnClicked() {
            base.OnClicked();

            Invoke(nameof(ActivateObject), delay);
        }

        private void ActivateObject() {
            PopupCanvas.Instance.ActivateUIObject(UIObject);
        }
    }
}