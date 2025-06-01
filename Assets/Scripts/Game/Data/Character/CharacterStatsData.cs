namespace Game.Data.Character
{
    [System.Serializable]
    public class CharacterStatsData
    {
        #region Public Fields
        [UnityEngine.Header("Health & Mana")]
        public float Health;
        public float MaxHealth;
        public float Mana;
        public float MaxMana;

        [UnityEngine.Header("Level & Experience")]
        public int Level;
        public double Experience;

        [UnityEngine.Header("Combat Stats")]
        public float AttackDamage;
        public float Defense;
        public float AttackSpeed;

        [UnityEngine.Header("Movement")]
        public float MovementSpeed;
        #endregion

        #region Constructor
        public CharacterStatsData(float maxHealth = 100f, float maxMana = 100f, double experience = 0, int level = 1,
                                 float attackDamage = 10f, float defense = 5f, float attackSpeed = 1f, float movementSpeed = 5f)
        {
            // Base stats
            MaxHealth = maxHealth;
            MaxMana = maxMana;
            Health = MaxHealth; // Start with full health
            Mana = MaxMana; // Start with full mana

            // Progression
            Level = UnityEngine.Mathf.Max(level, 1); // Minimum level 1
            Experience = System.Math.Max(experience, 0); // No negative experience

            // Combat stats
            AttackDamage = UnityEngine.Mathf.Max(attackDamage, 0f);
            Defense = UnityEngine.Mathf.Max(defense, 0f);
            AttackSpeed = UnityEngine.Mathf.Max(attackSpeed, 0.1f); // Minimum attack speed

            // Movement
            MovementSpeed = UnityEngine.Mathf.Max(movementSpeed, 0.1f); // Minimum movement speed
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Creates a copy of this CharacterStatsData
        /// </summary>
        public CharacterStatsData Clone()
        {
            return new CharacterStatsData(MaxHealth, MaxMana, Experience, Level, AttackDamage, Defense, AttackSpeed, MovementSpeed)
            {
                Health = this.Health,
                Mana = this.Mana
            };
        }

        /// <summary>
        /// Validates and clamps all stats to reasonable ranges
        /// </summary>
        public void ValidateStats()
        {
            // Ensure current stats don't exceed maximums
            Health = UnityEngine.Mathf.Clamp(Health, 0f, MaxHealth);
            Mana = UnityEngine.Mathf.Clamp(Mana, 0f, MaxMana);

            // Ensure positive values
            MaxHealth = UnityEngine.Mathf.Max(MaxHealth, 1f);
            MaxMana = UnityEngine.Mathf.Max(MaxMana, 0f);
            Level = UnityEngine.Mathf.Max(Level, 1);
            Experience = System.Math.Max(Experience, 0);
            AttackDamage = UnityEngine.Mathf.Max(AttackDamage, 0f);
            Defense = UnityEngine.Mathf.Max(Defense, 0f);
            AttackSpeed = UnityEngine.Mathf.Max(AttackSpeed, 0.1f);
            MovementSpeed = UnityEngine.Mathf.Max(MovementSpeed, 0.1f);
        }

        /// <summary>
        /// Returns a formatted string with all stats
        /// </summary>
        public override string ToString()
        {
            return $"Level {Level} - HP: {Health:F0}/{MaxHealth:F0}, MP: {Mana:F0}/{MaxMana:F0}, " +
                   $"ATK: {AttackDamage:F0}, DEF: {Defense:F0}, SPD: {MovementSpeed:F1}, EXP: {Experience:F0}";
        }
        #endregion
    }
}