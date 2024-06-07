using Blobber;
using RoboRiftRush.Audio;
using RoboRiftRush.Management;
using RoboRiftRush.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoboRiftRush {
	public class LevelButton : GameButton, IDataPersistance {

        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Image image;
        private int levelNum;

        private bool levelLocked;

        private float fastestTime;

        public void SetLevel(int num) {
            levelNum = num;
            levelText.text = levelNum.ToString();

            if (fastestTime != 0) {
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


        public void LoadData(GameData data) {
            int levelIndex = levelNum - 1;
            fastestTime = data.FastestTimes[levelIndex];
        }

        public void SaveData(ref GameData data) {
        }
    }
}