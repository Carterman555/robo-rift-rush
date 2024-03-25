using SpeedPlatformer.Utilities;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SpeedPlatformer.Environment
{
    public class DeathFogSpawner : MonoBehaviour
    {
        [SerializeField] private Transform pointsContainer;
        private List<DeathFogPoint> deathFogPoints = new List<DeathFogPoint>();

        private void Awake() {
            deathFogPoints = pointsContainer.GetComponentsInChildren<DeathFogPoint>().ToList();
        }

        private void Start() {
            SpawnFog();
        }

        [SerializeField] private DeathFogPrototype deathFogPrefab;
        private DeathFogPrototype currentDeathFog;

        private void SpawnFog() {

            bool allFogSpawned = deathFogPoints.Count < 2;
            if (allFogSpawned) {
                return;
            }

            // unsubscribe from previous death fog
            if (currentDeathFog != null) {
                currentDeathFog.OnReachedPoint -= SpawnFog;
            }

            DeathFogPoint currentPoint = deathFogPoints[0];
            DeathFogPoint nextPoint = deathFogPoints[1];

            DeathFogPrototype deathFog = ObjectPoolManager.SpawnObject(deathFogPrefab,
                currentPoint.transform.position,
                Quaternion.identity,
                transform);

            deathFog.Setup(nextPoint.transform.position, currentPoint.GetFogWidth(), currentPoint.GetFogSpeed());
            currentDeathFog = deathFog;

            // unsubscribe to new death fog to spawn another when this one reaches it's end
            deathFog.OnReachedPoint += SpawnFog;

            deathFogPoints.RemoveAt(0);
        }

        private void OnDrawGizmos() {

            Gizmos.color = Color.red;

            deathFogPoints = pointsContainer.GetComponentsInChildren<DeathFogPoint>().ToList();

            for (int i = 0; i < deathFogPoints.Count - 1; i++) {
                DeathFogPoint currentPoint = deathFogPoints[i];
                DeathFogPoint nextPoint = deathFogPoints[i + 1];

                Vector3 directionToNext = currentPoint.transform.position - nextPoint.transform.position;
                Vector3 perpendicularToNext = directionToNext.PerpendicularDirection().normalized;

                float width = currentPoint.GetFogWidth();

                Vector3 distance1 = perpendicularToNext * (width / 2f);
                Vector3 point1 = currentPoint.transform.position + distance1;

                Vector3 distance2 = -directionToNext;
                Vector3 point2 = point1 + distance2;

                Vector3 distance3 = -perpendicularToNext * width;
                Vector3 point3 = point2 + distance3;

                Vector3 distance4 = directionToNext;
                Vector3 point4 = point3 + distance4;

                // draw line connecting points to make shape
                Gizmos.DrawLine(point1, point2);
                Gizmos.DrawLine(point2, point3);
                Gizmos.DrawLine(point3, point4);
                Gizmos.DrawLine(point4, point1);
            }
        }
    }
}
