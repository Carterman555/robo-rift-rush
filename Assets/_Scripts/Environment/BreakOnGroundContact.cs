using System;
using Unity.VisualScripting;
using UnityEditor;
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

        private void Update() {
            breakTrigger.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

        public void CreateBreakTrigger(Transform breakTriggerContainer) {
            GameObject breakTriggerObj = Instantiate(new GameObject(), breakTriggerContainer);

            breakTriggerObj.transform.position = transform.position;
            breakTriggerObj.name = "BreakTrigger_" + name.TryGetEndingNumber('_');

            BoxCollider2D collider = breakTriggerObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            //... add and assign moveTrigger component
            breakTrigger = breakTriggerObj.AddComponent<TriggerEvent>();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BreakOnGroundContact))]
    public class BreakOnGroundContactEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            BreakOnGroundContact breakOnGroundContact = target as BreakOnGroundContact;

            // spawn in move trigger
            if (GUILayout.Button("Create Break Trigger")) {

                if (Helpers.TryFindByName(out GameObject triggerContainer, "BreakTriggers")) {
                    breakOnGroundContact.CreateBreakTrigger(triggerContainer.transform);
                }
            }
        }
    }
#endif
}
