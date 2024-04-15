using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using SpeedPlatformer.Environment;
using Codice.Client.Common;
using System.Linq;
using static System.Collections.Specialized.BitVector32;
using System.Collections.Generic;

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

            if (GUILayout.Button("Assign Triggers")) {
                foreach (GameObject section in Selection.gameObjects) {
                    if (section.GetComponent<MovingEnvironment>() != null)
                        section.GetComponent<MovingEnvironment>().SetTrigger(section.GetComponentInChildren<TriggerEvent>());
                }
            }
        }

        private void SetupSelection() {
            // go through each section, set parent, rename it, add components, add move trigger
            foreach (GameObject section in Selection.gameObjects) {

                int sectionNum = TryGetEndingNumber(section.name);

                if (sectionNum == -1) return;

                section.transform.SetParent(sectionContainer);
                section.name = "Section_" + sectionNum;
                section.layer = GameLayers.GroundLayer;

                section.AddComponent<Rigidbody2D>().isKinematic = true;
                section.AddComponent<CompositeCollider2D>();
                section.AddComponent<CopyMovementToPlayer>();
                section.AddComponent<FloatingIslandMovementPerlin>();

                // set section the position to the center of tiles and objects in section
                Transform[] children = section.transform.GetDirectChildren();
                Vector3 centerPos = GetCenterPosition(children);
                MoveParentWithoutChildren(section.transform, centerPos);
            }

            List<Transform> sortedSections = sectionContainer.GetDirectChildren().ToList();
            sortedSections.Sort((object1, object2) => TryGetEndingNumber(object1.name).CompareTo(TryGetEndingNumber(object2.name)));

            for (int i = 0; i < sortedSections.Count; i++) {
                sortedSections[i].SetSiblingIndex(i);
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

        private int TryGetEndingNumber(string objectName) {
            int endingNumber = 0;
            int lastUnderscoreIndex = objectName.LastIndexOf('_');
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
#endif
}