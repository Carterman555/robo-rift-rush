using SpeedPlatformer.Audio;
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
        private void OnDisable() {
            button.onClick.RemoveListener(OnClicked);
        }

        [SerializeField] private bool playClickAudio = true;

        protected virtual void OnClicked() {
            if (playClickAudio) {
                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.ButtonClick, 0f, 1f);
            }
        }
    }
}