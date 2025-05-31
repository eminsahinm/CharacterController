using UnityEngine;

namespace Game.Character.Interfaces
{
    public interface ICombat
    {
        void SetTarget(GameObject target);
        void Attack();
        void TakeDamage(float amount);
    }
}
