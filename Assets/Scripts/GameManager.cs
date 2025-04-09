using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChaosBall.Event;
using ChaosBall.Event.Game;
using ChaosBall.Event.Net;
using ChaosBall.Game;
using ChaosBall.Game.State;
using ChaosBall.Model;
using ChaosBall.Net;
using ChaosBall.Net.Request;
using Event.Game;
using UnityEngine;

namespace ChaosBall
{
    public enum GameState
    {
        NotStarted,
        WaitingStart,
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
        /// <summary>
        /// 最多回合
        /// </summary>
        public const int MaxRound = 4;
        /// <summary>
        /// 交换操作文本
        /// </summary>
        public const string CHANGE_OPERATION_TEXT = "交换投球权";
        
        [SerializeField] private GameObject[] birdPrefabArray;

        public static GameManager Instance { get; private set; }
        
        /// <summary>
        /// 玩家类型-玩家信息 字典
        /// </summary>
        public ReadOnlyDictionary<Entity.PlayerType, PlayerInfo> PlayerTypeToPlayerInfoDict => new(_playerTypeToPlayerInfoDict);
        
        /// <summary>
        /// 倒计时timer
        /// </summary>
        private float _countdownTimer;
        /// <summary>
        /// 整数倒计时
        /// </summary>
        private int _countdown;
        
        /// <summary>
        /// 当前正在操作的玩家id
        /// </summary>
        private int _currentOperationPlayerId;
        /// <summary>
        /// 第一回合先手玩家id
        /// </summary>
        private int _firstOperationPlayerId;
        
        /// <summary>
        /// 当前回合
        /// </summary>
        private int _currentRound = 1;
        
        /// <summary>
        /// 玩家id-玩家得分板 字典
        /// </summary>
        private readonly Dictionary<int, PlayerScoreBoard> _playerIdScoreBoardDict = new();
        /// <summary>
        /// 玩家id-玩家球集合 字典
        /// </summary>
        private readonly Dictionary<int, List<Bird>> _playerIdToBirdDict = new();
        private readonly Dictionary<int, List<Entity>> _playerIdToEntityDict = new();
        /// <summary>
        /// 玩家id-玩家集合 字典
        /// </summary>
        private readonly Dictionary<int, Player> _playerIdToPlayerDict = new();
        /// <summary>
        /// 玩家类型-玩家信息 字典
        /// </summary>
        private readonly Dictionary<Entity.PlayerType, PlayerInfo> _playerTypeToPlayerInfoDict = new();
        /// <summary>
        /// 玩家id-玩家信息 字典
        /// </summary>
        private readonly Dictionary<int, PlayerInfo> _playerIdToPlayerInfoDict = new();

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        private GameState _mCurrentGameState = GameState.NotStarted;
        
        
        /// <summary>
        /// Awake 设置单例
        /// </summary>
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
            GameInterface.Interface.EventSystem.Subscribe<RoomPlayerAllReadyEvent>(OnRoomPlayerAllReady);
            GameInterface.Interface.EventSystem.Subscribe<BirdStopEvent>(OnBirdStop);
            
            GameInterface.Interface.EventSystem.Subscribe<ChangeGameStateEvent>(OnChangeGameState);
            
            GameInterface.Interface.EventSystem.Subscribe<ChangeRoundEvent>(OnChangeRound);
            GameInterface.Interface.EventSystem.Subscribe<ChangeOperationEvent>(OnChangeOperation);
        }

        private void OnChangeOperation(ChangeOperationEvent e)
        {
            // 切换操作玩家
            ChangeCurrentOperationPlayer(e.currentOperationPlayerId);
        }

        private void OnChangeRound(ChangeRoundEvent e)
        {
            // TODO: 切换回合
            ChangeRound(e.currentRound);
        }

        private void OnChangeGameState(ChangeGameStateEvent e)
        {
            ChangeCurrentGameState(e.newState);
        }

        /// <summary>
        /// 游戏状态机
        /// </summary>
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

        /// <summary>
        /// 倒计时，每过一秒通知UI更新倒计时动画
        /// </summary>
        private void Countdown()
        {
            if (_countdown < 0)
            {
                return;
            }
            _countdownTimer -= Time.deltaTime;
            if (_countdownTimer <= 0f)
            {
                _countdown -= 1;
                _countdownTimer = 1f;
                Debug.Log("Countdown:" + _countdown);
                GameInterface.Interface.EventSystem.Publish(new CountdownChangeEvent
                {
                    countdown = _countdown
                });
            }
        }

        private void OnDestroy()
        {
            GameInterface.Interface.EventSystem.Unsubscribe<RoomPlayerAllReadyEvent>(OnRoomPlayerAllReady);
            GameInterface.Interface.EventSystem.Unsubscribe<BirdStopEvent>(OnBirdStop);
            GameInterface.Interface.EventSystem.Unsubscribe<ChangeGameStateEvent>(OnChangeGameState);
        }
        
