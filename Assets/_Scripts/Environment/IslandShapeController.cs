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
            Vector3 tangentPos = new Vector3(0, -5f);

            spline.SetPosition(0, new Vector3(width, Random.Range(-surfaceSlantRandomness, surfaceSlantRandomness)));
            spline.SetTangentMode(0, ShapeTangentMode.Broken);
            spline.SetLeftTangent(0, tangentPos);
            spline.SetRightTangent(0, Vector3.zero);

            spline.SetPosition(1, new Vector3(0, Random.Range(-surfaceSlantRandomness, surfaceSlantRandomness)));
            spline.SetTangentMode(1, ShapeTangentMode.Broken);
            spline.SetLeftTangent(1, Vector3.zero);
            spline.SetRightTangent(1, tangentPos);

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

            CenterIslandOnSprite();

            // for some reason the island is inside out so change sign of collider offset
            sprite.colliderOffset = -0.5f;
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

        public void SetSurfaceFromPoints(Vector3 oldIslandPosition, List<Vector3> surfacePoints) {

            //... reverse points to make points go from right to left because that is the direction the points
            //... are added in the Randomize Shape method
            surfacePoints.Reverse();

            //... the surface points and sline point are both a set of local positions that have parents with
            //... different positions, so positions won't match. To fix this find position difference and add
            //... it to each spline point
            Vector3 toNewToOldIsland = oldIslandPosition - transform.position;

            // line up right surface points by moving whole island (to keep new island shape and size)
            Vector3 toRightSurfacePoint = surfacePoints[0] + toNewToOldIsland - spline.GetPosition(0);
            transform.position += toRightSurfacePoint;

            //... update difference
            toNewToOldIsland = oldIslandPosition - transform.position;

            sprite = GetComponent<SpriteShapeController>();
            spline = sprite.spline;
            // starting at pointIndex = 1 and loops til end to last because the right and left spline points on surface
            // already exist. They just need to be moved.
            for (int pointIndex = 1; pointIndex < surfacePoints.Count - 1; pointIndex++) { 
                Vector3 point = surfacePoints[pointIndex] + toNewToOldIsland;

                // insert point between left corner and previous point on right
                spline.InsertPointAt(pointIndex, point);
                spline.SetTangentMode(pointIndex, ShapeTangentMode.Linear);
            }

            Vector3 leftPointPos = surfacePoints[surfacePoints.Count - 1] + toNewToOldIsland;
            spline.SetPosition(surfacePoints.Count - 1, leftPointPos);
        }
    }
}
