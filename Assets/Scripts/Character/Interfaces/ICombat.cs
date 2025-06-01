using UnityEngine;

namespace Character.Core.Interfaces
{
    public interface ICombat
    {
        // Target Management
        void SetTarget(GameObject target);
        GameObject GetCurrentTarget();
        bool HasTarget { get; }

        // Combat Actions
        void Attack();
        bool CanAttack();

        // Damage System
        void TakeDamage(float amount);
        void TakeDamage(float amount, GameObject attacker);

        // Combat State
        bool IsInCombat { get; }
        bool IsDead { get; }

        // Events (optional - for decoupled communication)
        System.Action<float> OnDamageTaken { get; set; }
        System.Action OnDeath { get; set; }
        System.Action OnAttackStarted { get; set; }
    }
}