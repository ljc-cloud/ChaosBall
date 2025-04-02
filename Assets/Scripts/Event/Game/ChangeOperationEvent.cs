namespace ChaosBall.Event.Net
{
    /// <summary>
    /// 切换当前操作玩家事件（服务端响应）
    /// </summary>
    public struct ChangeOperationEvent : IEvent
    {
        public int currentOperationPlayerId;
    }
}