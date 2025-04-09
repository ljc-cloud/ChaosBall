using System;
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
        private float _timer;
        private bool _reverse;
        private bool _quitReady;
        private readonly float _minShootForce;
        private readonly float _maxShootForce;
        private readonly float _readyShootDuration;
        
        private ArrowForceUI _arrowForceUI;
        
        public BirdReadyState(BirdStateMachine birdStateMachine, BirdAnimation birdAnimation, Entity entity,
            float minShootForce, float maxShootForce, float readyShootDuration, ArrowForceUI arrowForceUI) 
            : base(birdStateMachine, birdAnimation, entity)
        {
            _minShootForce = minShootForce;
            _maxShootForce = maxShootForce;
            _readyShootDuration = readyShootDuration;
            _arrowForceUI = arrowForceUI;
        }

        public override void Enter()
        {
            // Debug.Log($"Bird:{TargetTransform.gameObject.name} Ready State Enter");
            State = BirdStateEnum.Ready;
            _quitReady = false;
            Animation.PlayReady();
            _arrowForceUI.SetReady(true);

            if (Entity.IsLocal)
            {
                // ChaosBallInputRegister.Instance.OnPlayerShoot += Shoot;
                // ChaosBallInputRegister.Instance.OnPlayerQuitShoot += QuitShoot;
            }
            else
            {
                // Entity.OnPlayerInputChanged += OnRemotePlayerInputChanged;
            }
        }

        private void OnRemotePlayerInputChanged(object sender, Entity.OnPlayerInputChangedEventArgs e)
        {
            switch (e.inputType)
            {
                case GameFrameSyncManager.PlayerInputType.Shoot:
                    Shoot();
                    break;
                case GameFrameSyncManager.PlayerInputType.QuitReady:
                    QuitShoot();
                    break;
            }
        }

        private void Shoot()
        {
            if (_quitReady) return;
            BirdStateMachine.ChangeState(BirdStateEnum.Shoot);
        }
        private void QuitShoot()
        {
            _quitReady = true;
            BirdStateMachine.ChangeState(BirdStateEnum.UnReady);
        }

        public override void Update()
        {
            // if (!Entity.IsLocal)
            {
                ListenRemote();
            }
            CalculateShootForce();
        }

        private void ListenRemote()
        {
            GameFrameSyncManager.PlayerInputType playerInputType = Entity.playerInputType;
            switch (playerInputType)
            {
                case GameFrameSyncManager.PlayerInputType.Shoot:
                    Shoot();
                    break;
                case GameFrameSyncManager.PlayerInputType.QuitReady:
                    QuitShoot();
                    break;
            }
        }
        
        private void CalculateShootForce()
        {
            _timer += Time.deltaTime;

            if (_timer >= _readyShootDuration)
            {
                _reverse = !_reverse;
                _timer = 0f;
            }
            
            float force = _reverse? Mathf.Lerp(_maxShootForce, _minShootForce, _timer / _readyShootDuration)
                : Mathf.Lerp(_minShootForce, _maxShootForce, _timer / _readyShootDuration);
            if (Entity.IsLocal)
            {
                Entity.localShootForce = force;
            }

            float forceNormalized = force / _maxShootForce;
            _arrowForceUI.SetForceNormalized(forceNormalized);
        }
        
        public override void Exit()
        {
            // Debug.Log($"Bird:{TargetTransform.gameObject.name} Ready State Exit");
            if (Entity.IsLocal)
            {
                ChaosBallInputRegister.Instance.OnPlayerShoot -= Shoot;
                ChaosBallInputRegister.Instance.OnPlayerQuitShoot -= QuitShoot;
            }
            else
            {
                // Entity.OnPlayerInputChanged -= OnRemotePlayerInputChanged;
            }
        }
    }
}