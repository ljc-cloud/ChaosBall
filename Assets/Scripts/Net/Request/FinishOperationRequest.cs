using System.Linq;
using SocketProtocol;

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
            int playerId = pack.PlayerInfoPack.Id;
            PlayerScoreBoardPack playerScoreBoardPack = pack.PlayerScoreBoardPack;
            
            int operationLeft = playerScoreBoardPack.OperationLeft;
            int[] scoreArray = playerScoreBoardPack.ScoreArray.ToArray();

            int currentRound = pack.RoundPack.CurrentRound;

            GameManager.Instance.UpdatePlayerScoreBoard(playerId, operationLeft, scoreArray);
            GameManager.Instance.ChangeCurrentRound(currentRound);
        }

        protected override void HandleServerFailResponse(MainPack pack)
        {
            base.HandleServerFailResponse(pack);
        }

        public void SendFinishOperationRequest(int[] totalScore)
        {
            int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;

            PlayerInfoPack playerInfoPack = new PlayerInfoPack { Id = localPlayerId };
            RoomInfoPack roomInfoPack = new RoomInfoPack
                { RoomCode = GameInterface.Interface.RoomManager.CurrentRoomInfo.roomCode };

            FinishOperationPack finishOperationPack = new FinishOperationPack{ TotalScore = { totalScore }};

            MainPack mainPack = new MainPack
            {
                PlayerInfoPack = playerInfoPack,
                RoomInfoPack = roomInfoPack,
                FinishOperationPack = finishOperationPack
            };
            
            SendRequest(mainPack);
        }
    }
}