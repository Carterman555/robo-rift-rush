using SpeedPlatformer.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedPlatformer
{
    public class GameManager : StaticInstance<GameManager>
    {
        protected override void Awake() {
            base.Awake();

            GameProgress.Initialize(SceneManager.GetActiveScene().name.TryGetEndingNumber(' ')); // this probably shouldn't be on game manager
        }
    }
}
