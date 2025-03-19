using ChaosBall.Inputs;
using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall.Game.State
{
    /// <summary>
    /// Ready 状态机实现
    /// </summary>
    public class BirdReadyState : BirdState
    {
        private float _mTimer;
        private bool _mReverse;
        private bool _mQuitReady;
        private readonly float _mMinShootForce;
        private readonly float _mMaxShootForce;
        private readonly float _mReadyShootDuration;
        
        public BirdReadyState(BirdStateManager birdStateManager, Transform targetTransform,
            float minShootForce, float maxShootForce, float readyShootDuration) : base(birdStateManager, targetTransform)
        {
            _mMinShootForce = minShootForce;
            _mMaxShootForce = maxShootForce;
            _mReadyShootDuration = readyShootDuration;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Ready State Enter");
            State = BirdStateEnum.Ready;
            _mQuitReady = false;
            _mBirdStateManager.BirdAnimation.PlayReady();
            _mBirdStateManager.ArrowForceUI.SetReady(true);

            if (_mBirdStateManager.IsLocal)
            {
                ChaosBallInputRegister.Instance.OnPlayerShoot += PlayerShoot;
                ChaosBallInputRegister.Instance.OnPlayerQuitShoot += PlayerQuitShoot;
            }
        }
        
        private void PlayerShoot()
        {
            if (_mQuitReady) return;
            _mBirdStateManager.ChangeState(BirdStateEnum.Shoot);
        }
        private void PlayerQuitShoot()
        {
            _mQuitReady = true;
            _mBirdStateManager.ChangeState(BirdStateEnum.UnReady);
        }

        public override void Update()
        {
            if (!_mBirdStateManager.IsLocal)
            {
                
            }
            CalculateShootForce();
        }

        private void ListenRemote()
        {
            GameFrameSyncManager.PlayerInputType playerInputType = _mBirdStateManager.Entity.playerInputType;
            switch (playerInputType)
            {
                case GameFrameSyncManager.PlayerInputType.Shoot:
                    PlayerShoot();
                    break;
                case GameFrameSyncManager.PlayerInputType.QuitReady:
                    PlayerQuitShoot();
                    break;
                default: break;
            }
        }
        
        private void CalculateShootForce()
        {
            _mTimer += Time.deltaTime;

            if (_mTimer >= _mReadyShootDuration)
            {
                _mReverse = !_mReverse;
                _mTimer = 0f;
            }
            
            float force = _mReverse? Mathf.Lerp(_mMaxShootForce, _mMinShootForce, _mTimer / _mReadyShootDuration)
                : Mathf.Lerp(_mMinShootForce, _mMaxShootForce, _mTimer / _mReadyShootDuration);
            _mBirdStateManager.SetShootForce(force);
            
        }
        
        public override void Exit()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Ready State Exit");
            ChaosBallInputRegister.Instance.OnPlayerShoot -= PlayerShoot;
            ChaosBallInputRegister.Instance.OnPlayerQuitShoot -= PlayerQuitShoot;
        }
    }
}