using RoboRiftRush.Audio;
using RoboRiftRush.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RoboRiftRush {
    public class VolumeSliders : MonoBehaviour {

        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundEffectsSlider;

        private void OnEnable() {
            PopupCanvas.OnObjectActivated += ObjectActivated;
        }

        private void ObjectActivated(string panelName) {
            if (panelName == "Settings") {
                UpdateSliderValues();
            }
        }

        public void UpdateSliderValues() {
            _musicSlider.value = AudioSystem.Instance.GetMusicVolume();
            _soundEffectsSlider.value = AudioSystem.Instance.GetSFXVolume();
        }

        public void ChangeMusicVolume(float volume) {
            AudioSystem.Instance.SetMusicVolume(volume);
        }

        public void ChangeSoundEffectsVolume(float volume) {
            AudioSystem.Instance.SetSFXVolume(volume);
        }
    }
}