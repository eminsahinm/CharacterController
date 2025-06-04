using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Platform.Input.Desktop;
using Game.Platform.Input.Mobile;
using Game.Platform.Input.Console;
using UnityEngine.InputSystem;

namespace Game.Platform.Input
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;
        public static InputManager Instance => _instance;

        // Base input actions
        private PlayerInputActions _baseInputActions;
        public PlayerInputActions BaseInputActions => _baseInputActions;

        // Platform-spesifik input actions
        private IInputActionCollection2 _platformInputActions;
        public IInputActionCollection2 PlatformInputActions => _platformInputActions;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeInputSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeInputSystem()
        {
            // Load Base input actions
            _baseInputActions = new PlayerInputActions();
            _baseInputActions.Enable();

            // Load Platform-specific input actions
            LoadPlatformSpecificInputs();
        }

        private void LoadPlatformSpecificInputs()
        {
#if UNITY_ANDROID || UNITY_IOS
            _platformInputActions = new MobileInputOverrides();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            _platformInputActions = new DesktopInputOverrides();
#elif UNITY_XBOX || UNITY_PS4
            _platformInputActions = new ConsoleInputOverrides();
#endif

            if (_platformInputActions != null)
            {
                _platformInputActions.Enable();
            }
        }

        public void SwitchPlatformInputs(IInputActionCollection2 newPlatformInputs)
        {
            if (_platformInputActions != null)
            {
                _platformInputActions.Disable();
            }

            _platformInputActions = newPlatformInputs;
            _platformInputActions.Enable();
        }
    }
}
