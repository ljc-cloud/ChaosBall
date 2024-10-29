using System;
using UnityEngine;

public class Player1Input : IPlayerInput
{
    private BallAction _action;
    
    public event Action<Vector2> OnLRMove;
    public event Action OnReadyToLaunch;
    public event Action OnLaunch;
    
    public Player1Input()
    {
        _action = new BallAction();
        _action.Player1.LR.performed += context => OnLRMove?.Invoke(context.ReadValue<Vector2>());
        _action.Player1.LR.canceled += context => OnLRMove?.Invoke(context.ReadValue<Vector2>());
        _action.Player1.Launch.performed += _ => OnReadyToLaunch?.Invoke();
        _action.Player1.Launch.canceled += _ => OnLaunch?.Invoke();
        _action.Enable();
    }

    public void DisableInput()
    {
        _action.Disable();
    }
}
