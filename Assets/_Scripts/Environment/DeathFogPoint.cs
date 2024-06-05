using UnityEngine;

namespace RoboRiftRush.Environment
{
    public class DeathFogPoint : MonoBehaviour
    {
        [SerializeField] private float fogWidth;
        [SerializeField] private float fogSpeed;

        public float GetFogWidth() {
            return fogWidth;
        }

        public float GetFogSpeed() {
            return fogSpeed;
        }
    }
}
