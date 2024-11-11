using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChaosBall.Balls;
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

    // 修改墙壁 DONE!
    // TODO 设计NormalBall的其他功能性球
    // TODO 寻找特效
    // 超时切换投球权，该玩家没有剩余的球却能投球 DONE!
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private LevelDataList levelDataList;
        [SerializeField] private Color player1Color;
        [SerializeField] private Color player2Color;
        [SerializeField] private Vector3 cameraNormalPostion;
        [SerializeField] private Vector3 cameraNormalRotation;
        [SerializeField] private Vector3 cameraCheckingPostion;
        [SerializeField] private Vector3 cameraCheckingRotation;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float cameraTransModifyDuration = 1f;

        [SerializeField] private GameObject ballCrashParticlePrefab;
        public event Action<PlayerEnum> OnChangePlayer;
        public event Action<ReadOnlyDictionary<PlayerEnum, PlayerModel>> OnChangePlayerData;
        public event Action<string> OnSendMessage;

        private readonly Dictionary<PlayerEnum, PlayerModel> _playerDataDict = new();
        public Dictionary<PlayerEnum, PlayerModel> PlayerData => _playerDataDict;

        private static readonly string ROUND_SWITCH_TEXT = "交换投球权";
        public static readonly int MAX_ROUND = 4;

        private int _currentRound = 1;

        private Dictionary<PlayerEnum, int[]> _playerScoreDict = new();
        public Dictionary<PlayerEnum, int[]> PlayerScorePoints => _playerScoreDict;

        private Ball _currentBall;

        private LevelEnum _currentLevel;

        private PlayerEnum _firstRoundPlayer;

        public enum GameStateEnum
        {
            NotStarted,
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
            GameState = GameStateEnum.NotStarted;
            SceneLoader.OnLevelLoadComplete += OnLevelLoadComplete;
            _playerDataDict.Add(PlayerEnum.Player1, new PlayerModel { playerName = "Traveler", ballLeft = MAX_ROUND, score = 0 });
            _playerDataDict.Add(PlayerEnum.Player2, new PlayerModel { playerName = "Rika", ballLeft = MAX_ROUND, score = 0 });
            _playerScoreDict.Add(PlayerEnum.Player1, new int[MAX_ROUND]);
            _playerScoreDict.Add(PlayerEnum.Player2, new int[MAX_ROUND]);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneLoader.OnLevelLoadComplete -= OnLevelLoadComplete;
            BallManager.Instance.OnChangeRound -= BallManagerOnChangeRound;
        }

        private void OnLevelLoadComplete(LevelEnum currentLevel)
        {
            GameState = GameStateEnum.Started;
            _currentLevel = currentLevel;
            cameraTransform = Camera.main.transform;
            MusicManager.Instance.StartBGM();
            BallManager.Instance.OnChangeRound += BallManagerOnChangeRound;
            Timer.Instance.OnTimeComplete += CurrentPlayerTimeComplete;
            InitializePlayerUI();
            FirstRound();
        }

        private void BallManagerOnChangeRound()
        {
            Debug.Log("BallManager OnChangeRound");
            _playerDataDict[_currentPlayer].DecreaseBallLeft();
            OnChangePlayerData?.Invoke(new ReadOnlyDictionary<PlayerEnum, PlayerModel>(_playerDataDict));
            if (IsGameOver())
            {
                StartMessaging("游戏结束", ProcessGameOver);
                return;
            }
            RoundCheck(false); 
        }

        private void CurrentPlayerTimeComplete()
        {
            Debug.Log($"{_currentPlayer} Time Complete");
            _playerDataDict[_currentPlayer].DecreaseBallLeft();
            OnChangePlayerData?.Invoke(new ReadOnlyDictionary<PlayerEnum, PlayerModel>(_playerDataDict));
            if (IsGameOver())
            {
                StartMessaging("游戏结束", ProcessGameOver);
                return;
            }
            // destroy previous ball
            _currentBall.DisableInput();
            BallManager.Instance.RemoveBall(_currentBall);
            Destroy(_currentBall.gameObject);
            RoundCheck(true);
        }

        private void InitializePlayerUI()
        {
            OnChangePlayerData?.Invoke(new ReadOnlyDictionary<PlayerEnum, PlayerModel>(_playerDataDict));
        }

        public int GetPlayerScore(PlayerEnum player) => _playerDataDict[player].score;

        public string GetPlayerName(PlayerEnum player) => _playerDataDict[player].playerName;

        private void FirstRound()
        {
            var random = Random.Range(0, 101);
            _currentPlayer = random % 2 == 0 ? PlayerEnum.Player1 : PlayerEnum.Player2;
            _firstRoundPlayer = _currentPlayer;
            OnChangePlayer?.Invoke(_currentPlayer);
            var message = $"第一回合, {_playerDataDict[_currentPlayer].playerName}先手";
            StartMessaging(message, SpawnBall);
        }

        public void ReTry()
        {
            string message = $"{_playerDataDict[_currentPlayer].playerName}投球超出界限,重新投球!";
            StartMessaging(message, SpawnBall);
        }

        private void RoundCheck(bool timeout)
        {
            // ballLeft--
            // _playerDataDict[_currentPlayer].ballLeft--;
            if (_playerDataDict[PlayerEnum.Player1].ballLeft == _playerDataDict[PlayerEnum.Player2].ballLeft)
            {
                // 回合结束
                var message = $"第{_currentRound}回合结束";
                StartMessaging(message, null);
                _currentRound++;
                PlayerEnum player;
                if (GetPlayerScore(PlayerEnum.Player1) > GetPlayerScore(PlayerEnum.Player2))
                {
                    player = PlayerEnum.Player1;
                    message = $"第{_currentRound}回合, 由上回合胜方{_playerDataDict[player].playerName}先手";
                }
                else if (GetPlayerScore(PlayerEnum.Player1) < GetPlayerScore(PlayerEnum.Player2))
                {
                    player = PlayerEnum.Player2;
                    message = $"第{_currentRound}回合, 由上回合胜方{_playerDataDict[player].playerName}先手";
                }
                else
                {
                    player = _firstRoundPlayer;
                    message = $"第{_currentRound}回合, 由{_playerDataDict[player].playerName}先手";
                }
                _currentPlayer = player;
                OnChangePlayer?.Invoke(_currentPlayer);
                StartMessaging(message, SpawnBall);
            }
            else
            {
                var message = timeout ? $"{_playerDataDict[_currentPlayer].playerName}超时, 切换投球权!" : ROUND_SWITCH_TEXT;
                ChangePlayer();
                StartMessaging(message, SpawnBall);
            }
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
            SceneLoader.LoadSceneAsync(SceneEnum.GameOverScene, _ =>
            {
                ResetData();
            });
        }
        
        //  根据LevelData给的BallPrefab生成 Done!
        private void SpawnBall()
        {
            int ballIndex = MAX_ROUND - _playerDataDict[_currentPlayer].ballLeft;
            var levelData = GetCurrentLevelData();
            var ballGameObject = Instantiate(levelData.balls[ballIndex]);
            var ball = ballGameObject.GetComponent<Ball>();
            ball.SetBallIndex(ballIndex);
            if (_currentPlayer == PlayerEnum.Player1)
            {
                ballGameObject.GetComponentInChildren<BallVisual>().SetColor(player1Color);
                ball.SetPlayerInput(new Player1Input());
                ball.SetPlayerBelong(PlayerEnum.Player1);
            }
            else
            {
                ballGameObject.GetComponentInChildren<BallVisual>().SetColor(player2Color);
                ball.SetPlayerInput(new Player2Input());
                ball.SetPlayerBelong(PlayerEnum.Player2);
            }
            _currentBall = ball;
            BallManager.Instance.AddBall(ball);
            Timer.Instance.ReStartTimer();
        }

        public void UpdatePlayerScore(PlayerEnum playerEnum, int index, int score)
        {
            _playerScoreDict[playerEnum][index] = score;
            _playerDataDict[playerEnum].UpdateScore(_playerScoreDict[playerEnum].Aggregate(0, (pre, cur) => pre + cur));
            OnChangePlayerData?.Invoke(new ReadOnlyDictionary<PlayerEnum, PlayerModel>(_playerDataDict));
        }

        private void ChangePlayer()
        {
            _currentPlayer = _currentPlayer == PlayerEnum.Player1 ? PlayerEnum.Player2 : PlayerEnum.Player1;
            OnChangePlayer?.Invoke(_currentPlayer);
        }

        private LevelData GetCurrentLevelData()
        {
             return levelDataList.levelList.Find(item => item.level == _currentLevel);
        }

        private IEnumerator SetCameraRoundCheckingMode()
        {
            if (cameraTransform == null) yield break;
            float currentTime = 0;
            while (currentTime < cameraTransModifyDuration && (GameState == GameStateEnum.Started || GameState == GameStateEnum.OnMessaging))
            {
                currentTime += Time.deltaTime;
                cameraTransform.position = Vector3.Lerp(cameraNormalPostion, cameraCheckingPostion,
                    currentTime / cameraTransModifyDuration);
                cameraTransform.eulerAngles = Vector3.Lerp(cameraNormalRotation, cameraCheckingRotation, currentTime / cameraTransModifyDuration);
                yield return null;
            }
            if (cameraTransform == null) yield break;
            cameraTransform.position = cameraCheckingPostion;
            cameraTransform.eulerAngles = cameraCheckingRotation;
        }

        private IEnumerator SetCameraNormalMode()
        {
            if (cameraTransform == null) yield break;
            float currentTime = 0;
            while (currentTime < cameraTransModifyDuration && (GameState == GameStateEnum.Started || GameState == GameStateEnum.OnMessaging))
            {
                currentTime += Time.deltaTime;
                cameraTransform.position = Vector3.Lerp(cameraCheckingPostion,cameraNormalPostion,
                    currentTime / cameraTransModifyDuration);
                cameraTransform.eulerAngles = Vector3.Lerp(cameraCheckingRotation,cameraNormalRotation, currentTime / cameraTransModifyDuration);
                yield return null;
            }
            if (cameraTransform == null) yield break;
            cameraTransform.position = cameraNormalPostion;
            cameraTransform.eulerAngles = cameraNormalRotation;
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
            OnSendMessage?.Invoke(message);
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
            cameraTransform = null;
            _currentBall = null;
        }
    }
}

