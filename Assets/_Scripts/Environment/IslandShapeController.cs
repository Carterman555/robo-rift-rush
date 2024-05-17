using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace SpeedPlatformer.Environment {

    public class IslandShapeController : MonoBehaviour {

        private SpriteShapeController sprite;
        private Spline spline;

        [ContextMenu("Center Island On Sprite")]
        public void CenterIslandOnSprite() {

            // set the island object to the center of the island
            sprite = GetComponent<SpriteShapeController>();
            Vector3 centerPos = sprite.edgeCollider.bounds.center;
            Vector3 distanceMoved = centerPos - transform.position;
            transform.position = centerPos;

            // set the position of all the splines back so island remains in same position
            spline = sprite.spline;
            for (int i = 0; i < spline.GetPointCount(); i++) {
                spline.SetPosition(i, spline.GetPosition(i) - distanceMoved);
            }

            // set position of all children back, except move target
            Transform[] children = transform.GetDirectChildren();
            foreach (Transform child in children) {
                if (!child.name.Equals("Target")) {
                    child.position -= distanceMoved;
                }
            }
        }

        [SerializeField] private float islandSize = 3f;
        [SerializeField] private float surfaceSlantRandomness = 0f;

        /// <summary>
        /// This button a randomly shaped island with a flat top out of a sprite shape controller and it's spline 
        /// </summary>
        [ContextMenu("Randomize Shape")]
        private void RandomizeShape() {
            RandomizeShape(-1, -1);
        }
        public void RandomizeShape(float width = -1, float height = -1) {

            sprite = GetComponent<SpriteShapeController>();
            spline = sprite.spline;

            // save top left point to reposition island at end
            Vector3 oldTopLeftPoint = GetTopLeftPoint(spline);

            // remove all but 2 points
            for (int i = spline.GetPointCount() - 1; i >= 2; i--) {
                spline.RemovePointAt(i);
            }

            // get random width based on chosen island size
            if (width == -1) {
                float minWidthMult = 13f;
                float maxWidthMult = 15f;
                width = Random.Range(islandSize * minWidthMult, islandSize * maxWidthMult);
            }

            // Set the spline points that make up the surface. The inner tangents positions are Vector3.zero so the top is
            // flat.

            // top right point
            spline.SetPosition(0, new Vector3(width, Random.Range(-surfaceSlantRandomness, surfaceSlantRandomness)));
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetLeftTangent(0, new Vector3(3f, 0));
            spline.SetRightTangent(0, new Vector3(-3f, 0));

            // top left point
            spline.SetPosition(1, new Vector3(0, Random.Range(-surfaceSlantRandomness, surfaceSlantRandomness)));
            spline.SetTangentMode(1, ShapeTangentMode.Continuous);
            spline.SetLeftTangent(1, new Vector3(3f, 0));
            spline.SetRightTangent(1, new Vector3(-3f, 0));

            // get random height based on chosen island size
            if (height == -1) {
                float minHeightMult = 4f;
                float maxHeightMult = 5f;
                height = Random.Range(islandSize * minHeightMult, islandSize * maxHeightMult);
            }

            // set spline positions to the match the curve with some randomness
            float avgXPointDistance = 10f;
            int pointCount = Mathf.RoundToInt(width / avgXPointDistance) - 1;
            for (int i = 0; i < pointCount; i++) {

                // spread the x position of the points fairly evenly (with randomness)
                float xPos = avgXPointDistance * (i + 1);

                // use quadratic equation to get y pos to make island shape
                Vector3 point = CalculatePointOnCurve(xPos, width, height);

                // apply randomness to point
                float pointPosRandomness = 2f;
                point += GetRandomVector3(pointPosRandomness);

                int currentPointIndex = i + 2;
                spline.InsertPointAt(currentPointIndex, point);

                // set tangent direction to the direction of previous point (with randomness) to make more smooth
                Vector3 directionToPreviousPoint = spline.GetPosition(currentPointIndex - 1) - spline.GetPosition(currentPointIndex);
                directionToPreviousPoint.Normalize();

                spline.SetTangentMode(currentPointIndex, ShapeTangentMode.Continuous);

                float tangentRandomness = 50f;
                float randomRotation = Random.Range(-tangentRandomness, tangentRandomness);
                Vector3 randomDirection = Quaternion.Euler(0, randomRotation, 0) * directionToPreviousPoint;

                spline.SetLeftTangent(currentPointIndex, randomDirection * 3f);
                spline.SetRightTangent(currentPointIndex, -randomDirection * 3f);
            }

            Vector3 newTopLeftPoint = GetTopLeftPoint(spline);

            Vector3 toOldPosition = oldTopLeftPoint - newTopLeftPoint;
            MoveAllPoints(spline, toOldPosition);

            CenterIslandOnSprite();

            // for some reason the island is inside out so change sign of collider offset
            sprite.colliderOffset = -0.5f;
        }

        private Vector3 GetTopLeftPoint(Spline spline) {
            // Initialize with the first point
            Vector3 topLeftPoint = spline.GetPosition(0);

            for (int pointIndex = 0; pointIndex < spline.GetPointCount(); pointIndex++) {
                Vector3 point = spline.GetPosition(pointIndex);
                // Check if the current point is more top-left
                if (point.y > topLeftPoint.y || (point.y == topLeftPoint.y && point.x < topLeftPoint.x)) {
                    topLeftPoint = point;
                }
            }

            return topLeftPoint;
        }

        // get curve from width and height using quadratic equation
        // y=h\left(\left(\frac{2x}{w}-1\right)^{2}-1\right) in desmos to visualize curve
        private Vector3 CalculatePointOnCurve(float xPos, float width, float height) {
            float yPos = height * (Mathf.Pow((2 * xPos) / width - 1, 2) - 1);
            return new Vector3(xPos, yPos);
        }

        private Vector3 GetRandomVector3(float randomness) {
            return new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
        }

        private void MoveAllPoints(Spline spline, Vector3 distance) {
            for (int i = 0; i < spline.GetPointCount(); i++) {
                spline.SetPosition(i, spline.GetPosition(i) + distance);
            }
        }
    }
}
