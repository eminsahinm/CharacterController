using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Character.Core;
using Game.Character.Enums;
using Game.Character.Interfaces;

namespace Game.Character.Combat
{
    [RequireComponent(typeof(Character_StateHandler), typeof(Character_Stats))]
    public class Character_CombatHandler : MonoBehaviour, ICombat
    {
        #region Private Fields
        private Character_StateHandler _stateHandler;
        private Character_Stats _stats;
        private GameObject currentTarget;
        #endregion

        private void Awake()
        {
            _stateHandler = GetComponent<Character_StateHandler>();
            _stats = GetComponent<Character_Stats>();
        }
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }

        public void Attack()
        {
            if (currentTarget == null) return;
            // Attack logic
            _stateHandler.ChangeState(CharacterState.Attacking);
        }

        public void TakeDamage(float amount)
        {
            // Damage logic
            _stats.TakeDamage(amount);
        }
    }
}
