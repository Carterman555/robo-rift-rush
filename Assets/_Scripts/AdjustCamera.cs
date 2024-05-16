using Cinemachine;
using UnityEngine;
using DG.Tweening;

namespace SpeedPlatformer.Setup {

    public class AdjustCamera : StaticInstance<AdjustCamera> {

        private CinemachineVirtualCamera mainCam;
        [SerializeField] private CinemachineVirtualCamera staticCam;
        private Transform player;

        protected override void Awake() {
            base.Awake();
            mainCam = GetComponent<CinemachineVirtualCamera>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void SetPosition(Transform centerTransform) {
            staticCam.Follow = centerTransform;
        }

        public void FollowPlayer() {
            // change size
            mainCam.Follow = player;
        }

        private float originalSize;

        public void AdjustSize(float size) {
            originalSize = mainCam.m_Lens.OrthographicSize;

            DOTween.To(() => mainCam.m_Lens.OrthographicSize, newSize => mainCam.m_Lens.OrthographicSize = newSize, size, duration: 1f)
                    .SetEase(Ease.OutQuad);
        }

        public void ResetSize() {
            DOTween.To(() => mainCam.m_Lens.OrthographicSize, newSize => mainCam.m_Lens.OrthographicSize = newSize, originalSize, duration: 1f)
                    .SetEase(Ease.OutQuad);
        }
    }
}
