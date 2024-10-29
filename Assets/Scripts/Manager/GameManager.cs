using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform cameraTrans1;
    [SerializeField] private Transform cameraTrans2;
    [SerializeField] private Transform camera;
    [SerializeField] private float cameraTransModifyTime = 1f;

    private readonly Dictionary<PlayerEnum, PlayerModel> _playerDataDict = new ();

    public readonly string ROUND_SWITCH_TEXT = "交换投球权";

    private int _currentRound = 1;

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
        var ballGameObject = Instantiate(ballPrefab);
        if (random % 2 == 0)
        {
            ballGameObject.GetComponent<Ball>().SetPlayerInput(new Player1Input());
            ballGameObject.GetComponent<Ball>().SetPlayerBelong(PlayerEnum.Player1);
            _currentPlayer = PlayerEnum.Player1;
        }
        else
        {
            ballGameObject.GetComponent<Ball>().SetPlayerInput(new Player2Input());
            ballGameObject.GetComponent<Ball>().SetPlayerBelong(PlayerEnum.Player2);
            _currentPlayer = PlayerEnum.Player2;
        }
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
        var ballGameObject = Instantiate(ballPrefab);
        if (_currentPlayer == PlayerEnum.Player1)
        {
            ballGameObject.GetComponent<Ball>().SetPlayerInput(new Player1Input());
            ballGameObject.GetComponent<Ball>().SetPlayerBelong(PlayerEnum.Player1);
        }
        else
        {
            ballGameObject.GetComponent<Ball>().SetPlayerInput(new Player2Input());
            ballGameObject.GetComponent<Ball>().SetPlayerBelong(PlayerEnum.Player2);
        }
    }

    public void UpdatePlayerScore(PlayerEnum belongPlayer, int score)
    {
        _playerDataDict[belongPlayer].score = score;
        ChaosBallApp.Interface.SendEvent(new OnChangePlayerData { playerData = new Dictionary<PlayerEnum, PlayerModel>(_playerDataDict)});
    }

    public void SetRoundCheckStart()
    {
        StartCoroutine(SetCameraRoundCheckingMode());  
        GameState = GameStateEnum.RoundChecking;
    }

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

    public void SetRoundCheckOver()
    {
        StartCoroutine(SetCameraNormalMode());
        GameState = GameStateEnum.Started;
    }
}
