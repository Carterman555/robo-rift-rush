using RoboRiftRush;
using RoboRiftRush.Triggers;
using System;
using UnityEngine;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
    /// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats stats;
        private Rigidbody2D rb;
        private CapsuleCollider2D col;
        private FrameInput frameInput;
        private Vector2 frameVelocity;
        private bool cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => frameInput.Move;
        public Vector2 FrameVelocity => frameVelocity;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        public void Disable() {
            rb.velocity = Vector2.zero;
            frameInput.Move = Vector2.zero;
            enabled = false;
        }

        #endregion

        private float time;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CapsuleCollider2D>();

            cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            frameInput = new FrameInput {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            };

            if (stats.SnapInput)
            {
                frameInput.Move.x = Mathf.Abs(frameInput.Move.x) < stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(frameInput.Move.x);
                frameInput.Move.y = Mathf.Abs(frameInput.Move.y) < stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(frameInput.Move.y);
            }

            if (forcingRun) {
                frameInput.Move.x = forceRunDirection;
            }

            if (frameInput.JumpDown)
            {
                jumpToConsume = true;
                timeJumpWasPressed = time;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();
            HandleCannonForce();
            HandleSwing();

            ApplyMovement();
        }

        #region Collisions
        
        private float frameLeftGrounded = float.MinValue;
        private bool grounded;

        private void CheckCollisions()
        {
            // control whether raycasts or linecasts originating from within colliders should include those colliders in their results or not
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            bool groundHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down, stats.GrounderDistance, ~stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.up, stats.GrounderDistance, ~stats.PlayerLayer);

            // if hits ceiling, stop moving up
            if (ceilingHit) frameVelocity.y = Mathf.Min(0, frameVelocity.y);

            // Landed on the Ground
            if (!grounded && groundHit)
            {
                grounded = true;
                coyoteUsable = true;
                bufferedJumpUsable = true;
                endedJumpEarly = false;
                inAirFromSwing = false;
                inAirFromBoost = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(frameVelocity.y));
            }
            // Left the Ground
            else if (grounded && !groundHit)
            {
                grounded = false;
                frameLeftGrounded = time;
                GroundedChanged?.Invoke(false, 0);
            }

            // smash player
            if (groundHit && ceilingHit) {
                PlayerAnimator.Instance.StartDeathFade();
                col.enabled = false;
            }

            Physics2D.queriesStartInColliders = cachedQueryStartInColliders;
        }

        #endregion

        #region Jumping

        private bool jumpToConsume;
        private bool bufferedJumpUsable;
        private bool endedJumpEarly;
        private bool coyoteUsable;
        private float timeJumpWasPressed;

        private bool HasBufferedJump => bufferedJumpUsable && time < timeJumpWasPressed + stats.JumpBuffer;
        private bool CanUseCoyote => coyoteUsable && !grounded && time < frameLeftGrounded + stats.CoyoteTime;

        private bool InAirFromJump => !inAirFromSwing && !inAirFromBoost;

        private void HandleJump()
        {
            if (!endedJumpEarly && !grounded && !frameInput.JumpHeld && rb.velocity.y > 0 && InAirFromJump) endedJumpEarly = true;

            if (!jumpToConsume && !HasBufferedJump) return;

            if (grounded || CanUseCoyote) ExecuteJump();

            jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            endedJumpEarly = false;
            timeJumpWasPressed = 0;
            bufferedJumpUsable = false;
            coyoteUsable = false;
            frameVelocity.y = stats.JumpPower;

            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        // force player run for win animation
        private int forceRunDirection;
        private bool forcingRun = false;

        public void ForceRun(int direction) {
            forcingRun = true;
            forceRunDirection = direction;
        }

        public void StopForceRun() {
            forcingRun = false;
        }

        private void HandleDirection()
        {
            if (swinging) return;

            if (forcingRun) {
                frameInput.Move.x = forceRunDirection;
            }

            if (frameInput.Move.x == 0)
            {
                var deceleration = grounded ? stats.GroundDeceleration : stats.AirDeceleration;
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                float acceleration = stats.BoostAcceleration;

                // if the player is changing directions, allow them to do it much faster
                if (Mathf.Sign(frameVelocity.x) != Mathf.Sign(frameInput.Move.x)) {
                    var deceleration = grounded ? stats.GroundDeceleration : stats.AirDeceleration;
                    acceleration += deceleration;
                }

                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, frameInput.Move.x * stats.MaxSpeed, acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (swinging) return;

            if (grounded && frameVelocity.y <= 0f)
            {
                frameVelocity.y = stats.GroundingForce;
            }
            else
            {
                var inAirGravity = stats.FallAcceleration;

                if (endedJumpEarly && frameVelocity.y > 0) {
                    inAirGravity *= stats.JumpEndEarlyGravityModifier;
                }

                frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Tramp Boost

        public void HitTramp(Vector2 trampForward) {
            // Calculate the tramp force based on trampRotation and _frameVelocity

            float forceMagnitude = frameVelocity.magnitude * stats.TrampForceMult; // Use the player's current speed as the force magnitude
            forceMagnitude = Mathf.Max(forceMagnitude, stats.MinTrampForce);

            Vector2 forceDirection = Vector2.Reflect(frameVelocity.normalized, trampForward); // this should use the trampForward and the frameVelocity so it bounces off the opposite way the player bounced on

            Vector2 trampForce = forceDirection * forceMagnitude;

            frameVelocity = trampForce; 
        }

        #endregion

        #region Cannon Force
        
        private bool boostingInCannon;
        private Vector2 cannonForce;

        public void AddCannonForce(Vector2 direction) {
            boostingInCannon = true;
            cannonForce = direction * stats.CannonBoost;
        }

        private void HandleCannonForce() {
            if (grounded) {
                boostingInCannon = false;
            }

            if (boostingInCannon) {
                
                cannonForce.x = Mathf.MoveTowards(cannonForce.x, 0f, stats.CannonDeacceleration * Time.fixedDeltaTime);
                cannonForce.y = Mathf.MoveTowards(cannonForce.y, -stats.MaxFallSpeed, stats.FallAcceleration * Time.fixedDeltaTime);

                frameVelocity = cannonForce;
            }
        }


        #endregion

        #region Swing

        private bool swinging;
        private bool inAirFromSwing; // so doesn't fall fast after swing from JumpEndEarlyGravityModifier

        private float swingSpeed;

        private bool swingingLeft;
        private bool stoppedAtBottomOfSwing;

        private Vector3 grapplePosition;

        private Vector3 swingDirection;

        public void StartSwing(Vector3 grapplePosition) {

            swinging = true;
            inAirFromSwing = true;
            stoppedAtBottomOfSwing = false;
            this.grapplePosition = grapplePosition;

            //... direction from the player to the grapple point
            Vector3 toGrapple = (this.grapplePosition - transform.position).normalized;

            //... perpendicular to the direction
            Vector3 swingDirection = toGrapple.PerpendicularDirection().normalized;

            float projectedSpeed = Vector3.Dot(frameVelocity, swingDirection);
            swingSpeed = Mathf.Abs(projectedSpeed);
            swingingLeft = projectedSpeed < 0f;

            swingSpeed += stats.StartSwingBoost;

            if (swingSpeed < stats.MinStartSwingSpeed) {
                swingSpeed = stats.MinStartSwingSpeed;
            }

        }

        // used when the object the player is grappled to is moving
        public void UpdateGrapplePos(Vector3 grapplePosition) {
            this.grapplePosition = grapplePosition;
        }

        public void StopSwing() {

            //... so doesn't fall fast after swing
            endedJumpEarly = false;

            // apply a boost on release
            if (swinging) {
                frameVelocity += (Vector2)swingDirection * stats.ReleaseBoost;
            }

            swinging = false;
        }

        private void HandleSwing() {
            if (swinging) {

                if (frameInput.Move.x != 0) {
                    stoppedAtBottomOfSwing = false;
                }

                // to avoid jittering caused by player rapidly switching directions at bottom of swing
                if (stoppedAtBottomOfSwing) {
                    frameVelocity = Vector2.zero;
                    return;
                }

                //... direction from the player to the grapple point
                Vector3 toGrapple = (grapplePosition - transform.position).normalized;

                swingDirection = toGrapple.PerpendicularDirection().normalized;
                if (swingingLeft) {
                    swingDirection = -swingDirection;
                }

                // swing back and forth
                bool leftOfGrapple = toGrapple.x > 0f;
                bool swingingUpwards = (swingingLeft && leftOfGrapple) || (!swingingLeft && !leftOfGrapple);
                if (swingingUpwards) {
                    DecreaseSwingSpeed(stats.SwingAcceleration);
                }
                else {
                    TryIncreaseSwingSpeed(stats.SwingAcceleration);
                }

                // player can control momentum
                HandlePlayerControl();

                // when player slows down to a stop, switch directions and check if should stop swinging
                if (swingSpeed <= 0.01f) {
                    swingingLeft = !swingingLeft;

                    // switching directions when player is very close to bottom of swing causes jittering, so stop the player
                    if (Mathf.Abs(toGrapple.x) < 0.01f) {
                        transform.position = new Vector3(grapplePosition.x, transform.position.y);
                        stoppedAtBottomOfSwing = true;
                    }
                }

                frameVelocity = swingDirection * swingSpeed;
            }
        }

        /// <summary>
        /// Calculate the potential energy using speed and height. The higher the potential energy, the less control
        /// the player has in speeding up the momentum and the more control they have over slowing the momentum. This
        /// allows the player to gain momentum quickly when swinging slowly while limiting them on how fast they can swing.
        /// </summary>
        private void HandlePlayerControl() {
            Vector2 toGrapple = (Vector2)(grapplePosition - transform.position);

            float toBottomAngle = Vector2.Angle(toGrapple, new Vector2(0, toGrapple.magnitude));
            float travelDistanceToBottom = 2 * Mathf.PI * toGrapple.magnitude * (toBottomAngle / 360f); // circumference(2πr) * percent of circumference to bottom

            // predict the fastest speed (when at bottom of swing)
            int count = 0;
            float distanceRemaining = travelDistanceToBottom;
            float protentialSpeed = swingSpeed;
            while (distanceRemaining > 0) {
                count++;
                if (count > 9999) {
                    Debug.LogWarning("Loop Ran 9999 times!");
                    break;
                }

                protentialSpeed += stats.Acceleration * Time.fixedDeltaTime;
                distanceRemaining -= protentialSpeed * Time.fixedDeltaTime;
            }

            //... the higher the potential energy, the less control the player has in speeding up the momentum
            float speedUpControlMult = Mathf.Max(0, Mathf.Lerp(4, 0, protentialSpeed / stats.MaxSwingSpeedControl));
            float slowDownControlMult = Mathf.Max(0.1f, 1 / speedUpControlMult); // min at 0.1 because can't divide by 0

            // prevent glitch where the player can slowly gaining height when at top of swing
            bool atTopOfSwing = toBottomAngle > 1f && swingSpeed < 2f;
            if (atTopOfSwing) {
                return;
            }

            bool inputWithSwingDirection = (swingingLeft && frameInput.Move.x < 0) || (!swingingLeft && frameInput.Move.x > 0);
            bool inputAgainstSwingDirection = (swingingLeft && frameInput.Move.x > 0) || (!swingingLeft && frameInput.Move.x < 0);

            if (inputWithSwingDirection) {
                TryIncreaseSwingSpeed(stats.MomentumControlAcceleration * speedUpControlMult);
            }
            else if (inputAgainstSwingDirection) {
                DecreaseSwingSpeed(stats.MomentumControlAcceleration * slowDownControlMult);
            }
        }

        public bool GroundBlockingPlayer => (leftGroundBlockingPlayer && swingingLeft) || (rightGroundBlockingPlayer && !swingingLeft);

        /// <summary>
        /// There is a left and right trigger to detect if the player is touching the ground. This is so that the
        /// when the ground is blocking their swing movement so they can't gain speed and launch upon releasing.
        /// The two different triggers allows the player to increase speed when the ground is not blocking the swinging
        /// direction.
        /// </summary>
        private void TryIncreaseSwingSpeed(float acceleration) {
            if (!GroundBlockingPlayer) {
                swingSpeed += acceleration * Time.fixedDeltaTime;
            }
        }

        private void DecreaseSwingSpeed(float deacceleration) {
            swingSpeed = Mathf.MoveTowards(swingSpeed, 0, deacceleration * Time.fixedDeltaTime);
        }

        #region Ground Triggers
        private bool leftGroundBlockingPlayer; // ground is block player from swinging to the left
        private bool rightGroundBlockingPlayer; // ground is block player from swinging to the right

        [SerializeField] private TriggerEvent leftGroundTrigger;
        [SerializeField] private TriggerEvent rightGroundTrigger;

        private void OnEnable() {
            leftGroundTrigger.OnTriggerEntered += OnLeftGroundTriggerEnter;
            leftGroundTrigger.OnTriggerExited += OnLeftGroundTriggerExit;
            rightGroundTrigger.OnTriggerEntered += OnRightGroundTriggerEnter;
            rightGroundTrigger.OnTriggerExited += OnRightGroundTriggerExit;
        }

        private void OnLeftGroundTriggerEnter(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.GroundLayer) {
                leftGroundBlockingPlayer = true;
            }
        }

        private void OnLeftGroundTriggerExit(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.GroundLayer) {
                leftGroundBlockingPlayer = false;
            }
        }

        private void OnRightGroundTriggerEnter(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.GroundLayer) {
                rightGroundBlockingPlayer = true;
            }
        }

        private void OnRightGroundTriggerExit(Collider2D collision) {
            if (collision.gameObject.layer == GameLayers.GroundLayer) {
                rightGroundBlockingPlayer = false;
            }
        } 
        #endregion

        #endregion

        #region Boost Area Force

        private bool inAirFromBoost;

        public void Boost(Vector2 force) {
            frameVelocity += force;
            inAirFromBoost = true;
        }

        #endregion

        #region Move With Environment

        private Vector2 environmentVelocity;

        public void SetEnvironmentVelocity(Vector2 velocity) {
            environmentVelocity = velocity;
        }

        #endregion

        private void ApplyMovement() {
            rb.velocity = frameVelocity + environmentVelocity;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }

        public void Disable();
    }
}