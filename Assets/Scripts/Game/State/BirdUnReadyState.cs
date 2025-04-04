using ChaosBall.Inputs;
using ChaosBall.Net;
using ChaosBall.UI;
using UnityEngine;

namespace ChaosBall.Game.State
{
    /// <summary>
    /// UnReady 状态机实现
    /// </summary>
    public class BirdUnReadyState : BirdState
    {
        private Vector2 _mMoveVector;
        private readonly float _mSpeed;
        
        public BirdUnReadyState(BirdStateManager birdStateManager, Transform targetTransform, Entity entity
            , float speed) 
            : base(birdStateManager, targetTransform, entity)
        {
            _mSpeed = speed;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Unready State Enter");
            State = BirdStateEnum.UnReady;
            _mBirdStateManager.BirdAnimation.PlayIdle();
            _mBirdStateManager.ArrowForceUI.SetReady(false);

            if (Entity.IsLocal)
            {
                ChaosBallInputRegister.Instance.OnPlayerChangePosition += BirdChangePosition;
                ChaosBallInputRegister.Instance.OnPlayerStop += BirdMoveStop;
                ChaosBallInputRegister.Instance.OnPlayerReadyToShoot += PlayerReadyToShoot;
            }
        }
        private void BirdChangePosition(Vector2 vec)
        {
            _mMoveVector = vec;
        }
        private void BirdMoveStop()
        {
            _mMoveVector = Vector2.zero;
            _mBirdStateManager.BirdAnimation.PlayIdle();
        }
        private void PlayerReadyToShoot()
        {
            _mBirdStateManager.ChangeState(BirdStateEnum.Ready);
        }

        public override void Update()
        {
            if (!Entity.IsLocal)
            {
                ListenRemote();
            }
            DoMove();
        }

        private void ListenRemote()
        {
            GameFrameSyncManager.PlayerInputType playerInputType = Entity.playerInputType;
            switch (playerInputType)
            {
                case GameFrameSyncManager.PlayerInputType.None: _mMoveVector = Vector2.zero; break;
                case GameFrameSyncManager.PlayerInputType.MoveLeft: _mMoveVector = Vector2.left; break;
                case GameFrameSyncManager.PlayerInputType.MoveRight: _mMoveVector = Vector2.right; break;
                case GameFrameSyncManager.PlayerInputType.Ready: 
                    _mBirdStateManager.ChangeState(BirdStateEnum.Ready);
                    break;
            }
            // TODO: 移动插值
            Vector3 birdPosition = Entity.birdPosition;
            Debug.Log($"Lerp Position: {birdPosition}");
            _mTargetTransform.position =
                Vector3.Lerp(_mTargetTransform.position, birdPosition, 0.5f * Time.deltaTime);
        }

        private void DoMove()
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
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Unready State Exit");
            ChaosBallInputRegister.Instance.OnPlayerChangePosition -= BirdChangePosition;
            ChaosBallInputRegister.Instance.OnPlayerStop -= BirdMoveStop;
            ChaosBallInputRegister.Instance.OnPlayerReadyToShoot -= PlayerReadyToShoot;
        }
    }
}