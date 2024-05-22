using TMPro;
using UnityEngine;

namespace SpeedPlatformer {
	public class LevelTimer : StaticInstance<LevelTimer> {

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

            int centisecondsInSecond = 100;
            int secondsInMinute = 60;

            int centiseconds = (int)((time * centisecondsInSecond) % centisecondsInSecond);
            int seconds = (int)((time % secondsInMinute));
            int minutes = (int)(time / secondsInMinute);

            if (minutes == 0) {
                timerText.text = FormatToTwoDigits(seconds) + ":" + FormatToTwoDigits(centiseconds);
            }
            else {
                timerText.text = FormatToTwoDigits(minutes) + ":" + FormatToTwoDigits(seconds) + ":" + FormatToTwoDigits(centiseconds);
            }
        }

        private string FormatToTwoDigits(int num) {
            if (num >= 10) {
                return num.ToString();
            }
            else {
                return "0" + num.ToString();
            }
        }

        public void StopTimer() {
            runTimer = false;
        }
    }
}