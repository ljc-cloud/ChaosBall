using System;
using System.Collections.Generic;
using ChaosBall.Math;
using ChaosBall.UI;
using GameFrameSync;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChaosBall.Net
{
    public class Entity : MonoBehaviour
    {
        public enum PlayerType
        {
            Local,
            Remote
        }

        [SerializeField] private ArrowForceUI arrowForceUI;
        /// <summary>
        /// 玩家类型（本地、远程）
        /// </summary>
        public PlayerType playerType;
        /// <summary>
        /// 玩家id
        /// </summary>
        public int playerId;
        /// <summary>
        /// 玩家输入
        /// </summary>
        public GameFrameSyncManager.PlayerInputType playerInputType;
        /// <summary>
        /// 当前位置（远程）
        /// </summary>
        public Vector3 birdPosition;
        /// <summary>
        /// 当前位置（本地）
        /// </summary>
        public Vector3 localBirdPosition;
        /// <summary>
        /// 发射力（远程）
        /// </summary>
        public float shootForce;
        /// <summary>
        /// 发射力（本地）
        /// </summary>
        public float localShootForce;
        /// <summary>
        /// 发射方向（远程）
        /// </summary>
        public Vector3 shootDirection;
        /// <summary>
        /// 发射方向（本地）
        /// </summary>
        public Vector3 localShootDirection;
        /// <summary>
        /// 箭头旋转Z轴（远程）
        /// </summary>
        public float arrowRotationZ;
        /// <summary>
        /// 箭头旋转Z轴（本地）
        /// </summary>
        public float localArrowRotationZ;
        /// <summary>
        /// 是否正在操作
        /// </summary>
        public bool operation;
        
        // public Vector3 LocalBirdPosition { get; private set; }

        public bool IsLocal => playerType is PlayerType.Local;
        
        public class OnPlayerInputChangedEventArgs : EventArgs
        {
            public GameFrameSyncManager.PlayerInputType inputType;
        }
        
        public event EventHandler<OnPlayerInputChangedEventArgs> OnPlayerInputChanged;

        private void Start()
        {
            GameInterface.Interface.GameFrameSyncManager.OnFrameSync += OnFrameSync;
        }

        private void Update()
        {
            localBirdPosition = transform.position;
            localShootDirection = -arrowForceUI.transform.TransformDirection(arrowForceUI.transform.forward);
            localArrowRotationZ = arrowForceUI.transform.localRotation.z;
            // localShootDirection = arrowForceUI.transform.forward;
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
                    birdPosition = new Vector3(frameInputData.Position.X, 0
                        , frameInputData.Position.Y);
                }

                if (frameInputData.ShootDirection != null)
                {
                    shootDirection = new Vector3(frameInputData.ShootDirection.X, 0
                        , frameInputData.ShootDirection.Y);
                    Debug.Log($"Synced direction:({frameInputData.ShootDirection.X}" +
                              $",{frameInputData.ShootDirection.Y})");
                }
                shootForce = frameInputData.Force;
                Debug.Log($"player:{playerId}收到同步消息, frameId:{frameInputData.FrameId}, " +
                          $"{frameInputData.InputType}, force: {frameInputData.Force}," +
                          $"position: {birdPosition}");
            }
        }
    }
    
}