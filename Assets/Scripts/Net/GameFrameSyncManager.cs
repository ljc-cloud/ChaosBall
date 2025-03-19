using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Game;
using ChaosBall.Inputs;
using ChaosBall.Math;
using GameFrameSync;
using UnityEngine;

namespace ChaosBall.Net
{
    public class GameFrameSyncManager : BaseManager
    {
        public enum PlayerInputType
        {
            None = 1,
            MoveLeft = 2,
            MoveRight = 3,
            Ready = 4,
            Shoot = 5,
            QuitReady = 6,
        }
        
        struct PlayerInputEvent
        {
            public bool available;
            public PlayerInputType playerInputType;
        }
        
        private PlayerInputEvent _mLocalPlayerInputEvent;
        private Vector3 _mLocalCurrentPosition;
        private SortedList<int, ResFrameSyncData> _mHistoryFrameSyncData = new();
        private List<Entity> _mEntities = new();
        private int _mSyncedFrameId;

        public event Action<List<FrameInputData>> OnFrameSync;

        public override void OnInit()
        {
            GameInterface.Interface.UdpListener.OnReceiveFrameSync += ServerFrameSyncDataUpdate;
            base.OnInit();
        }

        public override void OnDestroy()
        {
            GameInterface.Interface.UdpListener.OnReceiveFrameSync -= ServerFrameSyncDataUpdate;
            base.OnDestroy();
        }

        [Obsolete]
        private void UpdatePlayerInput(PlayerInputType playerInputType)
        {
            _mLocalPlayerInputEvent.available = true;
            _mLocalPlayerInputEvent.playerInputType = playerInputType;
        }

        private void ServerFrameSyncDataUpdate(ResFrameSyncData resFrameSyncData)
        {
            if (resFrameSyncData.FrameId != -1)
            {
                // 缓存上一帧
                _mHistoryFrameSyncData[resFrameSyncData.FrameId] = resFrameSyncData;
            
                // 同步这一帧
                List<FrameInputData> frameInputDataList = resFrameSyncData.PlayersFrameInputData.ToList();

                List<FrameInputData> currentFrameInputDataList = frameInputDataList.FindAll(item => 
                    item.FrameId == _mSyncedFrameId);
                OnFrameSync?.Invoke(currentFrameInputDataList);
            }
            
            _mSyncedFrameId++;
            
            // 上传下一帧
            int localPlayerId = GameInterface.Interface.PlayerInfo.id;

            resFrameSyncData.FrameId = _mSyncedFrameId;
            _mLocalPlayerInputEvent.playerInputType = ChaosBallInputRegister.Instance.LocalPlayerInputType;
            ReqFrameInputData reqFrameInputData = new ReqFrameInputData
            {
                FrameId = _mSyncedFrameId,
                InputType = Enum.Parse<GameFrameSync.InputType>(_mLocalPlayerInputEvent.playerInputType.ToString()),
                PlayerId = localPlayerId,
            };

            Entity localEntity = _mEntities.Find(item => item.playerType is Entity.PlayerType.Local);
            if (localEntity != null)
            {
                Invoker.Instance.DelegateList.Add(() =>
                {
                    _mLocalCurrentPosition = localEntity.transform.position;
                    reqFrameInputData.Position = new Vector3D
                    {
                        X = new FixedPoint(_mLocalCurrentPosition.x),
                        Y = new FixedPoint(_mLocalCurrentPosition.y),
                        Z = new FixedPoint(_mLocalCurrentPosition.z)
                    };
                });
                
                reqFrameInputData.ShootDirection = new Vector3D
                {
                    X = new FixedPoint(localEntity.shootDirection.x),
                    Y = new FixedPoint(localEntity.shootDirection.y),
                    Z = new FixedPoint(localEntity.shootDirection.z),
                };
                reqFrameInputData.Force = new FixedPoint(localEntity.shootForce);
            }

            resFrameSyncData.ReqFrameInputData = reqFrameInputData;
            GameInterface.Interface.UdpListener.Send(resFrameSyncData);
        }

        public void AddEntity(Entity entity)
        {
            _mEntities.Add(entity);
        }
    }
}