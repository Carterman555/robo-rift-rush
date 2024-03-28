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
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);

            mainCamera.enabled = false;
            cannonCamera.enabled = true;
        }
    }
}
