using System.Collections.Generic;
using UnityEngine;

namespace Blobber {
    public class Parents : MonoBehaviour {
        private static Dictionary<string, Transform> parentsDict;
        [SerializeField] private Transform[] parents;

        private void Awake() {
            parentsDict = new Dictionary<string, Transform>();
            for (int i = 0; i < parents.Length; i++) {
                parentsDict.Add(parents[i].name, parents[i]);
            }
        }

        public static Transform GetParent(string key) {
            return parentsDict[key];
        }
    }
}
