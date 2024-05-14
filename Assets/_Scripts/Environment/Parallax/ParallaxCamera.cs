using UnityEngine;

public class ParallaxCamera : MonoBehaviour {
    public delegate void ParallaxCameraDelegate(Vector3 deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    private Vector3 oldPosition;

    private void Awake() {
        oldPosition = transform.position;
    }

    void Update() {
        if (transform.position != oldPosition) {
            if (onCameraTranslate != null) {
                Vector3 delta = oldPosition - transform.position;

                onCameraTranslate(delta);
            }

            oldPosition = transform.position;
        }
    }
}