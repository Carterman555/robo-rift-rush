using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedPlatformer
{
    public class ConstantRotateObject : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private bool rotateLeft;

        private void FixedUpdate() {

            //... change the direction based on rotateLeft bool
            float rotation = rotateLeft ? speed : -speed;

            transform.Rotate(new Vector3(0, 0, rotation * Time.fixedDeltaTime));
        }
    }
}
