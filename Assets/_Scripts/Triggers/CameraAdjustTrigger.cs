using UnityEngine;
using RoboRiftRush.Management;

namespace RoboRiftRush.Triggers
{
    public class CameraAdjustTrigger : MonoBehaviour
    {
        [SerializeField] private bool adjustPosition;
        [SerializeField] private Transform centerTransform;

        [SerializeField] private bool adjustSize;
        [SerializeField] private float size;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer) {
                if (adjustSize) {
                    CameraManager.Instance.AdjustSize(size);
                }
                if (adjustPosition) {
                    CameraManager.Instance.SwitchToStaticCamera(centerTransform.position);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.PlayerLayer) {
                if (adjustSize) {
                    CameraManager.Instance.ResetSize();
                }
                if (adjustPosition) {
                    CameraManager.Instance.SwitchToPlayerCamera();
                }
            }
        }
    }
}
