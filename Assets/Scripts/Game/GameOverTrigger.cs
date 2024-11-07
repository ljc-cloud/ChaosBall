using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Manager;
using ChaosBall.Model;
using ChaosBall.UI;
using UnityEngine;

namespace ChaosBall.Game
{
    public class GameOverTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject playerInfoUIPrefab;

        private bool _animOver;
        
        private void Start()
        {
            // GameManager.Instance.OnGameOverTriggered += TriggerGameOver;
            StartCoroutine(SpawnPlayerInfo());
        }

        private void TriggerGameOver(List<int[]> scores, List<PlayerModel> models)
        {
            scores.Sort((x, y) =>
            {
                return x.Aggregate(0, (pre, cur) => pre + cur) - y.Aggregate(0, (pre,cur)=> pre + cur);
            });
            models.Sort((x, y) => x.score - y.score);
            // StartCoroutine(SpawnPlayerInfo(scores, models));
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