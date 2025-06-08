using UnityEngine;
using Character.Core.Enums;
using Character.Core.State;
using UnityEngine.Events;

namespace Character.Core.Base
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Character_StateHandler))]
    public class Character_Movement : MonoBehaviour
    {
        #region Private Fields
        private Character_StateHandler _stateHandler;
        private CharacterController _characterController;
        private Camera _mainCamera;

        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _runSpeed = 6f;
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _turnSpeed = 10f; // Degrees per second

        [Header("Ground Detection")]
        [SerializeField] private float _groundCheckDistance = 0.05f;
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask _groundLayerMask = -1;
        [SerializeField] private float _groundedBuffer = 0.1f; // Coyote time


        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _wasGroundedLastFrame;
        private float _timeSinceGrounded;
        #endregion

        #region Properties
        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float CurrentSpeed { get; private set; }
        public bool IsGrounded => _isGrounded;
        public Vector3 Velocity => _velocity;
        #endregion

        #region Events
        [Header("Idle")]
        public UnityEvent OnIdle;
        [Header("Walking")]
        public UnityEvent OnWalking;
        [Header("Jumping")]
        public UnityEvent OnJumping;
        [Header("Running")]
        public UnityEvent OnRunning;
        [Header("Rolling")]
        public UnityEvent OnRolling;
        [Header("Flying")]
        public UnityEvent OnFlying;
        [Header("Climbing")]
        public UnityEvent OnClimbing;
        #endregion
        #region Unity Lifecycle
        public virtual void Awake()
        {
            InitializeComponents();
        }

        public virtual void Update()
        {
            UpdateGroundedState();
            ApplyGravity();
            
            // Sadece gravity uygula, horizontal movement ayrı olsun
            ApplyVerticalMovement();
            
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            _stateHandler = GetComponent<Character_StateHandler>();
            _characterController = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (_stateHandler == null)
                Debug.LogError($"Character_StateHandler not found on {gameObject.name}");

            if (_characterController == null)
                Debug.LogError($"CharacterController not found on {gameObject.name}");
            
            if (_mainCamera == null)
                Debug.LogError($"Main Camera not found in scene");
        }
        #endregion

        #region Movement Methods
        public virtual void Move(Vector3 direction)
        {
            if (!CanMove()) return;

            // Convert input direction to camera-relative direction
            Vector3 cameraForward = _mainCamera.transform.forward;
            Vector3 cameraRight = _mainCamera.transform.right;
            
            // Project camera vectors onto horizontal plane
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate camera-relative movement direction
            Vector3 moveDirection = cameraForward * direction.z + cameraRight * direction.x;
            moveDirection.y = 0;
            moveDirection.Normalize();

            // Handle rotation if we have a direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
            }

            // Apply horizontal movement
            Vector3 horizontalMovement = moveDirection * CurrentSpeed * Time.deltaTime;
            _characterController.Move(horizontalMovement);
        }

        private void ApplyVerticalMovement()
        {
            if (!CanMove()) return;

            // Sadece vertical movement uygula
            Vector3 verticalMovement = new Vector3(0, _velocity.y, 0) * Time.deltaTime;
            _characterController.Move(verticalMovement);
        }

        public virtual void Walk(Vector3 direction = default)
        {
            if (!CanChangeMovementState()) return;

            if(direction == Vector3.zero && _isGrounded)
            {
                _stateHandler.ChangeState(CharacterState.Idle);
                OnIdle?.Invoke();
            }
            else
            {
                _stateHandler.ChangeState(CharacterState.Walking);
            }

            if (direction != Vector3.zero && _isGrounded)
            {
                OnWalking?.Invoke();
            }

            CurrentSpeed = _walkSpeed;
            Move(direction);
        }

        public virtual void Run(Vector3 direction = default)
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Running);
            CurrentSpeed = _runSpeed;

            if (direction != Vector3.zero)
            {
                Move(direction);
            }
            if (direction == Vector3.zero && _isGrounded)
            {
                _stateHandler.ChangeState(CharacterState.Idle);
                OnIdle?.Invoke();
            }

            if (direction != Vector3.zero && _isGrounded)
            {
                OnRunning?.Invoke();
            }

        }

        public virtual void Jump()
        {
            if (!CanJump()) return;

            _stateHandler.ChangeState(CharacterState.Jumping);
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            OnJumping?.Invoke();
        }

        public virtual void Roll()
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Rolling);
            // Add roll logic here
            OnRolling?.Invoke();
        }

        public virtual void Fly()
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Flying);
            // Add flying logic here
            OnFlying?.Invoke();
        }

        public virtual void Climb()
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Climbing);
            // Add climbing logic here
            OnClimbing?.Invoke();
        }

        public virtual void Stop()
        {
            CurrentSpeed = 0f;
            _stateHandler.ChangeState(CharacterState.Idle);
        }
        #endregion

        #region State Validation
        private bool CanMove()
        {
            return _stateHandler != null &&
                   _characterController != null &&
                   _characterController.enabled;
        }

        private bool CanChangeMovementState()
        {
            if (!CanMove()) return false;

            var currentState = _stateHandler.CurrentState;
            return currentState != CharacterState.Dead &&
                   currentState != CharacterState.Stunned;
        }

        private bool CanJump()
        {
            // Coyote time: Kısa süre önce yerdeyse atlayabilir
            return CanChangeMovementState() && (_isGrounded || _timeSinceGrounded <= _groundedBuffer);
        }
        #endregion

        #region Physics
        private void UpdateGroundedState()
        {
            _wasGroundedLastFrame = _isGrounded;
            
            // Önce CharacterController'ın ground detection'ını kontrol et
            // Jump sırasında yukarı hareket ediyorken ground'u ignore et
            bool controllerGrounded = _characterController.isGrounded && _velocity.y <= 0.1f;
            
            // Sphere cast ile double check (sadece aşağı düşerken)
            bool spherecastGrounded = false;
            if (_velocity.y <= 0.1f) // Sadece düşerken sphere cast yap
            {
                spherecastGrounded = CheckGroundWithSphereCast();
            }
            
            _isGrounded = controllerGrounded || spherecastGrounded;
            
            // Jump başlangıcında bir frame ground'u false yap
            if (_velocity.y > 2f) // Jump threshold
            {
                _isGrounded = false;
            }
            
            // Coyote time tracking
            if (_isGrounded)
            {
                _timeSinceGrounded = 0f;
            }
            else
            {
                _timeSinceGrounded += Time.deltaTime;
            }
        }

        private bool CheckGroundWithSphereCast()
        {
            Vector3 spherePosition = transform.position + Vector3.up * (_characterController.radius + _groundCheckDistance);
            
            return Physics.SphereCast(
                spherePosition,
                _groundCheckRadius,
                Vector3.down,
                out RaycastHit hit,
                _groundCheckDistance + _characterController.radius,
                _groundLayerMask
            );
        }

        private void ApplyGravity()
        {
            if (_isGrounded)
            {
                // Yerdeyken ve aşağı düşüyorken velocity'yi sıfırla
                if (_velocity.y < 0)
                {
                    _velocity.y = -0.5f; // Slope'larda kaymaması için
                }
                // Yukarı hareket ediyorken (zıplama) dokunma
            }
            else
            {
                // Havadayken gravity uygula
                _velocity.y += _gravity * Time.deltaTime;
                
                // Terminal velocity sınırı
                _velocity.y = Mathf.Max(_velocity.y, _gravity * 2f);
            }
        }
        #endregion

        #region Debug
        private void OnDrawGizmos()
        {
            if (_characterController != null)
            {
                // Ground check sphere
                Vector3 spherePosition = transform.position + Vector3.up * (_characterController.radius + _groundCheckDistance);
                Gizmos.color = _isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(spherePosition, _groundCheckRadius);
                
                // Ground check ray
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(spherePosition, spherePosition + Vector3.down * (_groundCheckDistance + _characterController.radius));
                
                // Character position indicator
                Gizmos.color = _isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position, 0.1f);
            }
        }
        #endregion
    }
}