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

        private Transform islandContainer;

        private void OnGUI() {
            GUILayout.Label("Selected Objects: " + Selection.gameObjects.Length);

            islandContainer = EditorGUILayout.ObjectField("island Container", islandContainer, typeof(Transform), true) as Transform;

            // or just have context menu on islands
            if (GUILayout.Button("Center on Sprite Shape")) {
                SetupSelection();
            }
        }

        private void SetupSelection() {
            // go through each island, set parent, rename it, add components, add move trigger
            foreach (GameObject island in Selection.gameObjects) {

                island.transform.SetParent(islandContainer);

                // set island position to the center of sprite shape - TODO
                
            }
        }
    }
#endif
}