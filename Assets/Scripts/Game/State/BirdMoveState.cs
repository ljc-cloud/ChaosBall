using System;
using ChaosBall.Inputs;
using UnityEngine;

namespace ChaosBall.Game.State
{
    [Obsolete]
    public class BirdMoveState : BirdState
    {
        private Vector2 _mMoveVector;
        private float _mSpeed = 200f;

        public BirdMoveState(BirdStateManager birdStateManager, Transform targetTransform) : base(birdStateManager, targetTransform)
        {
            
        }

        private void BirdChangePosition(Vector2 vec)
        {
            _mMoveVector = vec;
        }
        private void BirdMoveStop()
        {
            _mMoveVector = Vector2.zero;
            _mBirdStateManager.ChangeState(BirdStateEnum.Idle);
        }

        public override void Enter()
        {
            Debug.Log("Bird Move State Enter");
            State = BirdStateEnum.Move;
            ChaosBallInputRegister.Instance.OnPlayerChangePosition += BirdChangePosition;
            ChaosBallInputRegister.Instance.OnPlayerStop += BirdMoveStop;
        }

        public override void Update()
        {
            _mTargetTransform.Translate(_mMoveVector * (_mSpeed * Time.deltaTime));
            if (_mMoveVector.x > 0f)
            {
                _mBirdStateManager.BirdAnimation.PlayMoveR();
            }
            else if(_mMoveVector.x < 0f)
            {
                _mBirdStateManager.BirdAnimation.PlayMoveL();
            }
        }

        public override void Exit()
        {
            Debug.Log("Bird Move State Exit");
            ChaosBallInputRegister.Instance.OnPlayerChangePosition -= BirdChangePosition;
            ChaosBallInputRegister.Instance.OnPlayerStop -= BirdMoveStop;
        }
    }
}