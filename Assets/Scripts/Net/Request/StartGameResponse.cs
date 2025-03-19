using SocketProtocol;
using UnityEngine;

namespace ChaosBall.Net.Request
{
    public class StartGameResponse : BaseRequest
    {

        public StartGameResponse()
        {
            Request = RequestCode.Room;
            Action = ActionCode.StartGame;
        }

        protected override void HandleServerSuccessResponse(MainPack pack)
        {
            Debug.Log("开始游戏！");
            
            Invoker.Instance.DelegateList.Add(() =>
            {
                GameInterface.Interface.UIManager.ShowMessage("开始游戏!");
                GameInterface.Interface.SceneLoader.LoadScene(Scene.GameScene);
            });

            GameInterface.Interface.UdpListener.StartListen();

            base.HandleServerSuccessResponse(pack);
        }

        protected override void HandleServerFailResponse(MainPack pack)
        {
            base.HandleServerFailResponse(pack);
        }
        
        
    }
}