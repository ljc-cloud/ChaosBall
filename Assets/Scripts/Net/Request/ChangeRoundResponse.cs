using ChaosBall.Event.Net;
using SocketProtocol;
using UnityEngine;

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
            Debug.Log("接收到切换回合响应");
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