using System;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Event;
using ChaosBall.Model;
using ChaosBall.Net.Request;
using UnityEngine;

namespace ChaosBall
{
    public class RoomManager : BaseManager
    {
        public RoomInfo CurrentRoomInfo { get; private set; }
        public bool JoinedRoom { get; private set; }
        public List<RoomPlayerInfo> RoomPlayerList { get; private set; }
        public List<int> RoomClientIdList { get; private set; }
        
        // public event Action<RoomPlayerInfo> OnRoomPlayerJoin;
        // public event Action<RoomPlayerInfo> OnRoomPlayerQuit;
        // public event Action<RoomPlayerInfo> OnRoomPlayerReadyChanged;

        // public event Action OnRoomPlayerAllReady;

        public RoomManager()
        {
            RoomPlayerList = new List<RoomPlayerInfo>();
            RoomClientIdList = new();
        }

        public override void OnInit()
        {
            GameInterface.Interface.TcpClient.OnClientCloseConnection += TcpClientCloseConnection;
            base.OnInit();
        }

        private void TcpClientCloseConnection()
        {
            if (CurrentRoomInfo != null)
            {
                Debug.Log("连接中断, 退出房间！");
                QuitRoomRequest quitRoomRequest = GameInterface.Interface.RequestManager.GetRequest<QuitRoomRequest>();
                quitRoomRequest.SendQuitRoomRequest();
            }
        }


        public void JoinRoom(RoomInfo roomInfo, List<RoomPlayerInfo> roomPlayerList)
        {
            Debug.Log($"JoinRoom::CurrentRoomInfo: {CurrentRoomInfo}");
            CurrentRoomInfo = roomInfo;
            JoinedRoom = true;
            RoomPlayerList = roomPlayerList;
        }

        public void AddAllRoomPlayer(RoomInfo roomInfo, List<RoomPlayerInfo> roomPlayerList)
        {
            CurrentRoomInfo = roomInfo;
            JoinedRoom = true;
            RoomPlayerList = roomPlayerList;
        }

        public void JoinNewRoomPlayer(RoomPlayerInfo roomPlayerInfo)
        {
            Debug.Log($"JoinNewRoomPlayer::CurrentRoomInfo: {CurrentRoomInfo}");
            RoomPlayerList.Add(roomPlayerInfo);
            // OnRoomPlayerJoin?.Invoke(roomPlayerInfo);
            GameInterface.Interface.EventSystem.Publish(new RoomPlayerJoinEvent{ roomPlayerInfo = roomPlayerInfo});
        }

        public void RoomPlayerReady(int playerId, bool ready)
        {
            RoomPlayerInfo roomPlayerInfo = RoomPlayerList.Find(item => item.id == playerId);
            roomPlayerInfo.ready = ready;
            GameInterface.Interface.EventSystem.Publish(new RoomPlayerReadyChangeEvent { roomPlayerInfo = roomPlayerInfo });
            // OnRoomPlayerReadyChanged?.Invoke(roomPlayerInfo);
            bool allReady = RoomPlayerList.All(item => item.ready);
            if (allReady)
            {
                GameInterface.Interface.EventSystem.Publish<RoomPlayerAllReadyEvent>();
                // OnRoomPlayerAllReady?.Invoke();
            }
        }

        public void QuitRoom(int playerId)
        {
            int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;
            if (localPlayerId == playerId)
            {
                CurrentRoomInfo = null;
                JoinedRoom = false;
            }
            RoomPlayerInfo roomPlayerInfo = RoomPlayerList.Find(item => item.id == playerId);
            RoomPlayerList.Remove(roomPlayerInfo);
            
            GameInterface.Interface.EventSystem.Publish(new RoomPlayerQuitEvent { roomPlayerInfo = roomPlayerInfo });
            // OnRoomPlayerQuit?.Invoke(roomPlayerInfo);
        }
    }
}