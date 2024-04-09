using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using SpeedPlatformer.Environment;
using Codice.Client.Common;

namespace SpeedPlatformer.Editor {

#if UNITY_EDITOR
	public class LevelCreatorPanel : EditorWindow {

        [MenuItem("Window/Level Creator Panel")]
        public static void ShowWindow() {
            GetWindow<LevelCreatorPanel>("Level Creator Panel");
        }

        private Transform sectionContainer;

        private Transform parent;
        private Transform parentTarget;

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

            GUILayout.Label("Move Parent Without Moving Children");

            parent = EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true) as Transform;
            parentTarget = EditorGUILayout.ObjectField("Parent Target", parentTarget, typeof(Transform), true) as Transform;

            // unparent the gameobjects, move the parent, reparent the gameobjects so the children stay while the parent moves
            if (GUILayout.Button("Move Parent")) {

                Transform[] children = new Transform[parent.childCount];

                for (int i = 0; i < parent.childCount; i++) {
                    children[i] = parent.transform.GetChild(i);
                }

                foreach (Transform child in children) {
                    child.SetParent(null);
                }

                parent.position = parentTarget.position;

                foreach (Transform child in children) {
                    child.SetParent(parent);
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
            // comment more
            foreach (GameObject section in Selection.gameObjects) {
                if (TryGetEndingNumber(section.name, out int sectionNum)) {
                    int siblingIndex = 0;

                    foreach (Transform child in sectionContainer) {
                        if (TryGetEndingNumber(section.name, out int childNum)) {
                            if (childNum < sectionNum) {
                                siblingIndex++;
                            }
                        }
                    }
                    section.transform.SetSiblingIndex(siblingIndex);
                    Debug.Log(section.name + " set to " + siblingIndex);
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