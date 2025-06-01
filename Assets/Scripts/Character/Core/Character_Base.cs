using Character.Core.State;
using Character.Core.Stats;
using UnityEngine;

namespace Character.Core.Base
{
    [RequireComponent(typeof(Character_Stats), typeof(Character_StateHandler))]
    public class Character_Base : MonoBehaviour
    {
        #region Private Fields
        private Character_StateHandler _stateHandler;
        private Character_Stats _stats;
        private bool _isInitialized;
        #endregion

        #region Properties
        public Character_StateHandler StateHandler => _stateHandler;
        public Character_Stats Stats => _stats;
        public bool IsInitialized => _isInitialized;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            InitializeComponents();
        }

        protected virtual void Start()
        {
            PostInitialize();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            _stateHandler = GetComponent<Character_StateHandler>();
            _stats = GetComponent<Character_Stats>();

            ValidateComponents();
            _isInitialized = true;
        }

        private void ValidateComponents()
        {
            if (_stateHandler == null)
                Debug.LogError($"Character_StateHandler not found on {gameObject.name}");

            if (_stats == null)
                Debug.LogError($"Character_Stats not found on {gameObject.name}");
        }

        protected virtual void PostInitialize()
        {
            // Override in derived classes for post-initialization logic
        }
        #endregion

        #region Public Methods
        public virtual void Initialize()
        {
            if (!_isInitialized)
                InitializeComponents();
        }
        #endregion
    }
}