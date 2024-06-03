using UnityEngine;

namespace SpeedPlatformer {
	public class LevelButtonManager : MonoBehaviour {

		[SerializeField] private Transform levelContainer;
		[SerializeField] private LevelButton levelPrefab;

		private static bool createdLevels;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            createdLevels = false;
        }

        private void Awake() {
            if (!createdLevels) {
                CreateLevels();
                createdLevels = true;
            }
        }

        private void CreateLevels() {
            int levelAmount = 18;
            for (int i = 0; i < levelAmount; i++) {
                LevelButton newLevel = Instantiate(levelPrefab, levelContainer);
                newLevel.SetLevel(i + 1);
            }
        }
    }
}