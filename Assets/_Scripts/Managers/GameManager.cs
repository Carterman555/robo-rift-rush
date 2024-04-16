using SpeedPlatformer.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedPlatformer
{
    public class GameManager : StaticInstance<GameManager>
    {
        protected override void Awake() {
            base.Awake();

            GameProgress.Initialize(TryGetEndingNumber(SceneManager.GetActiveScene().name)); // this probably shouldn't be on game manager
        }

        // this method probably shouldn't be on game manager
        private int TryGetEndingNumber(string objectName) {
            int endingNumber = 0;
            int lastUnderscoreIndex = objectName.LastIndexOf(' ');
            if (lastUnderscoreIndex != -1 && lastUnderscoreIndex < objectName.Length - 1) {
                string endingNumberString = objectName.Substring(lastUnderscoreIndex + 1);
                if (int.TryParse(endingNumberString, out endingNumber)) {
                    return endingNumber;
                }
            }

            Debug.LogError("Could Not Get Ending Number: " + objectName);
            return -1;
        }
    }
}
