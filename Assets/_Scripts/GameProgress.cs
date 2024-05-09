using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedPlatformer.Management
{
    public static class GameProgress
    {
        private static int level;

        public static void Initialize(int startingLevel = 1) {
            level = startingLevel;
        }

        public static void ResetLevel() {
            SceneManager.LoadScene(GetLevelScene());
        }

        public static void ContinueNextLevel() {
            level++;
            SceneManager.LoadScene(GetLevelScene());
        }

        private static string GetLevelScene() {
            return "Level " + level.ToString();
        }

        public static int GetLevel() {
            return level;
        }
    }
}
