using Blobber;
using RoboRiftRush.Management;
using RoboRiftRush.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoboRiftRush
{
    public class GameManager : StaticInstance<GameManager>, IDataPersistance
    {
        protected override void Awake() {
            base.Awake();

            // this probably shouldn't be on game manager
            if (SceneManager.GetActiveScene().name.TryGetEndingNumber(' ', out int levelNum)) {
                GameProgress.SetLevel(levelNum); 
            }
        }

        public void LoadData(GameData data) {
            GameProgress.SetHighestLevelCompleted(data.HighestLevelCompleted);
        }

        public void SaveData(ref GameData data) {
            data.HighestLevelCompleted = GameProgress.GetHighestLevelCompleted();
        }
    }
}
