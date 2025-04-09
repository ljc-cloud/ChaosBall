using System;
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
        private readonly Transform _transform;
        private Vector2 _moveVector;
        private readonly float _speed;
        private readonly ArrowForceUI _arrowForceUI;

        public BirdUnReadyState(BirdStateMachine birdStateMachine, BirdAnimation birdAnimation, Entity entity,
            Transform transform
            , float speed, ArrowForceUI arrowForceUI)
            : base(birdStateMachine, birdAnimation, entity)
        {
            _transform = transform;
            _speed = speed;
            _arrowForceUI = arrowForceUI;
        }

        public override void Enter()
        {
            // Debug.Log($"Bird:{TargetTransform.gameObject.name} Unready State Enter");
            State = BirdStateEnum.UnReady;
            Animation.PlayIdle();
            _arrowForceUI.SetReady(false);

            if (Entity.IsLocal)
            {
                // ChaosBallInputRegister.Instance.OnPlayerChangePosition += BirdChangePosition;
                // ChaosBallInputRegister.Instance.OnPlayerStop += BirdMoveStop;
                // ChaosBallInputRegister.Instance.OnPlayerReadyToShoot += PlayerReadyToShoot;
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
                case GameFrameSyncManager.PlayerInputType.None: _moveVector = Vector2.zero; break;
                case GameFrameSyncManager.PlayerInputType.MoveLeft: _moveVector = Vector2.left; break;
                case GameFrameSyncManager.PlayerInputType.MoveRight: _moveVector = Vector2.right; break;
                case GameFrameSyncManager.PlayerInputType.Ready: 
                    BirdStateMachine.ChangeState(BirdStateEnum.Ready);
                    break;
            }
        }

        private void BirdChangePosition(Vector2 vec)
        {
            _moveVector = vec;
        }
        private void BirdMoveStop()
        {
            _moveVector = Vector2.zero;
            Animation.PlayIdle();
        }
        private void PlayerReadyToShoot()
        {
            BirdStateMachine.ChangeState(BirdStateEnum.Ready);
        }

        public override void Update()
        {
            // if (!Entity.IsLocal)
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
                case GameFrameSyncManager.PlayerInputType.None: _moveVector = Vector2.zero; break;
                case GameFrameSyncManager.PlayerInputType.MoveLeft: _moveVector = Vector2.left; break;
                case GameFrameSyncManager.PlayerInputType.MoveRight: _moveVector = Vector2.right; break;
                case GameFrameSyncManager.PlayerInputType.Ready: 
                    BirdStateMachine.ChangeState(BirdStateEnum.Ready);
                    break;
            }
            // 移动插值
            Vector3 birdPosition = Entity.birdPosition;
            // Debug.Log($"Lerp Position: {birdPosition}");
            _transform.position =
                Vector3.Lerp(_transform.position, birdPosition, 0.5f * Time.deltaTime);
            
            // 方向插值
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, Entity.arrowRotationZ));
            Quaternion.Slerp(_arrowForceUI.transform.localRotation, targetRotation, 0.5f * Time.deltaTime);
        }

        private void DoMove()
        {
            _transform.Translate(_moveVector * (_speed * Time.deltaTime));
            if (_moveVector.x > 0f)
            {
                Animation.PlayMoveR();
            }
            else if(_moveVector.x < 0f)
            {
                Animation.PlayMoveL();
            }

            // if (!Entity.IsLocal)
            // {
            //     Vector3 birdPosition = Entity.birdPosition;
            //     // Debug.Log($"Lerp Position: {birdPosition}");
            //     _transform.position =
            //         Vector3.Lerp(_transform.position, birdPosition, 0.5f * Time.deltaTime);
            // }
        }
        
        public override void Exit()
        {
            Debug.Log($"Bird:{_transform.gameObject.name} Unready State Exit");
            if (Entity.IsLocal)
            {
                ChaosBallInputRegister.Instance.OnPlayerChangePosition -= BirdChangePosition;
                ChaosBallInputRegister.Instance.OnPlayerStop -= BirdMoveStop;
                ChaosBallInputRegister.Instance.OnPlayerReadyToShoot -= PlayerReadyToShoot;
            }
            else
            {
                // Entity.OnPlayerInputChanged -= OnRemotePlayerInputChanged;
            }
        }
    }
}