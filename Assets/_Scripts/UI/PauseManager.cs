using UnityEngine;
using RoboRiftRush.UI;

namespace RoboRiftRush.Management {
    public class PauseManager : Singleton<PauseManager> {

        public static bool Paused { get; private set; }

        // to reset the bool when domain is not reloaded
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            Paused = false;
        }

        /// <summary>
        /// if the player presses escape and the settings is open, switch back to pause panel
        /// if the player presses escape and the settings is not open, toggle the pause
        /// </summary>
        private void Update() {

            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (PopupCanvas.Instance.GetPopupActive("SettingsPanel")) {
                    PopupCanvas.Instance.ActivateUIObject("PausePanel");
                    PopupCanvas.Instance.DeactivateUIObject("SettingsPanel");
                }
                else {
                    TogglePause();
                }
            }
        }

        public void TogglePause() {
            Paused = !Paused;
            if (Paused) {
                Pause();
            }
            else {
                Unpause();
            }
        }

        public void Pause() {
            Time.timeScale = 0;
            PopupCanvas.Instance.ActivateUIObject("PausePanel");
        }

        public void Unpause() {
            Time.timeScale = 1;
            PopupCanvas.Instance.DeactivateUIObject("PausePanel");
        }
    }
}