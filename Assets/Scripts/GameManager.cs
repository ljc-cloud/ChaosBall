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
        public ReadOnlyDictionary<Entity.PlayerType, PlayerInfo> PlayerTypeToPlayerInfoDict => new(_mPlayerTypeToPlayerInfoDict);
        
        /// <summary>
        /// 倒计时timer
        /// </summary>
        private float _mCountdownTimer;
        /// <summary>
        /// 整数倒计时
        /// </summary>
        private int _mCountdown;
        
        /// <summary>
        /// 当前正在操作的玩家id
        /// </summary>
        private int _mCurrentOperationPlayerId;
        
        /// <summary>
        /// 当前回合
        /// </summary>
        private int _mCurrentRound = 1;
        
        /// <summary>
        /// 玩家id-玩家得分板 字典
        /// </summary>
        private readonly Dictionary<int, PlayerScoreBoard> _mPlayerIdScoreBoardDict = new();
        /// <summary>
        /// 玩家id-玩家球集合 字典
        /// </summary>
        private readonly Dictionary<int, List<Bird>> _mPlayerIdToBirdDict = new();
        private readonly Dictionary<int, List<Entity>> _mPlayerIdToEntityDict = new();
        /// <summary>
        /// 玩家id-玩家集合 字典
        /// </summary>
        private readonly Dictionary<int, Player> _mPlayerIdToPlayerDict = new();
        /// <summary>
        /// 玩家类型-玩家信息 字典
        /// </summary>
        private readonly Dictionary<Entity.PlayerType, PlayerInfo> _mPlayerTypeToPlayerInfoDict = new();

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
            // TODO: 切换操作玩家
            ChangeCurrentOperationPlayer(e.currentOperationPlayerId);
        }

        private void OnChangeRound(ChangeRoundEvent e)
        {
            // TODO: 切换回合
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
            if (_mCountdown < 0)
            {
                return;
            }
            _mCountdownTimer -= Time.deltaTime;
            if (_mCountdownTimer <= 0f)
            {
                _mCountdown -= 1;
                _mCountdownTimer = 1f;
                Debug.Log("Countdown:" + _mCountdown);
                GameInterface.Interface.EventSystem.Publish(new CountdownChangeEvent
                {
                    countdown = _mCountdown
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

        private void OnBirdStop(BirdStopEvent _)
        {
            foreach (var (_, birdList) in _mPlayerIdToBirdDict)
            {
                List<BirdState> birdStateList = birdList.Select(item => item.transform
                    .GetComponent<BirdStateManager>().CurrentState).ToList();
                bool allStop = birdStateList.All(item => item.State is BirdState.BirdStateEnum.Stop);
                if (allStop)
                {
                    FinishOperation(_mCurrentOperationPlayerId);
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
                _mPlayerIdScoreBoardDict[roomPlayerInfo.id] = new PlayerScoreBoard
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
            _mCurrentOperationPlayerId = playerId;
        }

        public void AddPlayer(Player player)
        {
            _mPlayerIdToPlayerDict[player.playerId] = player;
        }

        private void AddEntity(Entity entity)
        {
            if (_mPlayerIdToEntityDict.TryGetValue(entity.playerId, out var entities))
            {
                entities.Add(entity);
            }
            else
            {
                _mPlayerIdToEntityDict[entity.playerId] = new List<Entity> { entity };
            }
        }

        public Entity GetLocalOperationEntity()
        {
            Entity result = null;
            foreach (var (_, entities) in _mPlayerIdToEntityDict)
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
                    _mCountdownTimer = 1f;
                    _mCountdown = GameAssets.COUNT_DOWN;
                    break;
                case GameState.GamePlaying:
                    string nickname = GetCurrentOperationPlayerNickname();
                    string message = $"玩家{nickname}先手";
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

            GameObject birdPrefab = birdPrefabArray[_mCurrentRound];
            GameObject birdGameObject = Instantiate(birdPrefab);
            Entity entity = birdGameObject.GetComponent<Entity>();
            Player player = _mPlayerIdToPlayerDict[_mCurrentOperationPlayerId];
            Bird bird = birdGameObject.GetComponent<Bird>();
            bird.Index = MaxRound - player.operationLeft;
            if (localPlayerId == _mCurrentOperationPlayerId)
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
            foreach (var keyValuePair in _mPlayerTypeToPlayerInfoDict)
            {
                if (keyValuePair.Value.id == _mCurrentOperationPlayerId)
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
            List<Bird> birdList = _mPlayerIdToBirdDict[playerId];
            int[] totalScore = birdList.Select(item => item.Score).ToArray();
            
            Debug.Log($"玩家{playerId} score: {totalScore}");
            
            // 发送操作结束请求
            FinishOperationRequest finishOperationRequest = GameInterface
                .Interface.RequestManager.GetRequest<FinishOperationRequest>();
            finishOperationRequest.SendFinishOperationRequest(totalScore);

            _mPlayerIdToPlayerDict[playerId].operation = false;
        }

        private void ChangeCurrentOperationPlayer(int playerId)
        {
            if (_mPlayerIdToEntityDict.TryGetValue(playerId, out var entities))
            {
                entities.ForEach(item => item.operation = false);
            }
            _mCurrentOperationPlayerId = playerId;
            _mPlayerIdToPlayerDict[playerId].operation = true;
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
        /// 更新玩家得分板（服务端更新）
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="operationLeft"></param>
        /// <param name="scoreArray"></param>
        public void UpdatePlayerScoreBoard(int playerId, int operationLeft, int[] scoreArray)
        {
            PlayerScoreBoard playerScoreBoard = _mPlayerIdScoreBoardDict[playerId];
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
                _mCurrentRound = currentRound;
                GameInterface.Interface.EventSystem.Publish(new RoundChangeEvent
                {
                    currentRound = _mCurrentRound
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
