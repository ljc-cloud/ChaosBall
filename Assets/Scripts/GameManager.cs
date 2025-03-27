using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChaosBall.Game;
using ChaosBall.Model;
using ChaosBall.Net;
using ChaosBall.Net.Request;
using UnityEngine;

namespace ChaosBall
{
    public enum GameState
    {
        NotStarted,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }
    /// <summary>
    /// 游戏规则：
    /// 游戏开始时，随机决定先手
    /// 打完一回合后，由得分高的玩家先手
    /// 发射的球越界直接销毁
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        
        public const int MaxRound = 4;
        public const string CHANGE_OPERATION_TEXT = "交换投球权";

        public static GameManager Instance { get; private set; }
        public ReadOnlyDictionary<Entity.PlayerType, PlayerInfo> PlayerTypeToPlayerInfoDict => new(_mPlayerTypeToPlayerInfoDict);
        public event Action<Entity.PlayerType, PlayerScoreBoard> OnPlayerScoreBoardChanged;
        public event Action<int> OnCountdownChanged;
        public event Action<GameState> OnGameStateChanged;
        public event Action<int, string> OnRoundMessaging;
        public event Action<int> OnRoundChanged;

        private float _mCountdownTimer;
        private int _mCountdown;

        private int _mFirstPlayerId;
        
        private int _mCurrentRound = 1;
        private GameObject[] _mBirdPrefabArray;
        private int[] _mScoreArray;
        
        private readonly Dictionary<int, PlayerScoreBoard> _mPlayerIdScoreBoardDict = new();
        private readonly Dictionary<int, List<Bird>> _mPlayerIdToBirdDict = new();
        private readonly Dictionary<Entity.PlayerType, PlayerInfo> _mPlayerTypeToPlayerInfoDict = new();

        private GameState _mCurrentGameState = GameState.NotStarted;
        
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
            GameInterface.Interface.RoomManager.OnRoomPlayerAllReady += OnRoomPlayerAllReady;
        }

        private void Update()
        {
            switch (_mCurrentGameState)
            {
                case GameState.NotStarted:
                    break;
                case GameState.CountdownToStart:
                    Countdown();
                    break;
                case GameState.GamePlaying:
                    break;
                case GameState.GameOver:
                    break;
            }
        }

        private void Countdown()
        {
            if (_mCountdown <= 0)
            {
                SetCurrentGameState(GameState.GamePlaying);
                return;
            }
            _mCountdownTimer -= Time.deltaTime;
            if (_mCountdownTimer <= 0f)
            {
                _mCountdown -= 1;
                OnCountdownChanged?.Invoke(_mCountdown);
            }
        }

        private void OnDestroy()
        {
            GameInterface.Interface.RoomManager.OnRoomPlayerAllReady -= OnRoomPlayerAllReady;
        }

        private void OnRoomPlayerAllReady()
        {
            Debug.Log("OnRoomPlayerAllReady");
            List<RoomPlayerInfo> roomPlayerList = GameInterface.Interface.RoomManager.RoomPlayerList;
            foreach (var roomPlayerInfo in  roomPlayerList)
            {
                _mPlayerTypeToPlayerInfoDict.Add(
                    roomPlayerInfo.id == GameInterface.Interface.LocalPlayerInfo.id
                        ? Entity.PlayerType.Local
                        : Entity.PlayerType.Remote, new PlayerInfo
                    {
                        id = roomPlayerInfo.id,
                        nickname = roomPlayerInfo.nickname,
                        username = roomPlayerInfo.username,
                    });
            }
            InitPlayerScoreBoard();
        }

        private void InitPlayerScoreBoard()
        {
            List<RoomPlayerInfo> roomPlayerInfoList = GameInterface.Interface.RoomManager.RoomPlayerList;
            foreach (var roomPlayerInfo in roomPlayerInfoList)
            {
                _mPlayerIdScoreBoardDict[roomPlayerInfo.id] = new PlayerScoreBoard
                {
                    nickname = roomPlayerInfo.nickname,
                    operationLeft = 4,
                    scoreArray = new int[MaxRound]
                };
            }
        }

        public void SetFirst(int playerId)
        {
            _mFirstPlayerId = playerId;
        }

        public void SetCurrentGameState(GameState newGameState)
        {
            _mCurrentGameState = newGameState;
            OnGameStateChanged?.Invoke(_mCurrentGameState);
            switch (newGameState)
            {
                case GameState.NotStarted:
                    break;
                case GameState.CountdownToStart:
                    _mCountdownTimer = 1f;
                    break;
                case GameState.GamePlaying:
                    string nickname = GetFirstPlayerNickname();
                    string message = $"玩家{nickname}先手";
                    OnRoundMessaging?.Invoke(1, message);
                    break;
            }
        }

        private string GetFirstPlayerNickname()
        {
            foreach (var keyValuePair in _mPlayerTypeToPlayerInfoDict)
            {
                if (keyValuePair.Value.id == _mFirstPlayerId)
                {
                    return keyValuePair.Value.nickname;
                }
            }

            return string.Empty;
        }

        public void FinishOperation(int playerId)
        {
            List<Bird> birdList = _mPlayerIdToBirdDict[playerId];
            int[] totalScore = birdList.Select(item => item.score).ToArray();
            
            // 发送操作结束请求
            FinishOperationRequest finishOperationRequest = GameInterface
                .Interface.RequestManager.GetRequest<FinishOperationRequest>();
            finishOperationRequest.SendFinishOperationRequest(totalScore);
            
            
        }

        public void UpdatePlayerScoreBoard(int playerId, int operationLeft, int[] scoreArray)
        {
            PlayerScoreBoard playerScoreBoard = _mPlayerIdScoreBoardDict[playerId];
            playerScoreBoard.operationLeft = operationLeft;
            playerScoreBoard.scoreArray = scoreArray;
            
            int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;
            
            Entity.PlayerType playerType = localPlayerId == playerId? Entity.PlayerType.Local : Entity.PlayerType.Remote;
            
            OnPlayerScoreBoardChanged?.Invoke(playerType, playerScoreBoard);
        }

        public void ChangeCurrentRound(int currentRound)
        {
            if (_mCurrentRound != currentRound)
            {
                // 回合数改变
                _mCurrentRound = currentRound;
                OnRoundChanged?.Invoke(_mCurrentRound);
            }
        }

        public Dictionary<Entity.PlayerType, PlayerScoreBoard> GetPlayerScoreBoard()
        {
            var result = new Dictionary<Entity.PlayerType, PlayerScoreBoard>();
            int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;
            foreach (var kv in _mPlayerIdScoreBoardDict)
            {
                if (kv.Key == localPlayerId)
                {
                    result[Entity.PlayerType.Local] = kv.Value;
                }
                else
                {
                    result[Entity.PlayerType.Remote] = kv.Value;
                }
            }

            return result;
        }
    }
}
