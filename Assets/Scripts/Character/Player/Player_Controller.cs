using Character.Core.Base;
using Character.Core.Combat;
using UnityEngine;

namespace Character.Player
{
    [RequireComponent(typeof(Player_InputHandler), typeof(Player_Movement))]
    public class Player_Controller : Character_Base
    {
        #region Private Fields
        private Player_InputHandler _inputHandler;
        private Player_Movement _movement;
        [SerializeField] private Character_CombatHandler _combatHandler;
        #endregion

        #region Public Fields
        #endregion

        #region Properties
        public Player_InputHandler InputHandler { get { return _inputHandler; } private set { _inputHandler = value; } }
        public Player_Movement Movement { get { return _movement; } private set { _movement = value; } }
        public Character_CombatHandler CombatHandler { get { return _combatHandler; } private set { _combatHandler = value; } }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            InputHandler = GetComponent<Player_InputHandler>();
            Movement = GetComponent<Player_Movement>();
            CombatHandler = GetComponent<Character_CombatHandler>();
        }


    }
}
