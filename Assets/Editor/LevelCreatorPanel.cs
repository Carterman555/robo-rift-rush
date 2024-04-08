using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using SpeedPlatformer.Environment;

namespace SpeedPlatformer.Editor {

#if UNITY_EDITOR
	public class LevelCreatorPanel : EditorWindow {

        [MenuItem("Window/Level Creator Panel")]
        public static void ShowWindow() {
            GetWindow<LevelCreatorPanel>("Level Creator Panel");
        }

        private Transform sectionContainer;

        private void OnGUI() {
            GUILayout.Label("Selected Objects: " + Selection.gameObjects.Length);

            sectionContainer = EditorGUILayout.ObjectField("Section Container", sectionContainer, typeof(Transform), true) as Transform;

            if (GUILayout.Button("Turn to Floating Environments")) {
                OrganizeSelection();
            }

            if (GUILayout.Button("Turn to Moving Environments")) {
                OrganizeSelection();

                foreach (GameObject section in Selection.gameObjects) {
                    section.AddComponent<MovingEnvironment>();
                    section.GetComponent<FloatingIslandMovementPerlin>().enabled = false;
                }
            }
        }

        private void OrganizeSelection() {
            // go through each section, set parent, rename it, add components, add move trigger
            foreach (GameObject section in Selection.gameObjects) {

                if (TryGetEndingNumber(section.name, out int sectionNum)) {
                    section.transform.SetParent(sectionContainer);
                    section.name = "Section_" + sectionNum;

                    section.AddComponent<Rigidbody2D>().isKinematic = true;
                    section.AddComponent<CompositeCollider2D>();
                    section.AddComponent<CopyMovementToPlayer>();
                    section.AddComponent<FloatingIslandMovementPerlin>();
                }
                else {
                    Debug.LogError("Could Not Get Ending Number: " + section.name);
                }
            }

            // reordering is done after all sections have been parented so they are in the correct order
            foreach (GameObject section in Selection.gameObjects) {
                if (TryGetEndingNumber(section.name, out int sectionNum)) {
                    section.transform.SetSiblingIndex(sectionNum);
                }
                else {
                    Debug.LogError("Could Not Get Ending Number: " + section.name);
                }
            }
        }

        private bool TryGetEndingNumber(string objectName, out int endingNumber) {
            endingNumber = 0;
            int lastUnderscoreIndex = objectName.LastIndexOf('_');
            if (lastUnderscoreIndex != -1 && lastUnderscoreIndex < objectName.Length - 1) {
                string endingNumberString = objectName.Substring(lastUnderscoreIndex + 1);
                if (int.TryParse(endingNumberString, out endingNumber)) {
                    return true;
                }
            }
            return false;
        }
    }
#endif
}