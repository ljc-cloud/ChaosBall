using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class MainMenuUI : BaseUIPanel
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            startButton.onClick.AddListener(() =>
            {
                if (GameInterface.Interface.TcpClient.IsOnline)
                {
                    GameInterface.Interface.UIManager.PushUIPanel(UIPanelType.RoomListUI, ShowUIPanelType.FadeIn);
                }
                else
                {
                    GameInterface.Interface.UIManager.PushUIPanelAppend(UIPanelType.SignInUI, ShowUIPanelType.FadeIn);
                }
            });
            quitButton.onClick.AddListener(() =>
            {
                GameInterface.Interface.TcpClient.CloseSocket();
                Application.Quit();
            });
        }
    }
}