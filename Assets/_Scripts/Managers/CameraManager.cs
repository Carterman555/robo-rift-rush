using UnityEngine;
using Cinemachine;

namespace SpeedPlatformer.Management {
    public class CameraManager : StaticInstance<CameraManager> {

        [SerializeField] private CinemachineVirtualCamera mainCamera;
        [SerializeField] private CinemachineVirtualCamera cannonCamera;

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
        }

        public void SwitchToPlayerCamera() {
            // make camera transition slower
            float transitionDuration = 1f;
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, transitionDuration);

            mainCamera.enabled = true;
            cannonCamera.enabled = false;
        }
    }
}
