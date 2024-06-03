using SpeedPlatformer.Management;
using SpeedPlatformer.UI;
using TMPro;
using UnityEngine;

namespace SpeedPlatformer {
	public class LevelButton : GameButton {

        [SerializeField] private TextMeshProUGUI text;
        private int levelNum;

        public void SetLevel(int num) {
            levelNum = num;
            text.text = levelNum.ToString();
        }

        protected override void OnClicked() {
            base.OnClicked();
            GameProgress.Initialize(levelNum);
            GameProgress.ResetLevel();
        }
    }
}