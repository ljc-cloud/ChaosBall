using System;
using UnityEngine;

public interface IPlayerInput
{
    public event Action<Vector2> OnLRMove;
    public event Action OnReadyToLaunch;
    public event Action OnLaunch;

    void DisableInput();
}
