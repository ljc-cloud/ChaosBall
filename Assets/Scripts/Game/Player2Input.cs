using System;
using UnityEngine;

namespace ChaosBall.Game
{
    public class Player2Input : IPlayerInput
    {
        private BallAction _action;

        public event Action<Vector2> OnLRMove;
        public event Action OnReadyToLaunch;
        public event Action OnLaunch; 
    
        public Player2Input()
        {
            _action = new BallAction();
            _action = new BallAction();
            _action.Player2.LR.performed += context => OnLRMove?.Invoke(context.ReadValue<Vector2>());
            _action.Player2.LR.canceled += context => OnLRMove?.Invoke(context.ReadValue<Vector2>());
            _action.Player2.Launch.performed += _ => OnReadyToLaunch?.Invoke();
            _action.Player2.Launch.canceled += _ => OnLaunch?.Invoke();
            _action.Enable();
        }
    
        public void DisableInput()
        {
            _action.Disable();
        }
    }

}
