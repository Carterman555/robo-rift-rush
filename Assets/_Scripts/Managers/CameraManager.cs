using UnityEngine;
using Cinemachine;

namespace SpeedPlatformer.Management {
    public class CameraManager : StaticInstance<CameraManager> {

        [SerializeField] private CinemachineVirtualCamera mainCamera;
        [SerializeField] private CinemachineVirtualCamera cannonCamera;
        [SerializeField] private CinemachineVirtualCamera staticCamera;

        private CinemachineBrain cinemachineBrain;

        protected override void Awake() {
            base.Awake();
            cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        }

        public void SwitchToCannonCamera() {
            // make camera transition faster
            float transitionDuration = 0.5f;
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, transitionDuration);

            mainCamera.enabled = false;
            cannonCamera.enabled = true;
            staticCamera.enabled = false;
        }

        public void SwitchToPlayerCamera() {
            // make camera transition slower
            float transitionDuration = 1f;
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, transitionDuration);

            mainCamera.enabled = true;
            cannonCamera.enabled = false;
            staticCamera.enabled = false;
        }

        public void SwitchToStaticCamera(Vector2 center, float size = -1f) {
            // make camera transition slower
            float transitionDuration = 1f;
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, transitionDuration);

            mainCamera.enabled = false;
            cannonCamera.enabled = false;
            staticCamera.enabled = true;

            // if size not given, then keep same size
            float orthographicSize = size == -1 ? mainCamera.m_Lens.OrthographicSize : size;
            staticCamera.m_Lens.OrthographicSize = orthographicSize;

            staticCamera.ForceCameraPosition(new Vector3(center.x, center.y, -10), Quaternion.identity);
        }
    }
}
