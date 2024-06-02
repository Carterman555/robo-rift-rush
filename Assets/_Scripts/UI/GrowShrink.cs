using DG.Tweening;
using UnityEngine;

namespace SpeedPlatformer.UI {
    public class GrowShrink : MonoBehaviour {

        [SerializeField] private float growAmount;
        [SerializeField] private float duration;

        void Start() {
            transform.DOScale(growAmount, duration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
