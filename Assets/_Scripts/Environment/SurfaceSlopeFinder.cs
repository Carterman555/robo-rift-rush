using UnityEngine;
using UnityEngine.U2D;

namespace SpeedPlatformer.Environment {
	public class SurfaceSlopeFinder {

        public Vector2 GetDirectionAtPoint(Spline spline, Vector2 point) {
            int closestIndex = 0;
            float closestDistance = float.MaxValue;

            // Find the closest point on the spline
            for (int i = 0; i < spline.GetPointCount(); i++) {
                float distance = Vector2.Distance(point, spline.GetPosition(i));
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            // Calculate the direction based on the tangent at the closest point
            Vector3 pos1 = spline.GetPosition(closestIndex);
            Vector3 pos2;

            // Check if the closestIndex is not the first or the last point
            if (closestIndex > 0 && closestIndex < spline.GetPointCount() - 1) {
                Vector2 prev = spline.GetPosition(closestIndex - 1);
                Vector2 next = spline.GetPosition(closestIndex + 1);
                pos2 = (next - prev).normalized;
            }
            else if (closestIndex == 0) {
                // If it’s the first point, use the next point to determine the direction
                pos2 = (spline.GetPosition(closestIndex + 1) - pos1).normalized;
            }
            else {
                // If it’s the last point, use the previous point to determine the direction
                pos2 = (pos1 - spline.GetPosition(closestIndex - 1)).normalized;
            }

            return pos2;
        }

    }
}