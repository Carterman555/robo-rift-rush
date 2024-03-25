using Cinemachine;
using UnityEngine;
using DG.Tweening;
using TarodevController;

namespace SpeedPlatformer
{
    public class AdjustCamera : StaticInstance<AdjustCamera>
    {
        private CinemachineVirtualCamera cam;

        protected override void Awake() {
            base.Awake();
            cam = GetComponent<CinemachineVirtualCamera>();
        }

        public void Adjust(float size) {

            // change size
            DOTween.To(() => cam.m_Lens.OrthographicSize, newSize => cam.m_Lens.OrthographicSize = newSize, size, duration: 1f)
                    .SetEase(Ease.OutQuad);
        }
    }
}
