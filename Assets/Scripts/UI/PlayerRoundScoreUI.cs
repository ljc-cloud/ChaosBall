using System.Linq;
using ChaosBall.Model;
using ChaosBall.Net;
using TMPro;
using UnityEngine;

namespace ChaosBall.UI
{
    public class PlayerRoundScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Entity.PlayerType playerType;

        public void SetPlayerRoundScoreUI(PlayerScoreBoard playerScoreBoard)
        {
            nicknameText.text = playerScoreBoard.nickname;
            int totalScore = playerScoreBoard.scoreArray.Aggregate((pre, next) => pre + next);
            scoreText.text = totalScore.ToString();
        }
    }
}