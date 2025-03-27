using System;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Inputs;
using ChaosBall.Math;
using ChaosBall.Utility;
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
        private readonly SortedList<int, ResFrameSyncData> _mHistoryFrameSyncData = new();
        private readonly List<Entity> _mEntities = new();
        private int _mSyncedFrameId;
        
        private ObjectPool<ReqFrameInputData> _mReqFrameInputDataPool;
        private ObjectPool<Vector3D> _mVector3DPool;
        

        public event Action<List<FrameInputData>> OnFrameSync;

        public override void OnInit()
        {
            _mReqFrameInputDataPool = new ObjectPool<ReqFrameInputData>(()=> new ReqFrameInputData());
            _mVector3DPool = new ObjectPool<Vector3D>(() => new Vector3D());
            GameInterface.Interface.UdpListener.OnReceiveFrameSync += ServerFrameSyncDataUpdate;
            base.OnInit();
        }

        public override void OnDestroy()
        {
            GameInterface.Interface.UdpListener.OnReceiveFrameSync -= ServerFrameSyncDataUpdate;
            base.OnDestroy();
        }

        private void ServerFrameSyncDataUpdate(ResFrameSyncData resFrameSyncData)
        {
            ReqFrameInputData reqFrameInputData = _mReqFrameInputDataPool.Allocate();
            Vector3D position = _mVector3DPool.Allocate();
            Vector3D shootDir = _mVector3DPool.Allocate();
            try
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
                int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;

                resFrameSyncData.FrameId = _mSyncedFrameId;
                _mLocalPlayerInputEvent.playerInputType = ChaosBallInputRegister.Instance.LocalPlayerInputType;


                reqFrameInputData.FrameId = _mSyncedFrameId;
                reqFrameInputData.InputType = Enum.Parse<InputType>(_mLocalPlayerInputEvent.playerInputType.ToString());
                reqFrameInputData.PlayerId = localPlayerId;

                Entity localEntity = _mEntities.Find(item => item.playerType is Entity.PlayerType.Local);
                if (localEntity != null)
                {
                    _mLocalCurrentPosition = localEntity.LocalPlayerPosition;
                    position.X = new FixedPoint(_mLocalCurrentPosition.x);
                    position.Y = new FixedPoint(_mLocalCurrentPosition.y);
                    position.Z = new FixedPoint(_mLocalCurrentPosition.z);
                    reqFrameInputData.Position = position;
                    
                    shootDir.X = new FixedPoint(localEntity.shootDirection.x);
                    shootDir.Y = new FixedPoint(localEntity.shootDirection.y);
                    shootDir.Z = new FixedPoint(localEntity.shootDirection.z);
                    reqFrameInputData.Position = position;
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
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                _mReqFrameInputDataPool.Release(reqFrameInputData);
                _mVector3DPool.Release(position);
                _mVector3DPool.Release(shootDir);
            }
        }

        public void AddEntity(in Entity entity)
        {
            _mEntities.Add(entity);
        }
    }
}