using System;
using UnityEngine;

namespace ChaosBall.Game
{
    public interface IPlayerInput
    {
        public event Action<Vector2> OnLRMove;
        public event Action OnReadyToLaunch;
        public event Action OnLaunch;
        public event Action OnUnLaunch;

        void DisableInput();

        void EnableInput();
    }

}
