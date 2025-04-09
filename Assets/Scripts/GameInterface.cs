using System;
using ChaosBall.Event;
using ChaosBall.Model;
using ChaosBall.Net;
using ChaosBall.Net.Request;
using ChaosBall.So;
using ChaosBall.UI;
using UnityEngine;

namespace ChaosBall
{
    public class GameInterface : MonoBehaviour
    {
        [Header("UI")] [SerializeField] private UIPanelSoListSo uiPanelSoListSo;
        [Header("网络")]
        [SerializeField] private string serverIP;
        [SerializeField] private int serverPort;
        
        public static GameInterface Interface { get; private set; }
        public TcpClient TcpClient { get; private set; }
        public UdpListener UdpListener { get; private set; }
        public RequestManager RequestManager { get; private set; }
        public UIManager UIManager { get; private set; }
        public GameFrameSyncManager GameFrameSyncManager { get; private set; }
        public PlayerInfo LocalPlayerInfo { get; set; }
        public RoomManager RoomManager { get; private set; }
        public SceneLoader SceneLoader { get; private set; }
        public EventSystem EventSystem { get; private set; }
        
        private void Awake()
        {
            if (Interface != null)
            {
                Destroy(gameObject);
                return;
            }
            Interface = this;
            DontDestroyOnLoad(gameObject);
            
            TcpClient = new TcpClient(serverIP, serverPort);
            UdpListener= new UdpListener();
            RequestManager = new RequestManager();
            UIManager = new UIManager(uiPanelSoListSo);
            SceneLoader = new SceneLoader();
            RoomManager = new RoomManager();
            GameFrameSyncManager = new GameFrameSyncManager();
            EventSystem = new EventSystem();
            RequestManager.OnInit();
            UIManager.OnInit();
            RoomManager.OnInit();
            GameFrameSyncManager.OnInit();
            
            UIManager.PushUIPanel(UIPanelType.MainMenuUI, ShowUIPanelType.FadeIn);

#if UNITY_EDITOR
            serverIP = "127.0.0.1";
#endif
        }

        private void OnDestroy()
        {
            Debug.Log("GameInterface::OnDestroy");
            RequestManager?.OnDestroy();
            UIManager?.OnDestroy();
            TcpClient?.Dispose();
            UdpListener?.Dispose();
            GameFrameSyncManager?.OnDestroy();
            SceneLoader = null;
            TcpClient = null;
        }
    }
}