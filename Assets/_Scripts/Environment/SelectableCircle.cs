using UnityEngine;

namespace RoboRiftRush.Environment{
    public class SelectableCircle : MonoBehaviour {
        // draw circle to make selectable
        private void OnDrawGizmos() {

            Color lightBlue = new Color(82f / 255f, 219f / 255f, 255f / 255f, 80f/255f);
            Gizmos.color = lightBlue;

            float radius = 1.5f;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
