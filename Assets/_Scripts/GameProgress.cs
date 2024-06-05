using RoboRiftRush.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

namespace RoboRiftRush.Management {
    public static class GameProgress {

        private static int level;
        private static int levelHighestUnlocked;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            levelHighestUnlocked = 1;
        }

        public static void SetLevel(int _level = 1) {
            level = _level;

            if (_level > levelHighestUnlocked) {
                levelHighestUnlocked = _level;
            }
        }

        public static void ResetLevel() {
            SceneManager.LoadScene(GetLevelScene());
        }

        public static void ContinueNextLevel() {
            level++;

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

        private static string GetLevelScene() {
            return "Level " + level.ToString();
        }

        public static int GetLevel() {
            return level;
        }

        public static int GetHighestLevelUnlocked() {
            return levelHighestUnlocked;
        }
    }
}
