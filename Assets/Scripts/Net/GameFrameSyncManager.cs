using System;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Inputs;
using ChaosBall.Math;
using ChaosBall.Utility;
using GameFrameSync;
using UnityEditor;
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
        private ObjectPool<Vector2D> _mVector2DPool;
        

        public event Action<List<FrameInputData>> OnFrameSync;

        public override void OnInit()
        {
            _mReqFrameInputDataPool = new ObjectPool<ReqFrameInputData>(()=> new ReqFrameInputData());
            _mVector2DPool = new ObjectPool<Vector2D>(() => new Vector2D());
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
            var position2D = _mVector2DPool.Allocate();
            var shootDir2D = _mVector2DPool.Allocate();
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
                    position2D.X = MathUtil.GetFloat(position.x);
                    position2D.Y = MathUtil.GetFloat(position.z);

                    shootDir2D.X = MathUtil.GetFloat(localOperationEntity.localShootDirection.x);
                    shootDir2D.Y = MathUtil.GetFloat(localOperationEntity.localShootDirection.z);

                    float arrowRotationZ = MathUtil.GetFloat(localOperationEntity.localArrowRotationZ);
                    // reqFrameInputData.Position = position3D;
                    // Debug.Log($"Send Sync Direction:{shootDir3D.X},{shootDir3D.Y},{shootDir3D.Z}");
                    // reqFrameInputData.ShootDirection = shootDir3D;
                    
                    float force = MathUtil.GetFloat(localOperationEntity.localShootForce);
                    reqFrameInputData.Force = force;
                    reqFrameInputData.ArrowRotationZ = arrowRotationZ;
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
                
                GameInterface.Interface.UdpListener.Send(resFrameSyncData);
            }
        }
    }
}