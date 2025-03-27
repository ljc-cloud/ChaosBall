using System;
using System.Collections.Generic;
using GameFrameSync;
using UnityEngine;

namespace ChaosBall.Net
{
    public class Entity : MonoBehaviour
    {
        public enum PlayerType
        {
            Local,
            Remote
        }
        
        public PlayerType playerType;

        public int playerId;
        
        public GameFrameSyncManager.PlayerInputType playerInputType;
        public Vector3 playerPosition;
        public float shootForce;
        public Vector3 shootDirection;
        
        public Vector3 LocalPlayerPosition { get; private set; }

        public bool IsLocal => playerType is PlayerType.Local;

        private void Start()
        {
            GameInterface.Interface.GameFrameSyncManager.OnFrameSync += OnFrameSync;
        }

        private void Update()
        {
            LocalPlayerPosition = transform.position;
        }

        private void OnDestroy()
        {
            GameInterface.Interface.GameFrameSyncManager.OnFrameSync -= OnFrameSync;
        }

        private void OnFrameSync(List<FrameInputData> frameDataList)
        {
            FrameInputData frameInputData = frameDataList?.Find(item => item.PlayerId == playerId);
            if (frameInputData != null)
            {
                playerInputType = Enum.Parse<GameFrameSyncManager.PlayerInputType>(frameInputData.InputType.ToString());
                if (frameInputData.Position != null)
                {
                    playerPosition = new Vector3(frameInputData.Position.X, frameInputData.Position.Y, frameInputData.Position.Z);
                }

                if (frameInputData.ShootDirection != null)
                {
                    shootDirection = new Vector3(frameInputData.ShootDirection.X, frameInputData.ShootDirection.Y
                        , frameInputData.ShootDirection.Z);
                }
                shootForce = frameInputData.Force;
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