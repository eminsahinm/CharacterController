using Character.Core.Base;
using UnityEngine;

namespace Character.Player
{
    [RequireComponent(typeof(Player_InputHandler),typeof(Player_Controller))]
    public class Player_Movement : Character_Movement
    {
        private Player_Controller _playerController;

        public override void Awake()
        {
            base.Awake();
            _playerController = GetComponent<Player_Controller>();
        }
        public override void Update()
        {
            base.Update();

            if (_playerController.InputHandler.RunPressed)
                Run(_playerController.InputHandler.MoveDirection);
            else
                Walk(_playerController.InputHandler.MoveDirection);

            if (_playerController.InputHandler.AttackPressed && _playerController.CombatHandler != null) _playerController.CombatHandler.Attack();

            if (_playerController.InputHandler.JumpPressed) Jump();
        }

    }
}
