using RoboRiftRush.Management;
using RoboRiftRush.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoboRiftRush {
	public class LevelButton : GameButton {

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        private int levelNum;

        private bool levelLocked;

        public void SetLevel(int num) {
            levelNum = num;
            text.text = levelNum.ToString();

            levelLocked = levelNum > GameProgress.GetHighestLevelUnlocked();
            if (levelLocked) {
                image.color = Color.gray;
            }
        }

        protected override void OnClicked() {

            if (levelLocked) {
                return;
            }

            base.OnClicked();
            GameProgress.SetLevel(levelNum);
            GameProgress.ResetLevel();
        }
    }
}