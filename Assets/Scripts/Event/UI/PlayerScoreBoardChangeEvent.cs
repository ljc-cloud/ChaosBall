using ChaosBall.Model;
using ChaosBall.Net;

namespace ChaosBall.Event.Game
{
    /// <summary>
    /// 玩家得分板变化事件
    /// </summary>
    public struct PlayerScoreBoardChangeEvent : IEvent
    {
        public Entity.PlayerType playerType;
        public PlayerScoreBoard playerScoreBoard;
        public int operationLeft;
    }
}