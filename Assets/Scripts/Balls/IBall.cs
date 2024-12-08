using System;
using ChaosBall.Manager;

namespace ChaosBall.Balls
{
    public enum BallState
    {
        UnLaunched,
        ReadyToLaunch,
        Launched,
        Stopped,
        Scored,
        Effected,
    }
    public interface IBall
    {
        bool ScoreCounted { get; }
        BallState CurrentBallState { get; } 
        bool HasCollide { get; }
        bool IsStopped { get; }
        int BallIndex { get; }
        
        // index in BallManager
        event Action<int> OnBallOutSpace;
        // belongPlayer, BallIndex, Score 
        event Action<PlayerEnum, int, int> OnBallIncreaseScore;
        // belongPlayer, BallIndex
        event Action<PlayerEnum, int> OnBallScoreReset;
        event Action OnBallScoreCounted;
        
        void EffectOtherBall(Ball other);
        void ProcessEffect(Ball other);
    }
}