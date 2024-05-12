using UnityEngine;
using UnityEditor;
using Codice.Client.Common;
using System;
using SpeedPlatformer.Environment;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D;

namespace SpeedPlatformer.Editor {

#if UNITY_EDITOR
    public class LevelCreatorPanel : EditorWindow {

        [MenuItem("Window/Level Creator Panel")]
        public static void ShowWindow() {
            GetWindow<LevelCreatorPanel>("Level Creator Panel");
        }

        private Transform islandContainer;
        private Collider2D[] islandColliders = new Collider2D[0];
        private Collider2D[] objectColliders = new Collider2D[0];

        private IslandShapeController stationaryIslandPrefab;
        private IslandShapeController translatingIslandPrefab;
        private IslandShapeController rotatingIslandPrefab;
        private IslandShapeController translatingRotatingIslandPrefab;

        private void OnGUI() {

            GUILayout.Label("Setup Parents");

            GUILayout.Space(5);

            GUILayout.Label("Selected Objects: " + Selection.gameObjects.Length);

            islandContainer = EditorGUILayout.ObjectField("Island Container", islandContainer, typeof(Transform), true) as Transform;

            GUILayout.Label("Island Colliders Count: " + islandColliders.Length);
            if (GUILayout.Button("Set Island Colliders")) {
                islandColliders = GetColArrayFromSelection();
            }

            GUILayout.Label("Object Colliders Count: " + objectColliders.Length);
            if (GUILayout.Button("Set Object Colliders")) {
                objectColliders = GetColArrayFromSelection();
            }

            if (GUILayout.Button("Setup Parents")) {
                SetupParents();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Fix Traps")) {
                // Find all game objects with the custom tag "Trap"
                GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");

                foreach (GameObject trap in traps) {
                    string trapName = trap.name;

                    // Check if the name starts with "staticTrap" or "movingTrap"
                    if (trapName.StartsWith("StaticTrap") || trapName.StartsWith("MovingTrap")) {
                        Debug.Log("Found trap: " + trapName);

                        BoxCollider2D boxCollider = trap.GetComponent<BoxCollider2D>();

                        int width = Mathf.RoundToInt(boxCollider.size.x * trap.transform.localScale.x);

                        boxCollider.size = new Vector2(width, 1);
                        trap.GetComponentInChildren<SpriteRenderer>().size = new Vector2(width, 1);

                        trap.transform.localScale = Vector3.one;
                    }
                }
            }
        }

        private Collider2D[] GetColArrayFromSelection() {
            Collider2D[] colliders = new Collider2D[Selection.gameObjects.Length];
            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                if (Selection.gameObjects[i].TryGetComponent(out Collider2D collider)) {
                    colliders[i] = collider;
                }
                else {
                    Debug.LogWarning("Object " + Selection.gameObjects[i].name + " Does Not Have Collider2D!");
                }
            }
            return colliders;
        }

        private void SetupParents() {
            // go through each island, set parent, rename it, add components, add move trigger
            foreach (GameObject island in Selection.gameObjects) {

                island.transform.SetParent(islandContainer);

                ParentTouchingObjects();
            }
        }

        private void ParentTouchingObjects() {
            foreach (Collider2D islandCollider in islandColliders) {
                foreach (Collider2D objectCollider in objectColliders) {
                    if (IsOverlapping(islandCollider, objectCollider)) {
                        objectCollider.transform.SetParent(islandCollider.transform);
                    }
                }
            }
        }

        private bool IsOverlapping(Collider2D colliderA, Collider2D colliderB) {
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            Collider2D[] colliders = new Collider2D[1];
            int count = colliderA.OverlapCollider(filter, colliders);

            for (int i = 0; i < count; i++) {
                if (colliders[i] == colliderB) {
                    return true;
                }
            }

            return false;
        }
    }
#endif
}