using System;
using System.Collections;
using System.Linq;
using ChaosBall.Manager;
using ChaosBall.UI;
using ChaosBall.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.Game
{
    public class GameOverTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject playerInfoUIPrefab;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button tryOtherButton;
        [SerializeField] private Button quitOtherButton;

        private bool _animOver;
        
        private void Start()
        {
            playAgainButton.onClick.AddListener(() =>
            {
                SceneLoader.LoadLevelAsync(GameManager.Instance.CurrentLevel);
            });
            tryOtherButton.onClick.AddListener(() =>
            {
                SceneLoader.LoadScene(SceneEnum.LevelSelectScene);
            });
            quitOtherButton.onClick.AddListener(Application.Quit);
            StartCoroutine(SpawnPlayerInfo());
        }

        private IEnumerator SpawnPlayerInfo()
        {
            while (!_animOver)
            {
                yield return null;
            }
            
            var player1InfoUIGameObject = Instantiate(playerInfoUIPrefab, transform);
            var player2InfoUIGameObject = Instantiate(playerInfoUIPrefab, transform);
            
            var scorePoints = GameManager.Instance.PlayerScorePoints.Values.ToArray();
            Array.Sort(scorePoints, (x, y) => y.Aggregate(0, (pre, cur) => pre + cur) - x.Aggregate(0, (pre, cur) => pre + cur));
            
            var playerData = GameManager.Instance.PlayerData.Values.ToArray();
            Array.Sort(playerData, (x,y) => y.score - x.score);
            
            player1InfoUIGameObject.GetComponent<PlayerInfoUI>().SetData(true, playerData[0].playerName, scorePoints[0], playerData[0].score);
            player2InfoUIGameObject.GetComponent<PlayerInfoUI>().SetData(playerData[0].score == playerData[1].score, 
                playerData[1].playerName, scorePoints[1], playerData[1].score);
        }

        public void NotifyGameOverAnimOver()
        {
            _animOver = true;
        }

    }
}