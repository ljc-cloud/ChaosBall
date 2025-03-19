using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChaosBall.Game;
using ChaosBall.Model;
using ChaosBall.Net.Request;
using ChaosBallServer.Model;
using UnityEngine;

namespace ChaosBall
{
    public class GameManager : MonoBehaviour
    {
        public const int MaxRound = 4;
        public const string ChangeRoundText = "交换投球权";

        public static GameManager Instance { get; private set; }

        private int _mCurrentRound = 1;
        private GameObject[] _mBirdPrefabArray;
        private int[] _mScoreArray;
        
        private readonly Dictionary<int, PlayerScoreBoard> _mPlayerIdScoreBoard = new();
        private readonly Dictionary<int, List<Bird>> _mPlayerIdBird = new();
        private Dictionary<Entity.PlayerType, PlayerInfo> _mPlayerTypeInfoDict = new();

        public ReadOnlyDictionary<Entity.PlayerType, PlayerInfo> PlayerTypeInfoDict => new(_mPlayerTypeInfoDict);
        
        public event Action<PlayerScoreBoard> OnPlayerScoreBoardChanged;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitPlayerScoreBoard();
            GameInterface.Interface.RoomManager.OnRoomPlayerAllReady += OnRoomPlayerAllReady;
        }

        private void OnDestroy()
        {
            GameInterface.Interface.RoomManager.OnRoomPlayerAllReady -= OnRoomPlayerAllReady;
        }

        private void OnRoomPlayerAllReady()
        {
            List<RoomPlayerInfo> roomPlayerList = GameInterface.Interface.RoomManager.RoomPlayerList;
            foreach (var roomPlayerInfo in  roomPlayerList)
            {
                _mPlayerTypeInfoDict.Add(
                    roomPlayerInfo.id == GameInterface.Interface.PlayerInfo.id
                        ? Entity.PlayerType.Local
                        : Entity.PlayerType.Remote, new PlayerInfo
                    {
                        id = roomPlayerInfo.id,
                        nickname = roomPlayerInfo.nickname,
                        username = roomPlayerInfo.username,
                    });
            }
        }

        private void InitPlayerScoreBoard()
        {
            int[] playerIdArray = GameInterface.Interface.RoomManager.RoomPlayerList.Select(item => item.id).ToArray();
            int localPlayerId = GameInterface.Interface.PlayerInfo.id;
            foreach (var playerId in playerIdArray)
            {
                _mPlayerIdScoreBoard.Add(playerId, new PlayerScoreBoard
                {
                    playerType = localPlayerId == playerId ? Entity.PlayerType.Local : Entity.PlayerType.Remote,
                    currentRound = 1,
                    roundLeft = MaxRound - 1,
                    totalScore = new int[MaxRound]
                });
            }
        }

        public void SetFirst(int playerId)
        {
            // TODO: 设置先手
        }

        public void FinishOperation(int playerId)
        {
            PlayerScoreBoard playerScoreBoard = _mPlayerIdScoreBoard[playerId];
            playerScoreBoard.currentRound++;
            playerScoreBoard.roundLeft--;

            List<Bird> birdList = _mPlayerIdBird[playerId];
            int[] totalScore = birdList.Select(item => item.score).ToArray();
            
            playerScoreBoard.totalScore = totalScore;
            
            // 发送操作结束请求
            FinishOperationRequest finishOperationRequest = GameInterface
                .Interface.RequestManager.GetRequest<FinishOperationRequest>();
            finishOperationRequest.SendFinishOperationRequest(totalScore);
        }
    }
}
