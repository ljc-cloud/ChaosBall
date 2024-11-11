using System;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Balls;
using QFramework;

namespace ChaosBall.Manager
{
    public class BallManager : MonoSingleton<BallManager>
    {
        private List<Ball> _ballList;

        public event Action OnChangeRound;

        private void Awake()
        {
            _ballList = new List<Ball>();
        }

        public void AddBall(Ball ball)
        {
            ball.OnBallOutSpace += OnBallOutSpace;
            ball.OnBallIncreaseScore += IncreaseBallScore;
            ball.OnBallScoreReset += ResetBallScore;
            ball.OnBallScoreCounted += BallOnOnBallScored;
            _ballList.Add(ball);
        }

        private void BallOnOnBallScored()
        {
            var allScored = _ballList.All(ball => ball.CurrentBallState == BallState.Scored);
            if (allScored)
            {
                OnChangeRound?.Invoke();
            }
        }

        public void RemoveBall(Ball ball)
        {
            ball.OnBallOutSpace -= OnBallOutSpace;
            ball.OnBallIncreaseScore -= IncreaseBallScore;
            ball.OnBallScoreReset -= ResetBallScore;
            _ballList.Remove(ball);
        }

        public int IndexAt(Ball ball) => _ballList.IndexOf(ball);

        private void ResetBallScore(PlayerEnum player, int ballIndex)
        {
            GameManager.Instance.UpdatePlayerScore(player, ballIndex, 0);
        }

        private void IncreaseBallScore(PlayerEnum player, int ballIndex, int score)
        {
            GameManager.Instance.UpdatePlayerScore(player, ballIndex, score);
        }

        private void OnBallOutSpace(int index)
        {
            var ball = _ballList[index];
            Destroy(ball.gameObject);
            _ballList.RemoveAt(index);
        }
    }
}
