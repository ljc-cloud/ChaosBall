namespace ChaosBall.Event.Net
{
    /// <summary>
    /// 切换当前回合事件（服务端响应）
    /// </summary>
    public struct ChangeRoundEvent : IEvent
    {
        public int currentRound;
    }
}