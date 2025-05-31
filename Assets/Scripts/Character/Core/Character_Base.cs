using UnityEngine;

namespace Game.Character.Core 
{
    [RequireComponent(typeof(Character_Stats), typeof(Character_StateHandler))]
    public class Character_Base : MonoBehaviour
    {

        #region Private Fields
        private Character_StateHandler _stateHandler;
        private Character_Stats _stats;
        //private Item _itemHolder;
        #endregion

        #region Public Fields
        #endregion

        #region Properties
        public Character_StateHandler StateHandler { get { return _stateHandler; } private set { _stateHandler = value; } }
        public Character_Stats Stats { get { return _stats; } private set { _stats = value; } }
        //public Item ItemHolder { get { return _itemHolder; }  set { _itemHolder = value; } }
        #endregion


        protected virtual void Awake()
        {
            StateHandler = GetComponent<Character_StateHandler>();
            Stats = GetComponent<Character_Stats>();
        }
    }
}