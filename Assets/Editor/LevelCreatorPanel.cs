using UnityEngine;
using UnityEditor;
using Codice.Client.Common;
using System;
using SpeedPlatformer.Environment;
using System.Collections.Generic;
using System.Linq;

namespace SpeedPlatformer.Editor {

#if UNITY_EDITOR
    public class LevelCreatorPanel : EditorWindow {

        [MenuItem("Window/Level Creator Panel")]
        public static void ShowWindow() {
            GetWindow<LevelCreatorPanel>("Level Creator Panel");
        }

        private Transform islandContainer;
        private Collider2D[] islandColliders;
        private Collider2D[] objectColliders;

        private GameObject stationaryIslandPrefab;
        private GameObject translatingIslandPrefab;
        private GameObject rotatingIslandPrefab;
        private GameObject translatingRotatingIslandPrefab;

        private void OnGUI() {
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

            if (GUILayout.Button("Setup Selection")) {
                SetupSelection();
            }

            stationaryIslandPrefab = EditorGUILayout.ObjectField("Stationary Island Prefab", stationaryIslandPrefab, typeof(GameObject), true) as GameObject;
            translatingIslandPrefab = EditorGUILayout.ObjectField("Translating Island Prefab", translatingIslandPrefab, typeof(GameObject), true) as GameObject;
            rotatingIslandPrefab = EditorGUILayout.ObjectField("Rotating Island Prefab", rotatingIslandPrefab, typeof(GameObject), true) as GameObject;
            translatingRotatingIslandPrefab = EditorGUILayout.ObjectField("Translating Rotating Island Prefab", translatingRotatingIslandPrefab, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Remake Selected Islands")) {
                RemakeIslands(Selection.gameObjects);
            }

            if (GUILayout.Button("Test Surface")) {
                List<Transform> surfaceTile = GetSurfaceTiles(Selection.gameObjects[0]);
                foreach (Transform t in surfaceTile) {
                    t.GetComponent<SpriteRenderer>().color = Color.red;
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

        private void SetupSelection() {
            // go through each island, set parent, rename it, add components, add move trigger
            foreach (GameObject island in Selection.gameObjects) {

                island.transform.SetParent(islandContainer);

                ParentTouchingObjects();
            }
        }

        /// <summary>
        /// This method converts the old islands using tiles into the new island objects using sprite shapes. It goes
        /// through each old island and does these things:
        /// - create a prefab of new island object and randomize shape
        /// - match the surface shape of new island to the surface tiles of old island
        /// - set the movement values and target position
        /// </summary>
        private void RemakeIslands(GameObject[] oldIslands) {
            foreach (GameObject oldIsland in oldIslands) {
                if (!IsValidIsland(oldIsland)) return;

                GameObject newIsland = CreateMatchingNewIsland(oldIsland);

                // randomize new island shape based on old island width and height
                Vector3 oldIslandSize = oldIsland.GetComponent<CompositeCollider2D>().bounds.extents;
                newIsland.GetComponent<IslandShapeController>().RandomizeShape(oldIslandSize.x * 2, oldIslandSize.y * 2);
                //newIsland.GetComponent<IslandShapeController>().RandomizeShape(oldIslandSize.x * 2, oldIslandSize.x * 0.5f); // for just surface islands


            }
        }

        // makes sure the island has all the components to transform it
        private bool IsValidIsland(GameObject oldIsland) {
            return true;
        }

        // instantiate a certain prefab based how the old island moves
        private GameObject CreateMatchingNewIsland(GameObject oldIsland) {
            bool translatingIsland = false;
            bool rotatingIsland = false;

            if (oldIsland.TryGetComponent(out MovingEnvironment movingEnvironment)) {
                if (movingEnvironment.GetMaxMoveSpeed() != 0) {
                    translatingIsland = true;
                }
                if (movingEnvironment.GetMaxRotationSpeed() != 0) {
                    rotatingIsland = true;
                }
            }

            GameObject matchingPrefab = null;

            if (!translatingIsland && !rotatingIsland) {
                matchingPrefab = stationaryIslandPrefab;
            }
            else if (translatingIsland && !rotatingIsland) {
                matchingPrefab = translatingIslandPrefab;
            }
            else if (!translatingIsland && rotatingIsland) {
                matchingPrefab = rotatingIslandPrefab;
            }
            else if (translatingIsland && rotatingIsland) {
                matchingPrefab = translatingRotatingIslandPrefab;
            }

            GameObject newIsland = Instantiate(matchingPrefab, oldIsland.transform.position, oldIsland.transform.rotation, oldIsland.transform.parent);
            return newIsland;
        }

        // to get all the surface tiles: sort by x positions then add highest y pos of tiles with same x pos. This
        // works because the surface tiles are the tiles with the highest y pos out of all the other tiles with their
        // same x pos.
        private List<Transform> GetSurfaceTiles(GameObject oldIsland) {
            List<Transform> surfaceTiles = new List<Transform>();

            // filter only tiles (not traps)
            List<Transform> allTiles = new List<Transform>();
            Transform[] children = oldIsland.transform.GetDirectChildren();
            foreach (Transform child in children) {
                if (child.gameObject.layer == GameLayers.TileLayer) {
                    allTiles.Add(child);
                }
            }

            //... sort by x position
            Transform[] sortedTiles = allTiles.OrderBy(t => t.position.x).ToArray();

            // add highest y pos of tiles with same x pos to surface tiles list
            float currentX = sortedTiles[0].position.x;
            Transform highestTileWithCurrentX = null;
            float highestYWithCurrentX = float.MinValue;
            for (int i = 0; i < sortedTiles.Length; i++) {

                // when x position changes look for new highest y
                bool xPositionChanged = currentX != sortedTiles[i].position.x;
                if (xPositionChanged) {
                    surfaceTiles.Add(highestTileWithCurrentX);

                    currentX = sortedTiles[i].position.x;

                    highestYWithCurrentX = float.MinValue;
                    highestTileWithCurrentX = null;
                }

                if (sortedTiles[i].position.y > highestYWithCurrentX) {
                    highestYWithCurrentX = sortedTiles[i].position.y;
                    highestTileWithCurrentX = sortedTiles[i];
                }
            }

            surfaceTiles.Add(highestTileWithCurrentX);

            return surfaceTiles;
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