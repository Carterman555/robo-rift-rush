using UnityEngine;

namespace SpeedPlatformer.Environment.Traps {

    // TODO - maybe make line renderer instead
    public class ExpandingTrap : MonoBehaviour {

        private float size;
        private float center;

        [SerializeField] private float rightExpandSpeed;
        [SerializeField] private float leftExpandSpeed;

        [SerializeField] private float maxRightExpandAmount;
        [SerializeField] private float maxLeftExpandAmount;

        private float rightExpandAmount;
        private float leftExpandAmount;

        [SerializeField] private Transform visualTransform;
        private BoxCollider2D boxCollider;

        private bool hasBeenInFrame; // in frame currently or was in frame

        private void Awake() {
            size = visualTransform.localScale.x;
            center = 0;

            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                hasBeenInFrame = true;
            }
        }

        // increase size and get center, then set sprite renderer and collider
        private void Update() {

            if (!hasBeenInFrame) return;

            if (rightExpandAmount < maxRightExpandAmount) {
                float rightSizeIncrease = rightExpandSpeed * Time.deltaTime;
                rightExpandAmount += rightSizeIncrease;
                size += rightSizeIncrease;
                center += rightSizeIncrease * 0.5f; // move center so only expands out right
            }
            
            if (leftExpandAmount < maxLeftExpandAmount) {
                float leftSizeIncrease = leftExpandSpeed * Time.deltaTime;
                leftExpandAmount += leftSizeIncrease;
                size += leftSizeIncrease;
                center -= leftSizeIncrease * 0.5f; // move center so only expands out left
            }

            visualTransform.localScale = new Vector3(size, visualTransform.localScale.y);
            visualTransform.localPosition = new Vector3(center, 0);

            boxCollider.size = new Vector2(size, boxCollider.size.y);
            boxCollider.offset = new Vector2(center, 0);
        }

        private void OnDrawGizmos() {

            Gizmos.color = Color.red;

            // show right expansion
            Vector3 rightSidePos = visualTransform.position + transform.right * visualTransform.localScale.x * 0.5f;
            float rightAmountToExpand = maxRightExpandAmount - rightExpandAmount;
            Gizmos.DrawLine(rightSidePos, rightSidePos + transform.right * rightAmountToExpand);

            // show left expansion
            Vector3 leftSidePos = visualTransform.position - transform.right * visualTransform.localScale.x * 0.5f;
            float leftAmountToExpand = maxLeftExpandAmount - leftExpandAmount;
            Gizmos.DrawLine(leftSidePos, leftSidePos - transform.right * leftAmountToExpand);
        }
    }
}