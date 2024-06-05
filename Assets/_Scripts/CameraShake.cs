using Cinemachine;
using TarodevController;
using UnityEngine;

namespace RoboRiftRush.Setup {
	public class CameraShake : MonoBehaviour {

		private CinemachineVirtualCamera cinemachineCamera;
		[SerializeField] private float shakeTime = 0.2f;

		private float timer;
		private CinemachineBasicMultiChannelPerlin cinemachinePerlin;

        private void Awake() {
			cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
            cinemachinePerlin = cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Update() {
            timer += Time.deltaTime;
            if (timer > shakeTime) {
                StopShake();
            }
        }

        public void ShakeCamera(float intensity) {
            cinemachinePerlin.m_AmplitudeGain = intensity;
			timer = 0;
        }

		private void StopShake() {
            cinemachinePerlin.m_AmplitudeGain = 0f;
            timer = float.MinValue;
        }
    }
}