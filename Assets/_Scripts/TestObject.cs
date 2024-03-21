using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedPlatformer
{
    public class TestObject : MonoBehaviour
    {
        private Rigidbody2D _rb;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            
        }

        private void Update() {
            _rb.velocity = Vector2.right;
        }
    }
}
