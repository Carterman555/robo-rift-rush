using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedPlatformer.Management
{
    public static class GameProgress
    {
        public enum Level {
            _1,
            _2,
            _3,
            _4,
            _5,
            _6,
            _7,
            _8,
            _9,
            _10,
            _11,
        }

        private static Level level;

        public static void Initialize() {
            level = Level._9;
        }

        public static void ResetLevel() {
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
        }

        public static void ContinueNextLevel() {
            switch (level) {
                case Level._1:
                    level = Level._2;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._2:
                    level = Level._3;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._3:
                    level = Level._4;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._4:
                    level = Level._5;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._5:
                    level = Level._6;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._6:
                    level = Level._7;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._7:
                    level = Level._8;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._8:
                    level = Level._9;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._9:
                    level = Level._10;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._10:
                    level = Level._11;
                    SceneManager.LoadScene(GetLevelScene());
                    break;

            }
        }

        //private static string PRECISION_PROTOTYPE = "Precision Level Prototype";
        //private static string SPEED_PROTOTYPE = "Speed Level Prototype 1";
        //private static string SWING_PROTOTYPE = "Swing Level Prototype";

        private static string LEVEL_1 = "Level 1";
        private static string LEVEL_2 = "Level 2";
        private static string LEVEL_3 = "Level 3";
        private static string LEVEL_4 = "Level 4";
        private static string LEVEL_5 = "Level 5";
        private static string LEVEL_6 = "Level 6";
        private static string LEVEL_7 = "Level 7";
        private static string LEVEL_8 = "Level 8";
        private static string LEVEL_9 = "Level 9";
        private static string LEVEL_10 = "Level 10";
        private static string LEVEL_11 = "Level 11";

        private static string GetLevelScene() {
            switch (level) {
                default:
                case Level._1:
                    return LEVEL_1;
                case Level._2:
                    return LEVEL_2;
                case Level._3:
                    return LEVEL_3;
                case Level._4:
                    return LEVEL_4;
                case Level._5:
                    return LEVEL_5;
                case Level._6:
                    return LEVEL_6;
                case Level._7:
                    return LEVEL_7;
                case Level._8:
                    return LEVEL_8;
                case Level._9:
                    return LEVEL_9;
                case Level._10:
                    return LEVEL_10;
                case Level._11:
                    return LEVEL_11;
            }
        }

        public static Level GetLevel() {
            return level;
        }
    }
}