        private void OnRoomPlayerAllReady(RoomPlayerAllReadyEvent _)
        {
            Debug.Log("OnRoomPlayerAllReady");
            List<RoomPlayerInfo> roomPlayerList = GameInterface.Interface.RoomManager.RoomPlayerList;
            foreach (var roomPlayerInfo in roomPlayerList)
            {
                Entity.PlayerType playerType;
                if (roomPlayerInfo.id == GameInterface.Interface.LocalPlayerInfo.id)
                {
                    playerType = Entity.PlayerType.Local;
                }
                else
                {
                    playerType = Entity.PlayerType.Remote;
                }
                _playerTypeToPlayerInfoDict[playerType] = roomPlayerInfo;
                _playerIdToPlayerInfoDict[roomPlayerInfo.id] = roomPlayerInfo;
                Debug.Log($"添加玩家信息{playerType}, {roomPlayerInfo}");
            }
            
            InitPlayerScoreBoard();
        }

        private void OnBirdStop(BirdStopEvent _)
        {
            foreach (var (_, birdList) in _playerIdToBirdDict)
            {
                List<BirdState> birdStateList = birdList.Select(item => item.transform
                    .GetComponent<BirdStateMachine>().CurrentState).ToList();
                bool allStop = birdStateList.All(item => item.State is BirdState.BirdStateEnum.Stop);
                if (allStop)
                {
                    FinishOperation(_currentOperationPlayerId);
                }
            }
        }

        /// <summary>
        /// 初始化玩家得分板
        /// </summary>
        private void InitPlayerScoreBoard()
        {
            List<RoomPlayerInfo> roomPlayerInfoList = GameInterface.Interface.RoomManager.RoomPlayerList;
            foreach (var roomPlayerInfo in roomPlayerInfoList)
            {
                _playerIdScoreBoardDict[roomPlayerInfo.id] = new PlayerScoreBoard
                {
                    nickname = roomPlayerInfo.nickname,
                    operationLeft = 4,
                    scoreArray = new int[MaxRound]
                };
            }
        }

        /// <summary>
        /// 设置先手玩家
        /// </summary>
        /// <param name="playerId"></param>
        public void SetFirst(int playerId)
        {
            _currentOperationPlayerId = playerId;
        }

        public void AddPlayer(Player player)
        {
            _playerIdToPlayerDict[player.playerId] = player;
        }

        private void AddEntity(Entity entity)
        {
            if (_playerIdToEntityDict.TryGetValue(entity.playerId, out var entities))
            {
                entities.Add(entity);
            }
            else
            {
                _playerIdToEntityDict[entity.playerId] = new List<Entity> { entity };
            }
        }

        public Entity GetLocalOperationEntity()
        {
            Entity result = null;
            foreach (var (_, entities) in _playerIdToEntityDict)
            {
                result = entities.Find(item => item.operation && item.IsLocal);
            }

            return result;
        }

        /// <summary>
        /// 切换当前游戏状态
        /// </summary>
        /// <param name="newGameState"></param>
        private void ChangeCurrentGameState(GameState newGameState)
        {
            _mCurrentGameState = newGameState;
            GameInterface.Interface.EventSystem.Publish(new GameStateChangedEvent
            {
                state = _mCurrentGameState
            });
            switch (newGameState)
            {
                case GameState.NotStarted:
                    break;
                case GameState.CountdownToStart:
                    _countdownTimer = 1f;
                    _countdown = GameAssets.COUNT_DOWN;
                    break;
                case GameState.GamePlaying:
                    string nickname = GetCurrentOperationPlayerNickname();
                    string message = $"由玩家{nickname}先手";
                    GameInterface.Interface.EventSystem.Publish(new RoundMessageEvent
                    {
                        currentRound = 1,
                        message = message
                    });
                    SpawnBird();
                    break;
            }
        }

        private void SpawnBird()
        {
            PlayerInfo localPlayerInfo = GameInterface.Interface.LocalPlayerInfo;
            int localPlayerId = localPlayerInfo.id;

            GameObject birdPrefab = birdPrefabArray[_currentRound];
            GameObject birdGameObject = Instantiate(birdPrefab);
            
            Entity entity = birdGameObject.GetComponent<Entity>();
            Player player = _playerIdToPlayerDict[_currentOperationPlayerId];
            Bird bird = birdGameObject.GetComponent<Bird>();
            
            bird.Index = MaxRound - player.operationLeft;
            if (_playerIdToBirdDict.TryGetValue(_currentOperationPlayerId, out var birdList))
            {
                birdList.Add(bird);
            }
            else
            {
                _playerIdToBirdDict[_currentOperationPlayerId] = new List<Bird> { bird };
            }
            
            if (localPlayerId == _currentOperationPlayerId)
            {
                // 本地玩家
                entity.playerId = localPlayerId;
                entity.playerType = Entity.PlayerType.Local;
            }
            else
            {
                if (PlayerTypeToPlayerInfoDict.TryGetValue(Entity.PlayerType.Remote, out var remotePlayerInfo))
                {
                    entity.playerId = remotePlayerInfo.id;
                    entity.playerType = Entity.PlayerType.Remote;
                }
            }

            entity.birdPosition = birdGameObject.transform.position;
            entity.operation = true;
            AddEntity(entity);
        }

