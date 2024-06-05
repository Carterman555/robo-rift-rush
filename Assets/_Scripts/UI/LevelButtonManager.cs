using UnityEngine;

namespace RoboRiftRush {
	public class LevelButtonManager : MonoBehaviour {

		[SerializeField] private Transform levelContainer;
		[SerializeField] private LevelButton levelPrefab;

        private void Start() {
            CreateLevels();
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