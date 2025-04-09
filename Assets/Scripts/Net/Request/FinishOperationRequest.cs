using System.Linq;
using ChaosBall.Utility;
using SocketProtocol;
using UnityEngine;

namespace ChaosBall.Net.Request
{
    public class FinishOperationRequest : BaseRequest
    {

        public FinishOperationRequest()
        {
            Request = RequestCode.Game;
            Action = ActionCode.FinishOperation;
        }
        
        protected override void HandleServerSuccessResponse(MainPack pack)
        {
            base.HandleServerSuccessResponse(pack);
            Debug.Log("接收到操作结束响应");
            int playerId = pack.PlayerInfoPack.Id;
            PlayerScoreBoardPack playerScoreBoardPack = pack.PlayerScoreBoardPack;
            
            int operationLeft = playerScoreBoardPack.OperationLeft;
            int[] scoreArray = playerScoreBoardPack.ScoreArray.ToArray();

            GameManager.Instance.UpdatePlayerScoreBoard(playerId, operationLeft, scoreArray);
            // GameManager.Instance.ChangeCurrentRound(currentRound);
        }

        protected override void HandleServerFailResponse(MainPack pack)
        {
            base.HandleServerFailResponse(pack);
        }

        public void SendFinishOperationRequest(int playerId, int[] totalScore)
        {
            PlayerInfoPack playerInfoPack = new PlayerInfoPack { Id = playerId };
            string roomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode;
            RoomInfoPack roomInfoPack = new RoomInfoPack
                { RoomCode = CharsetUtil.DefaultToUTF8(roomCode) };

            FinishOperationPack finishOperationPack = new FinishOperationPack { TotalScore = { totalScore } };

            MainPack mainPack = new MainPack
            {
                RequestCode = Request,
                ActionCode = Action,
                PlayerInfoPack = playerInfoPack,
                RoomInfoPack = roomInfoPack,
                FinishOperationPack = finishOperationPack
            };
            
            SendRequest(mainPack);
        }
    }
}