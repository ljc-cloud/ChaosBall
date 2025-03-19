using System;
using ChaosBall.Input;
using ChaosBall.Net;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChaosBall.Inputs
{
    public class ChaosBallInputRegister : MonoBehaviour
    {
        private ChaosInput _mChaosInput;
        
        public static ChaosBallInputRegister Instance { get; private set; }
        
        public event Action<Vector2> OnPlayerChangePosition;
        public event Action OnPlayerStop;
        public event Action OnPlayerReadyToShoot;
        public event Action OnPlayerShoot;
        public event Action OnPlayerQuitShoot;

        private bool _mHasQuit;
        public event Action<GameFrameSyncManager.PlayerInputType> OnPlayerInputChanged; 
        
        public GameFrameSyncManager.PlayerInputType LocalPlayerInputType { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _mChaosInput = new ChaosInput();
            _mChaosInput.Enable();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _mChaosInput.Player.Move.performed += PlayerChangePositionPerformed;
            _mChaosInput.Player.Move.canceled += PlayerChangePositionCanceled;
            _mChaosInput.Player.Shoot.performed += PlayerReadyShoot;
            _mChaosInput.Player.Shoot.canceled += PlayerReleaseShoot;
            _mChaosInput.Player.QuitShoot.performed += PlayerQuitShoot;
        }

        private void OnDestroy()
        {
            _mChaosInput.Player.Move.performed -= PlayerChangePositionPerformed;
            _mChaosInput.Player.Move.canceled -= PlayerChangePositionCanceled;
            _mChaosInput.Player.Shoot.performed -= PlayerReadyShoot;
            _mChaosInput.Player.Shoot.canceled -= PlayerReleaseShoot;
            _mChaosInput.Player.QuitShoot.performed -= PlayerQuitShoot;
        }


        private void PlayerChangePositionPerformed(InputAction.CallbackContext context)
        {
            Vector2 vector2 = context.ReadValue<Vector2>();
            // GameFrameSyncManager.PlayerInputType type = vector2.x < 0
            //     ? GameFrameSyncManager.PlayerInputType.MoveLeft
            //     : GameFrameSyncManager.PlayerInputType.MoveRight;
            GameFrameSyncManager.PlayerInputType type = vector2.x switch
            {
                < 0 => GameFrameSyncManager.PlayerInputType.MoveLeft,
                > 0 => GameFrameSyncManager.PlayerInputType.MoveRight,
                _ => GameFrameSyncManager.PlayerInputType.None
            };
            LocalPlayerInputType = type;
            Debug.Log("PlayerChangePositionPerformed");
            OnPlayerInputChanged?.Invoke(type);
            OnPlayerChangePosition?.Invoke(vector2);
        }
        private void PlayerChangePositionCanceled(InputAction.CallbackContext context)
        {
            GameFrameSyncManager.PlayerInputType type = GameFrameSyncManager.PlayerInputType.None;
            LocalPlayerInputType = type;
            OnPlayerInputChanged?.Invoke(type);
            OnPlayerStop?.Invoke();
        }
        private void PlayerReadyShoot(InputAction.CallbackContext context)
        {
            _mHasQuit = false;
            GameFrameSyncManager.PlayerInputType type = GameFrameSyncManager.PlayerInputType.Ready;
            LocalPlayerInputType = type;
            OnPlayerInputChanged?.Invoke(type);
            OnPlayerReadyToShoot?.Invoke();
        }
        private void PlayerReleaseShoot(InputAction.CallbackContext context)
        { 
            if (_mHasQuit) return;
            GameFrameSyncManager.PlayerInputType type = GameFrameSyncManager.PlayerInputType.Shoot;
            LocalPlayerInputType = type;
            OnPlayerInputChanged?.Invoke(type);
            OnPlayerShoot?.Invoke();
        }
        private void PlayerQuitShoot(InputAction.CallbackContext context)
        {
            GameFrameSyncManager.PlayerInputType type = GameFrameSyncManager.PlayerInputType.QuitReady;
            LocalPlayerInputType = type;
            _mHasQuit = true;
            OnPlayerInputChanged?.Invoke(type);
            OnPlayerQuitShoot?.Invoke();
        }
    }
}