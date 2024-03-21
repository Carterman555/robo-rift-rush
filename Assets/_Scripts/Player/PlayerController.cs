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
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
                BoostHeld = Input.GetKey(KeyCode.LeftShift)
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();
            HandleCannonForce();
            //HandleSwing();
            
            ApplyMovement();
        }

        #region Collisions
        
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        [SerializeField] private float groundCheckDistance;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal
        private bool Boosting => _frameInput.BoostHeld;

        private void HandleDirection()
        {
            if (_swinging) return;

            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                float maxSpeed = Boosting ? _stats.BoostMaxSpeed : _stats.MaxSpeed;
                float acceleration = Boosting ? _stats.BoostAcceleration : _stats.BoostAcceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * maxSpeed, acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;

                if (_endedJumpEarly && _frameVelocity.y > 0) {
                    inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                }

                if (Boosting) {
                    inAirGravity = _stats.BoostGravity;
                }
                else if (_swinging) {
                    inAirGravity = _stats.SwingGravity;
                }

                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Tramp Boost

        public void HitTramp(Vector2 trampForward) {
            // Calculate the tramp force based on trampRotation and _frameVelocity

            float forceMagnitude = _frameVelocity.magnitude * _stats.TrampForceMult; // Use the player's current speed as the force magnitude
            forceMagnitude = Mathf.Max(forceMagnitude, _stats.MinTrampForce);

            Vector2 forceDirection = Vector2.Reflect(_frameVelocity.normalized, trampForward); // this should use the trampForward and the frameVelocity so it bounces off the opposite way the player bounced on

            Vector2 trampForce = forceDirection * forceMagnitude;

            _frameVelocity = trampForce; 
        }

        #endregion

        #region Cannon Force
        
        private bool _boostingInCannon;
        private Vector2 _cannonForce;

        public void AddCannonForce(Vector2 direction) {
            _boostingInCannon = true;
            _cannonForce = direction * _stats.CannonBoost;
        }

        private void HandleCannonForce() {
            if (_grounded) {
                _boostingInCannon = false;
            }

            if (_boostingInCannon) {
                
                _cannonForce.x = Mathf.MoveTowards(_cannonForce.x, 0f, _stats.CannonDeacceleration * Time.fixedDeltaTime);
                _cannonForce.y = Mathf.MoveTowards(_cannonForce.y, -_stats.MaxFallSpeed, _stats.FallAcceleration * Time.fixedDeltaTime);

                _frameVelocity = _cannonForce;
            }
        }
        

        #endregion

        #region Swing

        private bool _swinging;

        public void StartSwing(Vector3 grapplePosition) {
            _swinging = true;

            Vector3 directionToSwingPoint =  grapplePosition - transform.position; // Direction from player to swing point
            Vector3 swingDirection = new Vector3(directionToSwingPoint.y, -directionToSwingPoint.x).normalized; // Perpendicular to the directionToSwingPoint

            float currentSpeed = _frameVelocity.magnitude;

            //_frameVelocity = swingDirection * currentSpeed;
            print("Start swing");
        }

        public void StopSwing() {
            _swinging = false;
        }

        /// <summary>
        /// this method changes the velocity direction so that it move perpendicular the rope.
        /// it 
        /// </summary>

        /*
        private void HandleSwing() {
            
            if (_swinging) {
                Vector2 directionToSwingPoint =  _swingPoint - (Vector2)transform.position; // Direction from player to swing point

                float horDirection = Mathf.Sign(_swingPoint.x - transform.position.x);

                Vector2 swingDirection = new Vector2(directionToSwingPoint.y, -directionToSwingPoint.x) * horDirection; // Perpendicular to the directionToSwingPoint
                float originalSpeed = _frameVelocity.magnitude;

                _frameVelocity = swingDirection.normalized * originalSpeed; // keep the same speed but set direction to swingDirection
            }
        }
        */

        /*
        private void HandleSwing() {
            
            if (_swinging) {
                Vector2 directionToSwingPoint =  _swingPoint - (Vector2)transform.position; // Direction from player to swing point
                Vector2 swingDirection = new Vector2(directionToSwingPoint.y, -directionToSwingPoint.x); // Perpendicular to the directionToSwingPoint
                


                float distanceToSwingPoint = directionToSwingPoint.magnitude;

                float currentSpeed = _frameVelocity.magnitude;
                float swingStrengthModifier = Mathf.Clamp01(1 - distanceToSwingPoint / 10f);
                
                Vector2 desiredVelocity = swingDirection.normalized * _swingSpeed * swingStrengthModifier * currentSpeed; // Calculate the desired velocity vector
                
                _frameVelocity = desiredVelocity; // Directly set the Rigidbody2D's velocity
            }
        }
        */

        #endregion

        private void ApplyMovement() {
            _rb.velocity = _frameVelocity;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
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