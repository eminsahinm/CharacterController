using Game.Data.Character;
using UnityEngine;
using UnityEngine.Events;

namespace Character.Core.Stats
{
    public class Character_Stats : MonoBehaviour
    {
        #region Events
        [Header("Events")]
        public UnityEvent<float> onDamageTaken;
        public UnityEvent onDeath;
        public UnityEvent<int> onLevelUp;
        public UnityEvent<double> onExperienceGained;
        #endregion

        #region Private Fields
        private CharacterStatsData _statsData;
        private XPLevelTableSO _xpPerLevel;
        private bool _isDead;
        #endregion

        #region Properties
        public float CurrentHealth => _statsData?.Health ?? 0f;
        public float MaxHealth => _statsData?.MaxHealth ?? 0f;
        public float CurrentMana => _statsData?.Mana ?? 0f;
        public float MaxMana => _statsData?.MaxMana ?? 0f;
        public int Level => _statsData?.Level ?? 1;
        public double Experience => _statsData?.Experience ?? 0;
        public bool IsDead => _isDead;

        // Combat-related properties
        public float AttackDamage => _statsData?.AttackDamage ?? 0f;
        public float Defense => _statsData?.Defense ?? 0f;
        public float AttackSpeed => _statsData?.AttackSpeed ?? 1f;
        public float MovementSpeed => _statsData?.MovementSpeed ?? 1f;

        // Stats validation
        public bool IsInitialized => _statsData != null;
        #endregion

        #region Initialization
        public void Initialize(CharacterStatsData data)
        {
            if (data == null)
            {
                Debug.LogError($"CharacterStatsData is null for {gameObject.name}");
                return;
            }

            _statsData = data;
            _isDead = _statsData.Health <= 0;

            ValidateStats();
        }

        public virtual void SetXPPerLevel(XPLevelTableSO xpLevelTable)
        {
            if (xpLevelTable == null)
            {
                Debug.LogError($"XPLevelTableSO is null for {gameObject.name}");
                return;
            }

            _xpPerLevel = xpLevelTable;
        }

        private void ValidateStats()
        {
            if (_statsData == null) return;

            // Ensure health doesn't exceed max
            _statsData.Health = Mathf.Min(_statsData.Health, _statsData.MaxHealth);
            _statsData.Mana = Mathf.Min(_statsData.Mana, _statsData.MaxMana);

            // Ensure positive values
            _statsData.Health = Mathf.Max(_statsData.Health, 0f);
            _statsData.Mana = Mathf.Max(_statsData.Mana, 0f);
            _statsData.Level = Mathf.Max(_statsData.Level, 1);
            _statsData.Experience = System.Math.Max(_statsData.Experience, 0);
        }
        #endregion

        #region Experience System
        public void GainExperience(double exp)
        {
            if (!IsInitialized || _isDead || exp <= 0) return;
            if (_xpPerLevel == null)
            {
                Debug.LogWarning($"XP Level Table not set for {gameObject.name}");
                return;
            }

            int currentLevel = _statsData.Level;
            double remainingExp = exp;
            double currentExp = _statsData.Experience;

            while (remainingExp > 0 && currentLevel < _xpPerLevel.xpPerLevel.Count)
            {
                double expRequiredForCurrentLevel = _xpPerLevel.xpPerLevel[currentLevel - 1];
                double expNeededToLevelUp = expRequiredForCurrentLevel - currentExp;

                if (remainingExp >= expNeededToLevelUp)
                {
                    // Level up!
                    remainingExp -= expNeededToLevelUp;
                    currentLevel++;
                    currentExp = 0;

                    LevelUp(currentLevel);
                }
                else
                {
                    // Add remaining exp without leveling up
                    currentExp += remainingExp;
                    remainingExp = 0;
                }
            }

            _statsData.Level = currentLevel;
            _statsData.Experience = currentExp;

            onExperienceGained.Invoke(exp);
        }

        private void LevelUp(int newLevel)
        {
            Debug.Log($"{gameObject.name} leveled up to {newLevel}!");

            // Increase stats on level up (this could be data-driven)
            float healthIncrease = _statsData.MaxHealth * 0.1f; // 10% increase
            float manaIncrease = _statsData.MaxMana * 0.1f;

            _statsData.MaxHealth += healthIncrease;
            _statsData.MaxMana += manaIncrease;
            _statsData.Health = _statsData.MaxHealth; // Full heal on level up
            _statsData.Mana = _statsData.MaxMana; // Full mana on level up

            onLevelUp.Invoke(newLevel);
        }
        #endregion

        #region Health/Damage System
        public void TakeDamage(float damage)
        {
            if (!IsInitialized || _isDead || damage <= 0) return;

            // Apply defense (simple damage reduction)
            float actualDamage = Mathf.Max(damage - Defense, damage * 0.1f); // Minimum 10% damage goes through

            _statsData.Health -= actualDamage;
            _statsData.Health = Mathf.Max(_statsData.Health, 0);

            onDamageTaken.Invoke(actualDamage);

            if (_statsData.Health <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (!IsInitialized || _isDead || amount <= 0) return;

            _statsData.Health += amount;
            _statsData.Health = Mathf.Min(_statsData.Health, _statsData.MaxHealth);
        }

        public void RestoreMana(float amount)
        {
            if (!IsInitialized || _isDead || amount <= 0) return;

            _statsData.Mana += amount;
            _statsData.Mana = Mathf.Min(_statsData.Mana, _statsData.MaxMana);
        }

        public bool ConsumeMana(float amount)
        {
            if (!IsInitialized || _isDead || amount <= 0) return false;
            if (_statsData.Mana < amount) return false;

            _statsData.Mana -= amount;
            return true;
        }
        #endregion

        #region Combat Integration
        public float GetAttackDamage()
        {
            if (!IsInitialized || _isDead) return 0f;

            // Base damage with potential random variation
            float baseDamage = AttackDamage;
            float variation = baseDamage * 0.1f; // ±10% variation

            return Random.Range(baseDamage - variation, baseDamage + variation);
        }

        public bool CanAttack()
        {
            return IsInitialized && !_isDead;
        }

        public bool CanMove()
        {
            return IsInitialized && !_isDead;
        }
        #endregion

        #region Death System
        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            _statsData.Health = 0;

            Debug.Log($"{gameObject.name} has died!");
            onDeath.Invoke();
        }

        public void Revive(float healthPercent = 0.5f)
        {
            if (!_isDead) return;

            _isDead = false;
            _statsData.Health = _statsData.MaxHealth * Mathf.Clamp01(healthPercent);

            Debug.Log($"{gameObject.name} has been revived!");
        }
        #endregion

        #region Debug
        private void OnValidate()
        {
            if (Application.isPlaying && IsInitialized)
            {
                ValidateStats();
            }
        }

        // Debug info for inspector
        [System.Serializable]
        public class DebugInfo
        {
            [Header("Current Stats")]
            public float health;
            public float mana;
            public int level;
            public double experience;
            public bool isDead;
        }

        [Header("Debug Info (Read Only)")]
        public DebugInfo debugInfo;

        private void Update()
        {
            if (IsInitialized)
            {
                debugInfo.health = CurrentHealth;
                debugInfo.mana = CurrentMana;
                debugInfo.level = Level;
                debugInfo.experience = Experience;
                debugInfo.isDead = IsDead;
            }
        }
        #endregion
    }
}