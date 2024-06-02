using UnityEngine;

namespace SpeedPlatformer.UI {
	public class DeactivateButton : GameButton {

        [SerializeField] private string UIObject;
        [SerializeField] private float delay = 0f;

        protected override void OnClicked() {
            base.OnClicked();

            Invoke(nameof(DeactivateObject), delay);
        }

        private void DeactivateObject() {
            PopupCanvas.Instance.DeactivateUIObject(UIObject);
        }
    }
}