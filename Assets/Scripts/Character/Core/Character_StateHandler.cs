using Game.Character.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Character.Core
{
    public class Character_StateHandler : MonoBehaviour
    {
        #region Events
        public UnityEvent<CharacterState> OnStateChanged;
        #endregion

        #region Properties
        public CharacterState CurrentState { get; private set; }
        #endregion

        public void ChangeState(CharacterState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
