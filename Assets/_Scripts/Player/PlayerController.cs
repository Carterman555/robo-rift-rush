using System;
using System.Runtime.CompilerServices;
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
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        public bool BoostInput => frameInput.BoostHeld;

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
            frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
                BoostHeld = Input.GetKey(KeyCode.LeftShift)
            };

            if (stats.SnapInput)
            {
                frameInput.Move.x = Mathf.Abs(frameInput.Move.x) < stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(frameInput.Move.x);
                frameInput.Move.y = Mathf.Abs(frameInput.Move.y) < stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(frameInput.Move.y);
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
            HandleGrapple();

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

        private bool canBoost = true;
        private bool Boosting => BoostInput && canBoost;

        public void SetCanBoost(bool canBoost) {
            this.canBoost = canBoost;
        }

        private void HandleDirection()
        {
            if (swinging || grappling) return;

            if (frameInput.Move.x == 0)
            {
                var deceleration = grounded ? stats.GroundDeceleration : stats.AirDeceleration;
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                float maxSpeed = Boosting ? stats.BoostMaxSpeed : stats.MaxSpeed;
                float acceleration = Boosting ? stats.BoostAcceleration : stats.Acceleration;

                // if the player is changing directions, allow them to do it much faster
                if (Mathf.Sign(frameVelocity.x) != Mathf.Sign(frameInput.Move.x)) {
                    var deceleration = grounded ? stats.GroundDeceleration : stats.AirDeceleration;
                    acceleration += deceleration;
                }

                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, frameInput.Move.x * maxSpeed, acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (swinging || grappling) return;

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

                if (Boosting) {
                    inAirGravity = stats.BoostGravity;
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

        Vector3 grapplePosition;

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

            swingingLeft = projectedSpeed > 0f;
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
                frameVelocity += Vector2.one * swingSpeed * stats.ReleaseBoost;
            }

            swinging = false;
        }

        private void HandleSwing() {
            if (swinging) {

                // to avoid jittering caused by player rapidly switching directions at bottom of swing
                if (stoppedAtBottomOfSwing) {
                    frameVelocity = Vector2.zero;
                    return;
                }

                //... direction from the player to the grapple point
                Vector3 toGrapple = (grapplePosition - transform.position).normalized;

                //... perpendicular to the direction
                Vector3 swingDirection = toGrapple.PerpendicularDirection().normalized; 

                bool leftOfGrapple = toGrapple.x < 0f;

                // swing the player back and forth
                if (swingingLeft) {
                    if (leftOfGrapple) {
                        swingSpeed = Mathf.MoveTowards(swingSpeed, 0, stats.SwingAcceleration * Time.fixedDeltaTime);
                    }
                    else {
                        swingSpeed += stats.SwingAcceleration * Time.fixedDeltaTime;
                    }
                }
                else {
                    if (leftOfGrapple) {
                        swingSpeed += stats.SwingAcceleration * Time.fixedDeltaTime;
                    }
                    else {
                        swingSpeed = Mathf.MoveTowards(swingSpeed, 0, stats.SwingAcceleration * Time.fixedDeltaTime);
                    }

                    swingDirection = -swingDirection;
                }

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

        #endregion

        #region Grapple Force

        private bool grappling;
        private Vector3 grapplingHookPosition;
        private float grappleSpeed;

        public void StartGrappling(Vector3 grapplePosition) {
            grappling = true;
            grapplingHookPosition = grapplePosition;

            //... direction from the player to the grapple point
            Vector3 toGrapple = (grapplingHookPosition - transform.position).normalized;

            grappleSpeed = maxGrappleSpeed;
        }

        public void StopGrappling() {
            grappling = false;

            //... so doesn't fall fast after grapple
            endedJumpEarly = false;
        }

        [SerializeField] private float maxGrappleSpeed; 
        [SerializeField] private float grappleAcceleration; 

        private void HandleGrapple() {
            if (!grappling) return;

            //... direction from the player to the grapple point
            Vector3 toGrapple = (grapplingHookPosition - transform.position).normalized;

            //... increase speed
            grappleSpeed = Mathf.MoveTowards(grappleSpeed, maxGrappleSpeed, grappleAcceleration * Time.fixedDeltaTime);

            frameVelocity = toGrapple * grappleSpeed;
        }

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
        public bool BoostHeld;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}