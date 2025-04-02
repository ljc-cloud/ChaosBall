using ChaosBall.Model;

namespace ChaosBall.Event
{
    /// <summary>
    /// 玩家加入房间事件
    /// </summary>
    public struct RoomPlayerJoinEvent : IEvent
    {
        public RoomPlayerInfo roomPlayerInfo;
    }
}