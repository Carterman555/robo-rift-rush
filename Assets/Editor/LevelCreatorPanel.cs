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
                SetupSelection();
            }

            if (GUILayout.Button("Turn to Moving Environments")) {
                SetupSelection();

                foreach (GameObject section in Selection.gameObjects) {
                    section.AddComponent<MovingEnvironment>().CreateMoveTrigger();
                    section.GetComponent<FloatingIslandMovementPerlin>().enabled = false;
                }
            }

            GUILayout.Label("Move Parent Without Moving Children");

            parent = EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true) as Transform;
            parentTarget = EditorGUILayout.ObjectField("Parent Target", parentTarget, typeof(Transform), true) as Transform;

            if (GUILayout.Button("Move Parent")) {
                MoveParentWithoutChildren(parent, parentTarget.position);
            }
        }

        private void SetupSelection() {
            // go through each section, set parent, rename it, add components, add move trigger
            foreach (GameObject section in Selection.gameObjects) {

                if (!TryGetEndingNumber(section.name, out int sectionNum)) {
                    Debug.LogError("Could Not Get Ending Number: " + section.name);
                    return;
                }

                section.transform.SetParent(sectionContainer);
                section.name = "Section_" + sectionNum;

                section.AddComponent<Rigidbody2D>().isKinematic = true;
                section.AddComponent<CompositeCollider2D>();
                section.AddComponent<CopyMovementToPlayer>();
                section.AddComponent<FloatingIslandMovementPerlin>();

                // set section the position to the center of tiles and objects in section
                Transform[] children = section.transform.GetDirectChildren();
                Vector3 centerPos = GetCenterPosition(children);
                MoveParentWithoutChildren(section.transform, centerPos);
            }

            // reordering is done after all sections have been parented so they are in the correct order
            // TODO - comment more
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

        // unparent the gameobjects, move the parent, reparent the gameobjects so the children stay while the parent moves
        private void MoveParentWithoutChildren(Transform parent, Vector3 newPosition) {
            Transform[] children = parent.GetDirectChildren();
            
            foreach (Transform child in children) {
                child.SetParent(null);
            }

            parent.position = newPosition;

            foreach (Transform child in children) {
                child.SetParent(parent);
            }
        }

        public Vector3 GetCenterPosition(Transform[] transforms) {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (Transform trans in transforms) {
                Vector3 pos = trans.position;
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);
            }

            Vector3 centerPosition = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
            return centerPosition;
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