using System;
using SocketProtocol;

namespace ChaosBall.Net.Request
{
    public class LoadGameSceneCompleteRequest : BaseRequest
    {
        public LoadGameSceneCompleteRequest()
        {
            Request = RequestCode.Game;
            Action = ActionCode.LoadGameSceneComplete;
        }

        protected override void HandleServerSuccessResponse(MainPack pack)
        {
            // KitchenGameManager.Instance.SetCurrentGameState(KitchenGameManager.State.CountdownToStart);

            GameState gameState = Enum.Parse<GameState>(pack.CurrentGameState.ToString());
            GameManager.Instance.SetCurrentGameState(gameState);
            int? firstPlayerId = pack.StartGameResultPack?.FirstPlayerId;
            
            if (firstPlayerId.HasValue)
            {
                GameManager.Instance.SetFirst(firstPlayerId.Value);
            }
            
            base.HandleServerSuccessResponse(pack);
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
            RoomInfoPack roomInfoPack = new RoomInfoPack { RoomCode = roomCode };
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