using Blobber;
using RoboRiftRush.Audio;
using RoboRiftRush.Management;
using RoboRiftRush.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoboRiftRush {
	public class LevelButton : GameButton {

        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Image image;
        private int levelNum;

        private bool levelLocked;

        public void SetLevel(int num) {
            levelNum = num;
            levelText.text = levelNum.ToString();

            int levelIndex = levelNum - 1;

            float fastestTime = DataPersistenceManager.Instance.GetGameData().FastestTimes[levelIndex];
            if (fastestTime != float.MaxValue) {
                timeText.text = "Time: " + fastestTime.ToTimerFormat();
            }
            else {
                timeText.text = "Time: N/A";
            }

            levelLocked = levelNum > GameProgress.GetHighestLevelCompleted();
            if (levelLocked) {
                image.color = Color.gray;
            }
        }

        protected override void OnClicked() {

            if (levelLocked) {
                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.ClickLocked, 0f, 0.5f);
                return;
            }

            base.OnClicked();
            GameProgress.SetLevel(levelNum);
            GameProgress.ResetLevel();
        }
    }
}