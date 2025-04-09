using ChaosBall.Event;
using ChaosBall.Event.Net;
using SocketProtocol;
using UnityEngine;

namespace ChaosBall.Net.Request
{
    /// <summary>
    /// 切换当前操作玩家响应
    /// </summary>
    public class ChangeOperationResponse : BaseRequest
    {
        public ChangeOperationResponse()
        {
            Request = RequestCode.Game;
            Action = ActionCode.ChangeOperation;
        }

        protected override void HandleServerSuccessResponse(MainPack pack)
        {
            Debug.Log("接收到切换操作响应");
            int currentOperationPlayerId = pack.ChangeOperationPack.CurrentOperationPlayerId;
            
            GameInterface.Interface.EventSystem.Publish(new ChangeOperationEvent
            {
                currentOperationPlayerId = currentOperationPlayerId,
            });
            
            base.HandleServerSuccessResponse(pack);
        }

        protected override void HandleServerFailResponse(MainPack pack)
        {
            base.HandleServerFailResponse(pack);
        }
    }
}