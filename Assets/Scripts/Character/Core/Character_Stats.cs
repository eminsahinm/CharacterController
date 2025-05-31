using Game.Data.Character;
using UnityEngine;
using UnityEngine.Events;
namespace Game.Character.Core
{
    public class Character_Stats : MonoBehaviour
    {
        #region Events
        public UnityEvent<float> onDamageTaken;
        public UnityEvent onDeath;
        #endregion

        #region Private Fields
        private CharacterStatsData _statsData;
        private XPLevelTableSO _xpPerLevel;
        //private Item _itemHolder;
        #endregion


        public void Initialize(CharacterStatsData data)
        {
            _statsData = data;
        }

        public virtual void SetXPPerLevel(XPLevelTableSO xPLevelTable)
        {
            _xpPerLevel = xPLevelTable;
        }

        public void GainExperience(double Exp)
        {
            int currentLevel = _statsData.Level;
            double currentMaxExp = _xpPerLevel.xpPerLevel[currentLevel - 1];
            double tmp_exp = _statsData.Experience;

            while(tmp_exp + Exp > currentMaxExp)
            {
                double filler_exp = currentMaxExp - tmp_exp;
                Exp -= filler_exp;
                currentLevel += 1;
                tmp_exp = 0;
                currentMaxExp = _xpPerLevel.xpPerLevel[currentLevel - 1];
                _statsData.Experience = tmp_exp;
            }
            _statsData.Level = currentLevel;
            _statsData.Experience += Exp;
        }

        public void TakeDamage(float damage)
        {
            _statsData.Health -= damage;
            _statsData.Health = Mathf.Max(_statsData.Health, 0);

            if (_statsData.Health == 0)
            {
                Die();
            }
            onDamageTaken.Invoke(damage);
        }

        private void Die()
        {
            // Ölme durumunu yönet
            onDeath.Invoke();
        }

    }
}
