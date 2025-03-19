using System;
using ChaosBall.Inputs;
using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall.Game.State
{
    [Obsolete]
    public class BirdIdleState : BirdState
    {
        public BirdIdleState(BirdStateManager birdStateManager, Transform targetTransform, Entity entity) 
            : base(birdStateManager, targetTransform, entity)
        {
            
        }

        private void PlayerReadyToShoot()
        {
            _mBirdStateManager.ChangeState(BirdStateEnum.Ready);
        }

        private void BirdChangePosition(Vector2 vec)
        {
            _mBirdStateManager.ChangeState(BirdStateEnum.Move);
        }

        public override void Enter()
        {
            Debug.Log("Bird Idle State Enter");
            State = BirdStateEnum.Idle;
            _mBirdStateManager.BirdAnimation.PlayIdle();
            ChaosBallInputRegister.Instance.OnPlayerReadyToShoot += PlayerReadyToShoot;
            ChaosBallInputRegister.Instance.OnPlayerChangePosition += BirdChangePosition;
            _mBirdStateManager.ArrowForceUI.SetReady(false);
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            Debug.Log("Bird Idle State Exit");
            ChaosBallInputRegister.Instance.OnPlayerReadyToShoot -= PlayerReadyToShoot;
            ChaosBallInputRegister.Instance.OnPlayerChangePosition -= BirdChangePosition;
        }
    }
}