using RoboRiftRush.Audio;
using RoboRiftRush.Management;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

namespace TarodevController {
    public class PlayerAnimator : StaticInstance<PlayerAnimator> {

        [Header("References")]
        [SerializeField]
        private Animator anim;

        [SerializeField] private Transform playerTransform;

        [Header("Particles")]
        [SerializeField] private ParticleSystem jumpParticles;
        [SerializeField] private ParticleSystem landParticles;
        [SerializeField] private ParticleSystem moveGroundParticles;
        [SerializeField] private ParticleSystem moveAirParticles;

        private Material[] materials;

        private IPlayerController player;
        private bool grounded;
        private ParticleSystem.MinMaxGradient currentGradient;

        private static readonly int IsRunningKey = Animator.StringToHash("running");
        private static readonly int TakeOffKey = Animator.StringToHash("takeOff");
        private static readonly int GroundedKey = Animator.StringToHash("grounded");

        // static to persist through scene change
        private static AudioSource windSource;

        protected override void Awake() {
            base.Awake();
            player = GetComponentInParent<IPlayerController>();

            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            materials = spriteRenderers.Select(x => x.material).ToArray();
        }

        private IEnumerator Start() {

            //... wait a frame to allow audio system to initialize
            yield return null;
            if (windSource == null) {
                windSource = AudioSystem.Instance.PlayLoopingSound(AudioSystem.SoundClips.Wind, 0.5f);
            }

            AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.StartLevel, 0, 0.75f);
        }

        private void OnEnable() {
            player.Jumped += OnJumped;
            player.GroundedChanged += OnGroundedChanged;

            moveGroundParticles.Play();
        }

        private void OnDisable() {
            player.Jumped -= OnJumped;
            player.GroundedChanged -= OnGroundedChanged;

            moveGroundParticles.Stop();
        }

        private void Update() {
            if (player == null || PauseManager.Paused) {
                windSource.volume = 0;
                return;
            }

            DetectGroundColor();

            HandlePlayerDirection();

            HandleIdleSpeed();

            HandleDeathFade();
        }

        private bool facingLeft = false;

        private void HandlePlayerDirection() {
            // face right if right input
            if (player.FrameInput.x > 0 && facingLeft) {
                playerTransform.localScale = new Vector3(Mathf.Abs(playerTransform.localScale.x), playerTransform.localScale.y);
                facingLeft = false;
            }
            // face left if left input
            else if (player.FrameInput.x < 0 && !facingLeft) {
                playerTransform.localScale = new Vector3(-Mathf.Abs(playerTransform.localScale.x), playerTransform.localScale.y);
                facingLeft = true;
            }
        }

        private bool walking;

        private void HandleIdleSpeed() {

            bool hasHorizontalInput = player.FrameInput.x != 0;
            anim.SetBool(IsRunningKey, hasHorizontalInput);

            if (hasHorizontalInput && !moveAirParticles.isPlaying) {
                moveAirParticles.Play();
            }
            else if (!hasHorizontalInput && moveGroundParticles.isPlaying) {
                moveAirParticles.Stop();
            }

            bool nowWalking = hasHorizontalInput && grounded;
            if (!walking && nowWalking) {
                AudioSystem.Instance.SetWalking(true);
                walking = true;
            }
            if (walking && !nowWalking) {
                AudioSystem.Instance.SetWalking(false);
                walking = false;
            }

            float inputStrength = Mathf.Abs(player.FrameInput.x);
            moveGroundParticles.transform.localScale = Vector3.MoveTowards(moveGroundParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);

            if (windSource != null) {
                float windVolume = Mathf.InverseLerp(0, 60f, Mathf.Abs(playerTransform.GetComponent<Rigidbody2D>().velocity.x));
                windSource.volume = windVolume;
            }
        }

        private void OnJumped() {
            anim.SetTrigger(TakeOffKey);

            AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.Jump, 0);

            if (grounded) // Avoid coyote
            {
                SetColor(jumpParticles);
                jumpParticles.Play();
            }
        }

        private void OnGroundedChanged(bool grounded, float impact) {
            this.grounded = grounded;

            if (grounded) {
                anim.SetBool(GroundedKey, true);

                DetectGroundColor();
                SetColor(landParticles);

                moveGroundParticles.Play();
                moveAirParticles.Play();

                landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
                landParticles.Play();

                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.Land, 0, impact / 60f);
            }
            else {
                anim.SetBool(GroundedKey, false);
                moveGroundParticles.Stop();
                moveAirParticles.Stop();
            }
        }

        [SerializeField] private Color IslandParticlesEnv1;

        private void DetectGroundColor() {
            var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

            if (!hit || hit.collider.isTrigger) return;

            Color color;
            if (hit.transform.TryGetComponent(out SpriteRenderer renderer)) {
                Sprite sprite = renderer.sprite;

                Texture2D texture = sprite.texture;

                // Convert the sprite's rect from pixel space to UV space
                Rect rect = sprite.textureRect;
                rect.x /= texture.width;
                rect.y /= texture.height;
                rect.width /= texture.width;
                rect.height /= texture.height;

                // Sample the color from the center of the sprite
                color = texture.GetPixelBilinear(rect.center.x, rect.center.y);
            }
            else if (hit.transform.TryGetComponent(out SpriteShapeController shapeController)) {
                color = IslandParticlesEnv1;
            }
            else {
                return;
            }

            currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
            SetColor(moveGroundParticles);
        }

        private void SetColor(ParticleSystem ps) {
            var main = ps.main;
            main.startColor = currentGradient;
        }

        #region Death Fade

        [SerializeField] private float fadeDuration;
        private float fadeTimer;

        private bool fading;

        public void StartDeathFade() {
            fading = true;

            AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.TrapDeath, 0, 0.5f);
        }

        public bool IsFading() {
            return fading;
        }

        /// <summary>
        /// fade the player out then reset the level
        /// </summary>
        private void HandleDeathFade() {
            if (!fading) return;

            fadeTimer += Time.deltaTime;

            float amountVisible = Mathf.InverseLerp(fadeDuration, 0, fadeTimer);
            foreach (Material material in materials) {
                material.SetFloat("_Fade", amountVisible);
            }

            player.Disable();

            if (fadeTimer > fadeDuration) {
                fadeTimer = 0;
                fading = false;

                GameProgress.ResetLevel();
            }
        }

        #endregion
    }
}