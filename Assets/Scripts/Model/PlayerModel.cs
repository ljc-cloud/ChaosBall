using UnityEngine;

namespace ChaosBall.Model
{
    public class PlayerModel
    {
        public string playerName;
        public int ballLeft;
        public int score;

        public void UpdateScore(int newScore)
        {
            this.score = newScore;
        }

        public void DecreaseBallLeft()
        {
            this.ballLeft -= 1;
        }

        public override string ToString()
        {
            // Vector3
            return $"playerName: {playerName}, ballLeft:{ballLeft}, score: {score}";
        }
    }

}
