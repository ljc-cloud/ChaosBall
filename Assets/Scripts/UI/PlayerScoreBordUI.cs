using System.Linq;
using ChaosBall.Event.Game;
using ChaosBall.Game;
using ChaosBall.Model;
using ChaosBall.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class PlayerScoreBordUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image[] roundImageArray;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Entity.PlayerType playerType;
        [SerializeField] private Sprite currentOperationSprite;
        [SerializeField] private Sprite usedOperationSprite;
        [SerializeField] private Sprite unusedOperationSprite;
        
        private void Start()
        {
            GameInterface.Interface.EventSystem.Subscribe<PlayerScoreBoardChangeEvent>(OnPlayerScoreBoardChanged);
            InitScoreBoard();
        }

        private void OnDestroy()
        {
            GameInterface.Interface.EventSystem.Unsubscribe<PlayerScoreBoardChangeEvent>(OnPlayerScoreBoardChanged);
        }
        
        private void InitScoreBoard()
        {
            // string nickname = GameManager.Instance.PlayerTypeToPlayerInfoDict[playerType].nickname;
            if (GameManager.Instance.PlayerTypeToPlayerInfoDict.TryGetValue(playerType, out var playerInfo))
            {
                playerNameText.text = playerInfo.nickname;
                foreach (var image in roundImageArray)
                {
                    image.sprite = unusedOperationSprite;
                }
                scoreText.text = "0";
            }
        }

        private void OnPlayerScoreBoardChanged(PlayerScoreBoardChangeEvent e)
        {
            if (e.playerType != playerType) return;
            int totalScore = e.playerScoreBoard.scoreArray.Aggregate((pre, next) => pre + next);
            scoreText.text = totalScore.ToString();
            for (int i = 0; i < roundImageArray.Length; i++)
            {
                if (e.playerScoreBoard.operationLeft == i)
                {
                    roundImageArray[i].sprite = currentOperationSprite;
                    continue;
                }
                if (e.playerScoreBoard.operationLeft > i)
                {
                    roundImageArray[i].sprite = unusedOperationSprite;
                }
                else
                {
                    roundImageArray[i].sprite = usedOperationSprite;
                }
            }
        }
    }
}