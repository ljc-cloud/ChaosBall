using System.Linq;
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
            GameManager.Instance.OnPlayerScoreBoardChanged += OnPlayerScoreBoardChanged;
            InitScoreBoard();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnPlayerScoreBoardChanged -= OnPlayerScoreBoardChanged;
        }
        
        private void InitScoreBoard()
        {
            string nickname = GameManager.Instance.PlayerTypeToPlayerInfoDict[playerType].nickname;
            playerNameText.text = nickname;
            foreach (var image in roundImageArray)
            {
                image.sprite = unusedOperationSprite;
            }
            scoreText.text = "0";
        }

        private void OnPlayerScoreBoardChanged(Entity.PlayerType type, PlayerScoreBoard scoreBoard)
        {
            if (type != playerType) return;
            int totalScore = scoreBoard.scoreArray.Aggregate((pre, next) => pre + next);
            scoreText.text = totalScore.ToString();
            for (int i = 0; i < roundImageArray.Length; i++)
            {
                if (4 - scoreBoard.operationLeft == i)
                {
                    roundImageArray[i].sprite = currentOperationSprite;
                    continue;
                }
                if (scoreBoard.operationLeft > i)
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