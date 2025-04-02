using ChaosBall.Inputs;
using ChaosBall.Math;
using ChaosBall.Net;
using ChaosBall.UI;
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
        
        private ArrowForceUI _mArrowForceUI;
        
        public BirdReadyState(BirdStateManager birdStateManager, Transform targetTransform, Entity entity,
            float minShootForce, float maxShootForce, float readyShootDuration, ArrowForceUI arrowForceUI) : base(birdStateManager, targetTransform, entity)
        {
            _mMinShootForce = minShootForce;
            _mMaxShootForce = maxShootForce;
            _mReadyShootDuration = readyShootDuration;
            _mArrowForceUI = arrowForceUI;
        }

        public override void Enter()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Ready State Enter");
            State = BirdStateEnum.Ready;
            _mQuitReady = false;
            _mBirdStateManager.BirdAnimation.PlayReady();
            _mBirdStateManager.ArrowForceUI.SetReady(true);

            if (Entity.IsLocal)
            {
                ChaosBallInputRegister.Instance.OnPlayerShoot += Shoot;
                ChaosBallInputRegister.Instance.OnPlayerQuitShoot += QuitShoot;
            }
        }
        
        private void Shoot()
        {
            if (_mQuitReady) return;
            // if (Entity.IsLocal)
            // {
            //     Vector3 localDirection = _mBirdStateManager.GetDirection();
            //     float localForce = Entity.localShootForce;
            //     Entity.localShootDirection = localDirection;
            //     Entity.localShootForce = localForce;
            // }
            _mBirdStateManager.ChangeState(BirdStateEnum.Shoot);
        }
        private void QuitShoot()
        {
            _mQuitReady = true;
            _mBirdStateManager.ChangeState(BirdStateEnum.UnReady);
        }

        public override void Update()
        {
            if (!Entity.IsLocal)
            {
                ListenRemote();
            }
            CalculateShootForce();
        }

        private void ListenRemote()
        {
            GameFrameSyncManager.PlayerInputType playerInputType = _mBirdStateManager.Entity.playerInputType;
            switch (playerInputType)
            {
                case GameFrameSyncManager.PlayerInputType.Shoot:
                    Shoot();
                    break;
                case GameFrameSyncManager.PlayerInputType.QuitReady:
                    QuitShoot();
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
            if (Entity.IsLocal)
            {
                Entity.localShootForce = force;
            }

            float forceNormalized = force / _mMaxShootForce;
            _mArrowForceUI.SetForceNormalized(forceNormalized);
        }
        
        public override void Exit()
        {
            Debug.Log($"Bird:{_mTargetTransform.gameObject.name} Ready State Exit");
            ChaosBallInputRegister.Instance.OnPlayerShoot -= Shoot;
            ChaosBallInputRegister.Instance.OnPlayerQuitShoot -= QuitShoot;
        }
    }
}