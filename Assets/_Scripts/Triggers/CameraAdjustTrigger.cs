using UnityEngine;

namespace SpeedPlatformer.Triggers
{
    public class CameraAdjustTrigger : MonoBehaviour
    {
        [SerializeField] private float size;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer) {
                AdjustCamera.Instance.Adjust(size);
            }
        }
    }
}
