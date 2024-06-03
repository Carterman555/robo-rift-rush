using UnityEngine;

namespace SpeedPlatformer.UI {
    public class PausePanel : Singleton<PausePanel> {

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
                Time.timeScale = 0;
                PopupCanvas.Instance.ActivateUIObject("PausePanel");
            }
            else {
                Time.timeScale = 1;
                PopupCanvas.Instance.DeactivateUIObject("PausePanel");
            }
        }
    }
}