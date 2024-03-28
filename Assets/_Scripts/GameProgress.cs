using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedPlatformer.Management
{
    public static class GameProgress
    {
        public enum Level {
            SPEED_PROTOTYPE,
            SWING_PROTOTYPE,
            PRECISION_PROTOTYPE,
            _1,
            _2,
            _3
        }

        private static Level level;

        public static void Initialize() {
            //level = Level._1;
            level = Level.SWING_PROTOTYPE;
        }

        public static void ResetLevel() {
            SceneManager.LoadScene(GetLevelScene());
        }

        public static void ContinueNextLevel() {
            switch (level) {
                case Level.PRECISION_PROTOTYPE:
                    level = Level.SPEED_PROTOTYPE;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level.SPEED_PROTOTYPE:
                    level = Level.SWING_PROTOTYPE;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level.SWING_PROTOTYPE:
                    level = Level._1;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._1:
                    level = Level._2;
                    SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._2:
                    level = Level._3;
                    //SceneManager.LoadScene(GetLevelScene());
                    break;
                case Level._3:
                    //level = Level._2;
                    //SceneManager.LoadScene(GetLevelScene());
                    break;
            }
        }

        private static string PRECISION_PROTOTYPE = "Precision Level Prototype";
        private static string SPEED_PROTOTYPE = "Speed Level Prototype 1";
        private static string SWING_PROTOTYPE = "Swing Level Prototype";

        private static string LEVEL_1 = "Level 1";
        private static string LEVEL_2 = "Level 2";
        private static string LEVEL_3 = "Level 3";

        private static string GetLevelScene() {
            switch (level) {
                default:
                case Level.PRECISION_PROTOTYPE:
                    return PRECISION_PROTOTYPE;
                case Level.SPEED_PROTOTYPE:
                    return SPEED_PROTOTYPE;
                case Level.SWING_PROTOTYPE:
                    return SWING_PROTOTYPE;
                case Level._1:
                    return LEVEL_1;
                case Level._2:
                    return LEVEL_2;
                case Level._3:
                    return LEVEL_3;
            }
        }

        public static Level GetLevel() {
            return level;
        }
    }
}
