using UnityEngine;

namespace RoboRiftRush.UI {
	public class QuitButton : GameButton {

        protected override void OnClicked() {
            base.OnClicked();
            Application.Quit();
        }
    }
}