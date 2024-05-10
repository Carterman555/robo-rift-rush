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

            GUILayout.Label("Remake Islands");

            GUILayout.Space(5);

            stationaryIslandPrefab = EditorGUILayout.ObjectField("Stationary Island Prefab", stationaryIslandPrefab, typeof(IslandShapeController), true) as IslandShapeController;
            translatingIslandPrefab = EditorGUILayout.ObjectField("Translating Island Prefab", translatingIslandPrefab, typeof(IslandShapeController), true) as IslandShapeController;
            rotatingIslandPrefab = EditorGUILayout.ObjectField("Rotating Island Prefab", rotatingIslandPrefab, typeof(IslandShapeController), true) as IslandShapeController;
            translatingRotatingIslandPrefab = EditorGUILayout.ObjectField("Translating Rotating Island Prefab", translatingRotatingIslandPrefab, typeof(IslandShapeController), true) as IslandShapeController;

            if (GUILayout.Button("Remake Selected Islands")) {
                RemakeIslands(Selection.gameObjects);
            }

            if (GUILayout.Button("Undo New Islands")) {

                List<GameObject> newIslands = new List<GameObject>();
                foreach (Transform oldIsland in islandPairs.Keys) {
                    MoveNontileChildren(islandPairs[oldIsland], oldIsland);
                    newIslands.Add(islandPairs[oldIsland].gameObject);
                }

                for (int i = newIslands.Count - 1; i >= 0; i--) {
                    DestroyImmediate(newIslands[i], false);
                    newIslands.RemoveAt(i);
                }
            }

            if (GUILayout.Button("Add Copy Movement To Islands")) {
                GameObject[] translateIslands = FindObjectsOfType<TranslateIsland>().Select(img => img.gameObject).ToArray();
                GameObject[] gameObjectsWithSprite = FindObjectsOfType<RotateIsland>().Select(spr => spr.gameObject).ToArray();

                // Combine the two arrays and remove duplicates
                GameObject[] movingIslands = translateIslands.Union(gameObjectsWithSprite).ToArray();

                foreach (GameObject island in movingIslands) {
                    island.AddComponent<CopyMovementToPlayer>();
                }
            }

            if (GUILayout.Button("Set Corner and PPU")) {

                IslandShapeController[] islandShapeControllers = FindObjectsOfType<IslandShapeController>();

                foreach (IslandShapeController islandShapeController in islandShapeControllers) {
                    islandShapeController.SetAllCornerContinuous();
                    if (islandShapeController.TryGetComponent(out SpriteShapeController spriteShapeController)) {
                        spriteShapeController.fillPixelsPerUnit = 10f;
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

        private Dictionary<Transform, Transform> islandPairs = new Dictionary<Transform, Transform>();

        /// <summary>
        /// This method converts the old islands using tiles into the new island objects using sprite shapes. It goes
        /// through each old island and does these things:
        /// - create a prefab of new island object and randomize shape
        /// - match the surface shape of new island to the surface tiles of old island
        /// - set the movement values and target position
        /// </summary>
        private void RemakeIslands(GameObject[] oldIslands) {

            islandPairs = new Dictionary<Transform, Transform>();

            foreach (GameObject oldIsland in oldIslands) {
                if (!IsValidIsland(oldIsland)) {
                    Debug.LogWarning(oldIsland.name + " is Not a Valid Island");
                    continue;
                }

                IslandShapeController newIsland = CreateMatchingNewIsland(oldIsland);

                // randomize new island shape based on old island width and height
                Vector3 oldIslandSize = oldIsland.GetComponent<CompositeCollider2D>().bounds.extents;
                newIsland.RandomizeShape(oldIslandSize.x * 2, oldIslandSize.y * 2);
                //newIsland.RandomizeShape(oldIslandSize.x * 2, oldIslandSize.x * 0.5f); // for just surface islands

                List<Transform> surfaceTiles = GetSurfaceTiles(oldIsland);

                List<Vector3> surfacePoints = GetPointsFromTiles(surfaceTiles);
                newIsland.SetSurfaceFromPoints(oldIsland.transform.position, surfacePoints);

                MovingEnvironment oldIslandMovement = oldIsland.GetComponent<MovingEnvironment>();

                // set move values
                if (newIsland.TryGetComponent(out TranslateIsland translateIsland)) {
                    MatchTranslateIslandParameters(oldIslandMovement, translateIsland);
                }

                // set rotation values
                if (newIsland.TryGetComponent(out RotateIsland rotateIsland)) {
                    MatchRotateIslandParameters(oldIslandMovement, rotateIsland);
                }

                MoveNontileChildren(oldIsland.transform, newIsland.transform);

                islandPairs.Add(oldIsland.transform, newIsland.transform);
            }
        }

        // makes sure the island has all the components to transform it
        private bool IsValidIsland(GameObject oldIsland) {

            if (!oldIsland.TryGetComponent(out CompositeCollider2D compositeCollider2D)) {
                return false;
            }

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
            Transform[] sortedTiles = allTiles.OrderBy(t => t.localPosition.x).ToArray();

            // add highest y pos of tiles with same x pos to surface tiles list
            float currentX = sortedTiles[0].localPosition.x;
            Transform highestTileWithCurrentX = null;
            float highestYWithCurrentX = float.MinValue;
            for (int i = 0; i < sortedTiles.Length; i++) {

                // when x position changes look for new highest y
                bool xPositionChanged = currentX != sortedTiles[i].localPosition.x;
                if (xPositionChanged) {
                    surfaceTiles.Add(highestTileWithCurrentX);

                    currentX = sortedTiles[i].localPosition.x;

                    highestYWithCurrentX = float.MinValue;
                    highestTileWithCurrentX = null;
                }

                if (sortedTiles[i].localPosition.y > highestYWithCurrentX) {
                    highestYWithCurrentX = sortedTiles[i].localPosition.y;
                    highestTileWithCurrentX = sortedTiles[i];
                }
            }

            surfaceTiles.Add(highestTileWithCurrentX);

            return surfaceTiles;
        }

        private List<Vector3> GetPointsFromTiles(List<Transform> surfaceTiles) {

            List<Vector3> surfacePoints = new List<Vector3>();

            Vector3 firstTileLeftCorner = surfaceTiles[0].localPosition + new Vector3(-0.5f, 0.5f);
            surfacePoints.Add(firstTileLeftCorner);

            // for each y change, add 2 points to the list: one at the previous tile and one at the current tile
            float previousY = surfaceTiles[0].localPosition.y;
            for (int tileIndex = 1; tileIndex < surfaceTiles.Count; tileIndex++) {
                if (previousY != surfaceTiles[tileIndex].localPosition.y) {

                    // the sprite shapes have an outline that goes past the spline points. To make the edge of the outline
                    // align with the edge of the tiles, certain vectors have to be added based on whether the new spline
                    // points above the previous or not.
                    bool yIncreased = previousY < surfaceTiles[tileIndex].localPosition.y;

                    if (yIncreased) {
                        Vector3 previousTileRightCorner = surfaceTiles[tileIndex - 1].localPosition + new Vector3(1.5f, 0.5f);
                        surfacePoints.Add(previousTileRightCorner);

                        Vector3 currentTileLeftCorner = surfaceTiles[tileIndex].localPosition + new Vector3(-0.5f, 0.5f);
                        surfacePoints.Add(currentTileLeftCorner);
                    }
                    else {
                        Vector3 previousTileRightCorner = surfaceTiles[tileIndex - 1].localPosition + new Vector3(0.5f, 0.5f);
                        surfacePoints.Add(previousTileRightCorner);

                        Vector3 currentTileLeftCorner = surfaceTiles[tileIndex].localPosition + new Vector3(-1.5f, 0.5f); ;
                        surfacePoints.Add(currentTileLeftCorner);
                    }
                    
                    previousY = surfaceTiles[tileIndex].localPosition.y;
                }
            }

            Vector3 lastTileRightCorner = surfaceTiles[surfaceTiles.Count - 1].localPosition + new Vector3(0.5f, 0.5f);
            surfacePoints.Add(lastTileRightCorner);

            return surfacePoints;
        }

        private void MatchTranslateIslandParameters(MovingEnvironment movingEnvironment, TranslateIsland translateIsland) {
            
            // set pos for target
            float moveDistance;
            if (movingEnvironment.GetContinuousMovement()) {
                float distanceForContinuous = 30f;
                moveDistance = distanceForContinuous;
            }
            else {
                moveDistance = movingEnvironment.GetMoveDistance();
            }

            Vector3 targetPosition = (Vector3)(movingEnvironment.GetMoveAngle().AngleToDirection() * moveDistance);
            translateIsland.SetTargetLocalPosition(targetPosition);

            // set move speed and acceleration
            translateIsland.SetMaxMoveSpeed(movingEnvironment.GetMaxMoveSpeed());
            translateIsland.SetStartMaxSpeed(movingEnvironment.GetStartMaxSpeed());
            translateIsland.SetAcceleration(movingEnvironment.moveAcceleration);
        }

        private void MatchRotateIslandParameters(MovingEnvironment oldIslandMovement, RotateIsland rotateIsland) {

            bool rotateClockwise = oldIslandMovement.GetMaxRotationSpeed() < 0;
            rotateIsland.SetRotateClockwise(rotateClockwise);

            rotateIsland.SetMaxSpeed(Mathf.Abs(oldIslandMovement.GetMaxRotationSpeed()));
            rotateIsland.startAtMaxRotationSpeed = oldIslandMovement.startAtMaxRotationSpeed;
            rotateIsland.SetAcceleration(oldIslandMovement.rotationAcceleration);

            if (rotateIsland.TryGetComponent(out TranslateIsland translateIsland)) {
                rotateIsland.SetEndCondition(RotateIsland.RotationEndCondition.MatchTranslate);
            }
            else if (oldIslandMovement.continuousMovement) {
                rotateIsland.SetEndCondition(RotateIsland.RotationEndCondition.Continuous);
            }
            else {
                rotateIsland.SetEndCondition(RotateIsland.RotationEndCondition.Rotation);

                Debug.LogWarning("Rotate Island End Rotation Not Set. Set in Inspector: " + rotateIsland.name);
            }
        }

        private void MoveNontileChildren(Transform oldParent, Transform newParent) {
            Transform[] children = oldParent.GetDirectChildren();
            foreach (Transform child in children) {
                //... don't move tiles
                if (child.gameObject.layer != GameLayers.TileLayer && !child.name.Equals("Target")) {
                    child.SetParent(newParent);
                }
            }
        }
    }
#endif
}