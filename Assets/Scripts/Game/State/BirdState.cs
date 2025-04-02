using System;
using ChaosBall.Net;
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
            // Count,
            Effected,
        }
        
        public BirdStateEnum State { get; protected set; }
        public BirdStateEnum FromStateEnum { get; set; }
        protected Entity Entity { get; private set; }

        protected BirdStateManager _mBirdStateManager;
        protected Transform _mTargetTransform;

        public BirdState(BirdStateManager birdStateManager, Transform targetTransform, Entity entity)
        {
            _mBirdStateManager = birdStateManager;
            _mTargetTransform = targetTransform;
            Entity = entity;
        }
        
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}