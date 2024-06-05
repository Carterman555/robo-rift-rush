using RoboRiftRush.UI;
using RoboRiftRush.Management;
using UnityEngine.SceneManagement;

namespace RoboRiftRush {
	public class MenuButton : GameButton {

        protected override void OnClicked() {
            base.OnClicked();
            PauseManager.Instance.Unpause();
            SceneManager.LoadScene("Menu");
        }
    }
}