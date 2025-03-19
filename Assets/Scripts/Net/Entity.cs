using System;
using System.Collections.Generic;
using ChaosBall.Game.State;
using GameFrameSync;
using UnityEngine;

namespace ChaosBall.Net
{
    public enum PlayerType
    {
        Local,
        Remote
    }
    public class Entity : MonoBehaviour
    {
        private BirdStateManager _mBirdStateManager;
        
        public PlayerType playerType;

        public int playerId;
        
        public GameFrameSyncManager.PlayerInputType playerInputType;
        public Vector3 playerPosition;
        public float shootForce;
        public Vector3 shootDirection;

        private void Awake()
        {
            _mBirdStateManager = GetComponent<BirdStateManager>();
        }

        private void Start()
        {
            GameInterface.Interface.GameFrameSyncManager.OnFrameSync += OnFrameSync;
        }

        private void OnDestroy()
        {
            GameInterface.Interface.GameFrameSyncManager.OnFrameSync -= OnFrameSync;
        }

        private void OnFrameSync(List<FrameInputData> frameDataList)
        {
            FrameInputData frameInputData = frameDataList.Find(item => item.PlayerId == playerId);
            if (frameInputData != null)
            {
                playerInputType = Enum.Parse<GameFrameSyncManager.PlayerInputType>(frameInputData.InputType.ToString());
                playerPosition = new Vector3(frameInputData.Position.X, frameInputData.Position.Y, frameInputData.Position.Z);
                shootForce = frameInputData.Force;
                shootDirection = new Vector3(frameInputData.ShootDirection.X, frameInputData.ShootDirection.Y, frameInputData.ShootDirection.Z);
                Debug.Log($"player:{playerId}收到同步消息, {frameInputData.InputType}");
            }
        }

        public void SetLocalShoot(Vector3 direction, float force)
        {
            if (playerType == PlayerType.Local)
            {
                shootDirection = direction;
                shootForce = force;
            }
        }
    }
}