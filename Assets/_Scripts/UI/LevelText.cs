using RoboRiftRush.Management;
using TMPro;
using UnityEngine;

namespace RoboRiftRush {
	public class LevelText : MonoBehaviour {

		private TextMeshProUGUI text;

        private void Awake() {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Start() {
            text.text = GameProgress.GetLevel().ToString();
        }
    }
}