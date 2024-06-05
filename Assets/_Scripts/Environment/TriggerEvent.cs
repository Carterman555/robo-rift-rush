using System;
using UnityEngine;

namespace RoboRiftRush.Triggers {
    public class TriggerEvent : MonoBehaviour {
        public event Action<Collider2D> OnTriggerEntered;
        public event Action<Collider2D> OnTriggerExited;

        private void OnTriggerEnter2D(Collider2D collision) {
            OnTriggerEntered?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision) {
            OnTriggerExited?.Invoke(collision);
        }
    }
}
