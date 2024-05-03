using UnityEngine;
using UnityEditor;
using SpeedPlatformer.Environment;
using System.Linq;
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

        private Transform targetsContainer;

        private void OnGUI() {
            GUILayout.Label("Selected Objects: " + Selection.gameObjects.Length);

            sectionContainer = EditorGUILayout.ObjectField("Section Container", sectionContainer, typeof(Transform), true) as Transform;

            if (GUILayout.Button("Turn to Floating Environments")) {
                SetupSelection();
            }

            if (GUILayout.Button("Turn to Moving Environments")) {
                SetupSelection();

                //... create trigger containers if they don't exist
                TryCreateTriggerContainers();
                TurnSelectionToMovingEnvironments();
            }

            if (GUILayout.Button("Turn to Fragile Environments")) {
                SetupSelection();

                //... create trigger containers if they don't exist
                TryCreateTriggerContainers();
                TurnSelectionToMovingEnvironments();
                TurnSelectionToFragileEnvironments();
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

                int sectionNum = section.name.TryGetEndingNumber('_');

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
            sortedSections.Sort((object1, object2) => object1.name.TryGetEndingNumber('_').
                CompareTo(object2.name.TryGetEndingNumber('_')));

            for (int i = 0; i < sortedSections.Count; i++) {
                sortedSections[i].SetSiblingIndex(i);
            }
        }

        private void TurnSelectionToMovingEnvironments() {
            foreach (GameObject section in Selection.gameObjects) {
                section.AddComponent<MovingEnvironment>().CreateMoveTrigger(moveTriggerContainer);
                section.GetComponent<FloatingIslandMovementPerlin>().enabled = false;
            }
        }

        private void TurnSelectionToFragileEnvironments() {
            foreach (GameObject island in Selection.gameObjects) {
                island.AddComponent<BreakOnGroundContact>().CreateBreakTrigger(island.transform);
            }
        }

        #region Create Containers


        // Create empty gameobject named "Targets" if it doesn't already exist
        private void TryTargetContainer() {
            if (!Helpers.TryFindByName(out GameObject environment, "Environment")) return;

            if (Helpers.TryFindByName(out GameObject _targetContainer, "Targets")) {
                targetsContainer = _targetContainer.transform;
            }
            else {
                targetsContainer = new GameObject("Targets").transform;
                targetsContainer.SetParent(environment.transform);
            }
        }

        #endregion

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
    }
#endif
}