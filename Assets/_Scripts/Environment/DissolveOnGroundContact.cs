using RoboRiftRush.Audio;
using RoboRiftRush.Triggers;
using UnityEngine;
using UnityEngine.U2D;

namespace RoboRiftRush.Environment {
    public class DissolveOnGroundContact : MonoBehaviour {

        [SerializeField] private TriggerEvent breakTrigger;

        private void OnEnable() {
            breakTrigger.OnTriggerEntered += TryBreak;
        }
        private void OnDisable() {
            breakTrigger.OnTriggerEntered -= TryBreak;
        }

        private void TryBreak(Collider2D collision){
            bool collidedWithIsland = collision.gameObject.layer == GameLayers.GroundLayer || collision.gameObject.layer == GameLayers.GrappleSurfaceLayer;
            if (collidedWithIsland && collision.gameObject != gameObject) {
                StartDissolving();
            }
        }

        private void Update() {
            HandleDissolve();
        }

        #region Disolve

        private Material fillMaterial;
        private Material edgeMaterial;

        private float startFillVisible;

        private void Awake() {
            SpriteShapeRenderer spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
            fillMaterial = spriteShapeRenderer.materials[0];
            edgeMaterial = spriteShapeRenderer.materials[1];

            startFillVisible = fillMaterial.GetFloat("_Fade");
        }

        private bool inFrame;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                inFrame = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.CameraFrameLayer) {
                inFrame = false;
            }
        }

        private bool dissolving = false;

        [SerializeField] private float dissolveDuration;
        private float dissolveTimer;

        public void StartDissolving() {
            dissolving = true;

            // only play sound if on screen
            if (inFrame) {
                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.IslandDissolve, 0.25f, 1f);
            }
        }

        // disolve then destroy
        private void HandleDissolve() {
            if (!dissolving) return;

            dissolveTimer += Time.deltaTime;

            //... goes from startFillVisible to 0 as dissolveTimer increases
            float fillVisible = Mathf.InverseLerp(dissolveDuration, 0, dissolveTimer) * startFillVisible;
            fillMaterial.SetFloat("_Fade", fillVisible);

            //... goes from 1 to 0 as dissolveTimer increases
            float edgeVisible = Mathf.InverseLerp(dissolveDuration, 0, dissolveTimer);
            edgeMaterial.SetFloat("_Fade", edgeVisible);

            if (dissolveTimer > dissolveDuration) {
                dissolveTimer = 0;
                dissolving = false;

                Destroy(gameObject);
            }
        }

        #endregion

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
