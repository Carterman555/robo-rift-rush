using SpeedPlatformer.Triggers;
using UnityEngine;

namespace SpeedPlatformer.Environment {
    public class BreakOnGroundContact : MonoBehaviour {

        [SerializeField] private TriggerEvent breakTrigger;

        private void OnEnable() {
            breakTrigger.OnTriggerEntered += TryBreak;
        }
        private void OnDisable() {
            breakTrigger.OnTriggerEntered -= TryBreak;
        }

        private void TryBreak(Collider2D collision){
            if (collision.gameObject.layer == GameLayers.GroundLayer && collision.gameObject != gameObject) {
                Destroy(gameObject);
            }
        }

        [ContextMenu("Create Break Trigger")]
        public void CreateBreakTrigger() {
            GameObject breakTriggerObj = new GameObject("BreakTrigger");

            breakTriggerObj.transform.position = transform.position;
            breakTriggerObj.transform.SetParent(transform);

            BoxCollider2D collider = breakTriggerObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            //... add and assign moveTrigger component
            breakTrigger = breakTriggerObj.AddComponent<TriggerEvent>();
        }
    }
}
