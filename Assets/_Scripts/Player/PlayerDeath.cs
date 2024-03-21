using System;
using UnityEngine;

namespace SpeedPlatformer
{
    public class PlayerDeath : StaticInstance<PlayerDeath>
    {
        public static event Action OnPlayerDie;
        private Vector3 tempSpawnPos;

        protected override void Awake() {
            base.Awake();
            tempSpawnPos = transform.position;
        }

        public void Kill() {
            transform.position = tempSpawnPos;
            OnPlayerDie?.Invoke();
        }
    }
}
