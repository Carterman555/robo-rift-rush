using UnityEngine;

namespace RoboRiftRush.Setup {
    public class CalculateInframeTrigger : MonoBehaviour {
        private Camera cam;
        private BoxCollider2D inframeTrigger;

        private void Awake() {
            cam = GetComponent<Camera>();
            inframeTrigger = GetComponent<BoxCollider2D>();
        }

        // Update is called once per frame
        void Update() {
            float sizeY = cam.orthographicSize * 2;
            float ratio = (float)Screen.width / Screen.height;
            float sizeX = sizeY * ratio;
            inframeTrigger.size = new Vector2(sizeX, sizeY);
        }
    }
}
