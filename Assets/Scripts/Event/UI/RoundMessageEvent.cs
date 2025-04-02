namespace ChaosBall.Event.Game
{
    /// <summary>
    /// 回合消息事件
    /// </summary>
    public struct RoundMessageEvent : IEvent
    {
        public int currentRound;
        public string message;
    }
}