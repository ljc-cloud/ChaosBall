using System;
using ChaosBall.So;
using UnityEngine;

namespace ChaosBall.Game
{
    public class BirdAnimation : MonoBehaviour
    {
        [SerializeField] private BirdAnimationParamSo birdAnimationParamSo;
        
        private Animator _animator;

        private bool _quitShoot;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _quitShoot = false;
        }

        public void PlayMoveL()
        {
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Idle);
            string moveLeftParam = birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.MoveLeft);
            _animator.SetBool(moveLeftParam, true);
            
        }
        public void PlayMoveR()
        {
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Idle);
            string moveRightParam = birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.MoveRight);
            _animator.SetBool(moveRightParam, true);
           
        }

        public void PlayIdle()
        {
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.MoveLeft);
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.MoveRight);
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Ready);
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Shoot);
            string idleParam = birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.Idle);
            _animator.SetBool(idleParam, true);
            
        }

        public void PlayReady()
        {
            _quitShoot = false;
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Idle);
            string readyParam =
                birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.Ready);
            _animator.SetBool(readyParam, true);
            
        }

        public void PlayShoot()
        {
            if (_quitShoot) return;
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Idle);
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Ready);
            string shootParam =
                birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.Shoot);
            _animator.SetBool(shootParam, true);
            
        }

        public void PlayQuitShoot()
        {
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Shoot);
            string readyParam =
                birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.Ready);
            _animator.SetBool(readyParam, false);
            PlayIdle();
            _quitShoot = true;
        }

        public void SetCurrentAnimationSpeed(float speed)
        {
            if (speed < .3f) return;
            _animator.SetFloat("anim_speed", speed);
        }

        private void ResetAnimation(BirdAnimationParamSo.BirdAnimationType animationType)
        {
            switch (animationType)
            {
                default: break;
                case BirdAnimationParamSo.BirdAnimationType.MoveLeft:
                    string[] moveLeftParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.MoveLeft);
                    foreach (var param in moveLeftParams)
                    {
                        _animator.SetBool(param, false);
                    }
                    break;
                case BirdAnimationParamSo.BirdAnimationType.MoveRight:
                    string[] moveRightParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.MoveRight);
                    foreach (var param in moveRightParams)
                    {
                        _animator.SetBool(param, false);
                    }
                    break;
                case BirdAnimationParamSo.BirdAnimationType.Idle:
                    string[] idleParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Idle);
                    foreach (var param in idleParams)
                    {
                        _animator.SetBool(param, false);
                    }
                    break;
                case BirdAnimationParamSo.BirdAnimationType.Ready:
                    string[] readyParams =
                        birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Ready);
                    foreach (var param in readyParams)
                    {
                        _animator.SetBool(param, false);
                    }
                    break;
                case BirdAnimationParamSo.BirdAnimationType.Shoot:
                    string[] shootParams =
                        birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Shoot);
                    foreach (var param in shootParams)
                    {
                        _animator.SetBool(param, false);
                    }
                    break;
            }
        }

        #region Deprecated

        [Obsolete]
        private void ResetIdleAnimation()
        {
            string[] idleParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Idle);
            foreach (var param in idleParams)
            {
                _animator.SetBool(param, false);
            }
        }

        [Obsolete]
        private void ResetMoveAnimation()
        {
            string[] moveLeftParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.MoveLeft);
            string[] moveRightParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.MoveRight);
            foreach (var param in moveLeftParams)
            {
                _animator.SetBool(param, false);
            }
            foreach (var param in moveRightParams)
            {
                _animator.SetBool(param, false);
            }
        }
        
        [Obsolete]
        private void ResetShootAnimation()
        {
            string[] shootParams =
                birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Shoot);
            foreach (var param in shootParams)
            {
                _animator.SetBool(param, false);
            }
        }
        
        [Obsolete]
        private void ResetReadyAnimation()
        {
            string[] readyParams =
                birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Ready);
            foreach (var param in readyParams)
            {
                _animator.SetBool(param, false);
            }
        }

        #endregion
    }
}