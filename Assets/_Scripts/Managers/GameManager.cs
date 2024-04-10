using SpeedPlatformer.Management;
using UnityEngine;

namespace SpeedPlatformer
{
    public class GameManager : StaticInstance<GameManager>
    {
        protected override void Awake() {
            base.Awake();

            //Application.targetFrameRate = -1;
        }
    }
}
