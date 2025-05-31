namespace Game.Data.Character
{
    [System.Serializable]
    public class CharacterStatsData
    {
        #region Public  Fields
        public float Health;
        public double Experience;
        public float Stamina;
        public int Level;
        #endregion

        public CharacterStatsData(float health = 100, double experience = 0, float stamina = 100, int level = 0)
        {
            Health = health;
            Experience = experience;
            Stamina = stamina;
            Level = level;
        }
    }
}
