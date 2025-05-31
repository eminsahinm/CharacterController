using UnityEngine;
using Game.Character.Enums;

namespace Game.Character.Core
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Character_StateHandler))]
    public class Character_Movement : MonoBehaviour
    {
        #region Private Fields
        private Character_StateHandler _stateHandler;
        private float _speed;
        #endregion

        #region Properties
        public float Speed { get { return _speed; } set { _speed = value; } }
        #endregion

        private void Awake()
        {
            _stateHandler = GetComponent<Character_StateHandler>();
        }

        public virtual void Walk() {
            _stateHandler.ChangeState(CharacterState.Walking);
        }

        public virtual void Run()
        {
            _stateHandler.ChangeState(CharacterState.Running);
        }

        public virtual void Roll()
        {
            _stateHandler.ChangeState(CharacterState.Rolling);
        }

        public virtual void Jump()
        {
            _stateHandler.ChangeState(CharacterState.Jumping);
        }

        public virtual void Fly()
        {
            _stateHandler.ChangeState(CharacterState.Flying);
        }

        public virtual void Climb()
        {
            _stateHandler.ChangeState(CharacterState.Climbing);
        }
    }
}
