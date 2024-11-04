using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Balls;
using ChaosBall.Events;
using ChaosBall.Game;
using ChaosBall.Model;
using ChaosBall.Utility;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;
using Timer = ChaosBall.Utility.Timer;

namespace ChaosBall.Manager
{
    public enum PlayerEnum
    {
        Player1,
        Player2
    }

    // TODO 修改墙壁
    // TODO 设计NormalBall的其他功能性球
    // TODO 寻找特效
    // BUG 超时切换投球权，该玩家没有剩余的球却能投球
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] protected GameObject ballPrefab;
        [SerializeField] private Color player1Color;
        [SerializeField] private Color player2Color;
        [SerializeField] private Transform cameraTrans1;
        [SerializeField] private Transform cameraTrans2;
        [SerializeField] private Transform camera;
        [SerializeField] private float cameraTransModifyTime = 1f;

        [SerializeField] private GameObject ballCrashParticlePrefab;

        public event Action<PlayerEnum> OnChangePlayer;
        public event Action<List<int[]>, List<PlayerModel>> OnGameOverTriggered;

        private readonly Dictionary<PlayerEnum, PlayerModel> _playerDataDict = new();
        public Dictionary<PlayerEnum, PlayerModel> PlayerData => _playerDataDict;

        private static readonly string ROUND_SWITCH_TEXT = "交换投球权";
        public static readonly int MAX_ROUND = 4;

        private int _currentRound = 1;

        private Dictionary<PlayerEnum, int[]> _playerScoreDict = new();
        public Dictionary<PlayerEnum, int[]> PlayerScorePoints => _playerScoreDict;

        private Ball _currentBall;


        public enum GameStateEnum
        {
            CountToStart,
            Started,
            OnMessaging,
            GameOver
        }

        private PlayerEnum _currentPlayer;

        public GameStateEnum GameState { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            GameState = GameStateEnum.Started;
            _playerDataDict.Add(PlayerEnum.Player1, new PlayerModel { playerName = "Traveler", ballLeft = MAX_ROUND, score = 0 });
            _playerDataDict.Add(PlayerEnum.Player2, new PlayerModel { playerName = "Rika", ballLeft = MAX_ROUND, score = 0 });
            _playerScoreDict.Add(PlayerEnum.Player1, new int[MAX_ROUND]);
            _playerScoreDict.Add(PlayerEnum.Player2, new int[MAX_ROUND]);
        }

        private void Start()
        {
            MusicManager.Instance.StartBGM();
            ChaosBallApp.Interface.RegisterEvent<OnChangeRound>(e => { RoundCheck(); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            ChaosBallApp.Interface.SendEvent(new OnUpdatePlayerData { playerData = _playerDataDict });
            Timer.Instance.OnTimeComplete += CurrentPlayerTimeComplete;
            InitializePlayerUI();
            FirstRound();
        }

        private void Update()
        {
            IsGameOver();
        }

        private void CurrentPlayerTimeComplete()
        {
            Debug.Log($"{_currentPlayer} Time Complete");
            var message = $"{_playerDataDict[_currentPlayer].playerName}超时,切换投球权";
            StartMessaging(message, SwitchRound);
        }

        private void InitializePlayerUI()
        {
            ChaosBallApp.Interface.SendEvent(new OnChangePlayerData
            {
                playerData = new Dictionary<PlayerEnum, PlayerModel>(_playerDataDict)
            });
        }

        public int GetPlayerScore(PlayerEnum player) => _playerDataDict[player].score;

        public string GetPlayerName(PlayerEnum player) => _playerDataDict[player].playerName;

        private void FirstRound()
        {
            // SetMessagingStart();
            var random = Random.Range(0, 101);
            _currentPlayer = random % 2 == 0 ? PlayerEnum.Player1 : PlayerEnum.Player2;
            OnChangePlayer?.Invoke(_currentPlayer);
            var message = $"第一回合, {_playerDataDict[_currentPlayer].playerName}先手";
            StartMessaging(message, SpawnBall);
        }

        public void ReTry()
        {
            string message = $"{_playerDataDict[_currentPlayer].playerName}投球超出界限,重新投球!";
            StartMessaging(message, SpawnBall);
        }

        private void SwitchRound()
        {
            ChangePlayer();
            // destroy previous ball
            Destroy(_currentBall.gameObject);
            // spawn new ball
            SpawnBall();
        }

        private void RoundCheck()
        {
            // ballLeft--
            _playerDataDict[_currentPlayer].ballLeft--;
            ChaosBallApp.Interface.SendEvent(new OnChangePlayerData
                { playerData = new Dictionary<PlayerEnum, PlayerModel>(_playerDataDict) });
            if (IsGameOver())
            {
                StartMessaging("游戏结束", ProcessGameOver);
                
                return;
            }
            if (_playerDataDict[PlayerEnum.Player1].ballLeft == _playerDataDict[PlayerEnum.Player2].ballLeft)
            {
                // 回合结束
                var message = $"第{_currentRound}回合结束";
                StartMessaging(message, null);
                _currentRound++;
                var player = GetPlayerScore(PlayerEnum.Player1) > GetPlayerScore(PlayerEnum.Player2)
                    ? PlayerEnum.Player1
                    : PlayerEnum.Player2;
                message = $"第{_currentRound}回合, 由上回合胜方{_playerDataDict[player].playerName}先手";
                _currentPlayer = player;
                OnChangePlayer?.Invoke(_currentPlayer);
                StartMessaging(message, null);
            }
            else
            {
                ChangePlayer();
                var message = ROUND_SWITCH_TEXT;
                StartMessaging(message, null);
            }

            StartCoroutine(WaitMessaging(SpawnBall));
        }
        
        private bool IsGameOver()
        {
            return _playerDataDict[PlayerEnum.Player1].ballLeft <= 0 &&
                   _playerDataDict[PlayerEnum.Player2].ballLeft <= 0;
        }

        private void ProcessGameOver()
        {
            GameState = GameStateEnum.GameOver;
            StopCoroutine(SetCameraNormalMode());
            // 1. 切换场景
            SceneLoader.LoadSceneAsync(SceneEnum.GameOverScene, operation =>
            {
                ResetData();
                
                // 2. 触发事件
                operation.completed += _ =>
                {
                    Debug.Log("GameOver Scene!!!!");
                    // BUG 跨场景事件绑定，事件触发失效？？
                    OnGameOverTriggered?.Invoke(_playerScoreDict.Values.ToList(), _playerDataDict.Values.ToList());
                };
            });
        }

        private void SpawnBall()
        {
            var ballGameObject = Instantiate(ballPrefab);
            var ball = ballGameObject.GetComponent<Ball>();
            int ballIndex = MAX_ROUND - _playerDataDict[_currentPlayer].ballLeft;
            ball.SetBallIndex(ballIndex);
            if (_currentPlayer == PlayerEnum.Player1)
            {
                ballGameObject.GetComponentsInChildren<Renderer>().ForEach(item => item.material.color = player1Color);
                ball.SetPlayerInput(new Player1Input());
                ball.SetPlayerBelong(PlayerEnum.Player1);
            }
            else
            {
                ballGameObject.GetComponentsInChildren<Renderer>().ForEach(item => item.material.color = player2Color);
                ball.SetPlayerInput(new Player2Input());
                ball.SetPlayerBelong(PlayerEnum.Player2);
            }

            _currentBall = ball;
            Timer.Instance.ReStartTimer();
        }

        public void UpdatePlayerScore(PlayerEnum playerEnum, int index, int score)
        {
            _playerScoreDict[playerEnum][index] = score;
            _playerDataDict[playerEnum].score = _playerScoreDict[playerEnum].Aggregate(0, (pre, cur) => pre + cur);
            Debug.Log($"Update {playerEnum}, index:{index}, Score:{score}");
            ChaosBallApp.Interface.SendEvent(new OnChangePlayerData
                { playerData = new Dictionary<PlayerEnum, PlayerModel>(_playerDataDict) });
        }

        private void ChangePlayer()
        {
            _currentPlayer = _currentPlayer == PlayerEnum.Player1 ? PlayerEnum.Player2 : PlayerEnum.Player1;
            OnChangePlayer?.Invoke(_currentPlayer);
        }

        private IEnumerator SetCameraRoundCheckingMode()
        {
            if (camera == null) yield break;
            float currentTime = 0;
            while (currentTime < cameraTransModifyTime && (GameState == GameStateEnum.Started || GameState == GameStateEnum.OnMessaging))
            {
                currentTime += Time.deltaTime;
                camera.position = Vector3.Lerp(cameraTrans1.position, cameraTrans2.position,
                    currentTime / cameraTransModifyTime);
                yield return null;
            }
            if (camera == null) yield break;
            camera.position = cameraTrans2.position;
        }

        private IEnumerator SetCameraNormalMode()
        {
            if (camera == null) yield break;
            float currentTime = 0;
            while (currentTime < cameraTransModifyTime && (GameState == GameStateEnum.Started || GameState == GameStateEnum.OnMessaging))
            {
                currentTime += Time.deltaTime;
                camera.position = Vector3.Lerp(cameraTrans2.position, cameraTrans1.position,
                    currentTime / cameraTransModifyTime);
                yield return null;
            }
            if (camera == null) yield break;
            camera.position = cameraTrans1.position;
        }

        public void SetMessagingStart()
        {
            if (GameState != GameStateEnum.Started) return;
            StartCoroutine(SetCameraRoundCheckingMode());
            GameState = GameStateEnum.OnMessaging;
        }

        public void SetMessagingOver()
        {
            if (GameState != GameStateEnum.Started && GameState != GameStateEnum.OnMessaging) return;
            StartCoroutine(SetCameraNormalMode());
            GameState = GameStateEnum.Started;
        }

        public void CreateParticle(Vector3 position)
        {
            Instantiate(ballCrashParticlePrefab, position, Quaternion.identity);
        }

        public void StartMessaging(string message, Action onMessageOver)
        {
            SetMessagingStart();
            ChaosBallApp.Interface.SendEvent(new OnMessaging { message = message });
            StartCoroutine(WaitMessaging(onMessageOver));
        }

        private IEnumerator WaitMessaging(Action onComplete)
        {
            while (GameState == GameStateEnum.OnMessaging)
            {
                yield return null;
            }

            onComplete?.Invoke();
        }

        private void ResetData()
        {
            camera = null;
            _currentBall = null;
            cameraTrans1 = null;
            cameraTrans2 = null;
        }
    }
}

