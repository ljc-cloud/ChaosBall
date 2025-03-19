using System;
using UnityEngine;

namespace ChaosBall.Game.State
{
    /// <summary>
    /// 状态机抽象类实现
    /// </summary>
    public abstract class BirdState
    {
        public enum BirdStateEnum
        {
            UnReady,
            Idle, // Deprecated
            Move, // Deprecated
            Ready,
            Shoot,
            Collided,
            Stop,
            Count,
            Effected,
        }
        
        public BirdStateEnum State { get; protected set; }
        public BirdStateEnum FromStateEnum { get; set; }

        protected BirdStateManager _mBirdStateManager;
        protected Transform _mTargetTransform;

        public BirdState(BirdStateManager birdStateManager, Transform targetTransform)
        {
            _mBirdStateManager = birdStateManager;
            _mTargetTransform = targetTransform;
        }
        
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}