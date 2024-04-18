using System;
using UnityEditor;
using UnityEngine;

namespace SpeedPlatformer.Environment {
    public class BreakOnGroundContact : MonoBehaviour {

        [SerializeField] private TriggerEvent breakTrigger;

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.layer == GameLayers.GroundLayer) {
                Destroy(gameObject);
            }
        }

        public void CreateBreakTrigger(Transform moveTriggerContainer) {
            // TODO
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
