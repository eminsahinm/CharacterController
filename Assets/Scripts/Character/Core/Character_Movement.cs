using UnityEngine;
using Character.Core.Enums;
using Character.Core.State;

namespace Character.Core.Base
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Character_StateHandler))]
    public class Character_Movement : MonoBehaviour
    {
        #region Private Fields
        private Character_StateHandler _stateHandler;
        private CharacterController _characterController;

        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _runSpeed = 6f;
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _gravity = -9.81f;

        private Vector3 _velocity;
        private bool _isGrounded;
        #endregion

        #region Properties
        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float CurrentSpeed { get; private set; }
        public bool IsGrounded => _isGrounded;
        public Vector3 Velocity => _velocity;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Update()
        {
            UpdateGroundedState();
            ApplyGravity();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            _stateHandler = GetComponent<Character_StateHandler>();
            _characterController = GetComponent<CharacterController>();

            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (_stateHandler == null)
                Debug.LogError($"Character_StateHandler not found on {gameObject.name}");

            if (_characterController == null)
                Debug.LogError($"CharacterController not found on {gameObject.name}");
        }
        #endregion

        #region Movement Methods
        public virtual void Move(Vector3 direction)
        {
            if (!CanMove()) return;

            Vector3 moveVector = direction * CurrentSpeed;
            _characterController.Move((moveVector + _velocity) * Time.deltaTime);
        }

        public virtual void Walk(Vector3 direction = default)
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Walking);
            CurrentSpeed = _walkSpeed;

            if (direction != Vector3.zero)
                Move(direction);
        }

        public virtual void Run(Vector3 direction = default)
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Running);
            CurrentSpeed = _runSpeed;

            if (direction != Vector3.zero)
                Move(direction);
        }

        public virtual void Jump()
        {
            if (!CanJump()) return;

            _stateHandler.ChangeState(CharacterState.Jumping);
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        public virtual void Roll()
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Rolling);
            // Add roll logic here
        }

        public virtual void Fly()
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Flying);
            // Add flying logic here
        }

        public virtual void Climb()
        {
            if (!CanChangeMovementState()) return;

            _stateHandler.ChangeState(CharacterState.Climbing);
            // Add climbing logic here
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
            return CanChangeMovementState() && _isGrounded;
        }
        #endregion

        #region Physics
        private void UpdateGroundedState()
        {
            _isGrounded = _characterController.isGrounded;
        }

        private void ApplyGravity()
        {
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
            else
            {
                _velocity.y += _gravity * Time.deltaTime;
            }
        }
        #endregion

        #region Debug
        private void OnDrawGizmos()
        {
            if (_characterController != null)
            {
                Gizmos.color = _isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position, 0.1f);
            }
        }
        #endregion
    }
}