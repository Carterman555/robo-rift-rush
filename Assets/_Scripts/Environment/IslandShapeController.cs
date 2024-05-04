using UnityEngine;
using UnityEngine.U2D;

namespace SpeedPlatformer.Environment {

    public class IslandShapeController : MonoBehaviour {

        private SpriteShapeController sprite;
        private Spline spline;

        [ContextMenu("Center Island On Sprite")]
        private void SetPosition() {

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

        [ContextMenu("Randomize Shape")]
        private void RandomizeShape() {

            sprite = GetComponent<SpriteShapeController>();
            spline = sprite.spline;

            // remove all but 2 points
            for (int i = spline.GetPointCount() - 1; i >= 2; i--) {
                spline.RemovePointAt(i);
            }

            // get random width
            float minWidthMult = 13f;
            float maxWidthMult = 15f;
            float randomWidth = Random.Range(islandSize * minWidthMult, islandSize * maxWidthMult);

            // Set the spline points that make up the surface
            Vector3 tangentPos = new Vector3(0, -5f);

            spline.SetPosition(0, Vector3.right * randomWidth);
            spline.SetTangentMode(0, ShapeTangentMode.Broken);
            spline.SetLeftTangent(0, tangentPos);
            spline.SetRightTangent(0, Vector3.zero);

            spline.SetPosition(1, Vector3.zero);
            spline.SetTangentMode(1, ShapeTangentMode.Broken);
            spline.SetLeftTangent(1, Vector3.zero);
            spline.SetRightTangent(1, tangentPos);

            // get random height
            float minHeightMult = 4f;
            float maxHeightMult = 5f;
            float randomHeight = Random.Range(islandSize * minHeightMult, islandSize * maxHeightMult);

            // set spline positions to the match the curve with some randomness
            int pointCount = Mathf.RoundToInt(islandSize);
            for (int i = 0; i < pointCount; i++) {
                float avgXPointDistance = randomWidth / (pointCount + 1);

                float xPos = avgXPointDistance * (i + 1);
                Vector3 point = CalculatePointOnCurve(xPos, randomWidth, randomHeight);

                float pointPosRandomness = 2f;
                point += GetRandomVector3(pointPosRandomness);

                int currentPointIndex = i + 2;

                spline.InsertPointAt(currentPointIndex, point);

                Vector3 directionToPreviousPoint = spline.GetPosition(currentPointIndex - 1) - spline.GetPosition(currentPointIndex);
                directionToPreviousPoint.Normalize();

                print("Point " + currentPointIndex + ": " + directionToPreviousPoint);
                spline.SetTangentMode(currentPointIndex, ShapeTangentMode.Continuous);

                float tangentRandomness = 50f;
                float randomRotation = Random.Range(-tangentRandomness, tangentRandomness);
                Vector3 randomDirection = Quaternion.Euler(0, randomRotation, 0) * directionToPreviousPoint;

                spline.SetLeftTangent(currentPointIndex, randomDirection * 3f);
                spline.SetRightTangent(currentPointIndex, -randomDirection * 3f);

            }
        }

        // get curve from width and height
        // y=0.5h\cos\left(\frac{\left(2\pi x\right)}{w}\right)-0.5h in desmos to visualize curve
        private Vector3 CalculatePointOnCurveCos(float xPos, float width, float height) {
            float yPos = (0.5f * height * Mathf.Cos((2 * Mathf.PI * xPos) / width)) - 0.5f * height;
            return new Vector3(xPos, yPos);
        }

        // get curve from width and height
        // y=h\left(\left(\frac{2x}{w}-1\right)^{2}-1\right) in desmos to visualize curve
        private Vector3 CalculatePointOnCurve(float xPos, float width, float height) {
            float yPos = height * (Mathf.Pow((2 * xPos) / width - 1, 2) - 1);
            return new Vector3(xPos, yPos);
        }

        private Vector3 GetRandomVector3(float randomness) {
            return new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness));
        }
    }
}
