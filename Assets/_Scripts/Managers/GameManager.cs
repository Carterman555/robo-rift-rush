using RoboRiftRush.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoboRiftRush
{
    public class GameManager : StaticInstance<GameManager>
    {
        protected override void Awake() {
            base.Awake();

            // this probably shouldn't be on game manager
            if (SceneManager.GetActiveScene().name.TryGetEndingNumber(' ', out int levelNum)) {
                GameProgress.SetLevel(levelNum); 
            }
        }
    }
}
