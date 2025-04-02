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
            None = 0,
            MoveLeft = 1,
            MoveRight = 2,
            Ready = 3,
            Shoot = 4,
            QuitReady = 5,
        }
        
        struct PlayerInputEvent
        {
            public bool available;
            public PlayerInputType playerInputType;
        }
        
        private PlayerInputEvent _mLocalPlayerInputEvent;
        private Vector3 _mLocalCurrentPosition;
        private readonly SortedList<int, ResFrameSyncData> _mHistoryFrameSyncData = new();
        // private readonly List<Entity> _mEntities = new();
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
            Vector3D position3D = _mVector3DPool.Allocate();
            Vector3D shootDir3D = _mVector3DPool.Allocate();
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
                    resFrameSyncData.PlayersFrameInputData.Clear();
                }

                _mSyncedFrameId = resFrameSyncData.FrameId + 1;

                // 上传下一帧
                int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;

                resFrameSyncData.FrameId = _mSyncedFrameId;
                _mLocalPlayerInputEvent.playerInputType = ChaosBallInputRegister.Instance.LocalPlayerInputType;
                
                reqFrameInputData.FrameId = _mSyncedFrameId;
                reqFrameInputData.InputType = Enum.Parse<InputType>(_mLocalPlayerInputEvent.playerInputType.ToString());
                reqFrameInputData.PlayerId = localPlayerId;

                Entity localOperationEntity = GameManager.Instance.GetLocalOperationEntity();
                if (localOperationEntity != null)
                {
                    Vector3 position = localOperationEntity.localBirdPosition;
                    position3D.X = MathUtil.GetFloat(position.x);
                    position3D.Y = MathUtil.GetFloat(position.y);
                    position3D.Z = MathUtil.GetFloat(position.z);
                    
                    shootDir3D.X = MathUtil.GetFloat(localOperationEntity.localShootDirection.x);
                    shootDir3D.Y = MathUtil.GetFloat(localOperationEntity.localShootDirection.y);
                    shootDir3D.Z = MathUtil.GetFloat(localOperationEntity.localShootDirection.z);

                    reqFrameInputData.Position = position3D;
                    reqFrameInputData.ShootDirection = shootDir3D;
                    
                    float force = MathUtil.GetFloat(localOperationEntity.localShootForce);
                    Debug.Log($"上传Force:{force}");
                    reqFrameInputData.Force = force;
                }

                resFrameSyncData.ReqFrameInputData = reqFrameInputData;

                if (GameInterface.Interface.RoomManager.JoinedRoom)
                {
                    string roomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode;
                    resFrameSyncData.RoomCode = roomCode;
                }

                GameInterface.Interface.UdpListener.Send(resFrameSyncData);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
            finally
            {
                _mReqFrameInputDataPool.Release(reqFrameInputData);
                _mVector3DPool.Release(position3D);
                _mVector3DPool.Release(shootDir3D);
                GameInterface.Interface.UdpListener.Send(resFrameSyncData);
            }
        }
    }
}