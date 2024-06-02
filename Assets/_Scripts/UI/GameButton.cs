using UnityEngine;
using UnityEngine.UI;

namespace SpeedPlatformer.UI {
	public class GameButton : MonoBehaviour {

        private Button button;

        private void Awake() {
            button = GetComponent<Button>();
        }

        private void OnEnable() {
            button.onClick.AddListener(OnClicked);
        }

        protected virtual void OnClicked() {

        }
    }
}