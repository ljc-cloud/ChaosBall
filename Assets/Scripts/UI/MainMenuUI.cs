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
                GameInterface.Interface.UIManager.PushUIPanel(
                    GameInterface.Interface.TcpClient.IsOnline ? UIPanelType.RoomListUI : UIPanelType.SignInUI,
                    ShowUIPanelType.FadeIn);
            });
            quitButton.onClick.AddListener(() =>
            {
                GameInterface.Interface.TcpClient.CloseSocket();
                Application.Quit();
            });
        }
    }
}