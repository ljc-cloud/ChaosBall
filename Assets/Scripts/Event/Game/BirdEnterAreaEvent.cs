using ChaosBall.Game;

namespace ChaosBall.Event.Game
{
    /// <summary>
    /// 进入得分区域事件
    /// </summary>
    public struct BirdEnterAreaEvent : IEvent
    {
        public Area area;
    }
}