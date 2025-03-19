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
        }

        protected override void HandleServerFailResponse(MainPack pack)
        {
            base.HandleServerFailResponse(pack);
        }

        public void SendFinishOperationRequest(int[] totalScore)
        {
            int localPlayerId = GameInterface.Interface.PlayerInfo.id;

            PlayerInfoPack playerInfoPack = new PlayerInfoPack { Id = localPlayerId };

            FinishOperationPack finishOperationPack = new FinishOperationPack{ TotalScore = { totalScore }};

            MainPack mainPack = new MainPack
            {
                PlayerInfoPack = playerInfoPack,
                FinishOperationPack = finishOperationPack
            };
            
            SendRequest(mainPack);
        }
    }
}