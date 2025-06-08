using UnityEngine;
using Game.Platform.Input;
using Game.Platform.Input.Console;
using Game.Platform.Input.Desktop;
using Game.Platform.Input.Mobile;
using UnityEngine.InputSystem;

namespace Character.Player
{
    public class Player_InputHandler : MonoBehaviour
    {
        private PlayerInputActions _baseInputActions;
        private IInputActionCollection2 _platformInputActions;


        public Vector3 MoveDirection { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool RunPressed { get; private set; }

        private void Awake()
        {
            _baseInputActions = InputManager.Instance.BaseInputActions;
            _platformInputActions = InputManager.Instance.PlatformInputActions;
        }

        private void Update()
        {
            GlobalInputs();
            if (_platformInputActions is MobileInputOverrides)
            {
                MobileInputs();
            }
            else if (_platformInputActions is DesktopInputOverrides)
            {
                KeyboardInputs();
            }
            else if (_platformInputActions is ConsoleInputOverrides)
            {
                GamepadInputs();
            }
        }

        private void GlobalInputs()
        {
            MoveDirection = Movement();
            JumpPressed = Jump();
            AttackPressed = Attack();
            RunPressed = Run();
        }

        private Vector3 Movement()
        {
            Vector2 direction = _baseInputActions.Move.Keys.ReadValue<Vector2>();
            return new Vector3(direction.x,0,direction.y);
        }

        private bool Jump()
        {
            return _baseInputActions.Jump.Keys.triggered;
        }
        private bool Attack()
        {
            return _baseInputActions.SimpleAttack.Keys.IsPressed();
        }
        private bool Run()
        {
            return _baseInputActions.Run.Keys.IsPressed();
        }

        private void KeyboardInputs()
        {

        }

        private void GamepadInputs()
        {

        }

        private void MobileInputs()
        {

        }


    }
}
