using UnityEngine;
using UnityEngine.U2D;

namespace SpeedPlatformer.Environment {

    public class ShowMovementVisual : MonoBehaviour {

        public static bool ShowVisuals = true;

        [ContextMenu("Toggle Visuals")]
        private void ToggleVisuals() {
            ShowVisuals = !ShowVisuals;
        }

        private void OnDrawGizmosSelected() {

            if (!ShowVisuals) return;

            if (!TryGetComponent(out TranslateEnvironment translateEnvironment)) {
                return;
            }

            if (!TryGetComponent(out SpriteShapeController spriteShapeController)) {
                return;
            }

            // draw a line between each point in the spline to visualize where the island will move
            Spline spline = spriteShapeController.spline;
            int pointCount = spline.GetPointCount();
            for (int i = 0; i < pointCount; i++) {
                Vector3 center = translateEnvironment.GetTargetPosition();
                Vector3 startPosition = spline.GetPosition(i) + center;
                Vector3 endPosition = spline.GetPosition((i + 1) % pointCount) + center;
                Gizmos.DrawLine(startPosition, endPosition);
            }
        }

    }
}
