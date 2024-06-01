using SpeedPlatformer.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedPlatformer {
	public class PlayButton : GameButton {

        protected override void OnClicked() {
            base.OnClicked();
            GameProgress.Initialize();
            GameProgress.ResetLevel();
        }

    }
}