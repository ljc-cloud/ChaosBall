using ChaosBall.Event.Net;
using SocketProtocol;

namespace ChaosBall.Net.Request
{
    public class ChangeRoundResponse : BaseRequest
    {
        public ChangeRoundResponse()
        {
            Request = RequestCode.Game;
            Action = ActionCode.ChangeRound;
        }

        protected override void HandleServerSuccessResponse(MainPack pack)
        {
            int currentRound = pack.RoundPack.CurrentRound;
            
            GameInterface.Interface.EventSystem.Publish(new ChangeRoundEvent
            {
                currentRound = currentRound,
            });
            
            // GameManager.Instance.ChangeCurrentRound(currentRound);
            base.HandleServerSuccessResponse(pack);
        }
    }
}