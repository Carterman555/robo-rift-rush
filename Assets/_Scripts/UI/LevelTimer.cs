using Blobber;
using RoboRiftRush.Management;
using TMPro;
using UnityEngine;

namespace RoboRiftRush.UI {
	public class LevelTimer : StaticInstance<LevelTimer>, IDataPersistance {

        private TextMeshProUGUI timerText;
        private float time;
        private bool runTimer;

        protected override void Awake() {
            base.Awake();

            timerText = GetComponent<TextMeshProUGUI>();
            runTimer = true;
        }

        private void Update() {

            if (!runTimer) return;

            time += Time.deltaTime;

            timerText.text = time.ToTimerFormat();
        }

        private static SerializableSaveDictionary<int, float> fastestTimes;

        public void StopAndSaveTime() {
            runTimer = false;

            int levelIndex = GameProgress.GetLevel() - 1;

            print("Save");
            if (time < fastestTimes[levelIndex]) {
                fastestTimes[levelIndex] = time;
            }
        }

        public void LoadData(GameData data) {
            print("Load");
            fastestTimes = data.FastestTimes;
        }

        public void SaveData(ref GameData data) {
            data.FastestTimes = fastestTimes;
        }
    }
}