using ChaosBall.Event;

namespace Event.Game
{
    /// <summary>
    /// 当前操作玩家被修改事件
    /// </summary>
    public struct CurrentOperationPlayerChangedEvent : IEvent
    {
        public int currentOperationPlayerId;
    }
}