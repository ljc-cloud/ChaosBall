using System;
using System.Runtime.InteropServices;
using ChaosBall.Event.Game;
using ChaosBall.Utility;
using SocketProtocol;
using UnityEngine;

namespace ChaosBall.Net.Request
{
    public class LoadGameSceneCompleteRequest : BaseRequest
    {
        public LoadGameSceneCompleteRequest()
        {
            Request = RequestCode.Game;
            Action = ActionCode.LoadGameSceneComplete;
        }

        private GameState _mChangeState;

        protected override void HandleServerSuccessResponse(MainPack pack)
        {
            Debug.Log("LoadGameSceneCompleteRequest:LoadGameScene...");
            base.HandleServerSuccessResponse(pack);
            int? firstPlayerId = pack.StartGameResultPack?.FirstPlayerId;
            
            if (firstPlayerId.HasValue)
            {
                GameManager.Instance.SetFirst(firstPlayerId.Value);
            }
        }
        
        protected override void HandleServerFailResponse(MainPack pack)
        {
            base.HandleServerFailResponse(pack);
        }

        public void SendLoadGameSceneCompleteRequest(Action onSuccess = null, Action onFail = null)
        {
            int playerId = GameInterface.Interface.LocalPlayerInfo.id;
            string roomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode;

            PlayerInfoPack playerInfoPack = new PlayerInfoPack { Id = playerId };
            RoomInfoPack roomInfoPack = new RoomInfoPack
            {
                RoomCode = CharsetUtil.DefaultToUTF8(roomCode),
            };
            MainPack mainPack = new MainPack
            {
                RequestCode = Request,
                ActionCode = Action,
                PlayerInfoPack = playerInfoPack,
                RoomInfoPack = roomInfoPack,
            };
            OnServerSuccessResponse += onSuccess;
            OnServerFailResponse += onFail;
            
            SendRequest(mainPack);
        }
    }
}