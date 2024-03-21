using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpeedPlatformer.Utilities {
    public class ObjectPoolManager : MonoBehaviour {
        public static List<PooledObjectInfo> ObjectPoolList;

        // to reset list when domain is not loaded
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            ObjectPoolList = new List<PooledObjectInfo>(); 
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, Transform parent = null) {
            if (objectToSpawn == null) {
                Debug.LogError("objectToSpawn Is Null!");
                return null;
            }

            PooledObjectInfo pool = ObjectPoolList.Find(pool => pool.LookupString == objectToSpawn.name);

            // If the pool doesn't exist, create it
            if (pool == null) {
                pool = new PooledObjectInfo() {
                    LookupString = objectToSpawn.name
                };
                ObjectPoolList.Add(pool);
            }

            // Check if there are any iactive objects in the pool
            GameObject spawnableObject = pool.InactiveObject.FirstOrDefault();

            if (spawnableObject == null) {
                // If there are no inactive objects, create a new one
                spawnableObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation, parent);
            }
            else {
                // If there is an inactive object, reactive it
                spawnableObject.transform.position = spawnPosition;
                spawnableObject.transform.rotation = spawnRotation;
                spawnableObject.transform.SetParent(parent);
                pool.InactiveObject.Remove(spawnableObject);
                spawnableObject.SetActive(true);
            }

            return spawnableObject;
        }

        public static T SpawnObject<T>(T behaviourToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, Transform parent = null) where T : Component {
            if (behaviourToSpawn == null) {
                Debug.LogError("behaviourToSpawn Is Null!");
                return null;
            }

            GameObject spawnedObject = SpawnObject(behaviourToSpawn.gameObject, spawnPosition, spawnRotation, parent);
            return spawnedObject.GetComponent<T>();
        }

        public static T SpawnObject<T>(T behaviourToSpawn, Transform parent = null) where T : Component {
            if (behaviourToSpawn == null) {
                Debug.LogError("behaviourToSpawn Is Null!");
                return null;
            }

            GameObject spawnedObject = SpawnObject(behaviourToSpawn.gameObject, Vector3.zero, Quaternion.identity, parent);
            spawnedObject.transform.localPosition = Vector3.zero;
            return spawnedObject.GetComponent<T>();
        }

        public static void ReturnObjectToPool(GameObject objectToReturn) {
            if (objectToReturn == null) {
                Debug.LogError("objectToReturn Is Null!");
                return;
            }

            string goName = objectToReturn.name.Substring(0, objectToReturn.name.Length - 7);

            PooledObjectInfo pool = ObjectPoolList.Find(p => p.LookupString == goName);

            if (pool == null) {
                Debug.LogWarning("Trying to release an object that is not pooled: " + objectToReturn.name);
            }
            else {
                objectToReturn.SetActive(false);
                pool.InactiveObject.Add(objectToReturn);
            }
        }
    }

    public class PooledObjectInfo {
        public string LookupString;
        public List<GameObject> InactiveObject = new List<GameObject>();
    }
}