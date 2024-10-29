using Events;
using QFramework;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI player1ScoreText;
        [SerializeField] private TextMeshProUGUI player2ScoreText;
        private void Start()
        {
            ChaosBallApp.Interface.RegisterEvent<OnCountScore>(e =>
            {
                UpdateScoreUI(e);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void UpdateScoreUI(OnCountScore e)
        {
            player1ScoreText.text = $"Player1 Score: {e.player1Score}";
            player2ScoreText.text = $"Player2 Score: {e.player2Score}";
        }
    }
}