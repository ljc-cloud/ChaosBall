using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Balls;
using Events;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlayerEnum
{
    Player1,
    Player2
}

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

    private readonly Dictionary<PlayerEnum, PlayerModel> _playerDataDict = new ();

    public readonly string ROUND_SWITCH_TEXT = "交换投球权";

    private int _currentRound = 1;

    // FIXME
    private Dictionary<PlayerEnum, int[]> _playerScoreDic = new();
    

    public enum GameStateEnum
    {
        CountToStart,
        Started,
        RoundChecking,
        GameOver
    }

    private PlayerEnum _currentPlayer;

    public GameStateEnum GameState { get; private set; }

    private void Awake()
    {
        GameState = GameStateEnum.Started;
        _playerDataDict.Add(PlayerEnum.Player1, new PlayerModel { playerName = "Traveler", ballLeft = 4, score = 0});
        _playerDataDict.Add(PlayerEnum.Player2, new PlayerModel { playerName = "Rika", ballLeft = 4, score = 0});
        _playerScoreDic.Add(PlayerEnum.Player1, new int[4]);
        _playerScoreDic.Add(PlayerEnum.Player2, new int[4]);
    }

    private void Start()
    {
        FirstRound();
        ChaosBallApp.Interface.RegisterEvent<OnChangeRound>(e =>
        {
            RoundCheck();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        ChaosBallApp.Interface.SendEvent(new OnUpdatePlayerData { playerData = _playerDataDict});
        InitializePlayerUI();
    }

    private void Update()
    {
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (_playerDataDict[PlayerEnum.Player1].ballLeft <= 0 && _playerDataDict[PlayerEnum.Player2].ballLeft <= 0)
        {
            // GameOver
            Debug.Log("GAME OVER!");
        }
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
        SetRoundCheckStart();
        var random = Random.Range(0, 101);
        _currentPlayer = random % 2 == 0 ? PlayerEnum.Player1 : PlayerEnum.Player2;
        CreateBall();
        var message = $"第一回合, {_playerDataDict[_currentPlayer].playerName}先手";
        ChaosBallApp.Interface.SendEvent(new OnMessaging { message = message});
    }

    public void ReTry()
    {
        // 通知UI
        SetRoundCheckStart();
        string message = $"{_playerDataDict[_currentPlayer].playerName}投球超出界限,重新投球!";
        ChaosBallApp.Interface.SendEvent(new OnMessaging { message = message});
        StartCoroutine(CreateRound());
    }

    private void RoundCheck()
    {
        // ballLeft--
        _playerDataDict[_currentPlayer].ballLeft--;
        ChaosBallApp.Interface.SendEvent(new OnChangePlayerData { playerData = new Dictionary<PlayerEnum, PlayerModel>(_playerDataDict) });
        if (_playerDataDict[PlayerEnum.Player1].ballLeft == _playerDataDict[PlayerEnum.Player2].ballLeft)
        {
            // 回合结束
            SetRoundCheckStart();
            var message = $"第{_currentRound}回合结束";
            ChaosBallApp.Interface.SendEvent(new OnMessaging { message = message});
            _currentRound++;
            var player = GetPlayerScore(PlayerEnum.Player1) > GetPlayerScore(PlayerEnum.Player2)
                ? PlayerEnum.Player1
                : PlayerEnum.Player2;
            message = $"第{_currentRound}回合, 由上回合胜方{_playerDataDict[player].playerName}先手";
            _currentPlayer = player;
            ChaosBallApp.Interface.SendEvent(new OnMessaging { message = message});
        }
        else
        {
            SetRoundCheckStart();
            _currentPlayer = _currentPlayer == PlayerEnum.Player1 ? PlayerEnum.Player2 : PlayerEnum.Player1;
            var message = ROUND_SWITCH_TEXT;
            ChaosBallApp.Interface.SendEvent(new OnMessaging { message = message});
        }
        StartCoroutine(CreateRound());
    }
    
    private IEnumerator CreateRound()
    {
        while (GameState == GameStateEnum.RoundChecking)
        {
            yield return null;
        }
        // Instantiate Ball
        CreateBall();
    }

    private void CreateBall()
    {
        var ballGameObject = Instantiate(ballPrefab);
        var ball = ballGameObject.GetComponent<Ball>();
        int ballIndex = 4 - _playerDataDict[_currentPlayer].ballLeft;
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
    }

    public void UpdatePlayerScore(PlayerEnum playerEnum, int index, int score)
    {
        _playerScoreDic[playerEnum][index] = score;
        _playerDataDict[playerEnum].score = _playerScoreDic[playerEnum].Aggregate(0, (pre, cur) => pre + cur);
        Debug.Log($"Update {playerEnum}, index:{index}, Score:{score}");
        ChaosBallApp.Interface.SendEvent(new OnChangePlayerData { playerData = new Dictionary<PlayerEnum, PlayerModel>(_playerDataDict)});
    }

    #region BUG,FIXME

    public void IncreasePlayerScore(PlayerEnum playerEnum, int index, int score)
    {
        _playerScoreDic[playerEnum][index] = score;
        _playerDataDict[playerEnum].score = _playerScoreDic[playerEnum].Aggregate(0, (pre, cur) => pre + cur);
        Debug.Log($"Increase {playerEnum} Score, Cur:{_playerDataDict[playerEnum].score}");
        ChaosBallApp.Interface.SendEvent(new OnChangePlayerData { playerData = new Dictionary<PlayerEnum, PlayerModel>(_playerDataDict)});
    }

    public void DecreasePlayerScore(PlayerEnum playerEnum, int index, int score)
    {
        _playerScoreDic[playerEnum][index] = score;
        _playerDataDict[playerEnum].score = _playerScoreDic[playerEnum].Aggregate(0, (pre, cur) => pre + cur);
        Debug.Log($"Decrease {playerEnum} Score, Cur:{_playerDataDict[playerEnum].score}");
        ChaosBallApp.Interface.SendEvent(new OnChangePlayerData { playerData = new Dictionary<PlayerEnum, PlayerModel>(_playerDataDict)});
    }

    #endregion
   
    
    private IEnumerator SetCameraRoundCheckingMode()
    {
        float currentTime = 0;
        while (currentTime < cameraTransModifyTime)
        {
            currentTime += Time.deltaTime;
            camera.position = Vector3.Lerp(cameraTrans1.position, cameraTrans2.position, currentTime / cameraTransModifyTime);
            yield return null;
        }
        camera.position = cameraTrans2.position;
    }
    
    private IEnumerator SetCameraNormalMode()
    {
        float currentTime = 0;
        while (currentTime < cameraTransModifyTime)
        {
            currentTime += Time.deltaTime;
            camera.position = Vector3.Lerp(cameraTrans2.position, cameraTrans1.position, currentTime / cameraTransModifyTime);
            yield return null;
        }
        camera.position = cameraTrans1.position;
    }

    public void SetRoundCheckStart()
    {
        StartCoroutine(SetCameraRoundCheckingMode());  
        GameState = GameStateEnum.RoundChecking;
    }
    public void SetRoundCheckOver()
    {
        StartCoroutine(SetCameraNormalMode());
        GameState = GameStateEnum.Started;
    }

    public void CreateParticle(Vector3 position)
    {
        Instantiate(ballCrashParticlePrefab, position, Quaternion.identity);
    }
}
