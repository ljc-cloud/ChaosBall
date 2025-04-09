using System;
using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall.Game.State
{
    /// <summary>
    /// 状态机抽象类实现
    /// </summary>
    public abstract class BirdState : IState
    {
        public enum BirdStateEnum
        {
            UnReady,
            Ready,
            Shoot,
            Collided,
            Stop,
            Effected,
        }
        
        public BirdStateEnum State { get; protected set; }
        // public BirdStateEnum FromStateEnum { get; set; }
        protected Entity Entity { get; private set; }
        protected BirdAnimation Animation { get; private set; }

        protected BirdStateMachine BirdStateMachine;

        public BirdState(BirdStateMachine birdStateMachine, BirdAnimation birdAnimation, Entity entity)
        {
            BirdStateMachine = birdStateMachine;
            Animation = birdAnimation;
            Entity = entity;
        }
        
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}