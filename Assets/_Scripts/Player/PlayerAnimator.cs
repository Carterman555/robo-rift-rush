using System;
using UnityEngine;
using UnityEngine.U2D;

namespace TarodevController {
    /// <summary>
    /// VERY primitive animator example.
    /// </summary>
    public class PlayerAnimator : MonoBehaviour {
        [Header("References")]
        [SerializeField]
        private Animator anim;

        [SerializeField] private Transform playerTransform;

        [Header("Particles")]
        [SerializeField] private ParticleSystem jumpParticles;
        [SerializeField] private ParticleSystem landParticles;
        [SerializeField] private ParticleSystem moveGroundParticles;
        [SerializeField] private ParticleSystem moveAirParticles;

        //[Header("Audio Clips")] [SerializeField]
        //private AudioClip[] _footsteps;

        //private AudioSource _source;
        private IPlayerController player;
        private bool grounded;
        private ParticleSystem.MinMaxGradient currentGradient;

        private static readonly int IsRunningKey = Animator.StringToHash("running");
        private static readonly int TakeOffKey = Animator.StringToHash("takeOff");
        private static readonly int GroundedKey = Animator.StringToHash("grounded");

        private void Awake() {
            //_source = GetComponent<AudioSource>();
            player = GetComponentInParent<IPlayerController>();
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
            if (player == null) return;

            DetectGroundColor();

            HandlePlayerDirection();

            HandleIdleSpeed();
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

        private void HandleIdleSpeed() {

            bool hasHorizontalInput = player.FrameInput.x != 0;
            anim.SetBool(IsRunningKey, hasHorizontalInput);

            if (hasHorizontalInput && !moveAirParticles.isPlaying) {
                moveAirParticles.Play();
            }
            else if (!hasHorizontalInput && moveGroundParticles.isPlaying) {
                moveAirParticles.Stop();
            }

            float inputStrength = Mathf.Abs(player.FrameInput.x);
            moveGroundParticles.transform.localScale = Vector3.MoveTowards(moveGroundParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);
        }

        private void OnJumped() {
            anim.SetTrigger(TakeOffKey);

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

                //_source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
                moveGroundParticles.Play();
                moveAirParticles.Play();

                landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
                landParticles.Play();
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
    }
}