using Blobber;
using RoboRiftRush.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoboRiftRush.Management {
    public static class GameProgress {

        private static int level;
        private static int highestLevelCompleted;

        #region Get Methods

        public static int GetLevel() {
            return level;
        }

        public static int GetHighestLevelCompleted() {
            return highestLevelCompleted;
        }

        private static string GetLevelScene() {
            return "Level " + level.ToString();
        }

        #endregion

        #region Set Methods

        public static void SetLevel(int _level = 1) {
            level = _level;

            if (_level > highestLevelCompleted) {
                highestLevelCompleted = _level;
            }
        }

        public static void SetHighestLevelCompleted(int level) {
            highestLevelCompleted = level;
        }

        public static void ResetLevel() {
            SceneManager.LoadScene(GetLevelScene());
        }

        #endregion

        public static void ContinueNextLevel() {
            SetLevel(level + 1);

            int amountOfLevels = 18;
            if (level > amountOfLevels) {
                WinGame();
                return;
            }

            SceneManager.LoadScene(GetLevelScene());
        }

        private static void WinGame() {
            PopupCanvas.Instance.ActivateUIObject("WinPanel");
            Time.timeScale = 0;
        }
    }
}
