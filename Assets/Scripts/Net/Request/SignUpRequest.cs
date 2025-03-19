using System;
using ChaosBall.Model;
using SocketProtocol;
using Unity.VisualScripting;
using UnityEngine;

namespace ChaosBall.Net.Request
{
    public class SignUpRequest : BaseRequest
    {

        public SignUpRequest()
        {
            Request = RequestCode.User;
            Action = ActionCode.SignUp;
        }
        
        protected override void HandleServerSuccessResponse(MainPack pack)
        {
            Debug.Log("Sign Up Success!");
            PlayerInfoPack playerInfoPack = pack.PlayerInfoPack;
            PlayerInfo playerInfo = new PlayerInfo(playerInfoPack);
            GameInterface.Interface.PlayerInfo = playerInfo;
            
            base.HandleServerSuccessResponse(pack);
        }

        protected override void HandleServerFailResponse(MainPack pack)
        {
            Debug.Log("Sign Up Fail!");
            base.HandleServerFailResponse(pack);
        }

        public void SendSignUpRequest(string username, string nickname, string password, Action onResponse, Action onSignInFail = null)
        {
            MainPack mainPack = new MainPack
            {
                RequestCode = Request,
                ActionCode = Action
            };
            SignUpPack signUpPack = new SignUpPack
            {
                Username = username,
                Nickname = nickname,
                Password = password
            };
            mainPack.SignUpPack = signUpPack;
            OnServerSuccessResponse += onResponse;
            OnServerFailResponse += onSignInFail;
            SendRequest(mainPack);
        }
    }
}