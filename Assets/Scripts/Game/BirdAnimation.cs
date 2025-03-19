using System;
using ChaosBall.So;
using UnityEngine;

namespace ChaosBall.Game
{
    public class BirdAnimation : MonoBehaviour
    {
        [SerializeField] private BirdAnimationParamSo birdAnimationParamSo;
        
        private Animator _mAnimator;

        private bool _mQuitShoot;

        private void Awake()
        {
            _mAnimator = GetComponent<Animator>();
            _mQuitShoot = false;
        }

        public void PlayMoveL()
        {
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Idle);
            string moveLeftParam = birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.MoveLeft);
            _mAnimator.SetBool(moveLeftParam, true);
            
        }
        public void PlayMoveR()
        {
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Idle);
            string moveRightParam = birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.MoveRight);
            _mAnimator.SetBool(moveRightParam, true);
           
        }

        public void PlayIdle()
        {
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.MoveLeft);
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.MoveRight);
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Ready);
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Shoot);
            string idleParam = birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.Idle);
            _mAnimator.SetBool(idleParam, true);
            
        }

        public void PlayReady()
        {
            _mQuitShoot = false;
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Idle);
            string readyParam =
                birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.Ready);
            _mAnimator.SetBool(readyParam, true);
            
        }

        public void PlayShoot()
        {
            if (_mQuitShoot) return;
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Idle);
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Ready);
            string shootParam =
                birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.Shoot);
            _mAnimator.SetBool(shootParam, true);
            
        }

        public void PlayQuitShoot()
        {
            ResetAnimation(BirdAnimationParamSo.BirdAnimationType.Shoot);
            string readyParam =
                birdAnimationParamSo.GetRandomAnimationParam(BirdAnimationParamSo.BirdAnimationType.Ready);
            _mAnimator.SetBool(readyParam, false);
            PlayIdle();
            _mQuitShoot = true;
        }

        public void SetCurrentAnimationSpeed(float speed)
        {
            if (speed < .3f) return;
            _mAnimator.SetFloat("anim_speed", speed);
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
                        _mAnimator.SetBool(param, false);
                    }
                    break;
                case BirdAnimationParamSo.BirdAnimationType.MoveRight:
                    string[] moveRightParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.MoveRight);
                    foreach (var param in moveRightParams)
                    {
                        _mAnimator.SetBool(param, false);
                    }
                    break;
                case BirdAnimationParamSo.BirdAnimationType.Idle:
                    string[] idleParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Idle);
                    foreach (var param in idleParams)
                    {
                        _mAnimator.SetBool(param, false);
                    }
                    break;
                case BirdAnimationParamSo.BirdAnimationType.Ready:
                    string[] readyParams =
                        birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Ready);
                    foreach (var param in readyParams)
                    {
                        _mAnimator.SetBool(param, false);
                    }
                    break;
                case BirdAnimationParamSo.BirdAnimationType.Shoot:
                    string[] shootParams =
                        birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Shoot);
                    foreach (var param in shootParams)
                    {
                        _mAnimator.SetBool(param, false);
                    }
                    break;
            }
        }

        #region Deprecated

        private void ResetIdleAnimation()
        {
            string[] idleParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Idle);
            foreach (var param in idleParams)
            {
                _mAnimator.SetBool(param, false);
            }
        }

        private void ResetMoveAnimation()
        {
            string[] moveLeftParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.MoveLeft);
            string[] moveRightParams = birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.MoveRight);
            foreach (var param in moveLeftParams)
            {
                _mAnimator.SetBool(param, false);
            }
            foreach (var param in moveRightParams)
            {
                _mAnimator.SetBool(param, false);
            }
        }
        private void ResetShootAnimation()
        {
            string[] shootParams =
                birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Shoot);
            foreach (var param in shootParams)
            {
                _mAnimator.SetBool(param, false);
            }
        }
        private void ResetReadyAnimation()
        {
            string[] readyParams =
                birdAnimationParamSo.GetAllAnimationParams(BirdAnimationParamSo.BirdAnimationType.Ready);
            foreach (var param in readyParams)
            {
                _mAnimator.SetBool(param, false);
            }
        }

        #endregion
    }
}