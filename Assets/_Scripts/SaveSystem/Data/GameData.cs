using UnityEngine;

namespace Blobber
{
    [System.Serializable]
    public class GameData
    {
        public int HighestLevelCompleted;

        public SerializableSaveDictionary<int, float> FastestTimes;

        public GameData() {
            HighestLevelCompleted = 1;

            FastestTimes = new SerializableSaveDictionary<int, float>();
            float levelCount = 18;
            for (int i = 0; i < levelCount; i++) {
                FastestTimes.Add(i, float.MaxValue);
            }
        }
    }
}