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

            stationaryIslandPrefab = EditorGUILayout.ObjectField("Stationary Island Prefab", stationaryIslandPrefab, typeof(IslandShapeController), true) as IslandShapeController;
            translatingIslandPrefab = EditorGUILayout.ObjectField("Translating Island Prefab", translatingIslandPrefab, typeof(IslandShapeController), true) as IslandShapeController;
            rotatingIslandPrefab = EditorGUILayout.ObjectField("Rotating Island Prefab", rotatingIslandPrefab, typeof(IslandShapeController), true) as IslandShapeController;
            translatingRotatingIslandPrefab = EditorGUILayout.ObjectField("Translating Rotating Island Prefab", translatingRotatingIslandPrefab, typeof(IslandShapeController), true) as IslandShapeController;

            if (GUILayout.Button("Remake Selected Islands")) {
                RemakeIslands(Selection.gameObjects);
            }

            if (GUILayout.Button("Test Surface")) {
                List<Vector3> points = GetPointsFromTiles(GetSurfaceTiles(Selection.gameObjects[0]));
                for (int i = 0; i < points.Count; i++) {
                    Debug.DrawLine(points[i], points[i] + Vector3.up * 0.3f);
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

                IslandShapeController newIsland = CreateMatchingNewIsland(oldIsland);

                // randomize new island shape based on old island width and height
                Vector3 oldIslandSize = oldIsland.GetComponent<CompositeCollider2D>().bounds.extents;
                newIsland.RandomizeShape(oldIslandSize.x * 2, oldIslandSize.y * 2);
                //newIsland.RandomizeShape(oldIslandSize.x * 2, oldIslandSize.x * 0.5f); // for just surface islands

                List<Transform> surfaceTiles = GetSurfaceTiles(Selection.gameObjects[0]);

                //... set same position so relative position of children are same for matching surface
                //newIsland.transform.position = oldIsland.transform.position;

                List<Vector3> surfacePoints = GetPointsFromTiles(surfaceTiles);
                newIsland.SetSurfaceFromPoints(oldIsland.transform.position, surfacePoints);
            }
        }

        

        // makes sure the island has all the components to transform it
        private bool IsValidIsland(GameObject oldIsland) {
            return true;
        }

        // instantiate a certain prefab based how the old island moves
        private IslandShapeController CreateMatchingNewIsland(GameObject oldIsland) {
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

            IslandShapeController matchingPrefab = null;

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

            IslandShapeController newIsland = Instantiate(matchingPrefab, oldIsland.transform.position, oldIsland.transform.rotation, oldIsland.transform.parent);
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

        private List<Vector3> GetPointsFromTiles(List<Transform> surfaceTiles) {

            List<Vector3> surfacePoints = new List<Vector3>();

            // to get the corner positions from the centers of the tiles
            Vector3 toLeftCorner = new Vector3(-1f, 1f);
            Vector3 toRightCorner = new Vector3(1f, 1f);

            Vector3 firstTileLeftCorner = surfaceTiles[0].position + toLeftCorner;
            surfacePoints.Add(firstTileLeftCorner);

            float currentY = surfaceTiles[0].localPosition.y;
            for (int tileIndex = 1; tileIndex < surfaceTiles.Count; tileIndex++) {
                if (currentY != surfaceTiles[tileIndex].localPosition.y) {
                    currentY = surfaceTiles[tileIndex].localPosition.y;

                    Vector3 previousTileRightCorner = surfaceTiles[tileIndex - 1].localPosition + toRightCorner;
                    surfacePoints.Add(previousTileRightCorner);

                    Vector3 currentTileLeftCorner = surfaceTiles[tileIndex].localPosition + toLeftCorner;
                    surfacePoints.Add(currentTileLeftCorner);
                }
            }

            Vector3 lastTileRightCorner = surfaceTiles[surfaceTiles.Count - 1].localPosition + toRightCorner;
            surfacePoints.Add(lastTileRightCorner);

            return surfacePoints;
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