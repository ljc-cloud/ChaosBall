using ChaosBall.Model;
using ChaosBall.Net.Request;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class RoomUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private Button unreadyButton;
        
        private RoomPlayerReadyRequest _roomPlayerReadyRequest;
        private QuitRoomRequest _quitRoomRequest;
        
        private void Start()
        {
            RoomInfo roomInfo = GameInterface.Interface.RoomManager.CurrentRoomInfo;
            string roomName = roomInfo?.roomName ?? string.Empty;
            string roomCode = roomInfo?.roomCode ?? string.Empty;
            roomNameText.text = $"{roomName}({roomCode})";

            quitButton.onClick.AddListener(PlayerQuitRoom);
            readyButton.onClick.AddListener(PlayerReady);
            unreadyButton.onClick.AddListener(PlayerUnready);
            
            _roomPlayerReadyRequest = GameInterface.Interface.RequestManager.GetRequest<RoomPlayerReadyRequest>();
            _quitRoomRequest = GameInterface.Interface.RequestManager.GetRequest<QuitRoomRequest>();
        }

        private void PlayerQuitRoom()
        {
            _quitRoomRequest.SendQuitRoomRequest();
        }

        private void PlayerUnready()
        {
            _roomPlayerReadyRequest.SendRoomPlayerReadyRequest(false);
            readyButton.gameObject.SetActive(true);
            unreadyButton.gameObject.SetActive(false);
        }

        private void PlayerReady()
        {
            _roomPlayerReadyRequest.SendRoomPlayerReadyRequest(true);
            readyButton.gameObject.SetActive(false);
            unreadyButton.gameObject.SetActive(true);
        }
    }
}