        /// <summary>
        /// 获取当前操作玩家nickname
        /// </summary>
        /// <returns></returns>
        private string GetCurrentOperationPlayerNickname()
        {
            foreach (var keyValuePair in _playerTypeToPlayerInfoDict)
            {
                if (keyValuePair.Value.id == _currentOperationPlayerId)
                {
                    return keyValuePair.Value.nickname;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 玩家完成操作
        /// </summary>
        /// <param name="playerId">玩家id</param>
        public void FinishOperation(int playerId)
        {
            List<Bird> birdList = _playerIdToBirdDict[playerId];
            int[] score = birdList.Select(item => item.Score).ToArray();
            int totalScore = score.Sum();
            
            Debug.Log($"玩家{playerId} score: {totalScore}");
            
            // 发送操作结束请求
            FinishOperationRequest finishOperationRequest = GameInterface
                .Interface.RequestManager.GetRequest<FinishOperationRequest>();
            finishOperationRequest.SendFinishOperationRequest(playerId, score);

            _playerIdToPlayerDict[playerId].operation = false;
            // _mPlayerIdToEntityDict[playerId].ForEach(item => item.operation = false);
        }

        private void ChangeCurrentOperationPlayer(int playerId)
        {
            if (_playerIdToEntityDict.TryGetValue(playerId, out var entities))
            {
                entities.ForEach(item => item.operation = false);
            }
            _currentOperationPlayerId = playerId;
            _playerIdToPlayerDict[playerId].operation = true;
            GameInterface.Interface.EventSystem.Publish(new CurrentOperationPlayerChangedEvent
            {
                currentOperationPlayerId = playerId,
            });
            
            GameInterface.Interface.EventSystem.Publish(new GameMessageEvent
            {
                message = CHANGE_OPERATION_TEXT,
            });
            SpawnBird();
        }

        /// <summary>
        /// 切换回合
        /// 得分高者先手，得分相同由第一回合先手的玩家先手
        /// </summary>
        private void ChangeRound(int currentRound)
        {
            _currentRound = currentRound;

            int maxScore = -1000;
            int operationPlayerId = -1;
           
            foreach (var (playerId, scoreBoard) in _playerIdScoreBoardDict)
            {
                if (scoreBoard.scoreArray is { Length: > 0 })
                {
                    int score = scoreBoard.scoreArray.Aggregate((pre, next) => pre + next);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        operationPlayerId = playerId;
                    }
                    else if (score == maxScore)
                    {
                        operationPlayerId = -1;
                    }
                }
            }

            string roundMessage = string.Empty;
            if (operationPlayerId == -1)
            {
                string nickname = _playerIdToPlayerInfoDict[_firstOperationPlayerId].nickname;
                _currentOperationPlayerId = _firstOperationPlayerId;
                roundMessage = $"比分相同,由第一回合先手玩家{nickname}先手";
            }
            else
            {
                string nickname = _playerIdToPlayerInfoDict[operationPlayerId].nickname;
                _currentOperationPlayerId = operationPlayerId;
                roundMessage = $"由上一回合得分高玩家{nickname}先手";
            }

            GameInterface.Interface.EventSystem.Publish(new RoundMessageEvent
            {
                currentRound = _currentRound,
                message = roundMessage,
            });
            
            Invoke(nameof(SpawnBird), 1f);
        }

        /// <summary>
        /// 更新玩家得分板（服务端更新）
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="operationLeft"></param>
        /// <param name="scoreArray"></param>
        public void UpdatePlayerScoreBoard(int playerId, int operationLeft, int[] scoreArray)
        {
            PlayerScoreBoard playerScoreBoard = _playerIdScoreBoardDict[playerId];
            playerScoreBoard.operationLeft = operationLeft;
            playerScoreBoard.scoreArray = scoreArray;
            
            int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;
            
            Entity.PlayerType playerType = localPlayerId == playerId? Entity.PlayerType.Local : Entity.PlayerType.Remote;
            
            GameInterface.Interface.EventSystem.Publish(new PlayerScoreBoardChangeEvent
            {
                playerType = playerType,
                playerScoreBoard = playerScoreBoard,
                operationLeft = playerScoreBoard.operationLeft,
            });
            // OnPlayerScoreBoardChanged?.Invoke(playerType, playerScoreBoard);
        }

        /// <summary>
        /// 更新当前回合
        /// </summary>
        /// <param name="currentRound">当前回合</param>
        public void ChangeCurrentRound(int currentRound)
        {
            // if (_mCurrentRound != currentRound)
            {
                // 回合数改变
                _currentRound = currentRound;
                GameInterface.Interface.EventSystem.Publish(new RoundChangeEvent
                {
                    currentRound = _currentRound
                });
            }
        }
        
        /// <summary>
        /// 获取玩家得分板
        /// </summary>
        /// <returns>玩家类型-得分板 字典</returns>
        public Dictionary<Entity.PlayerType, PlayerScoreBoard> GetPlayerScoreBoard()
        {
            var result = new Dictionary<Entity.PlayerType, PlayerScoreBoard>();
            int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;
            foreach (var kv in _playerIdScoreBoardDict)
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
