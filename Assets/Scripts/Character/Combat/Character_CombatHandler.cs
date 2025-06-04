using UnityEngine;
using Character.Core.State;
using Character.Core.Stats;
using Character.Core.Interfaces;
using Character.Core.Enums;
using UnityEngine.Events;

namespace Character.Core.Combat
{
    [RequireComponent(typeof(Character_StateHandler), typeof(Character_Stats))]
    public class Character_CombatHandler : MonoBehaviour, ICombat
    {
        #region Private Fields
        private Character_StateHandler _stateHandler;
        private Character_Stats _stats;
        private GameObject _currentTarget;

        [Header("Combat Settings")]
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _stunChance = 0.1f;
        [SerializeField] private float _stunDuration = 1f;

        private float _lastAttackTime;
        private bool _isInCombat;
        #endregion

        #region Properties
        public GameObject GetCurrentTarget() => _currentTarget;
        public bool HasTarget => _currentTarget != null;
        public bool IsInCombat => _isInCombat;
        public bool IsDead => _stateHandler.CurrentState == CharacterState.Dead;

        // Events
        public System.Action<float> OnDamageTaken { get; set; }
        public System.Action OnDeath { get; set; }
        public System.Action OnAttackStarted { get; set; }
        #endregion


        #region Events
        [Header("Attacking")]
        public UnityEvent OnAttacking;
        #endregion
        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Update()
        {
            UpdateCombatState();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            _stateHandler = GetComponent<Character_StateHandler>();
            _stats = GetComponent<Character_Stats>();

            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (_stateHandler == null)
                Debug.LogError($"Character_StateHandler not found on {gameObject.name}");

            if (_stats == null)
                Debug.LogError($"Character_Stats not found on {gameObject.name}");
        }
        #endregion

        #region ICombat Implementation
        public void SetTarget(GameObject target)
        {
            if (IsDead) return;

            _currentTarget = target;
            _isInCombat = target != null;
        }

        public void Attack()
        {
            //if (!CanAttack()) return;

            _lastAttackTime = Time.time;
            _stateHandler.ChangeState(CharacterState.Attacking);

            OnAttackStarted?.Invoke();
            OnAttacking?.Invoke();

            // Apply damage to target if it has ICombat
            if (_currentTarget != null)
            {
                var targetCombat = _currentTarget.GetComponent<ICombat>();
                if (targetCombat != null)
                {
                    float damage = _stats.GetAttackDamage(); 
                    targetCombat.TakeDamage(damage, gameObject);
                }
            }
        }

        public bool CanAttack()
        {
            if (IsDead || !HasTarget) return false;
            if (_stateHandler.CurrentState == CharacterState.Stunned) return false;
            if (Time.time - _lastAttackTime < _attackCooldown) return false;

            return IsTargetInRange();
        }

        public void TakeDamage(float amount)
        {
            TakeDamage(amount, null);
        }

        public void TakeDamage(float amount, GameObject attacker)
        {
            if (IsDead) return;

            // Apply damage to stats
            _stats.TakeDamage(amount);

            // Trigger events
            OnDamageTaken?.Invoke(amount);


            // Check for stun
            if (ShouldBeStunned())
            {
                HandleStun();
            }

            // Set attacker as target if we don't have one
            if (!HasTarget && attacker != null)
            {
                SetTarget(attacker);
            }
        }
        #endregion

        #region Combat Logic
        private void UpdateCombatState()
        {
            if (!HasTarget)
            {
                _isInCombat = false;
                return;
            }

            // Check if target is still valid and in range
            if (!IsTargetValid() || !IsTargetInRange())
            {
                SetTarget(null);
            }
        }

        private bool IsTargetInRange()
        {
            if (!HasTarget) return false;

            float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);
            return distance <= _attackRange;
        }

        private bool IsTargetValid()
        {
            if (!HasTarget) return false;

            // Check if target still exists
            if (_currentTarget == null) return false;

            // Check if target is dead
            var targetCombat = _currentTarget.GetComponent<ICombat>();
            if (targetCombat != null && targetCombat.IsDead) return false;

            return true;
        }

        private bool ShouldBeStunned()
        {
            return Random.Range(0f, 1f) < _stunChance;
        }

        private void HandleStun()
        {
            _stateHandler.ChangeState(CharacterState.Stunned);
            StartCoroutine(RecoverFromStun());
        }

        private System.Collections.IEnumerator RecoverFromStun()
        {
            yield return new WaitForSeconds(_stunDuration);

            if (_stateHandler.CurrentState == CharacterState.Stunned)
            {
                _stateHandler.ChangeState(CharacterState.Idle);
            }
        }

        private void HandleDeath()
        {
            _stateHandler.ChangeState(CharacterState.Dead);
            _isInCombat = false;
            _currentTarget = null;

            OnDeath?.Invoke();
        }
        #endregion

        #region Debug
        private void OnDrawGizmos()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);

            // Draw line to target
            if (HasTarget)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
            }
        }
        #endregion
    }
}